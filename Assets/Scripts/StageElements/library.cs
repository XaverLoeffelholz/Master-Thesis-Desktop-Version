using UnityEngine;
using System.Collections;

public class library : Singleton<library>{

	private Selection controllerForMovement;
	private Vector3 lastPositionController;
	private bool moving = false;

    public Transform pos1;
    public Transform pos2;
    public Transform pos3;
    public Transform pos4;

	public GameObject container;
	public GameObject top;
	public GameObject bottom;
	public GameObject shell;

	public Infopanel libraryInfopanel;

	private  ModelingObject.ObjectType typeOfTakenObject;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Focus(Selection controller){
		LeanTween.scale (gameObject, new Vector3(0.725f, 0.725f,0.725f), 0.1f);
		LeanTween.color (top, new Color (0.9f, 0.9f, 1f, 0.8f), 0.1f);
		LeanTween.color (bottom, new Color (0.6f, 0.7f, 0.8f, 1f), 0.1f);
	}
	public void UnFocus(Selection controller){
		LeanTween.scale (gameObject, new Vector3(0.7f, 0.7f,0.7f), 0.1f);
		LeanTween.color (shell, new Color (1f, 1f, 1f, 0.3f), 0.1f);
		LeanTween.color (top, new Color (0.5f, 0.5f, 0.5f, 1f), 0.1f);
		LeanTween.color (bottom, new Color (0.5f, 0.5f, 0.5f, 1f), 0.1f);
	}


	public void ClearLibrary(ModelingObject.ObjectType type)
    {
		/*
        foreach(Transform modelingObject in transform)
        {
            if(modelingObject.CompareTag("ModelingObject")){
                Destroy(modelingObject.gameObject);
            }
        }
		*/

		typeOfTakenObject = type;

        Invoke("RefillLibrary", 0.3f);

		libraryInfopanel.CloseInfoPanel ();
    }

    public void RefillLibrary()
    {
		ObjectCreator.Instance.createObjectInLibrary (typeOfTakenObject);
    }

	// library should be movable as objects

	// what if object intersects with object


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
			Vector3 prevPosition = transform.position;
			Vector3 newPositionController = controllerForMovement.pointOfCollisionGO.transform.position;

			Vector3 newPositionWorld = prevPosition + (newPositionController - lastPositionController);
			lastPositionController = newPositionController;
			this.transform.position = new Vector3(newPositionWorld.x, newPositionWorld.y, newPositionWorld.z);
		}
	}


}
