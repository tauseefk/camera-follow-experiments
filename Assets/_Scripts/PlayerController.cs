using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private const string INPUT_AXIS = "Horizontal";
	private const string JUMP = "Jump";

	private bool _inAir = true;

	[SerializeField]
	[Tooltip("Horizontal acceleration of the player.")]
	private float _acceleration = 8.0f;

	[SerializeField]
	[Tooltip("Max horizontal move speed of the player.")]
	private float _maxSpeed = 8.0f;

	[SerializeField]
	[Tooltip("Y speed when bouncing off a platform (m/s)")]
	private float _bounceSpeed = 15.0f;

	[SerializeField]
	[Tooltip("Rotation speed of the player")]
	private float _rotationSpeed = 4f;

	private Vector3 _velocity;

	private Rigidbody _rb;

	private float _inputH;
	private float _previousInputH = 0.0f;
	private float _inputV;

	private Vector3 _LEFT = new Vector3 (-1.0f, 0.0f, 0.0f);
	private Vector3 _RIGHT = new Vector3 (1.0f, 0.0f, 0.0f);

	private Coroutine _leftRotationCoroutine;
	private Coroutine _rightRotationCoroutine;

	private Collider _playerCollider;
	private float _initialFriction;

	// Use this for initialization
	void Start () {
		_rb = GetComponent<Rigidbody> ();
		_rb.freezeRotation = true;
		_playerCollider = GetComponent<Collider>();
		_initialFriction = _playerCollider.material.dynamicFriction;
	}

	void Update () {
		_inputH = Input.GetAxis(INPUT_AXIS);
		_inputV = Input.GetAxis (JUMP);


		// XXX:TODO This is weird, simplify
		bool directionChanged = Mathf.Sign(Vector3.Dot(transform.forward.normalized, _rb.velocity.normalized)) == -1;

		if (Mathf.Approximately(Mathf.Abs(_inputH), Mathf.Epsilon) || directionChanged) {
			_playerCollider.material.dynamicFriction = 0.6f;
			_playerCollider.material.staticFriction = 0.6f;
		} else {
			_playerCollider.material.dynamicFriction = _initialFriction;
			_playerCollider.material.staticFriction = _initialFriction;
		}
		if (_inputH > 0 && _rightRotationCoroutine == null) {
			if (_leftRotationCoroutine != null) {
				StopCoroutine (_leftRotationCoroutine);
				_leftRotationCoroutine = null;
			}
			_rightRotationCoroutine = StartCoroutine (TurnToRight());

		} else if (_inputH < 0 && _leftRotationCoroutine == null) {
			if (_rightRotationCoroutine != null) {
				StopCoroutine (_rightRotationCoroutine);
				_rightRotationCoroutine = null;
			}
			_leftRotationCoroutine = StartCoroutine (TurnToLeft());
		}
	}
	void FixedUpdate () {

		if (!_inAir && _inputV > 0) {
			_rb.AddForce (Vector3.up * _bounceSpeed, ForceMode.Impulse);
			_inAir = true;
		}
			
		if (Mathf.Abs (_rb.velocity.x) < _maxSpeed) {
			_rb.AddForce (_inputH * _acceleration, 0.0f, 0.0f, ForceMode.VelocityChange);
		}
	}

	void OnCollisionEnter (Collision other) {
		if (other.gameObject.layer == LayerMask.NameToLayer ("Platform")) {
			_inAir = false;
		}
	}

	IEnumerator TurnToLeft() {
		float startTime = Time.time;
		float timeElapsed = 0.0f;

		while(transform.forward != _LEFT) {
			timeElapsed = Time.time - startTime;
			transform.rotation = Quaternion.Lerp (Quaternion.LookRotation(transform.forward), Quaternion.LookRotation(_LEFT), timeElapsed * _rotationSpeed);	
			yield return null;
		}
	}

	IEnumerator TurnToRight() {
		float startTime = Time.time;
		float timeElapsed = 0.0f;

		while(transform.forward != _RIGHT) {
			timeElapsed = Time.time - startTime;
			transform.rotation = Quaternion.Lerp (Quaternion.LookRotation(transform.forward), Quaternion.LookRotation(_RIGHT), timeElapsed * _rotationSpeed);	
			yield return null;
		}
	}
}
