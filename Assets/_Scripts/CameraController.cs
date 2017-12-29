using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	[SerializeField]
	private Transform _target;

	[SerializeField]
	private Vector3 _offset = new Vector3(0.0f, 2.0f, -5.0f);

	[SerializeField]
	private float _translationFraction = 0.125f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 desiredPosition = _target.position + _offset;
		Vector3 intermediatePosition = Vector3.Lerp (transform.position, desiredPosition, _translationFraction);
		transform.position = intermediatePosition;

		transform.LookAt (_target);
	}
}
