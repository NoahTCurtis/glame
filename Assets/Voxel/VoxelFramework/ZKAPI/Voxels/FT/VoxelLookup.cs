using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zarenikit.Voxels.Structure;
using Zarenikit.Voxels.FaceTest;


namespace ZKAPI{

	//[System.Serializable]
	public sealed class VoxelLookup : ScriptableObject{
		[HideInInspector()]
		public VoxelType[] types;
		[HideInInspector()]
		public Texture2D atlas;
		[HideInInspector()]
		public Texture2D[] sources;
		[HideInInspector()]
		public string[] floatAttribNames;
		[HideInInspector()]
		public string[] boolAttribNames;
		public VoxelType GetVoxel(int id){
			if(id < types.Length){
				return types[id];
			}else{
				return types[0];
			}
		}
	} 

}
