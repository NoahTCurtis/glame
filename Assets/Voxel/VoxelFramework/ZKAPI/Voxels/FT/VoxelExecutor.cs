using UnityEngine;
using System.Collections;

namespace ZKAPI{
	public struct VoxelData{
		public int ID;
		public Vector3 worldPos;
		public Vector3 coords;
		public Vector3 backWorldPos;
		public Vector3 backCoords;
		public int backID;
	}
}
