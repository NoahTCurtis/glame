using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveBeam : MonoBehaviour
{
	[System.Serializable]
	public class CylinderParameters
	{
		public Vector3 position = Vector3.zero;
		public Vector3 normal = Vector3.forward;
		public float radius = 1;
		public float height = 5;
	}

	private Renderer _renderer;
	private List<CylinderParameters> cylinders = new List<CylinderParameters>();

	private bool invert = false;
	private bool local = false;

	void Start()
	{
		_renderer = GetComponent<Renderer>();
		UpdateMaskKeyword();
		UpdateMaskCountKeyword(1); //TODO: make this take zero as valid arg
	}

	private void SetDissolveMaterial()
	{
		var renderer = GetComponent<Renderer>();

		var newMat = Game.Manager<MaterialManager>().GetDissolveMaterial(renderer.material);
		Debug.Assert(renderer.material != newMat);
		renderer.material = newMat;
	}

	/*
	private void SetStandardMaterial()
	{
		var renderer = GetComponent<Renderer>();
		for (int i = 0; i < renderer.materials.Length; i++)
		{
			var newMat = Game.Manager<MaterialManager>().GetStandardMaterial(renderer.materials[i]);
			Debug.Assert(renderer.materials[i] != newMat);
			renderer.materials[i] = newMat;
		}
	}
	*/

	public void AddBeam(GBE.BeamData beam)
	{
		if (cylinders.Count >= 4) return;

		Debug.Log($"{gameObject.name} added dissolve beam");

		if(cylinders.Count == 0)
			SetDissolveMaterial();

		CylinderParameters cp = new CylinderParameters();
		cylinders.Add(cp);

		float beamLength = 999.9f;
		cp.position = beam.ray.origin;// + beam.ray.direction * (beamLength / 2.0f);
		cp.normal = beam.ray.direction;
		cp.radius = beam.radius;
		cp.height = beamLength;

		for (int i = 0; i < cylinders.Count; i++)
		{
			UpdateShaderData(i + 1, cylinders[i]);
		}

		UpdateMaskCountKeyword(cylinders.Count);
	}

	void UpdateShaderData(int maskID, CylinderParameters cylinder)
	{
		Debug.Assert(cylinder != null, "DissolveBeam.UpdateShaderData() got null cylinder");

		Vector3 position = cylinder.position;
		Vector3 normal = cylinder.normal;
		float radius = cylinder.radius;
		float height = cylinder.height;

		
		for (int i = 0; i < _renderer.materials.Length; i++)
		{
			//Debug.Log($"DissolveBeam.Addbeam(): Updating material {_renderer.materials[i].name}. (Mask ID {maskID})");

			Debug.Assert(_renderer.materials[i] != null, "DissolveBeam tried to update a null material");

			_renderer.materials[i].SetFloat("_DissolveMaskSpace", local ? 1 : 0);
			_renderer.materials[i].SetFloat("_DissolveMaskInvert", invert ? 1 : -1);

			switch (maskID)
			{
				case 1:
					_renderer.materials[i].SetVector("_DissolveMaskPosition", position);
					_renderer.materials[i].SetVector("_DissolveMaskNormal", normal);
					_renderer.materials[i].SetFloat("_DissolveMaskRadius", radius);
					_renderer.materials[i].SetFloat("_DissolveMaskHeight", height);
					break;

				case 2:
					_renderer.materials[i].SetVector("_DissolveMask2Position", position);
					_renderer.materials[i].SetVector("_DissolveMask2Normal", normal);
					_renderer.materials[i].SetFloat("_DissolveMask2Radius", radius);
					_renderer.materials[i].SetFloat("_DissolveMask2Height", height);
					break;

				case 3:
					_renderer.materials[i].SetVector("_DissolveMask3Position", position);
					_renderer.materials[i].SetVector("_DissolveMask3Normal", normal);
					_renderer.materials[i].SetFloat("_DissolveMask3Radius", radius);
					_renderer.materials[i].SetFloat("_DissolveMask3Height", height);
					break;

				case 4:
					_renderer.materials[i].SetVector("_DissolveMask4Position", position);
					_renderer.materials[i].SetVector("_DissolveMask4Normal", normal);
					_renderer.materials[i].SetFloat("_DissolveMask4Radius", radius);
					_renderer.materials[i].SetFloat("_DissolveMask4Height", height);
					break;
			}
		}

	}

	public void UpdateMaskKeyword()
	{
		for (int i = 0; i < _renderer.materials.Length; i++)
		{
			if (_renderer.materials[i] == null)
				continue;

			//Enable proper keyword only
			_renderer.materials[i].DisableKeyword("_DISSOLVEMASK_XYZ_AXIS");
			_renderer.materials[i].DisableKeyword("_DISSOLVEMASK_PLANE");
			_renderer.materials[i].DisableKeyword("_DISSOLVEMASK_SPHERE");
			_renderer.materials[i].DisableKeyword("_DISSOLVEMASK_BOX");
			_renderer.materials[i].DisableKeyword("_DISSOLVEMASK_CONE");

			_renderer.materials[i].EnableKeyword("_DISSOLVEMASK_CYLINDER");

			//For material editor to select proper name inside dropdown
			_renderer.materials[i].SetFloat("_DissolveMask", 5);
		}
	}

	public void UpdateMaskCountKeyword(int count) //TODO: make this take zero as valid arg
	{
		for (int i = 0; i < _renderer.materials.Length; i++)
		{
			if (_renderer.materials[i] == null)
				continue;

			//Enable proper keyword only
			_renderer.materials[i].DisableKeyword("_DISSOLVEMASKCOUNT_FOUR");
			_renderer.materials[i].DisableKeyword("_DISSOLVEMASKCOUNT_THREE");
			_renderer.materials[i].DisableKeyword("_DISSOLVEMASKCOUNT_TWO");

			switch (count)
			{
				case 1: break;
				case 2: _renderer.materials[i].EnableKeyword("_DISSOLVEMASKCOUNT_TWO"); break;
				case 3: _renderer.materials[i].EnableKeyword("_DISSOLVEMASKCOUNT_THREE"); break;
				case 4: _renderer.materials[i].EnableKeyword("_DISSOLVEMASKCOUNT_FOUR"); break;
			}

			//For material editor to select proper name inside dropdown
			_renderer.materials[i].SetFloat("_DissolveMaskCount", count - 1);
		}
	}
}

[System.Serializable]
public class CylinderParameters
{
	public Vector3 position = Vector3.zero;
	public Vector3 normal = Vector3.forward;
	public float radius = 1;
	public float height = 5;
}
