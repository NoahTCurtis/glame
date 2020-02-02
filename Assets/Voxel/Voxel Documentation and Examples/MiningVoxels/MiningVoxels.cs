using UnityEngine;
using System.Collections;
using ZKAPI;
using Zarenikit.Voxels.FaceTest;
using Zarenikit.Voxels.Structure;

public class MiningVoxels : VoxelBehaviour {
	void OnLMBDown(VoxelData v){
		print ("Clicked voxel with damage: "+GetVoxelInstanceFloat(0,v.coords));
		if(GetVoxelInstanceFloat(0,v.coords)>GetVoxelTypeFloat(0,v.coords)){
			SetVoxel(0,v.coords);
			ReMesh();
		}else{
			switch (Ex_Mining_playertools.tool){
			case MiningExampleToolType.Hands:
				SetVoxelInstanceFloat(0,v.coords,GetVoxelInstanceFloat(0,v.coords)+1f);
				break;
			case MiningExampleToolType.Pickaxe:
				SetVoxelInstanceFloat(0,v.coords,GetVoxelInstanceFloat(0,v.coords)+GetVoxelTypeFloat(1,v.coords));
				break;
			case MiningExampleToolType.Shovel:
				SetVoxelInstanceFloat(0,v.coords,GetVoxelInstanceFloat(0,v.coords)+GetVoxelTypeFloat(2,v.coords));
				break;
			}
		}
	}
}

