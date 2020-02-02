using UnityEngine;
using System.Collections;
using ZKAPI;

public class Ex_Breaking_Chunk : ChunkObject{ //Very simple custom chunk for this example
	public VoxelLookup lut; // Specify a lookup table to use.
	public float size = 1;
	void Awake(){
		Initialize(lut,(int)(16f/size),(int)(16f/size),(int)(16f/size),size); //Initialize the chunk as usual.
		Generate(new SphericalGen(new Vector3(8,8,8)+transform.position,6)); //Generate the chunk.
		//Note the use of constructors for the SphericalGen generator, in order to give it a center and a radius.
	}
}