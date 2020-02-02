using UnityEngine;
using System.Collections;
using ZKAPI;

public class SphericalGen:IVoxelGenerator{ //Generating a sphere shape
	Vector3 center;
	float radius;
	public SphericalGen(Vector3 c, float r){ //Using constructors allows you to pass in data before generation.
		center = c;
		radius = r;
	}
	public int Sample(Vector3 worldPos){ //This is the important method where you actually determine the voxels.
		if(Vector3.Distance(worldPos,center)>radius){
			return 0; //If it's outside, make it 0.
		}else{
			if(Vector3.Distance(worldPos,center)<(radius/2f)){
				return 2; //If it's very far inside, make it 2.
			}else{
				return 1; //Otherwise, if its closer to the surface, make it 1.
			}
		}
	}
}
