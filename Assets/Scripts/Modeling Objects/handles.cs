using UnityEngine;
using System.Collections;

public class handles : MonoBehaviour {

    public GameObject faceTopScale;
    public GameObject faceBottomScale;
    public GameObject CenterTopPosition;
    public GameObject CenterBottomPosition;
    public GameObject HeightTop;
    public GameObject HeightBottom;

    public Transform RotationHandles;

    public GameObject RotateX;
    public GameObject RotateY;
    public GameObject RotateZ;
   
    public GameObject TopHandles;
    public GameObject BottomHandles;

	public GameObject NonUniformScalingHandles;
	public GameObject NonUniformScaleFront;
	public GameObject NonUniformScaleBack;
	public GameObject NonUniformScaleTop;
	public GameObject NonUniformScaleBottom;
	public GameObject NonUniformScaleLeft;
	public GameObject NonUniformScaleRight;

	public GameObject MovementHandleGroup;
	public GameObject YMovement;
	public GameObject UniformScale;

	public GameObject RotateAndTranslate;

	private ModelingObject connectedModelingObject;

    public bool objectFocused;

	public Transform Handlegroup;

	public GameObject linesGO;
	public GameObject rotationSliceGO;

	public GameObject connectingLines;

    // Use this for initialization
    void Start () {
        objectFocused = false;
		connectedModelingObject = transform.parent.GetComponent<ModelingObject> ();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void DisableHandles()
    {
        foreach (Transform handle in transform.GetChild(0)) {
            handle.gameObject.SetActive(false);
        }

        foreach (Transform handle in RotationHandles)
        {
            handle.gameObject.SetActive(false);
        }
    }

    public void ShowRotationHandles()
    {
        //DisableHandles();
		connectedModelingObject.PositionHandles(true);
		connectedModelingObject.RotateHandles();
		connectedModelingObject.ShowBoundingBox (false);
		connectingLines.SetActive (true);
    }

	public void HideRotationHandlesExcept(handle certainHandle){
		foreach (Transform handle in RotationHandles)
		{
			if (certainHandle == null || handle != certainHandle.transform) {
				handle.gameObject.SetActive(false);
			}
		}
	}

	public void HideScalingHandlesExcept(handle certainHandle){
		connectedModelingObject.connectingLinesHandles.ClearLines ();

		foreach (Transform handle in NonUniformScalingHandles.transform)
		{
			if (certainHandle == null || handle != certainHandle.transform) {
				handle.gameObject.SetActive(false);
			}
		}

		if (certainHandle == null || YMovement.transform != certainHandle.transform) {
			YMovement.SetActive (false);
		}

		if (certainHandle == null || UniformScale.transform != certainHandle.transform) {
			UniformScale.SetActive (false);
		}

		if (certainHandle == null || RotateAndTranslate.transform != certainHandle.transform) {
			RotateAndTranslate.SetActive (false);
		}

	}

	public void ShowNonUniformScalingHandles() {

		TopHandles.SetActive(false);
		BottomHandles.SetActive(false);

		faceBottomScale.SetActive(false);
		faceTopScale.SetActive(false);

		HeightTop.SetActive(false);
		HeightBottom.SetActive(false);

		if (connectedModelingObject.group == null) {

			ShowRotationHandles ();
			Handlegroup.gameObject.SetActive (true);

			YMovement.SetActive (true);
			MovementHandleGroup.SetActive (true);
			UniformScale.SetActive (true);

			connectedModelingObject.ShowBoundingBox (false);
			connectedModelingObject.DrawConnectingLines ();
			connectingLines.SetActive (true);

			//RotateAndTranslate.SetActive (true);

			if (!ProModeMananager.Instance.beginnersMode) {
				NonUniformScalingHandles.SetActive (true);
				NonUniformScaleFront.SetActive (true);
				NonUniformScaleBack.SetActive (true);
				NonUniformScaleTop.SetActive (true);
				NonUniformScaleBottom.SetActive (true);
				NonUniformScaleLeft.SetActive (true);
				NonUniformScaleRight.SetActive (true);

			} else {
				NonUniformScalingHandles.SetActive (false);
				NonUniformScaleFront.SetActive (false);
				NonUniformScaleBack.SetActive (false);
				NonUniformScaleTop.SetActive (false);
				NonUniformScaleBottom.SetActive (false);
				NonUniformScaleLeft.SetActive (false);
				NonUniformScaleRight.SetActive (false);
			}
		}


	}

    public void ShowFrustumHandles() {
		
        DisableHandles();
		connectedModelingObject.RepositionScalers ();
		connectedModelingObject.PositionHandles (false);
		connectedModelingObject.RotateHandles();

        //transform.parent.GetComponent<ModelingObject>().PositionHandles();
        //transform.parent.GetComponent<ModelingObject>().RotateHandles();

        TopHandles.SetActive(true);
        BottomHandles.SetActive(true);

        faceBottomScale.SetActive(true);
        faceTopScale.SetActive(true);

        HeightTop.SetActive(true);
        HeightBottom.SetActive(true);

        // change later
        //CenterBottomPosition.SetActive(true);
        //CenterTopPosition.SetActive(true);
    }

    public void ShowFrustumCenterHandles()
    {
        DisableHandles();
		connectedModelingObject.PositionHandles(false);
        CenterBottomPosition.SetActive(true);
        CenterTopPosition.SetActive(true);
    }

}
