using UnityEngine;
using UnityEngine.AI;

public class NavigationController : MonoBehaviour {

	[SerializeField]
	private NavMeshAgent agent;

	[SerializeField]
	private Camera cam;

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = cam.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit)) {
				agent.SetDestination (hit.point);
			}
		}
	}
}
