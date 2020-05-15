using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	[SerializeField] private Image Crosshair;

	private GBE gbe { get { return GBE.instance; } }
	
	void Update()
	{
		if (gbe == null) return;

		Crosshair.enabled = gbe.Charging && !gbe.ShotInProgress;
	}
}
