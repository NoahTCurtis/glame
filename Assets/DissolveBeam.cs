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

	public Material mat;
	private List<Material> materials = new List<Material>();

	private List<CylinderParameters> cylinders = new List<CylinderParameters>();

	private bool invert = false;
	private bool local = false;

	void Start()
	{
		UpdateMaskKeyword();
		UpdateMaskCountKeyword(1); //TODO: make this take zero as valid arg

		//fill out materials list
		//materials.Add(mat);
		foreach(var material in GetComponent<Renderer>().materials)
		{
			materials.Add(material);
		}
	}

	public void AddBeam(GBE.BeamData beam)
	{
		if (cylinders.Count >= 4) return;

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

		for (int i = 0; i < materials.Count; i++)
		{
			Debug.Log($"DissolveBeam.Addbeam(): Updating material {materials[i].name}. (Mask ID {maskID})");

			Debug.Assert(materials[i] != null, "DissolveBeam tried to update a null material");

			materials[i].SetFloat("_DissolveMaskSpace", local ? 1 : 0);
			materials[i].SetFloat("_DissolveMaskInvert", invert ? 1 : -1);

			switch (maskID)
			{
				case 1:
					materials[i].SetVector("_DissolveMaskPosition", position);
					materials[i].SetVector("_DissolveMaskNormal", normal);
					materials[i].SetFloat("_DissolveMaskRadius", radius);
					materials[i].SetFloat("_DissolveMaskHeight", height);
					break;

				case 2:
					materials[i].SetVector("_DissolveMask2Position", position);
					materials[i].SetVector("_DissolveMask2Normal", normal);
					materials[i].SetFloat("_DissolveMask2Radius", radius);
					materials[i].SetFloat("_DissolveMask2Height", height);
					break;

				case 3:
					materials[i].SetVector("_DissolveMask3Position", position);
					materials[i].SetVector("_DissolveMask3Normal", normal);
					materials[i].SetFloat("_DissolveMask3Radius", radius);
					materials[i].SetFloat("_DissolveMask3Height", height);
					break;

				case 4:
					materials[i].SetVector("_DissolveMask4Position", position);
					materials[i].SetVector("_DissolveMask4Normal", normal);
					materials[i].SetFloat("_DissolveMask4Radius", radius);
					materials[i].SetFloat("_DissolveMask4Height", height);
					break;
			}
		}

	}

	public void UpdateMaskKeyword()
	{
		if (materials != null)
		{
			for (int i = 0; i < materials.Count; i++)
			{
				if (materials[i] == null)
					continue;

				//Enable proper keyword only
				materials[i].DisableKeyword("_DISSOLVEMASK_XYZ_AXIS");
				materials[i].DisableKeyword("_DISSOLVEMASK_PLANE");
				materials[i].DisableKeyword("_DISSOLVEMASK_SPHERE");
				materials[i].DisableKeyword("_DISSOLVEMASK_BOX");
				materials[i].DisableKeyword("_DISSOLVEMASK_CONE");


				materials[i].EnableKeyword("_DISSOLVEMASK_CYLINDER");

				//For material editor to select proper name inside dropdown
				materials[i].SetFloat("_DissolveMask", 5);
			}
		}
	}

	public void UpdateMaskCountKeyword(int count) //TODO: make this take zero as valid arg
	{
		if (materials != null)
		{
			for (int i = 0; i < materials.Count; i++)
			{
				if (materials[i] == null)
					continue;

				//Enable proper keyword only
				materials[i].DisableKeyword("_DISSOLVEMASKCOUNT_FOUR");
				materials[i].DisableKeyword("_DISSOLVEMASKCOUNT_THREE");
				materials[i].DisableKeyword("_DISSOLVEMASKCOUNT_TWO");

				switch (count)
				{
					case 1: break;
					case 2: materials[i].EnableKeyword("_DISSOLVEMASKCOUNT_TWO"); break;
					case 3: materials[i].EnableKeyword("_DISSOLVEMASKCOUNT_THREE"); break;
					case 4: materials[i].EnableKeyword("_DISSOLVEMASKCOUNT_FOUR"); break;
				}

				//For material editor to select proper name inside dropdown
				materials[i].SetFloat("_DissolveMaskCount", count - 1);
			}
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
