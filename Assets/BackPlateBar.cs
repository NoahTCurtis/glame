using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackPlateBar : MonoBehaviour
{
	public bool IsCenterCircle = false;

	Transform bar;

	private void Start()
	{
		//make a changeable bar
		GameObject child = Instantiate(gameObject, transform);
		child.GetComponent<BackPlateBar>().enabled = false;
		bar = child.transform;
		bar.localPosition = Vector3.zero;
		bar.localScale = Vector3.zero;
		bar.localRotation = Quaternion.identity;
		//disable this one's visuals
		GetComponent<MeshRenderer>().enabled = false;
	}

	public void SetFill01(float fill)
	{
		if(fill == 0.0f)
		{
			bar.localScale = Vector3.zero;
		}
		else
		{
			if(IsCenterCircle)
			{
				bar.localScale = Vector3.one * fill;
			}
			else
			{
				bar.localScale = new Vector3(1, fill, 1);
				float posY = (0.5f * fill - 0.5f);
				bar.localPosition = Vector3.up * posY;
			}
		}
	}
}
