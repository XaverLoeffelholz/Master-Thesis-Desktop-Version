using UnityEngine;
using System.Collections;

public class StageDistanceController : MonoBehaviour {

	private Transform stageScaler;
	private Selection activeController;
	private bool moving = false;
	private float lastZValue;

	// Use this for initialization
	void Start () {
		stageScaler = transform.parent.parent;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (moving) {
			float newZValue = activeController.pointOfCollisionGO.transform.position.z;
			ChangeStageDistance (newZValue - lastZValue);
			lastZValue = newZValue;
		}
	}

	public void Focus(Selection controller){

		foreach (Transform child in transform) {
			if (child.gameObject.name == "Arrow") {
				LeanTween.scale(child.gameObject, new Vector3 (0.05f, 0.1f, 0.1f), 0.1f);
				LeanTween.color(child.gameObject, new Color (0.7f, 0.8f, 1f, 1f), 0.1f);
			}
		}

	}


	public void UnFocus(Selection controller){

		foreach (Transform child in transform) {
			if (child.gameObject.name == "Arrow") {
				LeanTween.scale(child.gameObject, new Vector3 (0.04f, 0.08f, 0.08f), 0.1f);
				LeanTween.color(child.gameObject, new Color (1f, 1f, 1f, 1f), 0.1f);
			}
		}
	}

	public void StartMoving(Selection controller){
		activeController = controller;
		moving = true;
		lastZValue = activeController.pointOfCollisionGO.transform.position.z;
	}

	public void StopMoving(Selection controller){
		moving = false;
		activeController = null;
	}

	public void ChangeStageDistance(float value){

		float newZ = stageScaler.transform.position.z + value;

		newZ = Mathf.Min (newZ, 5f);
		newZ = Mathf.Max (newZ, 0.2f);

		stageScaler.transform.position = new Vector3 (stageScaler.transform.position.x, stageScaler.transform.position.y, newZ);

	}
}
