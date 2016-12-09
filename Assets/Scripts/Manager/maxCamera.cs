using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


[AddComponentMenu("Camera-Control/3dsMax Camera Style")]
public class maxCamera : MonoBehaviour {

	public Selection selection;

	public Transform target;
	public Vector3 targetOffset;
	public float distance = 5.0f;
	public float maxDistance = 20;
	public float minDistance = .6f;
	public float xSpeed = 200.0f;
	public float ySpeed = 200.0f;
	public int yMinLimit = -80;
	public int yMaxLimit = 80;
	public int zoomRate = 40;
	public float panSpeed = 0.3f;
	public float zoomDampening = 5.0f;

	private float xDeg = 0.0f;
	private float yDeg = 0.0f;
	private float currentDistance;
	private float desiredDistance;
	private Quaternion currentRotation;
	private Quaternion desiredRotation;
	private Quaternion rotation;
	private Vector3 position;

	public bool moving;

	void Start() { Init(); }
	void OnEnable() { Init(); }

	public void Init()
	{
		//If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
		if (!target)
		{
			GameObject go = new GameObject("Cam Target");
			go.transform.position = transform.position + (transform.forward * distance);
			target = go.transform;
		}

		distance = Vector3.Distance(transform.position, target.position);
		currentDistance = distance;
		desiredDistance = distance;

		//be sure to grab the current rotations as starting points.
		position = transform.position;
		rotation = transform.rotation;
		currentRotation = transform.rotation;
		desiredRotation = transform.rotation;

		xDeg = Vector3.Angle(Vector3.right, transform.right );
		yDeg = Vector3.Angle(Vector3.up, transform.up );
	}

	public void MoveCameraCenterToObject(){
		moving = true;

		ModelingObject currentObj = selection.currentSelection.GetComponent<ModelingObject> ();

		currentObj.handles.HideRotationHandlesExcept (null);
		currentObj.handles.HideScalingHandlesExcept (null);
		currentObj.connectingLinesHandles.ClearLines ();

		LeanTween.move (target.gameObject, currentObj.GetBoundingBoxCenter(), 0.8f).setEase (LeanTweenType.easeInOutExpo).setOnComplete(ReachGoal);

		// maybe also implement move closer
	}

	public void ReachGoal(){

		ModelingObject currentObj = selection.currentSelection.GetComponent<ModelingObject> ();

		currentObj.PositionHandles (true);
		currentObj.RotateHandles ();
		currentObj.handles.ShowNonUniformScalingHandles ();
		currentObj.handles.ShowRotationHandles ();
		currentObj.DrawConnectingLines ();

		moving = false;
	}

	/*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
	void LateUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Space)) {
			if (selection.currentSelection != null) {
				MoveCameraCenterToObject ();
			}
		}

		if (!EventSystem.current.IsPointerOverGameObject ()) {
			
			// If Control and Alt and Middle button? ZOOM!
			if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
			{
				moving = true;

				desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate*0.125f * Mathf.Abs(desiredDistance);
			}
			// If middle mouse and left alt are selected? ORBIT
			else if (Input.GetMouseButton(1) || (selection.currentFocus == null && Input.GetMouseButton(0)))
			{
				moving = true;

				xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
				yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

				////////OrbitAngle

				//Clamp the vertical axis for the orbit
				yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
				// set camera rotation 
				desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
				currentRotation = transform.rotation;

				rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
				transform.rotation = rotation;
				//transform.localRotation = Quaternion.Euler (transform.localRotation.x, transform.localRotation.y, 0f);
			}
			// otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
			else if (Input.GetMouseButton(2))
			{
				moving = true;

				//grab the rotation of the camera so we can move in a psuedo local XY space
				target.rotation = transform.rotation;
				target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
				target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
			} 

		}

		if (Input.GetMouseButtonUp (0) || Input.GetMouseButtonUp (1) || Input.GetMouseButtonUp (2)) {
			moving = false;
		}

		////////Orbit Position

		// affect the desired Zoom distance if we roll the scrollwheel
		desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
		//clamp the zoom min/max
		desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
		// For smoothing of the zoom, lerp distance
		currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

		// calculate position based on the new currentDistance 
		position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
		transform.position = position;
	}

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp(angle, min, max);
	}
}
