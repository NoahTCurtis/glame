﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GK {
	public delegate void BreakEvent(BreakableByGBE other);

	public class BreakableByGBE : MonoBehaviour
	{
		public event BreakEvent OnBreakEvent;

		public MeshFilter Filter     { get; private set; }
		public MeshRenderer Renderer { get; private set; }
		public MeshCollider Collider { get; private set; }
		public Rigidbody Rigidbody   { get; private set; }

		public List<Vector2> PolygonPoints;
		private float Thickness = 1.0f;
		private float MinBreakArea = 0.01f;

		public List<BreakableByGBE> ParentsToAdd;
		protected Dictionary<BreakableByGBE, bool> Parents = new Dictionary<BreakableByGBE, bool>();
		protected Dictionary<BreakableByGBE, bool> Children = new Dictionary<BreakableByGBE, bool>();
		protected Vector3 originalScale; //used for UVs


		float? _Area = null;
		public float Area {
			get {
				if (_Area == null) {
					_Area = Mathf.Abs(Geom.Area(PolygonPoints));
				}

				return _Area ?? 1.0f;
			}
		}

		void Start()
		{
			Reload();

			foreach (var parent in ParentsToAdd)
				AddSupport(this, parent);
		}

		private void OnDestroy()
		{
			SendBreakEvent();
		}

		public static void AddSupport(BreakableByGBE child, BreakableByGBE parent)
		{
			if (child == null || parent == null) return;
			if (child?.GetComponent<Rigidbody>() != null || parent?.GetComponent<Rigidbody>() != null) return;

			parent.Children.Add(child, true);
			child.Parents.Add(parent, true);
			parent.OnBreakEvent += child.OnParentBroken;
			child.OnBreakEvent += parent.OnChildBroken;

			//Debug.Log("BBGBE: " + child.name + " supports " + parent.name);
		}

		public void OnParentBroken(BreakableByGBE parent)
		{
			Debug.Assert(Parents.ContainsKey(parent), "Child didn't know about parent");
			//Debug.Log("BBGBE: " + name + "'s parent (" + parent.name + ") broke");

			this.OnBreakEvent -= parent.OnChildBroken;
			parent.OnBreakEvent -= this.OnParentBroken;
			Parents.Remove(parent);

		}

		public void OnChildBroken(BreakableByGBE child)
		{
			Debug.Assert(Children.ContainsKey(child), "Parent didn't know about child");
			//Debug.Log("BBGBE: " + name + "'s child (" + child.name + ") broke");

			this.OnBreakEvent -= child.OnParentBroken;
			child.OnBreakEvent -= this.OnChildBroken;
			Children.Remove(child);

			if (Children.Count == 0 && GetComponent<Rigidbody>() == null)
			{
				Debug.Log("BBGBE: " + name + " has no more supports");
				gameObject.AddComponent<Rigidbody>();
				SendBreakEvent();
			}
		}

		private void SendBreakEvent()
		{
			//Debug.Log("BBGBE: " + name + " broke");
			OnBreakEvent?.Invoke(this);
		}

		public void Reload()
		{
			if (Filter == null) Filter = GetComponent<MeshFilter>();
			if (Renderer == null) Renderer = GetComponent<MeshRenderer>();
			if (Collider == null) Collider = GetComponent<MeshCollider>();
			if (Rigidbody == null) Rigidbody = GetComponent<Rigidbody>();

			Mesh mesh;
			if (PolygonPoints.Count == 0) //if this is an original, uncut piece
			{
				// Assume it's a cube with localScale dimensions
				var scale = 0.5f * transform.localScale;

				PolygonPoints.Add(new Vector2(-scale.x, -scale.y));
				PolygonPoints.Add(new Vector2(scale.x, -scale.y));
				PolygonPoints.Add(new Vector2(scale.x, scale.y));
				PolygonPoints.Add(new Vector2(-scale.x, scale.y));

				Thickness = 2.0f * scale.z;

				originalScale = transform.localScale;
				transform.localScale = Vector3.one;
			}

			mesh = MeshFromPolygon(PolygonPoints, Thickness);

			Filter.sharedMesh = mesh;
			Collider.sharedMesh = mesh;
		}

		void Update() {
			if (transform.position.y < -1000.0f) {
				Destroy(gameObject);
			}
		}

		static float NormalizedRandom(float mean, float stddev) {
			var u1 = UnityEngine.Random.value;
			var u2 = UnityEngine.Random.value;

			var randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
				Mathf.Sin(2.0f * Mathf.PI * u2);

			return mean + stddev * randStdNormal;
		}

		public void Break(Vector3 position, float holeRadius)
		{
			Break((Vector2)transform.InverseTransformPoint(position), holeRadius);
		}

		public void Break(Ray ray, float holeRadius)
		{
			//find contact point
			Plane plane = new Plane(transform.forward, transform.position);
			float distance;
			if (plane.Raycast(ray, out distance) == false) return; //quit on coplanar ray
			Vector2 position = transform.InverseTransformPoint(ray.GetPoint(distance)); //center of the beam's hole

			//find angles between ray and plane
			//float tiltAngle = (Mathf.PI / 2.0f) - Mathf.Acos(Mathf.Abs(Vector3.Dot(ray.direction, plane.normal) / (ray.direction.magnitude * plane.normal.magnitude)));
			float tiltAngle = (Mathf.PI / 2.0f) - Mathf.Acos(Mathf.Abs(Vector3.Dot(ray.direction, plane.normal))); //assume ray direction and plane normal are normalized
			float tiltExpansion = 1.0f + Mathf.Min(Mathf.Abs(Mathf.Tan(tiltAngle + Mathf.PI / 2.0f)), 99.0f);
			tiltExpansion = Mathf.Max(1.0f, tiltExpansion);
			tiltExpansion = 1.0f + (tiltExpansion - 1.0f) / 2.0f; //this line tries to reduce the expansion by 50% but it feels sloppy.

			//find hole rotation angle using the dumbest hack of all time
			GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			obj.transform.LookAt(obj.transform.position + plane.normal, ray.direction);
			obj.transform.localScale = Vector3.one;
			obj.transform.SetParent(transform, true);
			float holeRotation = obj.transform.localRotation.eulerAngles.z * Mathf.Deg2Rad;
			holeRotation = (Mathf.PI / 2.0f) - holeRotation;
			GameObject.DestroyImmediate(obj);

			//find angle to rotate the hole so the ellipse lines up correctly
			//V||P = P × (V×P / |P|) / |P| = Px(VxP)?
			//V:vector, P:normal of plane
			/*
			Vector2 rayDirOnPlane = Vector3.Cross(plane.normal, Vector3.Cross(ray.direction, plane.normal));
			Vector2 upDirOnPlane = Vector3.Cross(plane.normal, Vector3.Cross(transform.up, plane.normal));
			float holeRotation = Vector2.Angle(rayDirOnPlane, upDirOnPlane) * Mathf.Deg2Rad;
			//holeRotation = holeRotation + (Mathf.PI / 2.0f);
			//*/

			//begin break protocol
			var area = Area;
			if (area < MinBreakArea)
			{
				Debug.Log("Area too small: " + area);
				Destroy(gameObject);
				return;
			}

			var calc = new VoronoiCalculator();
			var clip = new VoronoiClipper();

			int shatterResolution = 14;
			var innerSites = new List<Vector2>();
			var sites = new List<Vector2>();

			//compute position of voronoi sites
			//the first site will not result in a new hull
			//innerSites.Add(position);

			//find the vertices of the ellipse
			List<Vector2> vertices = new List<Vector2>();
			float randAngleOffset = Random.value;
			for (int i = 0; i < shatterResolution; i++)
			{
				float t = (float)i / (float)shatterResolution;
				var angle = (randAngleOffset + t) * 2.0f * Mathf.PI;
				var dist = holeRadius;

				vertices.Add( new Vector2(
						dist * Mathf.Cos(angle) * tiltExpansion,
						dist * Mathf.Sin(angle)));
			}

			//create the lines that connect the vertices by placing a voronoi site on either side
			float borderSeperation = 0.05f; //should be pretty small; hopefully not too small
			for (int i = 0; i < vertices.Count; i++)
			{
				int iNext = (i+1 >= vertices.Count) ? 0 : i+1;

				Vector2 borderpoint = (vertices[i] + vertices[iNext]) * 0.5f;

				Vector2 normal = vertices[iNext] - vertices[i];
				normal = new Vector2(normal.y, -normal.x);
				normal.Normalize();

				innerSites.Add(borderpoint - normal * borderSeperation);
				sites.Add(borderpoint      + normal * borderSeperation * Random.value); //hack: see below
				//The Random.value tries to avoid verts in the voronoi dagram sharing 4 edges.
				//For some reason that causes an error. There's still a small chance of it happening though.
			}

			//add some outer sites to really shatter the whole piece
			randAngleOffset = Random.value;
			for (int i = 0; i < shatterResolution / 2; i++)
			{
				float t = (float)i / (float)(shatterResolution / 2);
				var angle = (randAngleOffset + t) * 2.0f * Mathf.PI;
				var dist = holeRadius * 3;

				sites.Add(new Vector2(
						dist * Mathf.Cos(angle) * tiltExpansion,
						dist * Mathf.Sin(angle)));
			}
			randAngleOffset = Random.value;
			for (int i = 0; i < shatterResolution / 2; i++)
			{
				float t = (float)i / (float)(shatterResolution / 2);
				var angle = (randAngleOffset + t) * 2.0f * Mathf.PI;
				var dist = holeRadius * 8;

				sites.Add(new Vector2(
						dist * Mathf.Cos(angle) * tiltExpansion,
						dist * Mathf.Sin(angle)));
			}

			//combine the non-generated sites and generated sites lists
			int sitesToNotGenerate = innerSites.Count;
			innerSites.AddRange(sites);
			sites = innerSites;
			innerSites = null;

			//rotate and move hole into place (moves all verts)
			for (int i = 0; i < sites.Count; i++)
			{
				float s = Mathf.Sin(holeRotation);
				float c = Mathf.Cos(holeRotation);
				// [ c, s] [x] + [a] = [X]
				// [-s, c] [y] + [b]   [Y]
				float X = c * sites[i].x + s * sites[i].y + position.x;
				float Y = -s * sites[i].x + c * sites[i].y + position.y;
				sites[i] = new Vector2(X, Y);
			}


			//make another circle of sites so the rubble falls away
			/*
			randAngleOffset = 2.0f * Mathf.PI * Random.value;
			for (int i = 0; i < shatterResolution * 2; i++)
			{
				float t = (float)i / (float)(shatterResolution * 2);
				var angle = randAngleOffset + 2.0f * Mathf.PI * t;
				var dist = holeRadius * 2.0f;
			
				sites.Add( position + new Vector2(
						dist * Mathf.Cos(angle),
						dist * Mathf.Sin(angle)));
			}
			//*/

			//voronoi diagram
			var diagram = calc.CalculateDiagram(sites);

			var clipped = new List<Vector2>();

			//foreach site in the diagram, clip a new gameoject
			//Skip the first one so that it leaves a hole
			for (int i = sitesToNotGenerate; i < sites.Count; i++)
			{
				clip.ClipSite(diagram, PolygonPoints, i, ref clipped);

				if (clipped.Count > 0) {
					var newGo = Instantiate(gameObject, transform.parent);

					newGo.transform.localPosition = transform.localPosition;
					newGo.transform.localRotation = transform.localRotation;

					var bs = newGo.GetComponent<BreakableByGBE>();

					bs.Thickness = Thickness;
					bs.PolygonPoints.Clear();
					bs.PolygonPoints.AddRange(clipped);
					bs.originalScale = originalScale;

					var childArea = bs.Area;

					//should it be a rigidbody?
					Rigidbody = GetComponent<Rigidbody>();
					if (Rigidbody != null)
					{
						//new thing already has rigidbody because this one did
						var rb = bs.GetComponent<Rigidbody>();
						rb.mass = Rigidbody.mass * (childArea / area);
						rb.velocity = Rigidbody.velocity;
						rb.angularVelocity = Rigidbody.angularVelocity;
					}
					else
					{
						bool addRigidbody = !Input.GetKey(KeyCode.LeftShift);
						if (addRigidbody)
						{
							var rb = newGo.AddComponent<Rigidbody>();
						}
					}

					//make this piece a random color
					bool randomizeColor = Input.GetKey(KeyCode.LeftControl);
					if(randomizeColor)
					{
						newGo.GetComponent<MeshRenderer>().material.color = new Color(Random.value, Random.value, Random.value);
					}
				}
			}

			gameObject.SetActive(false);
			Destroy(gameObject);
		}

		Mesh MeshFromPolygon(List<Vector2> polygon, float thickness) {
			var count = polygon.Count;
			// TODO: cache these things to avoid garbage
			var verts = new Vector3[6 * count];
			var norms = new Vector3[6 * count];
			var tris = new int[3 * (4 * count - 4)];
			var uvs = new Vector2[6 * count];
			// TODO: make UVs use scale properly

			var vi = 0; // vertex iterator
			var ni = 0; // normal iterator
			var ti = 0; // triangle iterator
			var ui = 0; // UV coord iterator

			var ext = 0.5f * thickness;

			// Top
			for (int i = 0; i < count; i++) {
				verts[vi++] = new Vector3(polygon[i].x, polygon[i].y, ext);
				norms[ni++] = Vector3.forward;
				uvs[ui++] = new Vector2(polygon[i].x / (originalScale.x * 1.0f) + 0.5f, polygon[i].y / (originalScale.y * 1.0f) + 0.5f);
			}

			// Bottom
			for (int i = 0; i < count; i++) {
				verts[vi++] = new Vector3(polygon[i].x, polygon[i].y, -ext);
				norms[ni++] = Vector3.back;
				uvs[ui++] = new Vector2(polygon[i].x / (originalScale.x * 1.0f) + 0.5f, polygon[i].y / (originalScale.y * 1.0f) + 0.5f);
			}

			// Sides
			for (int i = 0; i < count; i++) {
				var iNext = (i == count - 1) ? 0 : (i + 1); //index of next edge, circularly

				verts[vi++] = new Vector3(polygon[i].x, polygon[i].y, ext);
				verts[vi++] = new Vector3(polygon[i].x, polygon[i].y, -ext);
				verts[vi++] = new Vector3(polygon[iNext].x, polygon[iNext].y, -ext);
				verts[vi++] = new Vector3(polygon[iNext].x, polygon[iNext].y, ext);

				var norm = Vector3.Cross(polygon[iNext] - polygon[i], Vector3.forward).normalized;

				norms[ni++] = norm;
				norms[ni++] = norm;
				norms[ni++] = norm;
				norms[ni++] = norm;

				//TODO: add UVs to sides. Right now its just garbage
			}


			for (int vert = 2; vert < count; vert++) {
				tris[ti++] = 0;
				tris[ti++] = vert - 1;
				tris[ti++] = vert;
			}

			for (int vert = 2; vert < count; vert++) {
				tris[ti++] = count;
				tris[ti++] = count + vert;
				tris[ti++] = count + vert - 1;
			}

			for (int vert = 0; vert < count; vert++) {
				var si = 2*count + 4*vert;

				tris[ti++] = si;
				tris[ti++] = si + 1;
				tris[ti++] = si + 2;

				tris[ti++] = si;
				tris[ti++] = si + 2;
				tris[ti++] = si + 3;
			}

			Debug.Assert(ti == tris.Length);
			Debug.Assert(vi == verts.Length);
			//Debug.Assert(ui == uvs.Length);

			var mesh = new Mesh();
			mesh.vertices = verts;
			mesh.triangles = tris;
			mesh.normals = norms;
			mesh.uv = uvs;

			return mesh;
		}
	}
}