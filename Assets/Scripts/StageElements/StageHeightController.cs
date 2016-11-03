using UnityEngine;
using System.Collections;

public class StageHeightController : MonoBehaviour {

	private Transform stageScaler;
	private Selection activeController;
	private bool moving = false;
	private float lastYValue;


	// Use this for initialization
	void Start () {
		stageScaler = transform.parent.parent;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (moving) {
			float newYValue = activeController.pointOfCollisionGO.transform.position.y;
			ChangeStageHeight (newYValue - lastYValue);
			lastYValue = newYValue;
		}
	}

	public void Focus(Selection controller){
		foreach (Transform child in transform) {
			if (child.gameObject.name == "Arrow") {
				LeanTween.scale(child.gameObject, new Vector3 (0.04f, 0.07f, 0.07f), 0.1f);
			}

			LeanTween.color(child.gameObject, new Color (0.7f, 0.8f, 1f, 1f), 0.1f);
		}
			
	}


	public void UnFocus(Selection controller){
		
		foreach (Transform child in transform) {
			if (child.gameObject.name == "Arrow") {
				LeanTween.scale(child.gameObject, new Vector3 (0.03f, 0.06f, 0.06f), 0.1f);
			}
			LeanTween.color(child.gameObject, new Color (1f, 1f, 1f, 1f), 0.1f);
		}
	}

	public void StartMoving(Selection controller){
		activeController = controller;
		moving = true;
		lastYValue = activeController.pointOfCollisionGO.transform.position.y;
	}

	public void StopMoving(Selection controller){
		moving = false;
		activeController = null;
	}

	public void ChangeStageHeight(float value){

		float newY = stageScaler.transform.position.y + value;

		newY = Mathf.Min (newY, 1.2f);
		newY = Mathf.Max (newY, -2.8f);

		stageScaler.transform.position = new Vector3 (stageScaler.transform.position.x, newY, stageScaler.transform.position.z);
	}
}
