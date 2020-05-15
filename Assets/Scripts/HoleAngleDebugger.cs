using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleAngleDebugger : MonoBehaviour
{
	public GameObject Prefab;
	GameObject obj = null;
	
	void Update()
	{
		if(Input.GetMouseButtonDown(2))
		{
			if(Prefab != null)
				obj = GameObject.Instantiate(Prefab);
			else
				obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			obj.GetComponent<Collider>().enabled = false;
		}
		if(Input.GetMouseButton(2))
		{
			Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

			//find contact point
			float distance;
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit))
			{
				obj.transform.position = hit.point;

				Plane plane = new Plane(hit.collider.transform.forward, hit.collider.transform.position);

				//find angle to rotate the hole so the ellipse lines up correctly
				//V||P = P × (V×P / |P|) / |P| = Px(VxP)?
				//V:vector, P:normal of plane
				Vector2 rayDirOnPlane = Vector3.Cross(plane.normal, Vector3.Cross(ray.direction, plane.normal));
				Vector2 upDirOnPlane = Vector3.Cross(plane.normal, Vector3.Cross(transform.up, plane.normal));
				float holeRotation = Vector2.Angle(rayDirOnPlane, upDirOnPlane) * Mathf.Deg2Rad;

				//set obj rotation
				obj.transform.LookAt(obj.transform.position + plane.normal, ray.direction);
				obj.transform.localScale = Vector3.one;
				obj.transform.SetParent(hit.collider.transform, true);
				//obj.transform.eulerAngles = new Vector3(obj.transform.eulerAngles.x, obj.transform.eulerAngles.y, holeRotation);
			}
		}
	}
}
