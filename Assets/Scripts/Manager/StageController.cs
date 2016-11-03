using UnityEngine;
using System.Collections;

public class StageController : MonoBehaviour {

    public enum RotationScalingTechnique { touchpads, gesture };
    public RotationScalingTechnique currentRotationScalingTechnique;

    public enum controllerMode { scalingStage, rotatingStage, pullPushObject, toggleRotateScale, rotatingObject };
	public controllerMode standardControllerMode;
	public controllerMode currentControllerMode;

	public bool freeMovementStage;

    public Transform stage;
    public Transform library;
    public Transform trash;
    private float lastY;
    private float lastX;
    private bool touchDown;
	private bool grip;
    SteamVR_TrackedObject trackedObj;

    [HideInInspector]
    public bool scaleMode;
    public Selection selection;

	public GameObject pullIcon;
	public GameObject rotateIcon;
	public GameObject normalIcon;

	public GameObject line;
	private Vector3 lineInitialScale;
	public Color lineColorNormal;
	public Color lineColorTouch;
	public GameObject iconBG;

	public bool recognizeNewSwipe;

	public Vector3 minPosLine;
	public Vector3 maxPosLine;

	public GameObject touchpad;
	private Vector3 touchpadInitialScale;
	private Vector3 touchpadInitialPos;

	private float amountOfMovementForHapticFeedback;

	private float smoothTime = 0.01F;
	private float velocity = 0.0F;

	private Quaternion lastRotation;
	private float lastDistance;
	private Vector3 lastVector;
	public RotationScalingVisualization rotScalVis;
	public NavigationGestureHelp navHelp;

    void Awake()
    {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
       // selection = this.GetComponent<Selection>();

		lineInitialScale = line.transform.localScale;
		touchpadInitialScale = touchpad.transform.localScale;
		touchpadInitialPos = touchpad.transform.localPosition;

		if (selection.typeOfController == Selection.controllerType.mainController)
		{
			scaleMode = true;

			// move line to point
			float posLine = (stage.parent.localScale.x - 0.25f) / 3.75f;
			line.transform.localPosition = minPosLine + posLine * (maxPosLine - minPosLine);

			float scale = Mathf.Sqrt((Mathf.Abs(posLine - 0.5f)) / 0.5f);
			line.transform.localScale = new Vector3 (Mathf.Max(lineInitialScale.x * (1f-scale), lineInitialScale.x*0.7f), lineInitialScale.y, lineInitialScale.z);
		} else
		{
			scaleMode = false;

			// move line to point for rotation
			// move line to point
			float posLine = ((stage.localRotation.eulerAngles.y - (Mathf.Floor(stage.localRotation.eulerAngles.y/90f) * 90f)) / 90f);
			line.transform.localPosition = minPosLine + posLine * (maxPosLine - minPosLine);

			float scale = Mathf.Sqrt((Mathf.Abs(posLine - 0.5f)) / 0.5f);
			line.transform.localScale = new Vector3 (Mathf.Max(lineInitialScale.x * (1f-scale), lineInitialScale.x*0.7f), lineInitialScale.y, lineInitialScale.z);
		}

		if (selection.typeOfController == Selection.controllerType.mainController) {
			ShowPullVisual (false);
		} else {
			ShowRotateObjectVisual (false);
		}
			

    }

	public void UpdateRotationScalingTechnique(){
		if (selection.typeOfController == Selection.controllerType.mainController) {
			ShowPullVisual (false);
		} else {
			ShowRotateObjectVisual (false);
		}

		if (currentRotationScalingTechnique == RotationScalingTechnique.gesture) {
			selection.ShowHelpVisual ();
			navHelp.Hide (false);
		} else {
			navHelp.Hide (true);
		}

		// hide or show help for navigation

	}


	public void ShowPullVisual(bool value){		
		if (value) {
			currentControllerMode = controllerMode.pullPushObject;
			pullIcon.SetActive (true);
			rotateIcon.SetActive (false);
			normalIcon.SetActive (false);
			line.SetActive (true);
			iconBG.SetActive (true);
		} else {
			currentControllerMode = standardControllerMode;
			pullIcon.SetActive (false);
			rotateIcon.SetActive (false);
			if (currentRotationScalingTechnique == RotationScalingTechnique.touchpads) {
				normalIcon.SetActive (true);
				iconBG.SetActive (true);
			} else {
				normalIcon.SetActive (false);
				pullIcon.SetActive (true);
				line.SetActive (false);
				iconBG.SetActive (false);
			}
		}
	}

