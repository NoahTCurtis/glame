using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zarenikit.Voxels.Structure;
using ZKAPI;

namespace Zarenikit.Voxels.FaceTest{
	[System.Serializable]
	public class VoxelType : IVoxelType{
		public string name = "UNNAMED_VOXEL";
		public bool trans;
		public bool visible;
		public bool doubleside;
		public bool solid;
		//public Color32 color;
		public List<float> floatAttribs;
		public List<bool> boolAttribs;
		public Rect texCoords;
		public int instfloats;
		public int instbools;

		public VoxelType(){
			floatAttribs = new List<float>();
			boolAttribs = new List<bool>();
		}
	}

	public struct Voxel : IVoxel{
		public ushort id;
		public bool[] instbools;
		public float[] instfloats;
		public Voxel(ushort vid){
			id = vid;
			instbools = new bool[0];
			instfloats = new float[0];
		}
		public Voxel(ushort vid, VoxelLookup lut){
			id = vid;
			instbools = new bool[lut.GetVoxel(vid).instbools];
			instfloats = new float[lut.GetVoxel(vid).instfloats];
		}
		public Voxel(ushort vid, bool[] bools, float[] floats){
			id = vid;
			instbools = bools;
			instfloats = floats;
		}
		public static implicit operator Voxel(int index){
			return new Voxel((ushort)index);
		}
		public static implicit operator int(Voxel voxel){
			return voxel.id;
		}
		public bool IsVisible(VoxelLookup lut){
			return lut.GetVoxel(id).visible;
		}
		public bool IsSolid(VoxelLookup lut){
			return lut.GetVoxel(id).solid;
		}
		public bool IsDoubleSided(VoxelLookup lut){
			return lut.GetVoxel(id).doubleside;
		}
		public bool IsTransparent(VoxelLookup lut){
			return lut.GetVoxel(id).trans;
		}
		public string GetName(VoxelLookup lut){
			return lut.GetVoxel(id).name;
		}
		/*public Color32 GetColor(VoxelLookup lut){
			return lut.GetVoxel(id).color;
		}*/
		public Rect GetTexCoords(VoxelLookup lut){
			return lut.GetVoxel(id).texCoords;
		}
		public bool GetBool(int index, VoxelLookup lut){
			if(index<lut.GetVoxel(id).boolAttribs.Count && index>=0){
				return lut.GetVoxel(id).boolAttribs[index];
			}else{
				return false;
			}
		}
		public bool GetBool(string name, VoxelLookup lut){
			for(int i =0; i<lut.boolAttribNames.Length; i++){
				if(lut.boolAttribNames[i]==name){
					return lut.GetVoxel(id).boolAttribs[i];
				}
			}
			return false;
		}
		public float GetFloat(string name, VoxelLookup lut){
			for(int i =0; i<lut.floatAttribNames.Length; i++){
				if(lut.floatAttribNames[i]==name){
					return lut.GetVoxel(id).floatAttribs[i];
				}
			}
			return 0;
		}
		public float GetFloat(int index, VoxelLookup lut){
			if(index<lut.GetVoxel(id).floatAttribs.Count && index >=0){
				return lut.GetVoxel(id).floatAttribs[index];
			}else{
				return 0;
			}
		}
		public bool[] GetInstanceBools(){
			return instbools;
		}
		public void SetInstanceBools(bool[] bools){
			instbools = bools;
		}
		public bool GetInstanceBool(int index){
			if(index<instbools.Length){
				return instbools[index];
			}else{
				return false;
			}
		}
		public bool SetInstanceBool(int index, bool item){
			if(index<instbools.Length){
				instbools[index] = item;
				return true;
			}else{
				return false;
			}
		}

		public float[] GetInstanceFloats(){
			return instfloats;
		}
		public void SetInstanceFloats(float[] floats){
			instfloats = floats;
		}
		public float GetInstanceFloat(int index){
			if(index<instfloats.Length){
				return instfloats[index];
			}else{
				return 0;
			}
		}
		public bool SetInstanceFloat(int index, float item){
			if(index<instfloats.Length){
				instfloats[index] = item;
				return true;
			}else{
				return false;
			}
		}
	}

}
