using UnityEngine;
using System.Collections;

public class StageFreeMovement : MonoBehaviour {

    public enum stageMovement { free, handles }
    public stageMovement currentStageMovement;

    private bool moving = false;
	private Selection controllerForMovement;
	private Vector3 lastPositionController;

    public GameObject stageHandles;
    public MeshCollider stageCollider;
    

	// Use this for initialization
	void Start () {
	    if (currentStageMovement == stageMovement.free)
        {
            SetFreeStageMovement();
        } else
        {
            UseHandlesForStageMovement();
        }
	}

	public void UpdateStageMovement(){
		if (currentStageMovement == stageMovement.free)
		{
			SetFreeStageMovement();
		} else
		{
			UseHandlesForStageMovement();
		}
	}

    public void SetFreeStageMovement()
    {
        stageHandles.SetActive(false);
        stageCollider.enabled = true;
    }

    public void UseHandlesForStageMovement()
    {
        stageHandles.SetActive(true);
        stageCollider.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		if (moving) {
			Vector3 prevPosition = transform.parent.position;
			Vector3 newPositionController = controllerForMovement.pointOfCollisionGO.transform.position;

			Vector3 newPositionWorld = prevPosition + (newPositionController - lastPositionController);
			lastPositionController = newPositionController;
			transform.parent.position = new Vector3(Mathf.Max(Mathf.Min(newPositionWorld.x,5f), -5f), Mathf.Max(Mathf.Min(newPositionWorld.y,5f), -2.8f), Mathf.Max(Mathf.Min(newPositionWorld.z,12f), -5f));
		}
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
}