	public void ShowRotateObjectVisual(bool value){			
		if (value) {
			currentControllerMode = controllerMode.rotatingObject;
			rotateIcon.SetActive (true);
			pullIcon.SetActive (false);
			normalIcon.SetActive (false);
			line.SetActive (true);
		} else {
			currentControllerMode = standardControllerMode;
			rotateIcon.SetActive (false);
			pullIcon.SetActive (false);
		
			if (currentRotationScalingTechnique == RotationScalingTechnique.touchpads) {
				normalIcon.SetActive (true);
				iconBG.SetActive (true);
			} else {
				normalIcon.SetActive (false);
				line.SetActive (false);
				rotateIcon.SetActive (true);
				iconBG.SetActive (false);
			}
		}
	}


    // Update is called once per frame
    void Update()
    {		
        var device = SteamVR_Controller.Input((int)trackedObj.index);

		if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad)){
            touchDown = true;

            lastX = device.GetAxis().x;
            lastY = device.GetAxis().y;

			amountOfMovementForHapticFeedback = 0f;

			if (currentRotationScalingTechnique == RotationScalingTechnique.touchpads
				|| (currentRotationScalingTechnique == RotationScalingTechnique.gesture 
					&& (currentControllerMode == controllerMode.pullPushObject || currentControllerMode == controllerMode.rotatingObject))) {
				
				LeanTween.color (line, lineColorTouch, 0.1f);
				LeanTween.scale (touchpad, touchpadInitialScale * 1.2f, 0.2f); 
				LeanTween.moveLocalY (touchpad, touchpadInitialPos.y + 0.003f, 0.2f);

				if (currentControllerMode != controllerMode.pullPushObject) {
					device.TriggerHapticPulse (2000);
				}

				if (currentControllerMode == controllerMode.pullPushObject) {
					Logger.Instance.AddLine (Logger.typeOfLog.touchpadMoveObject);
				} else if (currentControllerMode == controllerMode.scalingStage) {
					Logger.Instance.AddLine (Logger.typeOfLog.touchpadScaleStage);
				} else if (currentControllerMode == controllerMode.rotatingStage) {
					Logger.Instance.AddLine (Logger.typeOfLog.touchpadRotateStage);
				} else if (currentControllerMode == controllerMode.rotatingObject) {
					Logger.Instance.AddLine (Logger.typeOfLog.touchpadRotateObject);
				}
			}

        }

		if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            touchDown = false;

			if (currentRotationScalingTechnique == RotationScalingTechnique.touchpads
			    || (currentRotationScalingTechnique == RotationScalingTechnique.gesture
			    && (currentControllerMode == controllerMode.pullPushObject || currentControllerMode == controllerMode.rotatingObject))) {
				
				LeanTween.color (line, lineColorNormal, 0.1f);
				LeanTween.scale (touchpad, touchpadInitialScale, 0.2f).setDelay (0.1f); 
				LeanTween.moveLocalY (touchpad, touchpadInitialPos.y, 0.2f).setDelay (0.1f);

				device.TriggerHapticPulse (1400);

				if (currentControllerMode == controllerMode.rotatingObject && selection.otherController.currentFocus != null) {
					ModelingObject currentModelingObject = selection.otherController.currentFocus.GetComponent<ModelingObject> ();
					currentModelingObject.handles.ShowNonUniformScalingHandles ();
				}
			}
        }

		if (currentRotationScalingTechnique == RotationScalingTechnique.gesture && device.GetTouchDown (SteamVR_Controller.ButtonMask.Grip) && selection.typeOfController == Selection.controllerType.mainController) {			
			// Check if other grip is also pressed, maybe one is enough

			selection.ActivateController (false);

			grip = true;

			// get angle between contollers at point of press 
			Vector3 VectorBetweenControllers = transform.position- selection.otherController.transform.position;
			VectorBetweenControllers = new Vector3 (VectorBetweenControllers.x, 0f, VectorBetweenControllers.z);

			Vector3 posController1 = new Vector3 (transform.position.x, 0f, transform.position.z);
			Vector3 posController2 = new Vector3 (selection.otherController.transform.position.x, 0f, selection.otherController.transform.position.z);
			lastRotation = Quaternion.LookRotation(posController1 - posController2);

			lastDistance = Mathf.Abs((transform.position - selection.otherController.transform.position).magnitude);
			lastVector = posController1 - posController2;
			// create line between both controllers

			Logger.Instance.AddLine (Logger.typeOfLog.gestureNavigation);
		}

		if (currentRotationScalingTechnique == RotationScalingTechnique.gesture && device.GetTouchUp (SteamVR_Controller.ButtonMask.Grip) && selection.typeOfController == Selection.controllerType.mainController) {			
			grip = false;
			selection.ActivateController (true);
			rotScalVis.HideVisualization();
		}


        if (touchDown)
        {
			if (currentControllerMode == controllerMode.pullPushObject) {
				// turn y value into scale of stage          
				float amountY = Mathf.SmoothDamp (lastY, device.GetAxis ().y, ref velocity, smoothTime) - lastY;
				lastY = device.GetAxis ().y;

				amountOfMovementForHapticFeedback += amountY;

				if (amountOfMovementForHapticFeedback > 2f) {
					device.TriggerHapticPulse (1000);
					amountOfMovementForHapticFeedback = 0f;
				}

				if (selection.pointOfCollisionGO != null) {
					
					// get vector between controller and current object
					Vector3 ObjectToController = selection.pointOfCollisionGO.transform.position - transform.position;

					// move object on line when using touchpad
					selection.pointOfCollisionGO.transform.position = selection.pointOfCollisionGO.transform.position + ObjectToController * amountY;

					if (selection.currentFocus.CompareTag ("ModelingObject")) {
						selection.pointOfCollisionGO.transform.position = selection.pointOfCollisionGO.transform.position;
					}

					float distance = (selection.pointOfCollisionGO.transform.position - transform.position).magnitude;

					// somehow not working
					selection.AdjustLengthPointer (distance);

					// move line to point
					float posLine = ((distance - (Mathf.Floor (distance / 5f) * 5f)) / 5f);
					line.transform.localPosition = minPosLine + posLine * (maxPosLine - minPosLine);

					float scale = Mathf.Sqrt ((Mathf.Abs (posLine - 0.5f)) / 0.5f);
					line.transform.localScale = new Vector3 (Mathf.Max (lineInitialScale.x * (1f - scale), lineInitialScale.x * 0.7f), lineInitialScale.y, lineInitialScale.z);
				} 

			} else if (currentRotationScalingTechnique == RotationScalingTechnique.touchpads && currentControllerMode == controllerMode.scalingStage) {
				// turn y value into scale of stage          
				float amountY = Mathf.SmoothDamp (lastY, device.GetAxis ().y, ref velocity, smoothTime) - lastY;
				lastY = device.GetAxis ().y;

				amountOfMovementForHapticFeedback += amountY;

				if (amountOfMovementForHapticFeedback > 2f) {
					device.TriggerHapticPulse (1000);
					amountOfMovementForHapticFeedback = 0f;
				}

				ScaleStage (amountY);

				if (selection.currentFocus != null) {
					if (selection.currentFocus.CompareTag ("ModelingObject")) {
						selection.currentFocus.GetComponent<ModelingObject> ().objectSelector.RePosition (Camera.main.transform.position);
					} else if (selection.currentFocus.CompareTag ("SelectionButton")) {
						selection.currentFocus.GetComponent<ObjectSelecter> ().RePosition (Camera.main.transform.position);
					}
				}
			} else if (currentRotationScalingTechnique == RotationScalingTechnique.touchpads && currentControllerMode == controllerMode.rotatingStage) {
				// turn x value into rotation of stage
				float amountX = Mathf.SmoothDamp (lastX, device.GetAxis ().x, ref velocity, smoothTime) - lastX;
				lastX = device.GetAxis ().x;

				amountOfMovementForHapticFeedback += amountX;

				if (amountOfMovementForHapticFeedback > 2f) {
					device.TriggerHapticPulse (1000);
					amountOfMovementForHapticFeedback = 0f;
				}

				RotateStage (amountX * 25f);

				if (selection.currentFocus != null) {
					if (selection.currentFocus.CompareTag ("ModelingObject")) {
						selection.currentFocus.GetComponent<ModelingObject> ().objectSelector.RePosition (Camera.main.transform.position);
					} else if (selection.currentFocus.CompareTag ("SelectionButton")) {
						selection.currentFocus.GetComponent<ObjectSelecter> ().RePosition (Camera.main.transform.position);
					}
				}

				if (selection.currentSelection != null) {
					selection.currentSelection.GetComponent<ModelingObject> ().PositionHandles (true);
				}
			} else if (currentControllerMode == controllerMode.rotatingObject) {
				if (selection.otherController.currentFocus != null && selection.otherController.currentFocus.CompareTag ("ModelingObject")) {

					// turn x value into rotation of stage
					float amountX = RasterManager.Instance.RasterAngle ((device.GetAxis ().x - lastX) * 15f);
					lastX = device.GetAxis ().x;

					//amountOfMovementForHapticFeedback += amountX;

					if (amountOfMovementForHapticFeedback > 2f) {
						device.TriggerHapticPulse (1000);
						amountOfMovementForHapticFeedback = 0f;
					}

					ModelingObject currentModelingObject = selection.otherController.currentFocus.GetComponent<ModelingObject> ();

					if (currentModelingObject.group == null) {
						currentModelingObject.handles.DisableHandles ();
						currentModelingObject.HideBoundingBox (false);

						//currentModelingObject
						Vector3 bbCenterBeforeRotation = currentModelingObject.transform.InverseTransformPoint (currentModelingObject.GetBoundingBoxCenter ());

						// define rotation axis
						Vector3 p1Rotation = currentModelingObject.GetBoundingBoxCenter ();
						Vector3 p2Rotation = p1Rotation + Vector3.up;

						Vector3 rotationAxis = p2Rotation - p1Rotation;

						currentModelingObject.RotateAround (rotationAxis, amountX, bbCenterBeforeRotation); 	

						// Should also have visual, or we kick it

					} else {
						// if there is time, group rotation

					}
				}
			}


        }

		if (grip) {
			Vector3 posController1 = new Vector3 (transform.position.x, 0f, transform.position.z);
			Vector3 posController2 = new Vector3 (selection.otherController.transform.position.x, 0f, selection.otherController.transform.position.z);
			Vector3 newVector = posController1 - posController2;

			Quaternion newRotation = Quaternion.LookRotation(newVector);
			float difference = Quaternion.Angle (newRotation, lastRotation);


			Vector3 crossProduct = Vector3.Cross (newVector, lastVector);
			lastVector = newVector;

			if (crossProduct.y > 0) {
				difference = difference * (-1f);
			}
			
			lastRotation = newRotation;

		//	Debug.Log ("difference bei " + difference);

			rotScalVis.ShowVisualization ();

			RotateStage (difference);

			float scalingAmount = (Mathf.Abs((transform.position - selection.otherController.transform.position).magnitude)) - lastDistance;
			lastDistance = (Mathf.Abs ((transform.position - selection.otherController.transform.position).magnitude));

			ScaleStage (scalingAmount);
		}

    }

    public void DoClick(object sender, ClickedEventArgs e)
    {

    }


	public void RotateStage(float amountX){
		stage.Rotate(0, amountX, 0);

		// move line to point
		float posLine = ((stage.localRotation.eulerAngles.y - (Mathf.Floor(stage.localRotation.eulerAngles.y/90f) * 90f)) / 90f);
		line.transform.localPosition = minPosLine + posLine * (maxPosLine - minPosLine);

		float scale = Mathf.Sqrt((Mathf.Abs(posLine - 0.5f)) / 0.5f);
		line.transform.localScale = new Vector3 (Mathf.Max(lineInitialScale.x * (1f-scale), lineInitialScale.x*0.7f), lineInitialScale.y, lineInitialScale.z);
	}

	public void ScaleStage(float amountY){
		
		Vector3 scaleStage = stage.parent.localScale;
		scaleStage = scaleStage + (scaleStage * amountY);

	//	Vector3 libraryStage = library.localScale;
	//	libraryStage = libraryStage + (libraryStage * amountY);

		if (scaleStage.x >= 0.5f && scaleStage.x <= 4f)
		{
			stage.parent.localScale = scaleStage; 
		//	library.localScale = libraryStage;

			// move line to point
			float posLine = (scaleStage.x - 0.25f) / 3.75f;
			line.transform.localPosition = minPosLine + posLine * (maxPosLine - minPosLine);

			float scale = Mathf.Sqrt((Mathf.Abs(posLine - 0.5f)) / 0.5f);
			line.transform.localScale = new Vector3 (Mathf.Max(lineInitialScale.x * (1f-scale), lineInitialScale.x * 0.7f), lineInitialScale.y, lineInitialScale.z);
		}
	}
}
