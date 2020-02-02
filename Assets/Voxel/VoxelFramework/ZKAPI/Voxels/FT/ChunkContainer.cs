using UnityEngine;
using System.Collections;

namespace ZKAPI{
	[DisallowMultipleComponent]
	[AddComponentMenu("Voxels/Chunk Container")]
	public sealed class ChunkContainer : MonoBehaviour {
		public ChunkObject chunk;
		public bool generateOnStartup = true;
		public bool contiguous = true;
		public Vector3 size = Vector3.one;
		ChunkObject[,,] chunks;
		Vector3 uniformChunkScale = Vector3.one;
		bool needScale = false;

		void Start(){
			if(generateOnStartup){
				GenerateChunks ();
			}
		}
			
		
		void GenerateChunks(){
			chunks = new ChunkObject[(int)size.x,(int)size.y,(int)size.z];
			for(int x =0; x< size.x; x++){
				for(int y =0; y< size.y; y++){
					for(int z =0; z< size.z; z++){
						chunks[x,y,z] = Instantiate<GameObject>(chunk.gameObject).GetComponent<ChunkObject>();
						//yield return new WaitForEndOfFrame();
						chunks[x,y,z].zk_ftco_container = this;
						chunks[x,y,z].transform.parent = transform;
						chunks[x,y,z].name = "Chunk "+x+","+y+","+z;
					}
				}
			}
			needScale = true;
		}
		void Update(){
			if(needScale){
				uniformChunkScale.x = chunks[0,0,0].GetPhysicalSize().x*chunk.transform.lossyScale.x;
				uniformChunkScale.y = chunks[0,0,0].GetPhysicalSize().y*chunk.transform.lossyScale.y;
				uniformChunkScale.z = chunks[0,0,0].GetPhysicalSize().z*chunk.transform.lossyScale.z;
				if(uniformChunkScale != Vector3.zero){
					for(int x =0; x< size.x; x++){
						for(int y =0; y< size.y; y++){
							for(int z =0; z< size.z; z++){
								chunks[x,y,z].transform.position = transform.TransformPoint(new Vector3(x*uniformChunkScale.x,y*uniformChunkScale.y,z*uniformChunkScale.z));
								chunks[x,y,z].transform.rotation = transform.rotation;
								if(generateOnStartup){
									chunks[x,y,z].Generate();
								}
							}
						}
					}
					needScale = false;
				}
			}
		}
		public void Remesharound(Vector3 chunkPos){
			if(contiguous){
				Vector3 scalePos = chunkPos-transform.position;
				scalePos.x = Mathf.Floor(chunkPos.x/uniformChunkScale.x);
				scalePos.y = Mathf.Floor(chunkPos.y/uniformChunkScale.y);
				scalePos.z = Mathf.Floor(chunkPos.z/uniformChunkScale.z);
				if(scalePos.x >= 0 && scalePos.x < chunks.GetLength(0) && scalePos.y >= 0 && scalePos.y < chunks.GetLength(1) && scalePos.z >= 0 && scalePos.z < chunks.GetLength(2)){
					chunks[(int)scalePos.x,(int)scalePos.y,(int)scalePos.z].ReMesh();
					scalePos += Vector3.up;
					if(scalePos.x >= 0 && scalePos.x < chunks.GetLength(0) && scalePos.y >= 0 && scalePos.y < chunks.GetLength(1) && scalePos.z >= 0 && scalePos.z < chunks.GetLength(2)){
						chunks[(int)scalePos.x,(int)scalePos.y,(int)scalePos.z].ReMesh();
					}
					scalePos += Vector3.down;
					if(scalePos.x >= 0 && scalePos.x < chunks.GetLength(0) && scalePos.y >= 0 && scalePos.y < chunks.GetLength(1) && scalePos.z >= 0 && scalePos.z < chunks.GetLength(2)){
						chunks[(int)scalePos.x,(int)scalePos.y,(int)scalePos.z].ReMesh();
					}
					scalePos += Vector3.forward;
					if(scalePos.x >= 0 && scalePos.x < chunks.GetLength(0) && scalePos.y >= 0 && scalePos.y < chunks.GetLength(1) && scalePos.z >= 0 && scalePos.z < chunks.GetLength(2)){
						chunks[(int)scalePos.x,(int)scalePos.y,(int)scalePos.z].ReMesh();
					}
					scalePos += Vector3.back;
					if(scalePos.x >= 0 && scalePos.x < chunks.GetLength(0) && scalePos.y >= 0 && scalePos.y < chunks.GetLength(1) && scalePos.z >= 0 && scalePos.z < chunks.GetLength(2)){
						chunks[(int)scalePos.x,(int)scalePos.y,(int)scalePos.z].ReMesh();
					}
					scalePos += Vector3.left;
					if(scalePos.x >= 0 && scalePos.x < chunks.GetLength(0) && scalePos.y >= 0 && scalePos.y < chunks.GetLength(1) && scalePos.z >= 0 && scalePos.z < chunks.GetLength(2)){
						chunks[(int)scalePos.x,(int)scalePos.y,(int)scalePos.z].ReMesh();
					}
					scalePos += Vector3.right;
					if(scalePos.x >= 0 && scalePos.x < chunks.GetLength(0) && scalePos.y >= 0 && scalePos.y < chunks.GetLength(1) && scalePos.z >= 0 && scalePos.z < chunks.GetLength(2)){
						chunks[(int)scalePos.x,(int)scalePos.y,(int)scalePos.z].ReMesh();
					}
				}
			}
		}
		public int GetVoxel(Vector3 worldPos){
			if(contiguous){
				Vector3 scalePos = transform.InverseTransformPoint(worldPos);
				scalePos.x = Mathf.Floor(scalePos.x/uniformChunkScale.x);
				scalePos.y = Mathf.Floor(scalePos.y/uniformChunkScale.y);
				scalePos.z = Mathf.Floor(scalePos.z/uniformChunkScale.z);
				if(scalePos.x >= 0 && scalePos.x < chunks.GetLength(0) && scalePos.y >= 0 && scalePos.y < chunks.GetLength(1) && scalePos.z >= 0 && scalePos.z < chunks.GetLength(2)){
					return chunks[(int)scalePos.x,(int)scalePos.y,(int)scalePos.z].GetVoxelWithin(chunks[(int)scalePos.x,(int)scalePos.y,(int)scalePos.z].WorldToChunkCoords(worldPos,true));
				}else{
					return 0;
				}
			}else{
				int result =0;
				foreach(ChunkObject c in chunks){
					result = Mathf.Max(result,c.GetVoxelWithin(c.WorldToChunkCoords(worldPos)));
				}
				return result;
			}
		}
		public bool SetVoxel(int voxel, Vector3 worldPos){
			if(contiguous){
				Vector3 scalePos = transform.InverseTransformPoint(worldPos);
				scalePos.x = Mathf.Floor(scalePos.x/uniformChunkScale.x);
				scalePos.y = Mathf.Floor(scalePos.y/uniformChunkScale.y);
				scalePos.z = Mathf.Floor(scalePos.z/uniformChunkScale.z);
				if(scalePos.x >= 0 && scalePos.x < chunks.GetLength(0) && scalePos.y >= 0 && scalePos.y < chunks.GetLength(1) && scalePos.z >= 0 && scalePos.z < chunks.GetLength(2)){
					return chunks[(int)scalePos.x,(int)scalePos.y,(int)scalePos.z].SetVoxelWithin(voxel,chunks[(int)scalePos.x,(int)scalePos.y,(int)scalePos.z].WorldToChunkCoords(worldPos,true));
				}else{
					return false;
				}
			}else{
				bool result =false;
				foreach(ChunkObject c in chunks){
					if(c.SetVoxelWithin(voxel,c.WorldToChunkCoords(worldPos,true))){
						result = true;
					}
				}
				return result;
			}
		}

	}
}

											
