using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GBE : MonoBehaviour
{
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

	public float charge = 0.0f;

	//current shot data
	private List<GK.BreakableByGBE> _beamTargets;
	private Ray _beamRay;
	private float _beamRadius;

	// Start is called before the first frame update
	void Start()
	{
		Beam.transform.localScale = Vector3.zero;
		Beam.transform.localPosition = Vector3.zero;
	}

	// Update is called once per frame
	bool JUST_SHOOT_ALREADY = true; //cheatcode lol
	void Update()
	{
		float secondsItTakesToCharge = 1.75f;
		if(Input.GetMouseButton(1))
		{
			charge += Time.deltaTime / secondsItTakesToCharge;
		}
		else
		{
			charge -= Time.deltaTime / secondsItTakesToCharge;
		}
		charge = Mathf.Clamp(charge, 0, 8);

		OpenRail01(Mathf.Clamp01(charge - 4));
		FillUIBars();
		
		float minRadius = 0.5f;
		float maxRadius = 2.0f;
		_beamRadius = Mathf.Lerp(minRadius, maxRadius, charge);

		if(Input.GetMouseButtonDown(0) && (charge > 1 || JUST_SHOOT_ALREADY))
		{
			Shoot();
		}
	}

	void FillUIBars()
	{
		barN.SetFill01( Mathf.Clamp(charge, 0, 1) - 0.0f);
		barE.SetFill01( Mathf.Clamp(charge, 1, 2) - 1.0f);
		barS.SetFill01( Mathf.Clamp(charge, 2, 3) - 2.0f);
		barW.SetFill01( Mathf.Clamp(charge, 3, 4) - 3.0f);
		barNE.SetFill01(Mathf.Clamp(charge, 4, 5) - 4.0f);
		barSE.SetFill01(Mathf.Clamp(charge, 5, 6) - 5.0f);
		barSW.SetFill01(Mathf.Clamp(charge, 6, 7) - 6.0f);
		barNW.SetFill01(Mathf.Clamp(charge, 7, 8) - 7.0f);
	}

	void Shoot() //All shooting stuff happens in the lifetime of this function
	{
		StartCoroutine(EmitBeam());
		CollectBeamTargets();
		StartCoroutine(BreakBeamTargets());

		charge = 0;
	}

	void OpenRail01(float value)
	{
		float t = value * 0.5f;
		Rail1.localPosition = new Vector3(0, +1.0f, +1.0f) * t;
		Rail2.localPosition = new Vector3(0, -1.0f, +1.0f) * t;
		Rail3.localPosition = new Vector3(0, -1.0f, -1.0f) * t;
		Rail4.localPosition = new Vector3(0, +1.0f, -1.0f) * t;
	}

	private IEnumerator EmitBeam()
	{
		float startTime = Time.time;
		float endTime = Time.time + 0.25f;
		float startRadius = _beamRadius;

		float length = 10.0f;
		while (Time.time <= endTime)
		{
			float t = Mathf.InverseLerp(startTime, endTime, Time.time);
			float diameter = startRadius * 2.0f * (1.0f - t);
			Beam.localPosition = new Vector3(length, 0, 0);
			Beam.localScale = new Vector3(diameter, length, diameter);
			yield return null;
		}

		//reset for next time
		Beam.transform.localScale = Vector3.zero;
		Beam.transform.localPosition = Vector3.zero;

		yield break;
	}

	void CollectBeamTargets()
	{
		Transform cam = Camera.main.transform;
		var hits = Physics.SphereCastAll(cam.position, _beamRadius, cam.forward);

		_beamTargets = new List<GK.BreakableByGBE>();
		_beamRay = new Ray(cam.position, cam.forward);

		System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

		foreach (var hit in hits)
		{
			GK.BreakableByGBE breaker = hit.collider.GetComponent<GK.BreakableByGBE>();
			if (breaker != null)
			{
				_beamTargets.Add(breaker);
			}
		}
	}

	IEnumerator BreakBeamTargets()
	{
		Ray ray = _beamRay;
		float radius = _beamRadius;

		int maxBreaksPerFrame = 16;
		int brokenOnThisFrame = 0;
		foreach(var breaker in _beamTargets)
		{
			breaker.Break(ray, radius);
			brokenOnThisFrame += 1;

			if(brokenOnThisFrame >= maxBreaksPerFrame)
			{
				yield return null;
				brokenOnThisFrame = 0;
			}
		}
	}
}
