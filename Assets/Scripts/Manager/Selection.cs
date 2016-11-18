using UnityEngine;
using System.Collections;
using System;

public class Selection : MonoBehaviour
{
	public enum controllerType { mainController, SecondaryController};
    public controllerType typeOfController;

    public enum selectionMode { laserpointer, directTouch };
    public selectionMode currentSelectionMode;

    public enum settingSelectionMode { alwaysOpen, SettingsButton };
    public settingSelectionMode currentSettingSelectionMode;
    
    public GameObject currentFocus;
    public GameObject currentSelection;
	public Vector3 pointOfCollision;
	public Face collidingFace;
    public GameObject objects;
    public Selection otherController;

    public bool triggerPressed = false;
	public bool menuButtonPressed = false;
    [HideInInspector]
    public bool scalingObject = false;
    public bool movingObject = false;
    private bool movingHandle = false;

    public GameObject LaserPointer;
    public GameObject pointOfCollisionPrefab;
    private float temps;
	private float tempsUI;
	private bool faceSelection;

    [HideInInspector]
    public GameObject pointOfCollisionGO;

    SteamVR_TrackedObject trackedObj;

	public Transform uiPositon;
    public bool groupItemSelection = false;

    public GameObject cam;

	public bool controllerActive;

	public CircleAnimaton circleAnimation;
	public Infopanel trashInfopanel;

	public GameObject grabIconPrefab;
	private GameObject grabIcon;
	private Vector3 initialScaleGrabIcon;

	public GameObject grabbedIconPrefab;
	private GameObject grabbedIcon;
	private Vector3 grabbedIconOffset;
	private Vector3 initialScaleGrabbedIcon;

	private Transform stageScaler;

	public bool duplicateMode;
	public bool scalingMode;
	private bool recheckFocus = false;

	public Material normalGrabIconMat;
	public Material duplicateGrabIconMat;
	public Material addToGroupMat;
	public Material scaleGrabIconMat;

	public Material grabbedIconMat;
	public Material scaleGrabbedIconMat;

	public SettingsButtonHelp settingsButtonHelp;

	public Transform buttonOnController;
	public Vector3 standardPosButton;

	public SphereColliderSelection colliderSphere;
	private GameObject collisionObject;
	public GrabAnimation grabAnimation;

	public Transform grip;
	public Transform grip2;
	public Transform trigger;

	bool showGripButton = false;

	int count = 0;

	public Plane dragPlane;

	// to do: delete everythin with controller

	// do raycasting from pos of of mouse

	// change movement to always be on ground (as other prototype)

	// also raycasting to ground / or plane looking at camera / for rotation it should already work

	// add uniform scale handle

	// implement orbit cam from other project




    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

		if (typeOfController == controllerType.SecondaryController) {
			ActivateController (false);
		} else {
			ActivateController (true);
		}

		tempsUI = Time.time;

		stageScaler = GameObject.Find ("StageScaler").transform;

		initialScaleGrabIcon = new Vector3(0.006f,0.006f,0.006f);
		initialScaleGrabbedIcon = new Vector3(0.006f,0.006f,0.006f);
		standardPosButton = buttonOnController.transform.localPosition;

