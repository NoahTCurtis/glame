using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
	void OnCollisionEnter(Collision col)
	{
		if(col.rigidbody != null)
		{
			Destroy(col.gameObject);
		}
	}
}
