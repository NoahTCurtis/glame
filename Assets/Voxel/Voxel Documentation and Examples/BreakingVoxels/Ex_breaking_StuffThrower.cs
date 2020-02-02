using UnityEngine;
using System.Collections;

public class Ex_breaking_StuffThrower : MonoBehaviour {
	public GameObject stuff;
	public float speed;
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			//Throw the stuff
			GameObject thing = (GameObject)Instantiate(stuff,transform.position,Quaternion.identity);
			thing.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.forward*speed),ForceMode.Impulse);
		}
	}
}