		if (currentSelectionMode == selectionMode.directTouch) {
			colliderSphere.ActivateCollider ();
			LaserPointer.transform.GetChild (0).gameObject.SetActive (false);
		} else {
			colliderSphere.DeActivateCollider ();
			grabAnimation.Hide ();
		}


    }

	void DeFocusCurrent(GameObject newObject){

		if (currentFocus != null) {
			if (currentFocus.CompareTag ("ModelingObject")) {
				// compare selection button and object
				if (newObject != null && newObject.CompareTag ("SelectionButton")) {
					if (newObject.GetComponent<ObjectSelecter> ().connectedObject != currentFocus.GetComponent<ModelingObject> ()) {
						currentFocus.GetComponent<ModelingObject> ().UnFocus (this);
					}
				} else {
					currentFocus.GetComponent<ModelingObject> ().UnFocus (this);
				}
			} else if (currentFocus.CompareTag ("Handle")) {
				currentFocus.GetComponent<handle> ().UnFocus (this);
			} else if (currentFocus.CompareTag ("UiElement")) {
				currentFocus.GetComponent<UiElement> ().UnFocus (this);
			} else if (currentFocus.CompareTag ("TeleportPosition")) {
				currentFocus.GetComponent<TeleportationPosition> ().UnFocus (this);
			} else if (currentFocus.CompareTag ("Stage")) {
				currentFocus.GetComponent<StageFreeMovement> ().UnFocus (this);
			} else if (currentFocus.CompareTag ("Library")) {
				library.Instance.UnFocus (this);
			} else if (currentFocus.CompareTag ("HeightControl")) {
				currentFocus.GetComponent<StageHeightController> ().UnFocus (this);
			} else if (currentFocus.CompareTag ("DistanceControl")) {
				currentFocus.GetComponent<StageDistanceController> ().UnFocus (this);
			} else if (currentFocus.CompareTag ("InfoPanel")) {
				currentFocus.GetComponent<Infopanel> ().UnFocus (this);
			} else if (currentFocus.CompareTag ("SelectionButton")) {
				
				// compare selection button and object
				if (newObject != null && newObject.CompareTag ("ModelingObject")) {
					if (currentFocus.GetComponent<ObjectSelecter> ().connectedObject != newObject.GetComponent<ModelingObject> ()) {
						currentFocus.GetComponent<ObjectSelecter> ().UnFocus (this);
						currentFocus.GetComponent<ObjectSelecter> ().connectedObject.UnFocus (this);
					} else {
						currentFocus.GetComponent<ObjectSelecter> ().UnFocus (this);
					}
				} else {
					currentFocus.GetComponent<ObjectSelecter> ().UnFocus (this);
					currentFocus.GetComponent<ObjectSelecter> ().connectedObject.UnFocus (this);
				} 
			}

			currentFocus = null;
		}

	}

	public void ShowHelpVisual (){
		showGripButton = true;
	}


	// Update is called once per frame
	void Update ()
	{
		RaycastHit hit;

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		if (pointOfCollisionGO != null) {
			float distance;
			if (dragPlane.Raycast (ray, out distance)) {
				pointOfCollisionGO.transform.position = ray.GetPoint(distance);
			}
		}

		// only change focus is the object is not moved at the moment
		if (!movingObject && !movingHandle && !scalingObject && !triggerPressed) {
				
			if (Physics.Raycast (ray, out hit)) {					

				if (currentSelectionMode == selectionMode.laserpointer) {
					pointOfCollision = hit.point;
				}

				if (currentSelectionMode == selectionMode.laserpointer) {
					if (hit.rigidbody != null && hit.rigidbody.transform.parent != null) {
						collisionObject = hit.rigidbody.transform.parent.gameObject;
					} else {
						collisionObject = null;
					}
				}

				if (collisionObject != null) {

					if (currentFocus != collisionObject || recheckFocus) {
						
						count++;

						recheckFocus = false;

						if (collisionObject.CompareTag ("ModelingObject")) {		
							DeFocusCurrent (collisionObject);
							currentFocus = collisionObject;
							currentFocus.GetComponent<ModelingObject> ().Focus (this);

						} else if (collisionObject.CompareTag ("Handle")) {
							DeFocusCurrent (collisionObject);
							currentFocus = collisionObject;
							currentFocus.GetComponent<handle> ().Focus (this);
						} else if (collisionObject.CompareTag ("UiElement") && UiCanvasGroup.Instance.visible) {
							DeFocusCurrent (collisionObject);
							currentFocus = collisionObject;
							currentFocus.GetComponent<UiElement> ().Focus (this);
						} else if (!UiCanvasGroup.Instance.visible && collisionObject.CompareTag ("TeleportTrigger")) {
							DeFocusCurrent (collisionObject);
							currentFocus = collisionObject;
							currentFocus.GetComponent<TeleportationTrigger> ().Focus (this);
						} else if (collisionObject.CompareTag ("SelectionButton")) {
							DeFocusCurrent (collisionObject);
							currentFocus = collisionObject;
							currentFocus.GetComponent<ObjectSelecter> ().Focus (this);
							if (currentFocus.GetComponent<ObjectSelecter> ().connectedObject != null) {
								currentFocus.GetComponent<ObjectSelecter> ().connectedObject.Focus (this);
							}
						} else if (!UiCanvasGroup.Instance.visible && collisionObject.CompareTag ("TeleportPosition")) {
							DeFocusCurrent (collisionObject);
							currentFocus = collisionObject;
							currentFocus.GetComponent<TeleportationPosition> ().Focus (this);
						} else if (!UiCanvasGroup.Instance.visible && collisionObject.CompareTag ("Library")) {
							DeFocusCurrent (collisionObject);
							currentFocus = collisionObject;
							library.Instance.Focus (this);

						} else if (collisionObject.CompareTag ("HeightControl")) {
							DeFocusCurrent (collisionObject);
							currentFocus = collisionObject;
							currentFocus.GetComponent<StageHeightController> ().Focus (this);
	
						} else if (collisionObject.CompareTag ("DistanceControl")) {
							DeFocusCurrent (collisionObject);
							currentFocus = collisionObject;
							currentFocus.GetComponent<StageDistanceController> ().Focus (this);

						} else if (collisionObject.CompareTag ("Stage") && typeOfController == controllerType.mainController) {
							DeFocusCurrent (collisionObject);
							currentFocus = collisionObject;

							currentFocus.GetComponent<StageFreeMovement> ().Focus (this);
						} else if (collisionObject.CompareTag ("InfoPanel")) {
							DeFocusCurrent (collisionObject);
							currentFocus = collisionObject;
							currentFocus.GetComponent<Infopanel> ().Focus (this);
						} else {
							DeFocusCurrent (null);
						}

					}

				} else {
					if (currentFocus != null && collisionObject == null) {
						DeFocusCurrent (null);
						temps = Time.time;
					}
				}
			} else {
				if (currentFocus != null || (currentSelectionMode == selectionMode.directTouch && collisionObject == null)) {
					DeFocusCurrent (null);
					temps = Time.time;
				}
					
			} 
		} 
			

		if (currentFocus != null) {
			if (Input.GetMouseButtonDown (0)) {
				triggerPressed = true;
				temps = Time.time;
			}

			if (triggerPressed && (currentFocus.CompareTag ("ModelingObject") || currentFocus.CompareTag ("TeleportPosition") || currentFocus.CompareTag ("Stage") || currentFocus.CompareTag ("Library") || currentFocus.CompareTag ("DistanceControl") || currentFocus.CompareTag ("HeightControl")) && typeOfController == controllerType.mainController) {

				if (!movingObject && !groupItemSelection) {

					CreatePointOfCollisionPrefab (Vector3.up);
					movingObject = true;

					if (currentFocus.CompareTag ("ModelingObject")) {

						Logger.Instance.AddLine (Logger.typeOfLog.triggerOnObject);

						if (currentSelection != null && currentFocus != currentSelection) {
							currentSelection.GetComponent<ModelingObject> ().DeSelect (this);
						}

						if (!duplicateMode) {			
							currentFocus.GetComponent<ModelingObject> ().StartMoving (this, currentFocus.GetComponent<ModelingObject> ());

							if (typeOfController == controllerType.mainController) {
								//settingsButtonHelp.HideCompletely (false);
							}

						} else {
							if (currentFocus.GetComponent<ModelingObject> ().group == null) {
								// needs to be local position
								ObjectCreator.Instance.DuplicateObject (currentFocus.GetComponent<ModelingObject> (), null, currentFocus.transform.localPosition);
							} else {
								ObjectCreator.Instance.DuplicateGroup (currentFocus.GetComponent<ModelingObject> ().group, Vector3.zero);
							}

							ModelingObject duplicatedObject = ObjectCreator.Instance.latestModelingObject;

							DeFocusCurrent (duplicatedObject.gameObject);

							if (currentSelection != null) {
								currentSelection.GetComponent<ModelingObject> ().DeSelect (this);
							}

							currentFocus = duplicatedObject.gameObject;
							duplicatedObject.Focus (this);
							duplicatedObject.StartMoving (this, duplicatedObject);
						}

					} else if (currentFocus.CompareTag ("Library")) {
						library.Instance.StartMoving (this);
					} else if (currentFocus.CompareTag ("HeightControl")) {
						currentFocus.GetComponent<StageHeightController> ().StartMoving (this);
					} else if (currentFocus.CompareTag ("DistanceControl"))
						currentFocus.GetComponent<StageDistanceController> ().StartMoving (this);
				} else if (currentFocus.CompareTag ("Stage")) {
					currentFocus.GetComponent<StageFreeMovement> ().StartMoving (this);
				}
			} else if (triggerPressed && (movingHandle || currentFocus.CompareTag ("Handle"))) {
				handle currentHandle = currentFocus.GetComponent<handle> ();

				if (!movingHandle) {
					CreatePointOfCollisionPrefab (currentFocus.transform.right * (-1f));				
				}

				currentHandle.ApplyChanges (pointOfCollisionGO, movingHandle, this);

				//currentHandle.connectedObject.GetComponent<ModelingObject> ().HideBoundingBox ();
				movingHandle = true;
			}
		}

		if (Input.GetMouseButtonUp(0)) {
				triggerPressed = false;
				grabbedIconOffset = Vector3.zero;

				if (movingHandle){
					currentFocus.GetComponent<handle> ().FinishUsingHandle (this);
					movingHandle = false;
				}


				if (movingObject && typeOfController == controllerType.mainController) {
					otherController.triggerPressed = false;
					otherController.movingHandle = false;

					recheckFocus = true;

					if (currentFocus.CompareTag ("ModelingObject")) {	

						currentFocus.GetComponent<ModelingObject> ().StopMoving (this, currentFocus.GetComponent<ModelingObject> ());			

						if (currentFocus.GetComponent<ModelingObject> ().inTrashArea) {
							UiCanvasGroup.Instance.CloseMenu (this);
							currentFocus.GetComponent<ModelingObject> ().TrashObject (true);
							trashInfopanel.CloseInfoPanel ();
						} else {
							if (currentSelection != null) {
								if (currentSelection != currentFocus) {
									//currentSelection.GetComponent<ModelingObject> ().DeSelect (this);

									if (currentSettingSelectionMode == settingSelectionMode.alwaysOpen) {
										currentFocus.GetComponent<ModelingObject> ().Select (this, uiPositon.position);
									}

								} else {
									UiCanvasGroup.Instance.ShowAgain (uiPositon.position);								
								}
							} else {
								if (currentSettingSelectionMode == settingSelectionMode.alwaysOpen) {
									currentFocus.GetComponent<ModelingObject> ().Select (this, uiPositon.position);
								}
							}			
						}
					} else if (currentFocus.CompareTag ("TeleportPosition")) {
						currentFocus.GetComponent<TeleportationPosition> ().StopMoving (this);
						Teleportation.Instance.JumpToPos (5);
					} else if (currentFocus.CompareTag ("Library")) {
						library.Instance.StopMoving (this);
					} else if (currentFocus.CompareTag ("HeightControl")) {
						currentFocus.GetComponent<StageHeightController> ().StopMoving (this);
					} else if (currentFocus.CompareTag ("DistanceControl")) {
						currentFocus.GetComponent<StageDistanceController> ().StopMoving (this);
					} else if (currentFocus.CompareTag ("Stage")) {
						currentFocus.GetComponent<StageFreeMovement> ().StopMoving (this);
					}

				}

				///Destroy (pointOfCollisionGO);
				if (pointOfCollisionGO != null) {
					pointOfCollisionGO.SetActive(false);
				}

				movingObject = false;

				if (currentFocus != null) {

					if (currentFocus.CompareTag ("ModelingObject")) {

						if (groupItemSelection) {
							if (currentFocus.GetComponent<ModelingObject> ().group == null) {
								ObjectsManager.Instance.AddObjectToGroup (ObjectsManager.Instance.currentGroup, currentFocus.GetComponent<ModelingObject> ());
							} else {
								if (currentFocus.GetComponent<ModelingObject> ().group != ObjectsManager.Instance.currentGroup) {
									ObjectsManager.Instance.AddAllObjectsOfGroupToGroup (ObjectsManager.Instance.currentGroup, currentFocus.GetComponent<ModelingObject> ().group);
								}

							}
						} 
					} else if (currentFocus.CompareTag ("SelectionButton")) {
						UiCanvasGroup.Instance.Show ();
						currentFocus.GetComponent<ObjectSelecter> ().Select (this, uiPositon.position);
					} else if (currentFocus.CompareTag ("Handle")) {
						handle currentHandle = currentFocus.GetComponent<handle> ();						
						currentHandle.ResetLastPosition ();
						currentHandle.UnLock ();
						currentHandle.UnFocus (this);
						currentHandle.connectedObject.GetComponent<ModelingObject> ().ShowBoundingBox (true);
					} else if (currentFocus.CompareTag ("UiElement") && UiCanvasGroup.Instance.visible && !movingObject && !movingHandle) {
						if (Time.time - tempsUI > 0.1f) {
							tempsUI = Time.time;
							currentFocus.GetComponent<UiElement> ().PerformAction (this);

							if (currentFocus.GetComponent<UiElement> ().goal != null) {
								currentFocus.GetComponent<UiElement> ().goal.ActivateMenu ();
							}

							Logger.Instance.AddLine (Logger.typeOfLog.uiElement);
						}
					} else if (currentFocus.CompareTag ("InfoPanel")) {
						currentFocus.GetComponent<Infopanel> ().CloseInfoPanel ();
					} else if (currentFocus.CompareTag ("TeleportTrigger")) {
						Teleportation.Instance.JumpToPos (currentFocus.GetComponent<TeleportationTrigger> ().triggerPos);
					}

				}

				temps = 0;
		}
 
		if (Input.GetKeyDown(KeyCode.LeftAlt)){
			duplicateMode = true;
		}

		if (Input.GetKeyUp(KeyCode.LeftAlt)){
			duplicateMode = false;
		}
    }

	public void ToggleOnOffHelp(){
		//settingsButtonHelp.ToggleOnOff ();
	}

	public void ActivateController(bool value){		
		LaserPointer.transform.GetChild(0).gameObject.SetActive(value);
		controllerActive = value;
	}

	//public void Show

	public void SelectLatestObject()
    {
		// deselect current object

        // directly select new object
        UiCanvasGroup.Instance.Show();
		ObjectsManager.Instance.GetlatestObject().Select(this, uiPositon.position); 
    }

    public void FindCollidingFace(Vector3 pointOfCollision){
		collidingFace = UiCanvasGroup.Instance.currentModelingObject.GetFaceFromCollisionCoordinate (pointOfCollision);
	}

	public void CreatePointOfCollisionPrefab(Vector3 planeNormal)
    {
		if (pointOfCollisionGO == null) {
			pointOfCollisionGO = (GameObject)Instantiate (pointOfCollisionPrefab, pointOfCollision, new Quaternion (0f, 0f, 0, 0f));
			pointOfCollisionGO.transform.SetParent (transform);
		} else {
			pointOfCollisionGO.SetActive (true);
			pointOfCollisionGO.transform.position = pointOfCollision;
			pointOfCollisionGO.transform.rotation = new Quaternion (0f, 0f, 0, 0f);
		}

		dragPlane.SetNormalAndPosition (planeNormal, pointOfCollision);
    }

    public void AssignCurrentFocus(GameObject newCollider)
    {
		currentFocus = newCollider;
        temps = 0f;
    }

    public void DeAssignCurrentFocus(GameObject collider)
    {
        if (currentFocus == collider)
        {
            currentFocus = null;
        }

        temps = 0f;
    }

    public void AssignCurrentSelection(GameObject newCollider)
    {
        currentSelection = newCollider;
    }

    public void DeAssignCurrentSelection(GameObject collider)
    {
        if (currentSelection == collider)
        {
            currentSelection = null;
        }
    }

	public void enableFaceSelection(bool value){

        if (value == false && collidingFace!=null)
        {
            collidingFace.UnHighlight();
        }

		faceSelection = value;	
	}

    public bool GetObjectMoving()
    {
        return movingObject;
    }

	public void StartScaling(){
		scalingObject = true;
	}
}
