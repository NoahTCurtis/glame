using UnityEngine;
using System.Collections;

namespace Zarenikit.Voxels.Structure
{
	#region Bases
	/// <summary>
	/// A grid of voxels.
	/// </summary>
	public interface IGrid 
	{
		/// <summary>
		/// Gets the size of the grid in a given dimension.
		/// </summary>
		/// <returns>The size in that dimension.</returns>
		/// <param name="dim">The dimension (0=x,1=y,2=z).</param>
		int GetSize(int dim);
		/// <summary>
		/// Gets the size of the grid.
		/// </summary>
		/// <returns>The grid size.</returns>
		Vector3 GetSize();
		/// <summary>
		/// Gets the voxel at position pos.
		/// </summary>
		/// <returns>The voxel at pos.</returns>
		/// <param name="pos">The position.</param>
		IVoxel GetVoxel(Vector3 pos);
		/// <summary>
		/// Sets the voxel at position pos.
		/// </summary>
		/// /// <returns>Whether the operation was successful. Returns false if position is outside grid bounds</returns>
		/// <param name="v">Voxel to use.</param>
		/// <param name="pos">Position to set.</param>
		/// <param name="mode">What parts to set.</param>
		bool SetVoxel(IVoxel v, Vector3 pos);
		/// <summary>
		/// Initialize the grid with the specified size.
		/// </summary>
		/// <param name="X">x.</param>
		/// <param name="Y">y.</param>
		/// <param name="Z">z.</param>
		void Initialize(int X, int Y, int Z);

	}
	#endregion
	#region DataStructureSetup
	/// <summary>
	/// Inherit from this when creating new voxel structures.
	/// </summary>
	public interface IVoxel{
		//Purposefully empty. Extended differently for each algorithm type.
	}
	public interface IVoxelType{
		//Purposefully empty. Extended differently for each algorithm type.
	}
	
	#endregion
}
