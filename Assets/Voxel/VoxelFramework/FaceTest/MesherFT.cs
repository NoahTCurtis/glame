using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ZKAPI;

namespace Zarenikit.Voxels.FaceTest{
	public static class MesherFT{
		public static MeshSet Render(VoxelGrid grid, VoxelLookup lut, float voxelSize = 1, float texpad = 0.005f){
			MeshSet m = new MeshSet();
			Mesh visible = new Mesh();
			int visvertexIndex = 0;
			List<Vector3> visverts = new List<Vector3>();
			List<int> vistris = new List<int>();
			List<int> vis2tris = new List<int>();
			List<Vector2> visuvs = new List<Vector2>();

			Mesh collide = new Mesh();
			int colvertexindex = 0;
			List<Vector3> colverts = new List<Vector3>();
			List<int> coltris = new List<int>();

			Vector3 v1 = new Vector3(voxelSize/2,voxelSize/2,voxelSize/2);
			Vector3 v2 = new Vector3(voxelSize/2,voxelSize/2,-voxelSize/2);
			Vector3 v3 = new Vector3(-voxelSize/2,voxelSize/2,-voxelSize/2);
			Vector3 v4 = new Vector3(-voxelSize/2,voxelSize/2,voxelSize/2);
			Vector3 v5 = new Vector3(voxelSize/2,-voxelSize/2,voxelSize/2);
			Vector3 v6 = new Vector3(voxelSize/2,-voxelSize/2,-voxelSize/2);
			Vector3 v7 = new Vector3(-voxelSize/2,-voxelSize/2,-voxelSize/2);
			Vector3 v8 = new Vector3(-voxelSize/2,-voxelSize/2,voxelSize/2);

			//Iterate through all voxels
			for(int x = 0; x< grid.GetSize(0); x++){
				for(int y = 0; y< grid.GetSize(1); y++){
					for(int z = 0; z< grid.GetSize(2); z++){
						//For each Voxel in the grid, with positions X,Y,and Z
						Voxel vox = (Voxel)grid.GetVoxel(new Vector3(x,y,z));
						Voxel top = (Voxel)grid.GetVoxel(new Vector3(x,y+1,z));
						Voxel bottom = (Voxel)grid.GetVoxel(new Vector3(x,y-1,z));
						Voxel front = (Voxel)grid.GetVoxel(new Vector3(x+1,y,z));
						Voxel back = (Voxel)grid.GetVoxel(new Vector3(x-1,y,z));
						Voxel left = (Voxel)grid.GetVoxel(new Vector3(x,y,z+1));
						Voxel right = (Voxel)grid.GetVoxel(new Vector3(x,y,z-1));
						Vector3 c = new Vector3(x*voxelSize,y*voxelSize,z*voxelSize);
						#region VisibleMesh
						if(vox.IsVisible(lut)&&!vox.IsTransparent(lut)){
							//Start Rendering
							Rect uv = vox.GetTexCoords(lut);

							if(top.IsTransparent(lut)){
								visvertexIndex=visverts.Count;
								visverts.Add(v1+c);
								visverts.Add(v2+c);
								visverts.Add(v3+c);
								visverts.Add(v4+c);
								//tri1
								vistris.Add(visvertexIndex);
								vistris.Add(visvertexIndex+1);
								vistris.Add(visvertexIndex+3);
								//tri2
								vistris.Add(visvertexIndex+1);
								vistris.Add(visvertexIndex+2);
								vistris.Add(visvertexIndex+3);
								//uvs
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));
							
							}
							if(bottom.IsTransparent(lut)){
								visvertexIndex=visverts.Count;
								visverts.Add(v5+c);
								visverts.Add(v6+c);
								visverts.Add(v7+c);
								visverts.Add(v8+c);
								//tri1
								vistris.Add(visvertexIndex+3);
								vistris.Add(visvertexIndex+1);
								vistris.Add(visvertexIndex);
								//tri2
								vistris.Add(visvertexIndex+3);
								vistris.Add(visvertexIndex+2);
								vistris.Add(visvertexIndex+1);
								//uvs
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));

							}
							if(front.IsTransparent(lut)){
								visvertexIndex=visverts.Count;
								visverts.Add(v2+c);
								visverts.Add(v1+c);
								visverts.Add(v6+c);
								visverts.Add(v5+c);
								//tri1
								vistris.Add(visvertexIndex);
								vistris.Add(visvertexIndex+1);
								vistris.Add(visvertexIndex+3);
								//tri2
								vistris.Add(visvertexIndex);
								vistris.Add(visvertexIndex+3);
								vistris.Add(visvertexIndex+2);
								//uvs
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));


							}
							if(back.IsTransparent(lut)){
								visvertexIndex=visverts.Count;
								visverts.Add(v3+c);
								visverts.Add(v4+c);
								visverts.Add(v7+c);
								visverts.Add(v8+c);
								//tri1
								vistris.Add(visvertexIndex+3);
								vistris.Add(visvertexIndex+1);
								vistris.Add(visvertexIndex);
								//tri2
								vistris.Add(visvertexIndex+2);
								vistris.Add(visvertexIndex+3);
								vistris.Add(visvertexIndex);
								//uvs
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));

							}
							if(left.IsTransparent(lut)){
								visvertexIndex=visverts.Count;
								visverts.Add(v1+c);
								visverts.Add(v4+c);
								visverts.Add(v5+c);
								visverts.Add(v8+c);
								//tri1
								vistris.Add(visvertexIndex);
								vistris.Add(visvertexIndex+1);
								vistris.Add(visvertexIndex+3);
								//tri2
								vistris.Add(visvertexIndex);
								vistris.Add(visvertexIndex+3);
								vistris.Add(visvertexIndex+2);
								//uvs
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));


								
							}
							if(right.IsTransparent(lut)){
								visvertexIndex=visverts.Count;
								visverts.Add(v2+c);
								visverts.Add(v3+c);
								visverts.Add(v6+c);
								visverts.Add(v7+c);
								//tri1
								vistris.Add(visvertexIndex+3);
								vistris.Add(visvertexIndex+1);
								vistris.Add(visvertexIndex);
								//tri2
								vistris.Add(visvertexIndex+2);
								vistris.Add(visvertexIndex+3);
								vistris.Add(visvertexIndex);
								//uvs
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));


								
							}

						}
						#endregion
						#region VisibleTransparentMesh
						if(vox.IsVisible(lut)&&vox.IsTransparent(lut)){
							//Start Rendering
							Rect uv = vox.GetTexCoords(lut);
							
							if(top.IsTransparent(lut) && top.id != vox.id){
								visvertexIndex=visverts.Count;
								visverts.Add(v1+c);
								visverts.Add(v2+c);
								visverts.Add(v3+c);
								visverts.Add(v4+c);
								//tri1
								vis2tris.Add(visvertexIndex);
								vis2tris.Add(visvertexIndex+1);
								vis2tris.Add(visvertexIndex+3);
								//tri2
								vis2tris.Add(visvertexIndex+1);
								vis2tris.Add(visvertexIndex+2);
								vis2tris.Add(visvertexIndex+3);
								//uvs
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));
								//Doublesided meshing
								if(vox.IsDoubleSided(lut)){
									visvertexIndex=visverts.Count;
									visverts.Add(v1+c);
									visverts.Add(v2+c);
									visverts.Add(v3+c);
									visverts.Add(v4+c);
									//tri1 flipped
									vis2tris.Add(visvertexIndex+3);
									vis2tris.Add(visvertexIndex+1);
									vis2tris.Add(visvertexIndex);
									//tri2 flipped
									vis2tris.Add(visvertexIndex+3);
									vis2tris.Add(visvertexIndex+2);
									vis2tris.Add(visvertexIndex+1);
									//uvs
									visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
									visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
									visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
									visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));
								}
							}
							if(bottom.IsTransparent(lut) && bottom.id != vox.id){
								visvertexIndex=visverts.Count;
								visverts.Add(v5+c);
								visverts.Add(v6+c);
								visverts.Add(v7+c);
								visverts.Add(v8+c);
								//tri1
								vis2tris.Add(visvertexIndex+3);
								vis2tris.Add(visvertexIndex+1);
								vis2tris.Add(visvertexIndex);
								//tri2
								vis2tris.Add(visvertexIndex+3);
								vis2tris.Add(visvertexIndex+2);
								vis2tris.Add(visvertexIndex+1);
								//uvs
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));
								//Doublesided meshing
								if(vox.IsDoubleSided(lut)){
									visvertexIndex=visverts.Count;
									visverts.Add(v5+c);
									visverts.Add(v6+c);
									visverts.Add(v7+c);
									visverts.Add(v8+c);
									//tri1 flipped
									vis2tris.Add(visvertexIndex);
									vis2tris.Add(visvertexIndex+1);
									vis2tris.Add(visvertexIndex+3);
									//tri2 flipped
									vis2tris.Add(visvertexIndex+1);
									vis2tris.Add(visvertexIndex+2);
									vis2tris.Add(visvertexIndex+3);
									//uvs
									visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
									visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
									visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
									visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));
								}
								
							}
							if(front.IsTransparent(lut) && front.id != vox.id){
								visvertexIndex=visverts.Count;
								visverts.Add(v2+c);
								visverts.Add(v1+c);
								visverts.Add(v6+c);
								visverts.Add(v5+c);
								//tri1
								vis2tris.Add(visvertexIndex);
								vis2tris.Add(visvertexIndex+1);
								vis2tris.Add(visvertexIndex+3);
								//tri2
								vis2tris.Add(visvertexIndex);
								vis2tris.Add(visvertexIndex+3);
								vis2tris.Add(visvertexIndex+2);
								//uvs
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));
								//Doublesided meshing
								if(vox.IsDoubleSided(lut)){
									visvertexIndex=visverts.Count;
									visverts.Add(v2+c);
									visverts.Add(v1+c);
									visverts.Add(v6+c);
									visverts.Add(v5+c);
									//tri1 flipped
									vis2tris.Add(visvertexIndex+3);
									vis2tris.Add(visvertexIndex+1);
									vis2tris.Add(visvertexIndex);
									//tri2 flipped
									vis2tris.Add(visvertexIndex+2);
									vis2tris.Add(visvertexIndex+3);
									vis2tris.Add(visvertexIndex);
									//uvs
									visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
									visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
									visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
									visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));
								}
								
							}
							if(back.IsTransparent(lut) && back.id != vox.id){
								visvertexIndex=visverts.Count;
								visverts.Add(v3+c);
								visverts.Add(v4+c);
								visverts.Add(v7+c);
								visverts.Add(v8+c);
								//tri1
								vis2tris.Add(visvertexIndex+3);
								vis2tris.Add(visvertexIndex+1);
								vis2tris.Add(visvertexIndex);
								//tri2
								vis2tris.Add(visvertexIndex+2);
								vis2tris.Add(visvertexIndex+3);
								vis2tris.Add(visvertexIndex);
								//uvs
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));
								//Doublesided meshing
								if(vox.IsDoubleSided(lut)){
									visvertexIndex=visverts.Count;
									visverts.Add(v3+c);
									visverts.Add(v4+c);
									visverts.Add(v7+c);
									visverts.Add(v8+c);
									//tri1 flipped
									vis2tris.Add(visvertexIndex);
									vis2tris.Add(visvertexIndex+1);
									vis2tris.Add(visvertexIndex+3);
									//tri2 flipped
									vis2tris.Add(visvertexIndex);
									vis2tris.Add(visvertexIndex+3);
									vis2tris.Add(visvertexIndex+2);
									//uvs
									visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
									visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
									visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
									visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));
								}
							}
							if(left.IsTransparent(lut) && left.id != vox.id){
								visvertexIndex=visverts.Count;
								visverts.Add(v1+c);
								visverts.Add(v4+c);
								visverts.Add(v5+c);
								visverts.Add(v8+c);
								//tri1
								vis2tris.Add(visvertexIndex);
								vis2tris.Add(visvertexIndex+1);
								vis2tris.Add(visvertexIndex+3);
								//tri2
								vis2tris.Add(visvertexIndex);
								vis2tris.Add(visvertexIndex+3);
								vis2tris.Add(visvertexIndex+2);
								//uvs
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));
								//Doublesided meshing
								if(vox.IsDoubleSided(lut)){
									visvertexIndex=visverts.Count;
									visverts.Add(v1+c);
									visverts.Add(v4+c);
									visverts.Add(v5+c);
									visverts.Add(v8+c);
									//tri1 flipped
									vis2tris.Add(visvertexIndex+3);
									vis2tris.Add(visvertexIndex+1);
									vis2tris.Add(visvertexIndex);
									//tri2 flipped
									vis2tris.Add(visvertexIndex+2);
									vis2tris.Add(visvertexIndex+3);
									vis2tris.Add(visvertexIndex);
									//uvs
									visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
									visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
									visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
									visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));
								}
								
								
							}
							if(right.IsTransparent(lut) && right.id != vox.id){
								visvertexIndex=visverts.Count;
								visverts.Add(v2+c);
								visverts.Add(v3+c);
								visverts.Add(v6+c);
								visverts.Add(v7+c);
								//tri1
								vis2tris.Add(visvertexIndex+3);
								vis2tris.Add(visvertexIndex+1);
								vis2tris.Add(visvertexIndex);
								//tri2
								vis2tris.Add(visvertexIndex+2);
								vis2tris.Add(visvertexIndex+3);
								vis2tris.Add(visvertexIndex);
								//uvs
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
								visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
								visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));
								//Doublesided meshing
								if(vox.IsDoubleSided(lut)){
									visvertexIndex=visverts.Count;
									visverts.Add(v2+c);
									visverts.Add(v3+c);
									visverts.Add(v6+c);
									visverts.Add(v7+c);
									//tri1 flipped
									vis2tris.Add(visvertexIndex);
									vis2tris.Add(visvertexIndex+1);
									vis2tris.Add(visvertexIndex+3);
									//tri2 flipped
									vis2tris.Add(visvertexIndex);
									vis2tris.Add(visvertexIndex+3);
									vis2tris.Add(visvertexIndex+2);
									//uvs
									visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+uv.height-texpad));
									visuvs.Add(new Vector2(uv.x+texpad,uv.y+uv.height-texpad));
									visuvs.Add(new Vector2(uv.x+uv.width-texpad,uv.y+texpad));
									visuvs.Add(new Vector2(uv.x+texpad,uv.y+texpad));
								}
								
								
							}
							
						}
						#endregion
						#region ColliderMesh
						if(vox.IsSolid(lut)){
							//Start Rendering
							if(!top.IsSolid(lut)){
								colvertexindex=colverts.Count;
								colverts.Add(v1+c);
								colverts.Add(v2+c);
								colverts.Add(v3+c);
								colverts.Add(v4+c);
								//tri1
								coltris.Add(colvertexindex);
								coltris.Add(colvertexindex+1);
								coltris.Add(colvertexindex+3);
								//tri2
								coltris.Add(colvertexindex+1);
								coltris.Add(colvertexindex+2);
								coltris.Add(colvertexindex+3);
							}
							if(!bottom.IsSolid(lut)){
								colvertexindex=colverts.Count;
								colverts.Add(v5+c);
								colverts.Add(v6+c);
								colverts.Add(v7+c);
								colverts.Add(v8+c);
								//tri1
								coltris.Add(colvertexindex+3);
								coltris.Add(colvertexindex+1);
								coltris.Add(colvertexindex);
								//tri2
								coltris.Add(colvertexindex+3);
								coltris.Add(colvertexindex+2);
								coltris.Add(colvertexindex+1);
								
							}
							if(!front.IsSolid(lut)){
								colvertexindex=colverts.Count;
								colverts.Add(v2+c);
								colverts.Add(v1+c);
								colverts.Add(v6+c);
								colverts.Add(v5+c);
								//tri1
								coltris.Add(colvertexindex);
								coltris.Add(colvertexindex+1);
								coltris.Add(colvertexindex+3);
								//tri2
								coltris.Add(colvertexindex);
								coltris.Add(colvertexindex+3);
								coltris.Add(colvertexindex+2);
								
							}
							if(!back.IsSolid(lut)){
								colvertexindex=colverts.Count;
								colverts.Add(v3+c);
								colverts.Add(v4+c);
								colverts.Add(v7+c);
								colverts.Add(v8+c);
								//tri1
								coltris.Add(colvertexindex+3);
								coltris.Add(colvertexindex+1);
								coltris.Add(colvertexindex);
								//tri2
								coltris.Add(colvertexindex+2);
								coltris.Add(colvertexindex+3);
								coltris.Add(colvertexindex);
								
							}
							if(!left.IsSolid(lut)){
								colvertexindex=colverts.Count;
								colverts.Add(v1+c);
								colverts.Add(v4+c);
								colverts.Add(v5+c);
								colverts.Add(v8+c);
								//tri1
								coltris.Add(colvertexindex);
								coltris.Add(colvertexindex+1);
								coltris.Add(colvertexindex+3);
								//tri2
								coltris.Add(colvertexindex);
								coltris.Add(colvertexindex+3);
								coltris.Add(colvertexindex+2);
								
							}
							if(!right.IsSolid(lut)){
								colvertexindex=colverts.Count;
								colverts.Add(v2+c);
								colverts.Add(v3+c);
								colverts.Add(v6+c);
								colverts.Add(v7+c);
								//tri1
								coltris.Add(colvertexindex+3);
								coltris.Add(colvertexindex+1);
								coltris.Add(colvertexindex);
								//tri2
								coltris.Add(colvertexindex+2);
								coltris.Add(colvertexindex+3);
								coltris.Add(colvertexindex);
								
								
							}
							
						}
						#endregion
					}
				}
			}

			visible.vertices = visverts.ToArray();
			visible.subMeshCount = 2;
			visible.SetTriangles(vis2tris.ToArray(),1);
			visible.SetTriangles(vistris.ToArray(),0);
			visible.uv = visuvs.ToArray();
			//visible.RecalculateBounds();
			visible.RecalculateNormals();

			collide.vertices = colverts.ToArray();
			collide.triangles = coltris.ToArray();
			//collide.RecalculateBounds();
			collide.RecalculateNormals();

			m.visibleMesh = visible;
			m.colliderMesh = collide;

			return m;
		}
	}
	public class MeshSet{
		public Mesh visibleMesh;
		public Mesh colliderMesh;
	}
}
