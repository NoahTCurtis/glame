using UnityEngine;
using System.Collections;
using ZKAPI;

public class BuildingVoxels : VoxelBehaviour {
	void OnLMBDown(VoxelData v){
		SetVoxel(0,v.coords);
	}
	void OnRMBDown(VoxelData v){
		SetVoxel(3,v.backCoords);
	}
}
