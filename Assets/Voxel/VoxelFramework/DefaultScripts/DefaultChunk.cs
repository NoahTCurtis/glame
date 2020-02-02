using UnityEngine;
using System.Collections;
using ZKAPI;
[AddComponentMenu("Voxels/Default Chunk")]
public class DefaultChunk : ChunkObject {
	public VoxelLookup lookup;
	public float vSize = 2.5f;

	void Start () {
		Initialize(lookup,16,16,16,vSize);
		Generate(new VoxelRandomGenerator());
	}
}
