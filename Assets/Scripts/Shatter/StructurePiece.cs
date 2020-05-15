using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void BreakEvent(StructurePiece other);

public class StructurePiece : MonoBehaviour
{
	public event BreakEvent OnBreakEvent;

	public List<StructurePiece> ParentsToAdd;
	public Dictionary<StructurePiece, bool> Parents = new Dictionary<StructurePiece, bool>();
	public Dictionary<StructurePiece, bool> Children = new Dictionary<StructurePiece, bool>();

	void Start()
	{
		foreach (var parent in ParentsToAdd)
			AddSupport(this, parent);
	}

	void OnDestroy()
	{
		BreakPiece();
	}

	public static void AddSupport(StructurePiece child, StructurePiece parent)
	{
		if (child == null || parent == null) return;
		if (child == parent) return;
		if (child?.GetComponent<Rigidbody>() != null || parent?.GetComponent<Rigidbody>() != null) return;

		if(parent.Children.ContainsKey(child) == false)
		{
			parent.Children.Add(child, true);
			parent.OnBreakEvent += child.OnParentBroken;
		}

		if(child.Parents.ContainsKey(parent) == false)
		{
			child.Parents.Add(parent, true);
			child.OnBreakEvent += parent.OnChildBroken;
		}

		//Debug.Log("BBGBE: " + child.name + " supports " + parent.name);
	}

	public void OnParentBroken(StructurePiece parent)
	{
		Debug.Assert(Parents.ContainsKey(parent), "Child didn't know about parent");
		//Debug.Log("BBGBE: " + name + "'s parent (" + parent.name + ") broke");

		this.OnBreakEvent -= parent.OnChildBroken;
		parent.OnBreakEvent -= this.OnParentBroken;
		Parents.Remove(parent);

	}

	public void OnChildBroken(StructurePiece child)
	{
		Debug.Assert(Children.ContainsKey(child), "Parent didn't know about child");
		//Debug.Log("BBGBE: " + name + "'s child (" + child.name + ") broke");

		this.OnBreakEvent -= child.OnParentBroken;
		child.OnBreakEvent -= this.OnChildBroken;
		Children.Remove(child);

		if (Children.Count == 0 && GetComponent<Rigidbody>() == null)
		{
			//Debug.Log("BBGBE: " + name + " has no more supports");
			gameObject.AddComponent<Rigidbody>();
			BreakPiece();
		}
	}

	public void BreakPiece()
	{
		OnBreakEvent?.Invoke(this);
	}
}
