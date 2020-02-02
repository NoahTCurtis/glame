using UnityEngine;
using System.Collections;
using Zarenikit.Voxels.Structure;

namespace ZKAPI{
	public enum ChunkStatus{
		Initialize,
		Generating,
		Render,
		GridOp
	}
	public interface IVoxelGenerator{
		/// <summary>
		/// Sample the generator function at the specified position.
		/// </summary>
		/// <param name="position">Position.</param>
		int Sample(Vector3 pos);
	}
	public class VoxelRandomGenerator : IVoxelGenerator{
		public int Sample(Vector3 pos){
			return Mathf.RoundToInt (Random.Range(0,pos.y));
		}
	}
}
