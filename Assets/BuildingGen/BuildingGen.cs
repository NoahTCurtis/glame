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
		if (RoomCountX == 0 || RoomCountY == 0 || RoomCountZ == 0)
		{
			Debug.Log("BuildingGen got size of 0");
			yield break;
		}

		float offset = roomSize / 2.0f + pieceThickness / 2.0f;
		float size = roomSize + pieceThickness;

		for(int z = 0; z <= RoomCountZ; z++)
		{
			for(int y = 0; y <= RoomCountY; y++)
			{
				for(int x = 0; x <= RoomCountX; x++)
				{
					//GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					//cube.transform.parent = transform;
					//cube.transform.rotation = transform.rotation;
					//cube.transform.localPosition = new Vector3(offset + size * x, offset + size * y, offset + size * z);
					//yield return null;

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
						if(ShouldYield()) yield return null;
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
						if (ShouldYield()) yield return null;
					}
				}
			}
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
