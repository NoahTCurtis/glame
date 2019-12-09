using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGen : MonoBehaviour
{
	//how many rooms
	public int RoomCountX;
	public int RoomCountY;
	public int RoomCountZ;
	public int PiecesPerFrame = 1;
	public bool PressBToActivate = false;
	public bool StructureCanCollapse = false;

	[Header("Wall")]
	public GameObject WallPrefab; //Z-forward by default. Gotta spin it 

	[Header("Floor")]
	public GameObject FloorPrefab;

	private float pieceThickness = 0.5f;
	private float roomSize = 5.0f;

	private Quaternion deg90x = Quaternion.Euler(90, 0, 0);
	private Quaternion deg90y = Quaternion.Euler(0, 90, 0);
	private Quaternion deg90z = Quaternion.Euler(0, 0, 90);

	private bool activated = false;

	//3d list XYZ of lists of gameobjects that make up a room
	private List<List<List<List<GameObject>>>> roomRefs = new List<List<List<List<GameObject>>>>();

	private void Update()
	{
		if((Input.GetKeyDown(KeyCode.B) || !PressBToActivate) && !activated)
		{
			activated = true;
			StartCoroutine(Generate());
		}
	}

	private IEnumerator Generate()
	{
		if (RoomCountX <= 0 || RoomCountY <= 0 || RoomCountZ <= 0)
		{
			Debug.Log("BuildingGen got size < 0");
			yield break;
		}

		//reserve memory for room references list
		for (int z = 0; z < RoomCountZ; z++)
		{
			roomRefs.Add(new List<List<List<GameObject>>>());
			for (int y = 0; y < RoomCountY * 2 + 1; y++)
			{
				roomRefs[z].Add(new List<List<GameObject>>());
				for (int x = 0; x < RoomCountX; x++)
				{
					roomRefs[z][y].Add(new List<GameObject>());
				}
			}
		}

		float offset = roomSize / 2.0f + pieceThickness / 2.0f;
		float size = roomSize + pieceThickness;

		for (int z = 0; z <= RoomCountZ; z++)
		{
			for (int y = 0; y <= RoomCountY; y++)
			{
				for (int x = 0; x <= RoomCountX; x++)
				{
					bool shapeCorrect;
					int depth;
					bool internalPiece;
					bool randomNotHoles;

					shapeCorrect = (y != RoomCountY && z != RoomCountZ);
					depth = Mathf.Min(x, RoomCountX - x);
					internalPiece = depth > 0;
					randomNotHoles = Random.Range(0, 2) != 0;

					if (shapeCorrect && (!internalPiece || randomNotHoles))
					{
						GameObject wallX = GameObject.Instantiate(WallPrefab, transform);
						wallX.transform.localRotation = deg90y;
						wallX.transform.localPosition = new Vector3(size * x, offset + size * y, offset + size * z);

						AddRoomRef(wallX, x, y, z, "x");

						if (ShouldYield()) yield return null;
					}

					shapeCorrect = (x != RoomCountX && z != RoomCountZ);
					depth = Mathf.Min(y, RoomCountY - y);
					internalPiece = depth > 0;
					randomNotHoles = Random.Range(0, 6) != 0;

					if (shapeCorrect && (!internalPiece || randomNotHoles))
					{
						GameObject floor = GameObject.Instantiate(FloorPrefab, transform);
						floor.transform.localRotation = deg90x;
						floor.transform.localPosition = new Vector3(offset + size * x, size * y, offset + size * z);

						AddRoomRef(floor, x, y, z, "y");

						if (ShouldYield()) yield return null;
					}

					shapeCorrect = (x != RoomCountX && y != RoomCountY);
					depth = Mathf.Min(z, RoomCountZ - z);
					internalPiece = depth > 0;
					randomNotHoles = Random.Range(0, 2) != 0;

					if (shapeCorrect && (!internalPiece || randomNotHoles))
					{
						GameObject wallZ = GameObject.Instantiate(WallPrefab, transform);
						wallZ.transform.localRotation = Quaternion.identity;
						wallZ.transform.localPosition = new Vector3(offset + size * x, offset + size * y, size * z);

						AddRoomRef(wallZ, x, y, z, "z");

						if (ShouldYield()) yield return null;
					}
				}
			}
		}

		//add collapse-support connections
		if (StructureCanCollapse)
		{
			for (int z = 0; z < roomRefs.Count; z++)
			{
				for (int y = 0; y < roomRefs[z].Count; y++)
				{
					for (int x = 0; x < roomRefs[z][y].Count; x++)
					{
						if (y == 0) continue;

						var thisRoom = roomRefs[z][y][x];
						var roomBelow = roomRefs[z][y-1][x];
						Debug.Log("[" + x + "," + y + "," + z + "] (" + thisRoom.Count + " pieces)");

						foreach(var pieceAbove in thisRoom)
						{
							foreach(var piecebelow in roomBelow)
							{
								StructurePiece child = piecebelow.GetComponent<StructurePiece>();
								StructurePiece parent = pieceAbove.GetComponent<StructurePiece>();
								StructurePiece.AddSupport(child, parent);
							}
						}
					}
				}
			}
		}
	}

	private void AddRoomRef(GameObject obj, int x, int y, int z, string direction)
	{
		if (x < 0 || y < 0 || z < 0) return;
		if (x > RoomCountX || y > RoomCountY || z > RoomCountZ) return;

		int realY;
		switch(direction)
		{
			case "x":
				realY = y * 2 + 1;
				if (x < RoomCountX) roomRefs[z][realY][x].Add(obj);
				if (x > 0)           roomRefs[z][realY][x-1].Add(obj);
				break;

			case "z":
				realY = y * 2 + 1;
				if (z < RoomCountZ) roomRefs[z][realY][x].Add(obj);
				if (z > 0)          roomRefs[z-1][realY][x].Add(obj);
				break;

			case "y": // floor/ceiling
				realY = y * 2;
				if (y <= RoomCountY) roomRefs[z][realY][x].Add(obj);
				break;
		}
	}

	private int piecesSoFarThisFrame = 0;
	bool ShouldYield()
	{
		piecesSoFarThisFrame++;
		if (piecesSoFarThisFrame >= PiecesPerFrame)
		{
			piecesSoFarThisFrame = 0;
			return true;
		}
		else return false;
	}
}
