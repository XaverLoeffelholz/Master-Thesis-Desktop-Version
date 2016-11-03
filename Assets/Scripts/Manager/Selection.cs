using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(SteamVR_TrackedObject))]
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
	public DuplicateHelp duplicateHelp;

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

		if (grabIcon != null) {
			grabIcon.SetActive(false);
		}

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

    public void AdjustLengthPointer(float MaxDistance)
    {
        LaserPointer.transform.localScale = new Vector3 (1,1,Mathf.Clamp(MaxDistance, 0f,50f));

        // add visual for contact
    }
	
    public void TriggerIfPressed(ushort duration)
    {
        if (triggerPressed)
        {
            SteamVR_Controller.Input((int)trackedObj.index).TriggerHapticPulse(duration);
        }
    }

	public void ShowHelpVisual (){
		showGripButton = true;
	}


	// Update is called once per frame
	void Update () {

        var device = SteamVR_Controller.Input((int)trackedObj.index);    

		if (trigger == null  && typeOfController == controllerType.SecondaryController) {
			if (transform.GetChild (0).childCount == 16) {
				trigger = transform.GetChild (0).GetChild (15);

				trigger.GetComponent<Renderer> ().material.color = Color.green;
				trigger.GetComponent<Renderer> ().material.EnableKeyword ("_EMISSION");
				trigger.GetComponent<Renderer> ().material.SetColor ("_EmissionColor", Color.green);
				//trigger.GetComponent<Renderer> ().material.SetColor ("_EmissionColor", Color.white);
			}
		}

		if (grip == null && typeOfController == controllerType.mainController && showGripButton) {
			if (transform.GetChild (0).childCount == 16) {
				grip = transform.GetChild(0).GetChild(7);
				grip2 = transform.GetChild(0).GetChild(6);

				grip.GetComponent<Renderer> ().material.color = Color.green;
				grip.GetComponent<Renderer> ().material.EnableKeyword ("_EMISSION");
				grip.GetComponent<Renderer> ().material.SetColor ("_EmissionColor", Color.green);
				grip2.GetComponent<Renderer> ().material.EnableKeyword ("_EMISSION");
				grip2.GetComponent<Renderer> ().material.color = Color.green;
				grip2.GetComponent<Renderer> ().material.SetColor ("_EmissionColor", Color.green);
			}
		}


		if (controllerActive) {
			if ((movingObject || scalingObject) && currentFocus != null) {
				if (grabbedIcon != null && grabbedIcon.activeSelf) {
					grabbedIcon.transform.LookAt (Camera.main.transform);
					grabbedIcon.transform.Rotate (new Vector3 (90, 0f, 0f));

					if (scalingMode) {
						grabbedIcon.GetComponent<Renderer> ().material = scaleGrabbedIconMat;
						grabbedIcon.transform.position = pointOfCollisionGO.transform.position;
						Vector3 newScale = initialScaleGrabbedIcon * (transform.position - currentFocus.transform.position).magnitude;
						grabbedIcon.transform.localScale = new Vector3 (Mathf.Min (newScale.x, grabbedIcon.transform.localScale.x * 1.4f), Mathf.Min (newScale.y, grabbedIcon.transform.localScale.y * 1.4f), Mathf.Min (newScale.z, grabbedIcon.transform.localScale.z * 1.4f)); 

					} else {
						grabbedIcon.GetComponent<Renderer> ().material = grabbedIconMat;
						grabbedIcon.transform.position = currentFocus.transform.position + grabbedIconOffset;
						Vector3 newScale = initialScaleGrabbedIcon * (transform.position - currentFocus.transform.position).magnitude;
						grabbedIcon.transform.localScale = new Vector3 (Mathf.Min (newScale.x, grabbedIcon.transform.localScale.x * 1.4f), Mathf.Min (newScale.y, grabbedIcon.transform.localScale.y * 1.4f), Mathf.Min (newScale.z, grabbedIcon.transform.localScale.z * 1.4f)); 
					}
				}
			}

			/*
			if (device.GetTouchDown (SteamVR_Controller.ButtonMask.Trigger)) {
				grabAnimation.Close ();
			} else if (device.GetTouchUp (SteamVR_Controller.ButtonMask.Trigger)) {
				grabAnimation.Open ();
			}*/

			RaycastHit hit;

			// only change focus is the object is not moved at the moment
			if (!movingObject && !movingHandle && !scalingObject && !triggerPressed) {
				
				if ((currentSelectionMode == selectionMode.laserpointer && Physics.Raycast (LaserPointer.transform.position, LaserPointer.transform.forward, out hit)) 
					|| currentSelectionMode == selectionMode.directTouch) {					

					if (currentSelectionMode == selectionMode.laserpointer) {
						pointOfCollision = hit.point;
						AdjustLengthPointer (hit.distance);
					}

					if (grabIcon != null && grabIcon.activeSelf && currentSelectionMode == selectionMode.laserpointer) {
						if (duplicateMode) {
							grabIcon.GetComponent<Renderer> ().material = duplicateGrabIconMat;
							Vector3 newScale = initialScaleGrabIcon * hit.distance * 1.3f;
							grabIcon.transform.localScale = new Vector3 (Mathf.Min (newScale.x, grabIcon.transform.localScale.x * 1.2f), Mathf.Min (newScale.y, grabIcon.transform.localScale.y * 1.2f), Mathf.Min (newScale.z, grabIcon.transform.localScale.z * 1.2f)); 
						} else if (scalingMode) {
							grabIcon.GetComponent<Renderer> ().material = scaleGrabIconMat;
							Vector3 newScale = initialScaleGrabIcon * hit.distance;
							grabIcon.transform.localScale = new Vector3 (Mathf.Min (newScale.x, grabIcon.transform.localScale.x * 1.2f), Mathf.Min (newScale.y, grabIcon.transform.localScale.y * 1.2f), Mathf.Min (newScale.z, grabIcon.transform.localScale.z * 1.2f)); 
						} else if (groupItemSelection) {
							grabIcon.GetComponent<Renderer> ().material = addToGroupMat;
							Vector3 newScale = initialScaleGrabIcon * hit.distance * 1.3f;
							grabIcon.transform.localScale = new Vector3 (Mathf.Min (newScale.x, grabIcon.transform.localScale.x * 1.2f), Mathf.Min (newScale.y, grabIcon.transform.localScale.y * 1.2f), Mathf.Min (newScale.z, grabIcon.transform.localScale.z * 1.2f)); 
						}
						else {
							grabIcon.GetComponent<Renderer> ().material = normalGrabIconMat;
							Vector3 newScale = initialScaleGrabIcon * hit.distance;
							grabIcon.transform.localScale = new Vector3 (Mathf.Min (newScale.x, grabIcon.transform.localScale.x * 1.2f), Mathf.Min (newScale.y, grabIcon.transform.localScale.y * 1.2f), Mathf.Min (newScale.z, grabIcon.transform.localScale.z * 1.2f)); 
						}

					}

					if (currentSelectionMode == selectionMode.laserpointer) {
						if (hit.rigidbody != null && hit.rigidbody.transform.parent != null) {
							collisionObject = hit.rigidbody.transform.parent.gameObject;
						} else {
							collisionObject = null;
						}
					} else {
						if (colliderSphere.currentCollider != null && colliderSphere.currentCollider.transform.parent != null) {
							collisionObject = colliderSphere.currentCollider.transform.parent.gameObject;
						} else {
							collisionObject = null;
						}
					}

					if (collisionObject != null) {

						if (grabIcon == null && currentSelectionMode == selectionMode.laserpointer) {
							grabIcon = Instantiate (grabIconPrefab);
						}


						if (currentFocus != collisionObject || recheckFocus) {
							count++;

							recheckFocus = false;

							if (collisionObject.CompareTag ("ModelingObject")) {		
								DeFocusCurrent (collisionObject);
								currentFocus = collisionObject;
								currentFocus.GetComponent<ModelingObject> ().Focus (this);
								device.TriggerHapticPulse (300);
								if (currentSelectionMode == selectionMode.laserpointer) {
									grabIcon.SetActive (true);
								}
							} else if (collisionObject.CompareTag ("Handle")) {
								DeFocusCurrent (collisionObject);
								currentFocus = collisionObject;
								currentFocus.GetComponent<handle> ().Focus (this);
								device.TriggerHapticPulse (600);
							} else if (collisionObject.CompareTag ("UiElement") && UiCanvasGroup.Instance.visible) {
								DeFocusCurrent (collisionObject);
								currentFocus = collisionObject;
								currentFocus.GetComponent<UiElement> ().Focus (this);
								device.TriggerHapticPulse (600);
							} else if (!UiCanvasGroup.Instance.visible && collisionObject.CompareTag ("TeleportTrigger")) {
								DeFocusCurrent (collisionObject);
								currentFocus = collisionObject;
								currentFocus.GetComponent<TeleportationTrigger> ().Focus (this);
								device.TriggerHapticPulse (600);
							} else if (collisionObject.CompareTag ("SelectionButton")) {
								DeFocusCurrent (collisionObject);
								currentFocus = collisionObject;
								currentFocus.GetComponent<ObjectSelecter> ().Focus (this);
								if (currentFocus.GetComponent<ObjectSelecter> ().connectedObject != null) {
									currentFocus.GetComponent<ObjectSelecter> ().connectedObject.Focus (this);
								}
								device.TriggerHapticPulse (600);
							} else if (!UiCanvasGroup.Instance.visible && collisionObject.CompareTag ("TeleportPosition")) {
								DeFocusCurrent (collisionObject);
								currentFocus = collisionObject;
								currentFocus.GetComponent<TeleportationPosition> ().Focus (this);
								device.TriggerHapticPulse (600);
							} else if (!UiCanvasGroup.Instance.visible && collisionObject.CompareTag ("Library")) {
								DeFocusCurrent (collisionObject);
								currentFocus = collisionObject;
								library.Instance.Focus (this);
								device.TriggerHapticPulse (600);

								if (currentSelectionMode == selectionMode.laserpointer) {
									grabIcon.transform.localScale = initialScaleGrabIcon * hit.distance; 
									grabIcon.SetActive (true);
								}

							} else if (collisionObject.CompareTag ("HeightControl")) {
								DeFocusCurrent (collisionObject);
								currentFocus = collisionObject;
								currentFocus.GetComponent<StageHeightController> ().Focus (this);
								device.TriggerHapticPulse (600);
								if (currentSelectionMode == selectionMode.laserpointer) {
									grabIcon.transform.localScale = initialScaleGrabIcon * hit.distance; 
									grabIcon.SetActive (true);
								}
							} else if (collisionObject.CompareTag ("DistanceControl")) {
								DeFocusCurrent (collisionObject);
								currentFocus = collisionObject;
								currentFocus.GetComponent<StageDistanceController> ().Focus (this);
								device.TriggerHapticPulse (600);
								if (currentSelectionMode == selectionMode.laserpointer) {
									grabIcon.transform.localScale = initialScaleGrabIcon * hit.distance; 
									grabIcon.SetActive (true);
								}
							} else if (collisionObject.CompareTag ("Stage") && typeOfController == controllerType.mainController) {
								DeFocusCurrent (collisionObject);
								currentFocus = collisionObject;

								currentFocus.GetComponent<StageFreeMovement> ().Focus (this);
								device.TriggerHapticPulse (300);
								if (currentSelectionMode == selectionMode.laserpointer) {
									grabIcon.transform.localScale = initialScaleGrabIcon * hit.distance; 
									grabIcon.SetActive (true);
								}
							} else if (collisionObject.CompareTag ("InfoPanel")) {
								DeFocusCurrent (collisionObject);
								currentFocus = collisionObject;
								currentFocus.GetComponent<Infopanel> ().Focus (this);
								device.TriggerHapticPulse (600);
							} else {
								DeFocusCurrent (null);
							}

						}

						if ((!UiCanvasGroup.Instance.visible || groupItemSelection) && collisionObject.CompareTag ("ModelingObject") && currentSelectionMode == selectionMode.laserpointer) {
							if (!grabIcon.activeSelf) {
								grabIcon.transform.localScale = initialScaleGrabIcon * hit.distance; 
								grabIcon.SetActive (true);
							}
						}



						if (grabIcon != null && grabIcon.activeSelf) {
							grabIcon.transform.LookAt (Camera.main.transform);
							grabIcon.transform.Rotate (new Vector3 (90, 0f, 0f));
							grabIcon.transform.position = pointOfCollision;
						}

						if (faceSelection) {
							// highlight face that is selected
							if (collidingFace != null) {
								collidingFace.UnHighlight ();
							}

							FindCollidingFace (pointOfCollision);

							if (collidingFace != null) {
								collidingFace.Highlight ();
							}

						}

					} else {
						if (currentFocus != null && collisionObject == null) {
							DeFocusCurrent (null);
							temps = Time.time;

							if (currentSelectionMode == selectionMode.laserpointer) {
								AdjustLengthPointer (50f);
							}
						}
					}
				} else {
					if (currentFocus != null || (currentSelectionMode == selectionMode.directTouch && collisionObject == null)) {
						DeFocusCurrent (null);
						temps = Time.time;
					}

					if (currentSelectionMode == selectionMode.laserpointer) {
						AdjustLengthPointer (50f);
					}
				} 
			} 

			/*
			if (device.GetTouchDown (SteamVR_Controller.ButtonMask.ApplicationMenu)) {
				buttonOnController.transform.localPosition = standardPosButton + new Vector3 (0f, -0.002f, 0f);

				if (!UiCanvasGroup.Instance.visible && currentFocus != null) {

					if (currentFocus.CompareTag ("ModelingObject")) {
						UiCanvasGroup.Instance.Show ();
						currentFocus.GetComponent<ModelingObject> ().objectSelector.Select (this, uiPositon.position);
					}

					if (currentFocus.CompareTag ("SelectionButton")) {
						UiCanvasGroup.Instance.Show ();
						currentFocus.GetComponent<ObjectSelecter> ().Select (this, uiPositon.position);
					}
				} else if (UiCanvasGroup.Instance.visible) {					
					UiCanvasGroup.Instance.CloseMenu (this);
				}
			} */

			if (device.GetTouchUp (SteamVR_Controller.ButtonMask.ApplicationMenu)) {
				buttonOnController.transform.localPosition = standardPosButton;
			}

			if (currentFocus != null) {
				if (device.GetTouchDown (SteamVR_Controller.ButtonMask.Trigger)) {
					triggerPressed = true;
					temps = Time.time;
				}

				if (triggerPressed && (currentFocus.CompareTag ("ModelingObject") || currentFocus.CompareTag ("TeleportPosition") || currentFocus.CompareTag ("Stage") || currentFocus.CompareTag ("Library") || currentFocus.CompareTag ("DistanceControl") || currentFocus.CompareTag ("HeightControl"))
				    && typeOfController == controllerType.mainController) {
					if (!movingObject && !faceSelection && !groupItemSelection) {

						if (grabbedIcon == null) {
							grabbedIcon = Instantiate (grabbedIconPrefab);
						}

						if (grabbedIcon != null && grabIcon != null) {
							grabbedIcon.SetActive (true);
							grabbedIconOffset = grabIcon.transform.position - currentFocus.transform.position; 

							grabbedIcon.transform.position = currentFocus.transform.position + grabbedIconOffset;
						}

						if (grabIcon != null) {
							grabIcon.SetActive (false);
						}

						CreatePointOfCollisionPrefab ();
						movingObject = true;

						duplicateHelp.Hide (false);

						if (currentFocus.CompareTag ("ModelingObject")) {
							//UiCanvasGroup.Instance.Hide ();

							Logger.Instance.AddLine (Logger.typeOfLog.triggerOnObject);

							if (circleAnimation != null) {
								//circleAnimation.StartAnimation ();
							}

							this.GetComponent<StageController> ().ShowPullVisual (true);
							otherController.transform.GetComponent<StageController> ().ShowRotateObjectVisual (true);

							device.TriggerHapticPulse (1800);
							// deactived fo push conferences
							//otherController.ActivateController (true);

						
							if (currentSelection != null && currentFocus != currentSelection){
								currentSelection.GetComponent<ModelingObject> ().DeSelect (this);
							}

							if (!duplicateMode) {			
								currentFocus.GetComponent<ModelingObject> ().StartMoving (this, currentFocus.GetComponent<ModelingObject> ());

								if (typeOfController == controllerType.mainController) {
									//settingsButtonHelp.HideCompletely (false);
								}

							} else {
								Logger.Instance.AddLine(Logger.typeOfLog.duplicateObject);

								if (currentFocus.GetComponent<ModelingObject> ().group == null){
                                    // needs to be local position
									ObjectCreator.Instance.DuplicateObject (currentFocus.GetComponent<ModelingObject> (), null, currentFocus.transform.localPosition);
								} else {
									ObjectCreator.Instance.DuplicateGroup(currentFocus.GetComponent<ModelingObject> ().group, Vector3.zero);
								}

								ModelingObject duplicatedObject = ObjectCreator.Instance.latestModelingObject;

								DeFocusCurrent (duplicatedObject.gameObject);

								if (currentSelection != null) {
									currentSelection.GetComponent<ModelingObject> ().DeSelect (this);
								}

								currentFocus = duplicatedObject.gameObject;
								duplicatedObject.Focus (this);
								duplicatedObject.StartMoving (this, duplicatedObject);

								if (typeOfController == controllerType.mainController) {
									//settingsButtonHelp.HideCompletely (false);
								}
							}

						} else if (currentFocus.CompareTag ("TeleportPosition")) {
							if (circleAnimation != null) {
								//circleAnimation.StartAnimation ();
							}
							this.GetComponent<StageController> ().ShowPullVisual (true);
							//otherController.transform.GetComponent<StageController> ().ShowRotateObjectVisual (true);
							//otherController.transform.GetComponent<StageController> ().ShowScaleRotationToggle (true);
							currentFocus.GetComponent<TeleportationPosition> ().StartMoving (this);

						} else if (currentFocus.CompareTag ("Library")) {
							if (circleAnimation != null) {
								//circleAnimation.StartAnimation ();
							}
							this.GetComponent<StageController> ().ShowPullVisual (true);
							library.Instance.StartMoving (this);
						} else if (currentFocus.CompareTag ("HeightControl")) {
							
							this.GetComponent<StageController> ().ShowPullVisual (true);
							currentFocus.GetComponent<StageHeightController> ().StartMoving (this);
							Logger.Instance.AddLine (Logger.typeOfLog.stage);

						} else if (currentFocus.CompareTag ("DistanceControl")) {
							
							this.GetComponent<StageController> ().ShowPullVisual (true);
							currentFocus.GetComponent<StageDistanceController> ().StartMoving (this);
							Logger.Instance.AddLine (Logger.typeOfLog.stage);


						} else if (currentFocus.CompareTag ("Stage")) {
							
							if (circleAnimation != null) {
								//circleAnimation.StartAnimation ();
							}

							this.GetComponent<StageController> ().ShowPullVisual (true);
							currentFocus.GetComponent<StageFreeMovement> ().StartMoving (this);
							Logger.Instance.AddLine (Logger.typeOfLog.stage);
						}
					}


				} else if (triggerPressed && (movingHandle || currentFocus.CompareTag ("Handle"))) {
					handle currentHandle = currentFocus.GetComponent<handle> ();

					if (!movingHandle) {
						CreatePointOfCollisionPrefab ();				

						if (currentHandle.typeOfHandle == handle.handleType.Rotation) {
							Logger.Instance.AddLine (Logger.typeOfLog.RotationHandle);
						} else if (currentHandle.typeOfHandle == handle.handleType.Height || currentHandle.typeOfHandle == handle.handleType.ScaleFace) {
							Logger.Instance.AddLine (Logger.typeOfLog.FrustumHandle);
						} else if (currentHandle.typeOfHandle == handle.handleType.ScaleX || currentHandle.typeOfHandle == handle.handleType.ScaleY || currentHandle.typeOfHandle == handle.handleType.ScaleZ
						            || currentHandle.typeOfHandle == handle.handleType.ScaleMinusX || currentHandle.typeOfHandle == handle.handleType.ScaleMinusY || currentHandle.typeOfHandle == handle.handleType.ScaleMinusZ) {
							Logger.Instance.AddLine (Logger.typeOfLog.nonUniformScaleHandle);
						} 
					}

					this.GetComponent<StageController> ().ShowPullVisual (true);
					currentHandle.ApplyChanges (pointOfCollisionGO, movingHandle, this);

					//currentHandle.connectedObject.GetComponent<ModelingObject> ().HideBoundingBox ();
					movingHandle = true;
				}
			}


			if (device.GetTouchUp (SteamVR_Controller.ButtonMask.Trigger)) {
				triggerPressed = false;
				grabbedIconOffset = Vector3.zero;

				if (movingHandle){
					currentFocus.GetComponent<handle> ().FinishUsingHandle ();
					movingHandle = false;

					if (typeOfController == controllerType.mainController) {
						this.GetComponent<StageController> ().ShowPullVisual (false);
					} else {
						if (otherController.movingObject) {
							this.GetComponent<StageController> ().ShowRotateObjectVisual (true);
						} else {
							this.GetComponent<StageController> ().ShowPullVisual (false);
						}
					}
				}


				if (movingObject && typeOfController == controllerType.mainController) {
					otherController.triggerPressed = false;
					otherController.movingHandle = false;

					recheckFocus = true;

					if (currentFocus.CompareTag ("ModelingObject")) {	
						if (circleAnimation != null) {
							//circleAnimation.StartAnimation ();
						}

						device.TriggerHapticPulse (1800);
						currentFocus.GetComponent<ModelingObject> ().StopMoving (this, currentFocus.GetComponent<ModelingObject> ());

						if (!duplicateMode) {
							otherController.ActivateController (false);
						}

						this.GetComponent<StageController> ().ShowPullVisual (false);
						otherController.transform.GetComponent<StageController> ().ShowRotateObjectVisual (false);

						if (currentFocus.GetComponent<ModelingObject> ().inTrashArea) {
							UiCanvasGroup.Instance.CloseMenu (this);
							currentFocus.GetComponent<ModelingObject> ().TrashObject (true);
							trashInfopanel.CloseInfoPanel ();

							//currentFocus = null;
							device.TriggerHapticPulse (1000);
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
						this.GetComponent<StageController> ().ShowPullVisual (false);
						Teleportation.Instance.JumpToPos (5);
					} else if (currentFocus.CompareTag ("Library")) {
						this.GetComponent<StageController> ().ShowPullVisual (false);
						library.Instance.StopMoving (this);
					} else if (currentFocus.CompareTag ("HeightControl")) {
						this.GetComponent<StageController> ().ShowPullVisual (false);
						currentFocus.GetComponent<StageHeightController> ().StopMoving (this);
					} else if (currentFocus.CompareTag ("DistanceControl")) {
						this.GetComponent<StageController> ().ShowPullVisual (false);
						currentFocus.GetComponent<StageDistanceController> ().StopMoving (this);
					} else if (currentFocus.CompareTag ("Stage")) {
						this.GetComponent<StageController> ().ShowPullVisual (false);
						currentFocus.GetComponent<StageFreeMovement> ().StopMoving (this);
					}

				}

				///Destroy (pointOfCollisionGO);
				if (pointOfCollisionGO != null) {
					pointOfCollisionGO.SetActive(false);
				}

				movingObject = false;

				duplicateHelp.Hide (true);

				if (currentFocus != null) {

					if (currentFocus.CompareTag ("ModelingObject")) {

						/*
						// this is not working
						if (!groupItemSelection && currentSelection != null && currentSelection != currentFocus) {
							UiCanvasGroup.Instance.CloseMenu (this);
							this.enableFaceSelection (false);
						}
						*/

						if (groupItemSelection) {
							// we could also display and icon (add to group)
							if (currentFocus.GetComponent<ModelingObject> ().group == null) {
								ObjectsManager.Instance.AddObjectToGroup (ObjectsManager.Instance.currentGroup, currentFocus.GetComponent<ModelingObject> ());
							} else {
								if (currentFocus.GetComponent<ModelingObject> ().group != ObjectsManager.Instance.currentGroup) {
									ObjectsManager.Instance.AddAllObjectsOfGroupToGroup (ObjectsManager.Instance.currentGroup, currentFocus.GetComponent<ModelingObject> ().group);
								}

							}


						} else if (faceSelection) {
							if (collidingFace != null) {
								this.enableFaceSelection (false);
								otherController.enableFaceSelection (false);
								collidingFace.CreateNewModelingObject ();
								SelectLatestObject ();
								UiCanvasGroup.Instance.shapeMenu.ActivateMenu ();
								collidingFace = null;
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
							device.TriggerHapticPulse (1000);
							tempsUI = Time.time;
							currentFocus.GetComponent<UiElement> ().PerformAction (this);

							if (currentFocus.GetComponent<UiElement> ().goal != null) {
								currentFocus.GetComponent<UiElement> ().goal.ActivateMenu ();
							}

							Logger.Instance.AddLine (Logger.typeOfLog.uiElement);
						}
					} else if (currentFocus.CompareTag ("InfoPanel")) {
						device.TriggerHapticPulse (1000);
						currentFocus.GetComponent<Infopanel> ().CloseInfoPanel ();
					} else if (currentFocus.CompareTag ("TeleportTrigger")) {
						Teleportation.Instance.JumpToPos (currentFocus.GetComponent<TeleportationTrigger> ().triggerPos);
					}

				}

				temps = 0;
			}
		} else {
			if (grabbedIcon != null) {
				grabbedIcon.SetActive (false);
			}

			if (grabIcon != null) {
				grabIcon.SetActive (false);
			}
		}

		if (device.GetTouchUp (SteamVR_Controller.ButtonMask.Trigger)) {
			if (grabbedIcon != null) {
				grabbedIcon.SetActive (false);
			}

			if (currentSelectionMode == selectionMode.directTouch) {
				grabAnimation.Open ();
			}
		}

		if (typeOfController == controllerType.mainController) {
			if (device.GetTouchDown (SteamVR_Controller.ButtonMask.Trigger) && currentFocus == null) {
				Logger.Instance.AddLine (Logger.typeOfLog.triggerNoTarget);
			}
		}


		if (typeOfController == controllerType.SecondaryController) {
			if (device.GetTouchDown (SteamVR_Controller.ButtonMask.Trigger) && !scalingMode) {
				otherController.duplicateMode = true;
				duplicateHelp.DuplicateActive ();
			}

			if (device.GetTouchUp (SteamVR_Controller.ButtonMask.Trigger)) {
				otherController.duplicateMode = false;
				duplicateHelp.DuplicateNotActive ();
			}
		}   

		if (device.GetTouchDown (SteamVR_Controller.ButtonMask.ApplicationMenu)) {
			buttonOnController.transform.localPosition = standardPosButton + new Vector3 (0f, -0.002f, 0f);
			ToggleOnOffHelp ();
		}

		if (device.GetTouchUp (SteamVR_Controller.ButtonMask.ApplicationMenu)) {
			buttonOnController.transform.localPosition = standardPosButton;
		}
    }

	public void ToggleOnOffHelp(){
		duplicateHelp.ToggleOnOff ();
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

	public void CreatePointOfCollisionPrefab()
    {
		if (pointOfCollisionGO == null) {
			pointOfCollisionGO = (GameObject)Instantiate (pointOfCollisionPrefab, pointOfCollision, new Quaternion (0f, 0f, 0, 0f));
			pointOfCollisionGO.transform.SetParent (transform);
		} else {
			pointOfCollisionGO.SetActive (true);
			pointOfCollisionGO.transform.position = pointOfCollision;
			pointOfCollisionGO.transform.rotation = new Quaternion (0f, 0f, 0, 0f);
		}
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

		if (typeOfController == controllerType.SecondaryController) {
			CreatePointOfCollisionPrefab();
		}

		scalingObject = true;

		if (grabbedIcon == null) {
			grabbedIcon = Instantiate (grabbedIconPrefab);
			grabbedIcon.SetActive (false);
		}

		if (!grabbedIcon.activeSelf) {
			grabbedIcon.SetActive (true);

			if (grabIcon != null) {
				grabIcon.SetActive (false);
			}

			//grabbedIconOffset = grabIcon.transform.position - currentFocus.transform.position; 
			//grabbedIcon.transform.position = currentFocus.transform.position + grabbedIconOffset;
		}
	}
}
