using UnityEngine;
using System.Collections;

public class RotationVisual : MonoBehaviour {

	public GameObject firstSlice;
    public Transform SlicesContainer;

	public GameObject RightangleVisual;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
      
    }

    public void RotationVisualisation(Vector3 point1, Vector3 point2)
    {
        float startAngle = Vector3.Angle(transform.up, point1 - transform.position);
        float directionStart = Mathf.Sign(Vector3.Dot(Vector3.Cross(transform.up, point1 - transform.position), transform.forward));

        float endangle = Vector3.Angle(transform.up, point2 - transform.position);
        float directionEnd = Mathf.Sign(Vector3.Dot(Vector3.Cross(transform.up, point2 - transform.position), transform.forward));

       
		CreateSlices(RasterManager.Instance.RasterAngle(startAngle * directionStart), RasterManager.Instance.RasterAngle(endangle * directionEnd));
    }

	public void ShowRightAngleVisual(bool value){
		RightangleVisual.SetActive (value);
	}

    public void CreateSlices(float startAngle, float endangle){

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

        int numberOfSlices = Mathf.Abs(Mathf.RoundToInt(distance / 5f));

        for (int i=0; i < numberOfSlices; i++)
        {
            GameObject newSlice = Instantiate(firstSlice);
            newSlice.transform.position = transform.position;

            newSlice.transform.localEulerAngles = transform.localEulerAngles;
            newSlice.transform.Rotate(new Vector3(0f, 0f, startAngle + i * 5f));

            newSlice.transform.SetParent(SlicesContainer);
			newSlice.transform.localScale = new Vector3 (0.15f,0.15f,0.15f);
        }

     
    }
}
