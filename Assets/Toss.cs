using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toss : MonoBehaviour
{
	public GameObject Prefab;
	GameObject obj = null;

	void Update()
	{
		if (Input.GetMouseButtonDown(2))
		{
			if (Prefab != null)
				obj = GameObject.Instantiate(Prefab);
			else
				obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);

			Vector3 camForward = Camera.main.transform.forward;
			obj.transform.position = transform.position + camForward;
			obj.transform.rotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));
			var rb = obj.GetComponent<Rigidbody>();
			rb.velocity = camForward * 10;
			float v = 2;
			rb.angularVelocity = new Vector3(Random.Range(-v, v), Random.Range(-v, v), Random.Range(-v, v));
		}
	}
}
