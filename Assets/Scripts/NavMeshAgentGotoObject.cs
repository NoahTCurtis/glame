using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAgentGotoObject : MonoBehaviour
{
	public Transform TargetTransform;

	private NavMeshAgent _agent;

	void Start()
	{
		_agent = GetComponent<NavMeshAgent>();
	}
	
	void Update()
	{
		_agent.SetDestination(TargetTransform.position);
	}
}
