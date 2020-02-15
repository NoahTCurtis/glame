using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
	public float MoveForce = 10.0f;
	public float PullSpeed = 1;

	private bool _grappling = false;
	private GameObject _grappledObject = null;
	private Vector3 _grappledPosition;
	private bool _falling = false;

	private GameObject _player;
	private LineRenderer _lr;
	private Rigidbody _rb;
	private CapsuleCollider _c;
	private ConfigurableJoint _cj;
	private FirstPersonDrifter _fpd;
	private CharacterController _cc;

	void Start()
	{
		_lr = GetComponent<LineRenderer>();
		_player = GetComponentInParent<FirstPersonDrifter>().gameObject;
		_rb = _player.GetComponent<Rigidbody>();
		_c = _player.GetComponent<CapsuleCollider>();
		_cj = _player.GetComponent<ConfigurableJoint>();
		_fpd = _player.GetComponent<FirstPersonDrifter>();
		_cc = _player.GetComponent<CharacterController>();

		SetPlayerGrappleMode(false);
	}

	
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
			StartGrapple();

		if (_grappling)
			UpdateGrapple();

		if (Input.GetMouseButtonUp(0) && _grappling)
			EndGrapple();

		if (_falling)
			UpdateFalling();
	}

	void StartGrapple()
	{
		ConfigurableJoint cj = _player.GetComponent<ConfigurableJoint>();

		RaycastHit hit = new RaycastHit();
		Ray ray = new Ray(transform.position, transform.forward);
		int layer = ~LayerMask.GetMask("Player");
		if(Physics.Raycast(ray, out hit, float.MaxValue, layer))
		{
			_grappledObject = hit.transform.gameObject;
			_grappledPosition = ray.GetPoint(hit.distance);

			_grappling = true;
			_falling = false;
			SetPlayerGrappleMode(true);
		}
	}

	void UpdateGrapple()
	{
		float forceX = Input.GetAxis("Horizontal") * MoveForce;
		float forceY = Input.GetAxis("Vertical") * MoveForce;
		Vector3 forceVec = Camera.main.transform.forward * forceY + Camera.main.transform.right * forceX;
		_rb.AddForce(forceVec, ForceMode.Acceleration);

		if(Input.GetMouseButton(1))
		{
			SoftJointLimit limit = new SoftJointLimit();
			float limitA = _cj.linearLimit.limit - PullSpeed;
			float limitB = Vector3.Distance(_player.transform.position, _grappledPosition);
			limit.limit = Mathf.Min(limitA, limitB);

			limit.bounciness = limit.contactDistance = 0;
			_cj.linearLimit = limit;
		}
		

		_lr.positionCount = 2;
		_lr.SetPosition(0, transform.position);
		_lr.SetPosition(1, _grappledPosition);

		if (_grappledObject == null)
			EndGrapple();
	}

	void EndGrapple()
	{
		_grappling = false;
		_lr.positionCount = 0;

		_cj.xMotion = ConfigurableJointMotion.Free;
		_cj.yMotion = ConfigurableJointMotion.Free;
		_cj.zMotion = ConfigurableJointMotion.Free;

		_falling = true;
	}

	void UpdateFalling()
	{
		if(Physics.Raycast(_player.transform.position, Vector3.down, _c.bounds.extents.y + 0.1f))
		{
			SetPlayerGrappleMode(false);
			_falling = false;
		}
	}

	void SetPlayerGrappleMode(bool value)
	{
		if (value)
		{
			if(_cc.enabled) _rb.velocity = _cc.velocity;

			_fpd.enabled = false;
			_cc.enabled = false;

			_rb.isKinematic = false;
			_rb.useGravity = true;

			_cj.xMotion = ConfigurableJointMotion.Limited;
			_cj.yMotion = ConfigurableJointMotion.Limited;
			_cj.zMotion = ConfigurableJointMotion.Limited;

			_cj.connectedAnchor = _grappledPosition;

			SoftJointLimit limit = new SoftJointLimit();
			limit.limit = Vector3.Distance(_player.transform.position, _grappledPosition);
			limit.bounciness = 0;
			limit.contactDistance = 0;
			_cj.linearLimit = limit;
		}
		else
		{
			_fpd.enabled = true;
			_cc.enabled = true;

			_rb.isKinematic = true;
			_rb.useGravity = false;

			_cj.xMotion = ConfigurableJointMotion.Free;
			_cj.yMotion = ConfigurableJointMotion.Free;
			_cj.zMotion = ConfigurableJointMotion.Free;
		}
	}
}
