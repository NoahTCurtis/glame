using UnityEngine;
using System.Collections;

namespace ZKAPI{
	[AddComponentMenu("")]
	public class VoxelBehaviour : MonoBehaviour{
		ChunkObject chunk;
		void OnChunkInit(){
			chunk = gameObject.GetComponent<ChunkObject>();
			if(chunk == null){
				enabled = false;
			}else{
				SendMessage("OnInitialize",SendMessageOptions.DontRequireReceiver);
			}
		}
		protected bool SetVoxel(int voxel, Vector3 coordinates){
			return chunk.SetVoxel(voxel,coordinates);
		}
		protected int GetVoxel(Vector3 coordinates){
			return chunk.GetVoxel(coordinates);
		}
		protected void ReMesh(){
			chunk.ReMesh();
		}
		protected void AbortReMesh(){
			chunk.AbortReMesh();
		}
		protected void ReMeshImmediate(){
			chunk.ReMeshImmediate();
		}
		protected bool GetVoxelInstanceBool(int index, Vector3 coordinates){
			return chunk.GetVoxelInstanceBool(coordinates,index);
		}
		protected float GetVoxelInstanceFloat(int index, Vector3 coordinates){
			return chunk.GetVoxelInstanceFloat(coordinates,index);
		}
		protected bool SetVoxelInstanceBool(int index, Vector3 coordinates,bool value){
			return chunk.SetVoxelInstanceBool(coordinates,index,value);
		}
		protected bool SetVoxelInstanceFloat(int index, Vector3 coordinates,float value){
			return chunk.SetVoxelInstanceFloat(coordinates,index,value);
		}
		protected bool GetVoxelTypeBool(int index, Vector3 coordinates){
			return chunk.GetVoxelTypeBool(coordinates,index);
		}
		protected float GetVoxelTypeFloat(int index, Vector3 coordinates){
			return chunk.GetVoxelTypeFloat(coordinates,index);
		}
		protected bool GetVoxelTypeBool(string name, Vector3 coordinates){
			return chunk.GetVoxelTypeBool(coordinates,name);
		}
		protected float GetVoxelTypeFloat(string name, Vector3 coordinates){
			return chunk.GetVoxelTypeFloat(coordinates,name);
		}
		protected string GetVoxelName(Vector3 coordinates){
			return chunk.GetVoxelName(coordinates);
		}
	}
}