using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable_Destroy : BreakableByGBE
{
	public override void Break(GBE.BeamData beam)
	{
		Destroy(gameObject);
	}
}
