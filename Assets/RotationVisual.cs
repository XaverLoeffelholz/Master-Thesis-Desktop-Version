using UnityEngine;
using System.Collections;

public class RotationVisual : MonoBehaviour {

	public GameObject firstSlice;
	public GameObject bigSlice;
    public Transform SlicesContainer;

	public GameObject RightangleVisual;

	public GameObject RotationPlane;
	public GameObject RotationPlaneBack;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
      
    }

	public void ColorCircle(Color color){
		RotationPlane.GetComponent<Renderer> ().material.color = color;
		RotationPlaneBack.GetComponent<Renderer> ().material.color = color;
	}

	public void RotationVisualisation(Vector3 point1, Vector3 point2, bool smooth)
    {
        float startAngle = Vector3.Angle(transform.up, point1 - transform.position);
        float directionStart = Mathf.Sign(Vector3.Dot(Vector3.Cross(transform.up, point1 - transform.position), transform.forward));

        float endangle = Vector3.Angle(transform.up, point2 - transform.position);
        float directionEnd = Mathf.Sign(Vector3.Dot(Vector3.Cross(transform.up, point2 - transform.position), transform.forward));
       
		if (smooth) {
			CreateSlices(RasterManager.Instance.RasterAngleSmooth(startAngle * directionStart), RasterManager.Instance.RasterAngleSmooth(endangle * directionEnd), smooth);
		} else {
			CreateSlices(RasterManager.Instance.RasterAngle(startAngle * directionStart), RasterManager.Instance.RasterAngle(endangle * directionEnd), smooth);
		}

    }

	public void ShowRightAngleVisual(bool value){
		RightangleVisual.SetActive (value);
	}

	public void CreateSlices(float startAngle, float endangle, bool smooth){

        foreach(Transform child in SlicesContainer)
        {
            Destroy(child.gameObject);
        }

        if (startAngle < 0)
        {
            startAngle += 360;
        }

        if (endangle < 0)
        {
            endangle += 360;
        }

        float smallerValue;
        float biggerValue;

        if (startAngle > endangle)
        {
            smallerValue = endangle;
            biggerValue = startAngle;
        } else
        {
            smallerValue = startAngle;
            biggerValue = endangle;
        }

        float distance = biggerValue - smallerValue;

        if (distance > 180f)
        {
            startAngle = biggerValue;
            endangle = smallerValue;
            distance = 360-distance;
        } else
        {
            startAngle = smallerValue;
            endangle = biggerValue;
        }     

		int numberOfSlices = 1;

		if (smooth) {
			numberOfSlices = Mathf.Abs(Mathf.RoundToInt(distance / RasterManager.Instance.rasterLevelAnglesSmooth));
		} else {
			numberOfSlices = Mathf.Abs(Mathf.RoundToInt(distance / RasterManager.Instance.rasterLevelAngles));
		}


        for (int i=0; i < numberOfSlices; i++)
        {	
			GameObject newSlice;
			
			if (smooth) {
				newSlice = Instantiate (firstSlice);
			} else {
				newSlice = Instantiate (bigSlice);
			}
            newSlice.transform.position = transform.position;

            newSlice.transform.localEulerAngles = transform.localEulerAngles;

			if (smooth) {
				newSlice.transform.Rotate(new Vector3(0f, 0f, startAngle + i * RasterManager.Instance.rasterLevelAnglesSmooth));
			} else {
				newSlice.transform.Rotate(new Vector3(0f, 0f, startAngle + i * RasterManager.Instance.rasterLevelAngles));
			}



            newSlice.transform.SetParent(SlicesContainer);
			newSlice.transform.localScale = new Vector3 (0.15f,0.15f,0.15f);
        }

     
    }
}
