using UnityEngine;
using System.Collections;

public class TeleportationPosition : MonoBehaviour {
	private Vector3 initialPosition;

	private Selection controllerForMovement;
	private Vector3 lastPositionController;
	private bool moving = false;

	public Transform center;

	// Use this for initialization
	void Start () {
		initialPosition = transform.position;
	}

	public void Focus(Selection controller){

	}
	public void UnFocus(Selection controller){

	}


	public void StartMoving(Selection controller)
	{
		moving = true;
		controllerForMovement = controller;
		lastPositionController = controller.pointOfCollisionGO.transform.position;
	}

	public void StopMoving(Selection controller)
	{
		moving = false;
		controllerForMovement = null;
	}

	
	// Update is called once per frame
	void FixedUpdate () {
		if (moving) {
			transform.LookAt (center);
			transform.localRotation = Quaternion.Euler (new Vector3 (0f, transform.localRotation.eulerAngles.y, 0f));

			Vector3 prevPosition = transform.position;
			Vector3 newPositionController = controllerForMovement.pointOfCollisionGO.transform.position;

			Vector3 newPositionWorld = prevPosition + (newPositionController - lastPositionController);
			lastPositionController = newPositionController;
			this.transform.position = new Vector3(newPositionWorld.x, 0f, newPositionWorld.z);
		}
	}

	public void ResetPosition(){
		this.transform.position = initialPosition;
	}
}
