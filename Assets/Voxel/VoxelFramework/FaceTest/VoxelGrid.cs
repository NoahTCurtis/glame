using UnityEngine;
using System.Collections;
using Zarenikit.Voxels.Structure;

namespace Zarenikit.Voxels.FaceTest{
	public class VoxelGrid : IGrid{
		int SizeX;
		int SizeY;
		int SizeZ;
		Voxel[,,] voxels;
		public int GetSize(int dim){
			switch(dim){
			case 0:
				return SizeX;
				//break;
			case 1:
				return SizeY;
				//break;
			case 2:
				return SizeZ;
				//break;
			default:
				return 0;
				//break;
			}
		}
		/// <summary>
		/// Gets the size of the grid.
		/// </summary>
		/// <returns>The grid size.</returns>
		public Vector3 GetSize(){
			Vector3 o = new Vector3(SizeX,SizeY,SizeZ);
			return o;
		}
		/// <summary>
		/// Gets the voxel at position pos.
		/// </summary>
		/// <returns>The voxel at pos.</returns>
		/// <param name="pos">The position.</param>
		public IVoxel GetVoxel(Vector3 pos){
			if(pos.x >=0 && pos.x < voxels.GetLength(0) && pos.y >=0 && pos.y < voxels.GetLength(1) && pos.z >=0 && pos.z < voxels.GetLength(2)){
				return voxels[(int)pos.x,(int)pos.y,(int)pos.z];
			}else{
				return new Voxel(0);
			}
		}
		/// <summary>
		/// Sets the voxel at position pos.
		/// </summary>
		/// <returns>Whether the operation was successful. Returns false if position is outside grid bounds</returns>
		/// <param name="v">Voxel to use.</param>
		/// <param name="pos">Position to set.</param>
		/// <param name="mode">What parts to set.</param>
		public bool SetVoxel(IVoxel v, Vector3 pos){
			if(pos.x >=0 && pos.x < voxels.GetLength(0) && pos.y >=0 && pos.y < voxels.GetLength(1) && pos.z >=0 && pos.z < voxels.GetLength(2)){
				voxels[(int)pos.x,(int)pos.y,(int)pos.z] = (Voxel)v;
				return true;
			}else{
				return false;
			}
		}
		/// <summary>
		/// Initialize the grid with the specified size.
		/// </summary>
		/// <param name="X">x.</param>
		/// <param name="Y">y.</param>
		/// <param name="Z">z.</param>
		public void Initialize(int X, int Y, int Z){
			SizeX = X;
			SizeY = Y;
			SizeZ = Z;
			voxels = new Voxel[X,Y,Z];
		}
	}
}
