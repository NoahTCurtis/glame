using UnityEngine;
using System;
using System.Collections;
using Zarenikit.Voxels.Structure;
using Zarenikit.Voxels.FaceTest;
using System.Threading;

namespace ZKAPI{
	[DisallowMultipleComponent]
	[AddComponentMenu("")]
	/// <summary>
	/// The base chunk object. Inherit from this to make a custom chunk.
	/// </summary>
	[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer),typeof(MeshCollider))]
	public class ChunkObject : MonoBehaviour {
		[Range(0.0f,0.1f)][Tooltip("Amount of texture border assumed to be padding.\nHigher values prevent black lines, but can introduce seams between textures.")]
		public float texturePadding = 0.01f;
		public bool immediatelyRemesh = false;
		[HideInInspector]
		public ChunkContainer zk_ftco_container; //Used to make chunk modification in a container much more reliable.
		#region private fields
		VoxelLookup zk_ftco_lut; // Lookup table
		VoxelGrid zk_ftco_grid; //Voxel Grid
		float zk_ftco_vxlsize = 1; // Voxel Size
		IVoxelGenerator zk_ftco_gen; // Generator
		MeshFilter zk_ftco_mfilter; // Mesh filter
		MeshCollider zk_ftco_mcollide; // Mesh collider
		MeshRenderer zk_ftco_meshrender; // Mesh renderer
		bool zk_ftco_mustRemesh; // Used for scheduled remeshing.
		bool zk_ftco_initialized;
		#endregion
		#region field getters
		public VoxelLookup Lookup(){
			return zk_ftco_lut;
		}
		public float VoxelSize(){
			return zk_ftco_vxlsize;
		}
		#endregion
		#region Chunk Operations
		public void Initialize( VoxelLookup lut, int x=16, int y=16, int z=16, float vSize = 1){
			if(vSize <= 0){
				Debug.LogWarning("CHUNK INITIALIZATION WARNING: Cannot use a zero or negative voxel size. Using a default value of 1.");
				vSize = 1;
			}
			if(x <= 0){
				Debug.LogWarning("CHUNK INITIALIZATION WARNING: Cannot use a zero or negative grid size. Using a default value of 16 for the X dimension.");
				x = 16;
			}
			if(y <= 0){
				Debug.LogWarning("CHUNK INITIALIZATION WARNING: Cannot use a zero or negative grid size. Using a default value of 16 for the Y dimension.");
				y = 16;
			}
			if(z <= 0){
				Debug.LogWarning("CHUNK INITIALIZATION WARNING: Cannot use a zero or negative grid size. Using a default value of 16 for the Z dimension.");
				z = 16;
			}
			if(lut == null){
				Debug.LogError("CHUNK INITIALIZATION ERROR: Cannot create a chunk without a lookup table.");
			}

			zk_ftco_lut = lut;
			zk_ftco_grid = new VoxelGrid();
			zk_ftco_grid.Initialize(x,y,z);
			zk_ftco_vxlsize = vSize;
			zk_ftco_mfilter = GetComponent<MeshFilter>();
			zk_ftco_mcollide = GetComponent<MeshCollider>();
			zk_ftco_meshrender = GetComponent<MeshRenderer>();
			if(zk_ftco_meshrender){
				foreach(Material m in zk_ftco_meshrender.sharedMaterials){
					m.mainTexture = lut.atlas;
					m.SetInt("_ZWrite",1);
				}
			}
			Feedback(ChunkStatus.Initialize);
			SendMessage("OnChunkInit",SendMessageOptions.DontRequireReceiver);
			zk_ftco_initialized = true;
		}
		public void SwapTables(VoxelLookup table){
			zk_ftco_lut = table;
			if(zk_ftco_meshrender){
				zk_ftco_meshrender.sharedMaterial.mainTexture = zk_ftco_lut.atlas;
			}
			Feedback(ChunkStatus.GridOp);
		}
		public void Generate(IVoxelGenerator generator, bool remesh = true){
			zk_ftco_gen = generator;
			for(int x = 0; x< zk_ftco_grid.GetSize(0); x++){
				for(int y = 0; y< zk_ftco_grid.GetSize(1); y++){
					for(int z = 0; z< zk_ftco_grid.GetSize(2); z++){
						zk_ftco_grid.SetVoxel (new Voxel((ushort)zk_ftco_gen.Sample (ChunkToWorldCoords(new Vector3(x,y,z))),zk_ftco_lut),new Vector3(x,y,z));
					}
				}
			}
			SendMessage("OnGenerate",SendMessageOptions.DontRequireReceiver);
			Feedback(ChunkStatus.Generating);
			if(remesh){
				ReMesh();
			}
		}
		public void Generate(bool remesh = true){
			for(int x = 0; x< zk_ftco_grid.GetSize(0); x++){
				for(int y = 0; y< zk_ftco_grid.GetSize(1); y++){
					for(int z = 0; z< zk_ftco_grid.GetSize(2); z++){
						if(zk_ftco_gen != null){
							zk_ftco_grid.SetVoxel (new Voxel((ushort)zk_ftco_gen.Sample (ChunkToWorldCoords(new Vector3(x,y,z))),zk_ftco_lut),new Vector3(x,y,z));
						}else{
							zk_ftco_grid.SetVoxel(new Voxel(0,zk_ftco_lut),new Vector3(x,y,z));
						}
					}
				}
			}
			SendMessage("OnGenerate",SendMessageOptions.DontRequireReceiver);
			Feedback(ChunkStatus.Generating);
			if(remesh){
				ReMesh();
			}
		}
		#endregion
		#region Grid Operations
		/// <summary>
		/// Gets the voxel at position pos.
		/// </summary>
		/// <returns>The voxel.</returns>
		/// <param name="localPos">Local position.</param>
		public int GetVoxel(Vector3 localPos){
			if(zk_ftco_container){
				return zk_ftco_container.GetVoxel(ChunkToWorldCoords(localPos));
			}else{
				return (int)((Voxel)zk_ftco_grid.GetVoxel(localPos)).id;
			}
		}
		public int GetVoxelWithin(Vector3 localPos){
			return (int)((Voxel)zk_ftco_grid.GetVoxel(localPos)).id;
		}
		/// <summary>
		/// Sets the voxel at position pos.
		/// </summary>
		/// <returns><c>true</c>, if voxel was set, <c>false</c> otherwise.</returns>
		/// <param name="v">Voxel id.</param>
		/// <param name="localPos">Local position.</param>
		public bool SetVoxel(int v, Vector3 localPos){
			if(zk_ftco_container){
				return zk_ftco_container.SetVoxel(v,ChunkToWorldCoords(localPos));
			}else{
				return zk_ftco_grid.SetVoxel(new Voxel((ushort)v,zk_ftco_lut), localPos);
			}
		}
		public bool SetVoxelWithin(int v, Vector3 localPos){
			Feedback(ChunkStatus.GridOp);
			ReMesh();
			return zk_ftco_grid.SetVoxel(new Voxel((ushort)v,zk_ftco_lut), localPos);
		}
		/// <summary>
		/// Gets the grid size in a particular dimension.
		/// </summary>
		/// <returns>The size.</returns>
		/// <param name="dim">Dimension (0=x,1=y,2=z).</param>
		public int GetSize(int dim){
			if(zk_ftco_initialized){
				return zk_ftco_grid.GetSize(dim);
			}else{
				return -1;
			}
		}
		/// <summary>
		/// Gets the grid size.
		/// </summary>
		/// <returns>The size.</returns>
		public Vector3 GetSize(){
			try{
				return zk_ftco_grid.GetSize();
			}
			catch{
				Debug.Log("Chunk not initialized. Returning 0.");
				return Vector3.zero;
			}
		}
		/// <summary>
		/// Gets the voxel instance boolean attribute.
		/// </summary>
		/// <returns>The attribute</returns>
		/// <param name="localpos">Local Position.</param>
		/// <param name="index">Index.</param>
		public bool GetVoxelInstanceBool(Vector3 localPos, int index){
			return ((Voxel)zk_ftco_grid.GetVoxel(localPos)).GetInstanceBool(index);
		}
		/// <summary>
		/// Gets the voxel instance float attribute.
		/// </summary>
		/// <returns>The attribute.</returns>
		/// <param name="localPos">Local position.</param>
		/// <param name="index">Index.</param>
		public float GetVoxelInstanceFloat(Vector3 localPos, int index){
			return ((Voxel)zk_ftco_grid.GetVoxel(localPos)).GetInstanceFloat(index);
		}
		/// <summary>
		/// Sets the voxel instance boolean attribute.
		/// </summary>
		/// <returns><c>true</c>, if voxel instance bool was set, <c>false</c> otherwise.</returns>
		/// <param name="localPos">Local position.</param>
		/// <param name="index">Index.</param>
		/// <param name="value">New value of the attribute.</param>
		public bool SetVoxelInstanceBool(Vector3 localPos, int index, bool value){
			Voxel v = (Voxel)zk_ftco_grid.GetVoxel(localPos);
			if(v.SetInstanceBool(index, value)){
				Feedback(ChunkStatus.GridOp);
				return zk_ftco_grid.SetVoxel(v,localPos);
			}else{
				return false;
			}
		}
		/// <summary>
		/// Sets the voxel instance float attribute.
		/// </summary>
		/// <returns><c>true</c>, if voxel instance float was set, <c>false</c> otherwise.</returns>
		/// <param name="localPos">Local position.</param>
		/// <param name="index">Index.</param>
		/// <param name="value">New value of the attribute.</param>
		public bool SetVoxelInstanceFloat(Vector3 localPos, int index, float value){
			Voxel v = (Voxel)zk_ftco_grid.GetVoxel(localPos);
			if(v.SetInstanceFloat(index, value)){
				Feedback(ChunkStatus.GridOp);
				return zk_ftco_grid.SetVoxel(v,localPos);
			}else{
				return false;
			}
		}
		/// <summary>
		/// Determines whether this voxel is visible the specified localPos.
		/// </summary>
		/// <returns><c>true</c> if this voxel is visible the specified localPos; otherwise, <c>false</c>.</returns>
		/// <param name="localPos">Local position.</param>
		public bool IsVisible(Vector3 localPos){
			return ((Voxel)zk_ftco_grid.GetVoxel(localPos)).IsVisible(zk_ftco_lut);
		}
		/// <summary>
		/// Determines whether this voxel is transparent the specified localPos.
		/// </summary>
		/// <returns><c>true</c> if this voxel is transparent the specified localPos; otherwise, <c>false</c>.</returns>
		/// <param name="localPos">Local position.</param>
		public bool IsTransparent(Vector3 localPos){
			return ((Voxel)zk_ftco_grid.GetVoxel(localPos)).IsTransparent(zk_ftco_lut);
		}
		/// <summary>
		/// Determines whether this voxel is solid the specified localPos.
		/// </summary>
		/// <returns><c>true</c> if this voxel is solid the specified localPos; otherwise, <c>false</c>.</returns>
		/// <param name="localPos">Local position.</param>
		public bool IsSolid(Vector3 localPos){
			return ((Voxel)zk_ftco_grid.GetVoxel(localPos)).IsSolid(zk_ftco_lut);
		}
		/// <summary>
		/// Gets the name of the voxel.
		/// </summary>
		/// <returns>The voxel name.</returns>
		/// <param name="localPos">Local position.</param>
		public string GetVoxelName(Vector3 localPos){
			return ((Voxel)zk_ftco_grid.GetVoxel(localPos)).GetName(zk_ftco_lut);
		}
		/// <summary>
		/// Gets the voxel type bool.
		/// </summary>
		/// <returns>The voxel bool attribute</returns>
		/// <param name="localpos">Local Positon.</param>
		/// <param name="index">Index.</param>
		public bool GetVoxelTypeBool(Vector3 localpos, int index){
			return ((Voxel)zk_ftco_grid.GetVoxel(localpos)).GetBool(index, zk_ftco_lut);
		}
		/// <summary>
		/// Gets the voxel type bool.
		/// </summary>
		/// <returns>The voxel bool attribute</returns>
		/// <param name="localpos">Local Positon.</param>
		/// <param name="name">Name of the attribute.</param>
		public bool GetVoxelTypeBool(Vector3 localpos, string name){
			return ((Voxel)zk_ftco_grid.GetVoxel(localpos)).GetBool(name, zk_ftco_lut);
		}
		/// <summary>
		/// Gets the voxel type float.
		/// </summary>
		/// <returns>The voxel float attribute.</returns>
		/// <param name="localpos">Local Position.</param>
		/// <param name="index">Index.</param>
		public float GetVoxelTypeFloat(Vector3 localpos, int index){
			return ((Voxel)zk_ftco_grid.GetVoxel(localpos)).GetFloat(index, zk_ftco_lut);
		}
		/// <summary>
		/// Gets the voxel type float.
		/// </summary>
		/// <returns>The voxel float attribute.</returns>
		/// <param name="localpos">Local Position.</param>
		/// <param name="name">Name of the attribute.</param>
		public float GetVoxelTypeFloat(Vector3 localpos, string name){
			return ((Voxel)zk_ftco_grid.GetVoxel(localpos)).GetFloat(name, zk_ftco_lut);
		}
		#endregion
		#region Coordinate Utilities
		/// <summary>
		/// Gets the actual size of the chunk as it compares to the world.
		/// </summary>
		/// <returns>The physical size.</returns>
		public Vector3 GetPhysicalSize(){
			return GetSize()*zk_ftco_vxlsize;
		}
		/// <summary>
		/// Convert coordinates from world to chunk space.
		/// </summary>
		/// <returns>The coordinates in chunk space.</returns>
		/// <param name="coords">Coordinates in world space.</param>
		/// <param name="round">If set to <c>true</c> round the value.</param>
		public Vector3 WorldToChunkCoords(Vector3 coords, bool round = false){
			//coords -= transform.position;
			coords = transform.InverseTransformPoint(coords);
			coords = coords/zk_ftco_vxlsize;
			if(round){
				coords.x = Mathf.Round(coords.x);
				coords.y = Mathf.Round(coords.y);
				coords.z = Mathf.Round(coords.z);
			}
			return coords;
		}
		/// <summary>
		/// Convert coordinates from chunk to world space.
		/// </summary>
		/// <returns>The coordinates in world space.</returns>
		/// <param name="coords">The coodinates in chunk space.</param>
		public Vector3 ChunkToWorldCoords(Vector3 coords){
			coords = coords*zk_ftco_vxlsize;
			//coords += transform.position;
			coords = transform.TransformPoint (coords);
			//coords = coords*zk_ftco_vxlsize;
			return coords;
		}
		#endregion
		#region Behavior
		/// <summary>
		/// Updates the mesh of the chunk immediately. It is strongly recommended to use ReMesh() instead.
		/// </summary>
		[ContextMenu ("RERENDER")]
		public void ReMeshImmediate(){
			MeshSet m = MesherFT.Render(zk_ftco_grid,zk_ftco_lut,zk_ftco_vxlsize,texturePadding);
			zk_ftco_mfilter.mesh = m.visibleMesh;
			zk_ftco_mcollide.sharedMesh = m.colliderMesh;
			Feedback(ChunkStatus.Render);
		}
		public IEnumerator ReMeshLazy(){
			MeshSet m = MesherFT.Render(zk_ftco_grid,zk_ftco_lut,zk_ftco_vxlsize,texturePadding);
			yield return new WaitForEndOfFrame();
			zk_ftco_mfilter.mesh = m.visibleMesh;
			yield return new WaitForEndOfFrame();
			zk_ftco_mcollide.sharedMesh = m.colliderMesh;
			yield return new WaitForEndOfFrame();
			Feedback(ChunkStatus.Render);
		}
		/// <summary>
		/// Updates the mesh of the chunk. Call this in order to see the changes made using SetVoxel().
		/// </summary>
		public void ReMesh(){
			zk_ftco_mustRemesh = true;
		}
		/// <summary>
		/// Unschedules the remeshing operation. Any previous calls to ReMesh() during this frame will not be performed. Nothe that this has no effect on ReMeshImmediate().
		/// </summary>
		public void AbortReMesh(){
			zk_ftco_mustRemesh = false;
		}
		/// <summary>
		/// Use to syncronize other operations with the status of the chunk.
		/// </summary>
		/// <param name="status">The status of the chunk; the reason this method was called.</param>
		protected virtual void Feedback(ChunkStatus status){
			//Unimplemented. Must be extended by user.
		}
		void LateUpdate(){
			if(zk_ftco_mustRemesh){
				//if(zk_ftco_container){
				//	zk_ftco_container.Remesharound(transform.position);
				//}
				if(immediatelyRemesh){
					ReMeshImmediate();
				}else{
					StartCoroutine(ReMeshLazy());
				}
				zk_ftco_mustRemesh = false;
			}
		}
		#endregion
		#region Debug
		void OnDrawGizmosSelected(){
			if(zk_ftco_grid != null){
				Gizmos.DrawWireCube(transform.position+((GetSize()/2)*zk_ftco_vxlsize)-(new Vector3(0.5f,0.5f,0.5f)*zk_ftco_vxlsize),GetSize()*zk_ftco_vxlsize);
			}
		}
		#endregion
		#region VoxelBehaviour relays
		public void RelayLeftClickDown(RaycastHit hit){
			VoxelData o = new VoxelData();
			o.coords = WorldToChunkCoords(hit.point-(hit.normal*0.01f), true);
			o.worldPos = ChunkToWorldCoords(o.coords);
			o.ID = GetVoxel(o.coords);
			o.backCoords = WorldToChunkCoords(hit.point+(hit.normal*0.01f), true);
			o.backWorldPos = ChunkToWorldCoords(o.backCoords);
			o.backID = GetVoxel(o.backCoords);
			SendMessage("OnLMBDown",o,SendMessageOptions.DontRequireReceiver);
		}
		public void RelayLeftClickUp(RaycastHit hit){
			VoxelData o = new VoxelData();
			o.coords = WorldToChunkCoords(hit.point-(hit.normal*0.01f), true);
			o.worldPos = ChunkToWorldCoords(o.coords);
			o.ID = GetVoxel(o.coords);
			o.backCoords = WorldToChunkCoords(hit.point+(hit.normal*0.01f), true);
			o.backWorldPos = ChunkToWorldCoords(o.backCoords);
			o.backID = GetVoxel(o.backCoords);
			SendMessage("OnLMBUp",o,SendMessageOptions.DontRequireReceiver);
		}
		public void RelayRightClickDown(RaycastHit hit){
			VoxelData o = new VoxelData();
			o.coords = WorldToChunkCoords(hit.point-(hit.normal*0.01f), true);
			o.worldPos = ChunkToWorldCoords(o.coords);
			o.ID = GetVoxel(o.coords);
			o.backCoords = WorldToChunkCoords(hit.point+(hit.normal*0.01f), true);
			o.backWorldPos = ChunkToWorldCoords(o.backCoords);
			o.backID = GetVoxel(o.backCoords);
			SendMessage("OnRMBDown",o,SendMessageOptions.DontRequireReceiver);
		}
		public void RelayRightClickUp(RaycastHit hit){
			VoxelData o = new VoxelData();
			o.coords = WorldToChunkCoords(hit.point-(hit.normal*0.01f), true);
			o.worldPos = ChunkToWorldCoords(o.coords);
			o.ID = GetVoxel(o.coords);
			o.backCoords = WorldToChunkCoords(hit.point+(hit.normal*0.01f), true);
			o.backWorldPos = ChunkToWorldCoords(o.backCoords);
			o.backID = GetVoxel(o.backCoords);
			SendMessage("OnRMBUp",o,SendMessageOptions.DontRequireReceiver);
		}
		public void RelayMiddleClickDown(RaycastHit hit){
			VoxelData o = new VoxelData();
			o.coords = WorldToChunkCoords(hit.point-(hit.normal*0.01f), true);
			o.worldPos = ChunkToWorldCoords(o.coords);
			o.ID = GetVoxel(o.coords);
			o.backCoords = WorldToChunkCoords(hit.point+(hit.normal*0.01f), true);
			o.backWorldPos = ChunkToWorldCoords(o.backCoords);
			o.backID = GetVoxel(o.backCoords);
			SendMessage("OnMMBDown",o,SendMessageOptions.DontRequireReceiver);
		}
		public void RelayMiddleClickUp(RaycastHit hit){
			VoxelData o = new VoxelData();
			o.coords = WorldToChunkCoords(hit.point-(hit.normal*0.01f), true);
			o.worldPos = ChunkToWorldCoords(o.coords);
			o.ID = GetVoxel(o.coords);
			o.backCoords = WorldToChunkCoords(hit.point+(hit.normal*0.01f), true);
			o.backWorldPos = ChunkToWorldCoords(o.backCoords);
			o.backID = GetVoxel(o.backCoords);
			SendMessage("OnMMBUp",o,SendMessageOptions.DontRequireReceiver);
		}
		#endregion
		#region VoxelBehaviour triggers
		void OnCollisionEnter(Collision col){
			foreach(ContactPoint contact in col.contacts){
				VoxelData o = new VoxelData();
				o.coords = WorldToChunkCoords(contact.point+(contact.normal*0.01f),true);
				o.worldPos = ChunkToWorldCoords(o.coords);
				o.ID = GetVoxel(o.coords);
				o.backCoords = WorldToChunkCoords(contact.point-(contact.normal*0.01f),true);
				o.backWorldPos = ChunkToWorldCoords(o.backCoords);
				o.ID = GetVoxel(o.backCoords);
				SendMessage("OnVoxelCollisionEnter",o,SendMessageOptions.DontRequireReceiver);
			}
		}
		void OnCollisionExit(Collision col){
			foreach(ContactPoint contact in col.contacts){
				VoxelData o = new VoxelData();
				o.coords = WorldToChunkCoords(contact.point+(contact.normal*0.01f),true);
				o.worldPos = ChunkToWorldCoords(o.coords);
				o.ID = GetVoxel(o.coords);
				o.backCoords = WorldToChunkCoords(contact.point-(contact.normal*0.01f),true);
				o.backWorldPos = ChunkToWorldCoords(o.backCoords);
				o.ID = GetVoxel(o.backCoords);
				SendMessage("OnVoxelCollisionExit",o,SendMessageOptions.DontRequireReceiver);
			}
		}
		#endregion
	}
}