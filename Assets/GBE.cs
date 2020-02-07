using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GBE : MonoBehaviour
{
	public class BeamData
	{
		public BeamData(Ray ray, float radius)
		{
			this.ray = ray;
			this.radius = radius;

			targets = new List<BreakableByGBE>();
			newDebrisPieces = new List<GameObject>();
		}

		public void AddTarget(BreakableByGBE target)
		{
			targets.Add(target);
		}

		public void AddNewDebrisPiece(GameObject piece)
		{
			newDebrisPieces.Add(piece);
		}
		
		public Ray ray;
		public float radius;
		public List<BreakableByGBE> targets;
		public List<GameObject> newDebrisPieces;

		public int targetsBrokenSoFar = 0;
	}


	#region GameObjectReferences
	[Header("Rail Pieces")]
	public Transform Rail1;
	public Transform Rail2;
	public Transform Rail3;
	public Transform Rail4;

	[Header("Barrel Pieces")]
	public Transform b1;
	public Transform b2;
	public Transform b3;
	public Transform b4;
	public Transform b5;
	public Transform b6;
	public Transform b7;
	public Transform b8;

	[Header("Backplate UI")]
	public BackPlateBar barN;
	public BackPlateBar barE;
	public BackPlateBar barS;
	public BackPlateBar barW;
	public BackPlateBar barNE;
	public BackPlateBar barSE;
	public BackPlateBar barSW;
	public BackPlateBar barNW;
	public BackPlateBar centerCircle;

	[Header("Beam")]
	public Transform BeamOrigin;
	public Transform Beam;

	[Header("Other Pieces")]
	public Transform Handle;
	public Transform Trigger;

	[Header("VFX")]
	public List<ParticleSystem> Particles;
	#endregion

	//Singleton pattern
	public static GBE instance = null;

	//GBE animation data
	public float charge = 0.0f;

	//current shot data
	private BeamData _beam;
	public bool ShotInProgress { get => _beam != null; }
	public bool Charging { get => charge > 0.0f; }

	// Start is called before the first frame update
	void Start()
	{
		instance = this;
		Beam.transform.localScale = Vector3.zero;
		Beam.transform.localPosition = Vector3.zero;
	}

	// Update is called once per frame
	bool JUST_SHOOT_ALREADY = false; //cheatcode lol
	void Update()
	{
		//find charge level
		float secondsItTakesToCharge = 1.75f;
		float secondsItTakesToUncharge = 0.75f;
		if (Input.GetMouseButton(1))
		{
			charge += Time.deltaTime / secondsItTakesToCharge;
		}
		else
		{
			charge -= Time.deltaTime / secondsItTakesToUncharge;
		}
		charge = Mathf.Clamp(charge, 0, 6);

		//animate the gun
		float openness01 = Mathf.Clamp01(charge - 4);
		OpenRail01(openness01);
		FillUIBars();
		GunShake01(openness01);

		//shoot, if possible
		if(Input.GetMouseButtonDown(0) && !ShotInProgress && (charge > 1 || JUST_SHOOT_ALREADY))
		{
			Shoot();
		}
	}

	void OpenRail01(float value)
	{
		float t = value * 0.5f;
		Rail1.localPosition = Vector3.Lerp(new Vector3(0, -0.23f, -0.23f), new Vector3(0, +1.0f, +1.0f), t);
		Rail2.localPosition = Vector3.Lerp(new Vector3(0, +0.23f, -0.23f), new Vector3(0, -1.0f, +1.0f), t);
		Rail3.localPosition = Vector3.Lerp(new Vector3(0, +0.23f, +0.23f), new Vector3(0, -1.0f, -1.0f), t);
		Rail4.localPosition = Vector3.Lerp(new Vector3(0, -0.23f, +0.23f), new Vector3(0, +1.0f, -1.0f), t);
	}

	void FillUIBars()
	{
		barN.SetFill01(charge > 5 ? 0 : Mathf.Clamp(charge, 0, 1) - 0.0f);
		barE.SetFill01(charge > 5 ? 0 : Mathf.Clamp(charge, 1, 2) - 1.0f);
		barS.SetFill01(charge > 5 ? 0 : Mathf.Clamp(charge, 2, 3) - 2.0f);
		barW.SetFill01(charge > 5 ? 0 : Mathf.Clamp(charge, 3, 4) - 3.0f);

		centerCircle.SetFill01( Mathf.Clamp(charge, 4, 5) - 4.0f);

		barNE.SetFill01(charge > 5 ? 1 : 0);
		barSE.SetFill01(charge > 5 ? 1 : 0);
		barSW.SetFill01(charge > 5 ? 1 : 0);
		barNW.SetFill01(charge > 5 ? 1 : 0);
	}

	void GunShake01(float value)
	{
		float shakeLateral = 0.0015f;
		float y = Random.Range(-shakeLateral, shakeLateral);
		float z = Random.Range(-shakeLateral, shakeLateral);
		transform.localPosition = new Vector3(transform.localPosition.x, y, z) * value;

		foreach(var ps in Particles)
		{
			var emission = ps.emission;
			emission.rateOverTime = (int)(1000.0f * value * value * value);
		}
	}

	void Shoot() //All shooting stuff happens in the lifetime of this function
	{
		//find the shot radius at this charge level
		float beamRadius = 0;
		switch ((int)Mathf.Floor(charge))
		{
			case 0: beamRadius = 0.5f; break; //no bars
			case 1: beamRadius = 1.0f; break; //1 bar
			case 2: beamRadius = 2.0f; break; //2 bars
			case 3: beamRadius = 4.0f; break; //3 bars
			case 4: beamRadius = 8.0f; break; //4 bars
			case 5: beamRadius = 16.0f; break;//X shot
			case 6: beamRadius = 16.0f; break;//X shot again
		}

		//create beam data
		Transform cam = Camera.main.transform;
		Ray beamRay = new Ray(cam.position, cam.forward);
		_beam = new BeamData(beamRay, beamRadius);

		//shot pipeline
		CollectBeamTargets(_beam);
		
		IEnumerator enablePhysicsOnDebris = EnablePhysicsOnDebris(_beam, EndShot());
		IEnumerator breakBeamTargets = BreakBeamTargets(_beam, enablePhysicsOnDebris);

		StartCoroutine(breakBeamTargets);

		//animation
		StartCoroutine(EmitBeam(_beam));

		charge = 0;
	}

	void CollectBeamTargets(BeamData beam)
	{
		Transform cam = Camera.main.transform;
		var hits = Physics.SphereCastAll(cam.position, beam.radius, cam.forward);

		System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

		foreach (var hit in hits)
		{
			BreakableByGBE breaker = hit.collider.GetComponent<BreakableByGBE>();
			if (breaker != null)
			{
				beam.targets.Add(breaker);
			}
		}
	}

	private IEnumerator EndShot()
	{
		_beam = null;
		yield return null;
	}

	/*
	private IEnumerator EmitBeam(BeamData beam, IEnumerator next = null)
	{
		float length = 10.0f;
		while (beam.targetsBrokenSoFar < beam.targets.Count)
		{
			float t = (float)beam.targetsBrokenSoFar / (float)beam.targets.Count;
			float diameter = beam.radius * 2.0f * (1.0f - t);
			Beam.localPosition = new Vector3(length, 0, 0);
			Beam.localScale = new Vector3(diameter, length, diameter);
			yield return null;
		}

		//reset for next time
		Beam.transform.localScale = Vector3.zero;
		Beam.transform.localPosition = Vector3.zero;

		if (next != null) StartCoroutine(next);
	}

	/*/

	private IEnumerator EmitBeam(BeamData beam, IEnumerator next = null)
	{
		float startTime = Time.time;
		float endTime = Time.time + 0.25f;
		float startRadius = beam.radius;

		float length = 10.0f;
		float t = 0;
		while (t < 1)
		{
			float timeT = Mathf.InverseLerp(startTime, endTime, Time.time);
			float breakT = 1;
			if(beam != null)
				breakT = (float)beam.targetsBrokenSoFar / (float)beam.targets.Count;
			t = Mathf.Min(timeT, breakT);
			Debug.Log(t + " (" + beam?.targetsBrokenSoFar + "/" + beam?.targets.Count + ") [time" + timeT + " / break" + breakT + "]");

			float diameter = startRadius * 2.0f * (1.0f - t);
			Beam.localPosition = new Vector3(length, 0, 0);
			Beam.localScale = new Vector3(diameter, length, diameter);
			yield return null;
		}

		//reset for next time
		Beam.transform.localScale = Vector3.zero;
		Beam.transform.localPosition = Vector3.zero;

		if (next != null) StartCoroutine(next);
	}
	//*/

	IEnumerator BreakBeamTargets(BeamData beam, IEnumerator next = null)
	{
		int maxBreaksPerFrame = 5;
		int brokenOnThisFrame = 0;
		foreach(var breaker in beam.targets)
		{
			breaker.Break(beam);
			brokenOnThisFrame += 1;
			beam.targetsBrokenSoFar += 1;

			if(brokenOnThisFrame >= maxBreaksPerFrame)
			{
				yield return null;
				brokenOnThisFrame = 0;
			}
		}

		if (next != null) StartCoroutine(next);
	}

	IEnumerator EnablePhysicsOnDebris(BeamData beam, IEnumerator next = null)
	{
		foreach(var debris in beam.newDebrisPieces)
		{
			Rigidbody rb = debris.GetComponent<Rigidbody>();
			if (rb != null)
				rb.isKinematic = false;
		}

		yield return null;

		if (next != null) StartCoroutine(next);
	}
}
