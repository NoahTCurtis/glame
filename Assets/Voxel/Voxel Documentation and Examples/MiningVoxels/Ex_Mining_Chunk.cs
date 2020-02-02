using UnityEngine;
using System.Collections;
using ZKAPI;

public class Ex_Mining_Chunk : ChunkObject{ //Very simple custom chunk for this example.
	public VoxelLookup lut; // Specify a lookup table to use.
	public float size = 1;
	void Awake(){
		Initialize(lut,(int)(16f/size),(int)(16f/size),(int)(16f/size),size); //Initialize the chunk as usual.
		Generate(new TriLayerSphericalGen(new Vector3(8,8,8)+transform.position,6)); //Generate the chunk.
		//Note the use of constructors for the SphericalGen generator, in order to give it a center and a radius.
	}
}



/*The generator is in here as well.
 * This is just a modified version
 * of the sphere generator in the 
 * breaking voxels example.     */
public class TriLayerSphericalGen:IVoxelGenerator{ //Generating a sphere shape with 3 layers.
	Vector3 center;
	float radius;
	public TriLayerSphericalGen(Vector3 c, float r){ //Using constructors allows you to pass in data before generation.
		center = c;
		radius = r;
	}
	public int Sample(Vector3 worldPos){ //This is the important method where you actually determine the voxels.
		if(Vector3.Distance(worldPos,center)>radius){
			return 0;
		}else{
			if(Vector3.Distance(worldPos,center)>(radius/1.5f)){
				return 3;
			}else{
				if(Vector3.Distance(worldPos,center)>(radius/3f)){
					return 1;
				}else{
					return 2;
				}
			}
		}
	}
}
