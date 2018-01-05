using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private const string INPUT_AXIS = "Horizontal";
	private const string JUMP = "Jump";

	private bool _inAir = true;
	private bool _isCharacterStopping = false;

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

	[SerializeField]
	[Tooltip("Dust trail prefab")]
	private GameObject _dustTrailPrefab;

	private GameObject _dustTrailInstance;
	private ParticleSystem _slideParticleSystem;

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
		_slideParticleSystem = GetComponent<ParticleSystem> ();
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
			if (!_isCharacterStopping && !_inAir) {
				_isCharacterStopping = true;
				// XXX:TODO instantiate particle system here
				CreateDustTrail();
			}
		} else {
			_playerCollider.material.dynamicFriction = _initialFriction;
			_playerCollider.material.staticFriction = _initialFriction;
			_isCharacterStopping = false;
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
		// XXX:TODO this is causing loss of character control while in air at certain speeds
		if (Mathf.Abs (_rb.velocity.x) < _maxSpeed) {
			_rb.AddForce (_inputH * _acceleration, 0.0f, 0.0f, ForceMode.VelocityChange);
		}
	}

	void OnCollisionEnter (Collision other) {
		// XXX:TODO the velocity condition is stupid
		if (other.gameObject.layer == LayerMask.NameToLayer ("Platform") && other.contacts[0].normal == Vector3.up) {
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

	void CreateDustTrail() {
		if (_dustTrailInstance == null) {
			_dustTrailInstance = Instantiate (_dustTrailPrefab, transform.position, Quaternion.LookRotation (transform.forward));
			_dustTrailInstance.GetComponent<ParticleSystem> ().Play ();
			_slideParticleSystem.Play ();
		} else {
			_dustTrailInstance.transform.position = transform.position;
			_dustTrailInstance.transform.rotation = Quaternion.LookRotation (transform.forward);
			_dustTrailInstance.GetComponent<ParticleSystem> ().Play ();
			_slideParticleSystem.Play ();
		}
	}
}
