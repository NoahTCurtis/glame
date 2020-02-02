using UnityEngine;
using System.Collections;
using ZKAPI;

public class BreakingVoxels : VoxelBehaviour {
	public GameObject replacement; // The replacement object to be created.

	void OnVoxelCollisionEnter(VoxelData v){ //This is called whenever a voxel is hit.
		if(GetVoxelTypeBool(0,v.coords)){ //Uses voxel type attributes to determine what can break.
			                              //Attribute 0 is the 'breakable' property in this example.
			SetVoxel(0,v.coords);// Sets the voxel at the hit voxel's position to 0.
			ReMesh();// Ensures that the chunk's mesh is updated.
			if(replacement){
				Instantiate(replacement,v.worldPos,Quaternion.identity); // makes the replacement object
			}
		}
	}

}
