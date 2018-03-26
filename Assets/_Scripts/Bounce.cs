using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour {

	[SerializeField]
	[Tooltip("Y speed when bouncing off a platform (m/s)")]
	private float _bounceSpeed = 15.0f;

	private Rigidbody _rb;

	void Start() {
		_rb = GetComponent<Rigidbody> ();
	}
	// Update is called once per frame
	void OnCollisionEnter (Collision other) {
		_rb.AddForce (Vector3.up * _bounceSpeed, ForceMode.Impulse);
	}
}
