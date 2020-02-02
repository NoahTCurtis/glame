using UnityEngine;
using System.Collections;

[AddComponentMenu("Voxels/Voxel Manipulator")]
public class VoxelManipulator : MonoBehaviour {
	public Camera rayCam;
	RaycastHit hit = new RaycastHit();
	Ray ray;
	void Start(){
		if(rayCam == null){
			rayCam = GetComponent<Camera>();
			if(rayCam == null){
				rayCam = Camera.current;
				if(rayCam == null){
					enabled = false;
				}
			}
		}
	}
	void Update(){
		ray = rayCam.ViewportPointToRay(new Vector3(0.5f,0.5f,0f));
		#region input
		if(Input.GetMouseButtonDown(0)){
			if(Physics.Raycast(ray,out hit,Mathf.Infinity)){
				hit.collider.gameObject.SendMessage("RelayLeftClickDown",hit,SendMessageOptions.DontRequireReceiver);
			}
		}
		#endregion
		#region input
		if(Input.GetMouseButtonUp(0)){
			if(Physics.Raycast(ray,out hit,Mathf.Infinity)){
				hit.collider.gameObject.SendMessage("RelayLeftClickUp",hit,SendMessageOptions.DontRequireReceiver);
			}
		}
		#endregion
		#region input
		if(Input.GetMouseButtonDown(1)){
			if(Physics.Raycast(ray,out hit,Mathf.Infinity)){
				hit.collider.gameObject.SendMessage("RelayRightClickDown",hit,SendMessageOptions.DontRequireReceiver);
			}
		}
		#endregion
		#region input
		if(Input.GetMouseButtonUp(1)){
			if(Physics.Raycast(ray,out hit,Mathf.Infinity)){
				hit.collider.gameObject.SendMessage("RelayRightClickUp",hit,SendMessageOptions.DontRequireReceiver);
			}
		}
		#endregion
		#region input
		if(Input.GetMouseButtonDown(2)){
			if(Physics.Raycast(ray,out hit,Mathf.Infinity)){
				hit.collider.gameObject.SendMessage("RelayMiddleClickDown",hit,SendMessageOptions.DontRequireReceiver);
			}
		}
		#endregion
		#region input
		if(Input.GetMouseButtonUp(2)){
			if(Physics.Raycast(ray,out hit,Mathf.Infinity)){
				hit.collider.gameObject.SendMessage("RelayMiddleClickUp",hit,SendMessageOptions.DontRequireReceiver);
			}
		}
		#endregion
	}
}
