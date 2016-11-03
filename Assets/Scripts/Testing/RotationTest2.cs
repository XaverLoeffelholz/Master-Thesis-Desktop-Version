using UnityEngine;
//using UnityEditor;
using System.Collections;

public class RotationTest2 : MonoBehaviour {

    public enum rotationDirection { x,y,z };

    public rotationDirection currenteRotationDirection;
    private bool rotating = false;
    private bool newRotation = false;
    public GameObject cube;

    GameObject parentGO;

    public GameObject pointX;

    public GameObject pointY;

    public GameObject pointZ;

    float prevRotationAmountX;
    float prevRotationAmountY;
    float prevRotationAmountZ;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		/*

        if (Input.GetMouseButtonDown(0))
        {
            rotating = true;
            newRotation = true;

            Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit1;
            
            if (Physics.Raycast(ray1, out hit1, 100))
            {
                // first check which handle is clicked
                if(hit1.collider.gameObject == pointX)
                {
                    currenteRotationDirection = rotationDirection.x;
                }
                else if (hit1.collider.gameObject == pointY)
                {
                    currenteRotationDirection = rotationDirection.y;
                }
                else if (hit1.collider.gameObject == pointZ)
                {
                    currenteRotationDirection = rotationDirection.z;
                }

                parentGO = new GameObject();
                parentGO.transform.position = new Vector3(0f, 0f, 0f);
                parentGO.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));

                cube.transform.SetParent(parentGO.transform);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            rotating = false;
            cube.transform.SetParent(transform);
            GameObject.Destroy(parentGO);
        }

        if (rotating)
        {
            Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.main.transform.position - cube.transform.position).magnitude));

            if (currenteRotationDirection == rotationDirection.x)
            {
                float newRotationAmount = 15f * HandleUtility.PointOnLineParameter(p, pointX.transform.position, pointX.transform.GetChild(0).transform.position - pointX.transform.position);

                if (newRotation)
                {
                    prevRotationAmountX = newRotationAmount;
                    newRotation = false;
                }

                parentGO.transform.Rotate(new Vector3(newRotationAmount-prevRotationAmountX, 0f, 0f));
                prevRotationAmountX = newRotationAmount;
            }
            else if (currenteRotationDirection == rotationDirection.y)
            {
                float newRotationAmount = 15f * HandleUtility.PointOnLineParameter(p, pointY.transform.position, pointY.transform.GetChild(0).transform.position - pointY.transform.position);

                if (newRotation)
                {
                    prevRotationAmountY = newRotationAmount;
                    newRotation = false;
                }

                parentGO.transform.Rotate(new Vector3(0f, newRotationAmount-prevRotationAmountY, 0f));
                prevRotationAmountY = newRotationAmount;
            }
            else if (currenteRotationDirection == rotationDirection.z)
            {
                float newRotationAmount = -15f * HandleUtility.PointOnLineParameter(p, pointZ.transform.position, pointZ.transform.GetChild(0).transform.position - pointZ.transform.position);

                if (newRotation)
                {
                    prevRotationAmountZ = newRotationAmount;
                    newRotation = false;
                }

                parentGO.transform.Rotate(new Vector3(0f, 0f, newRotationAmount-prevRotationAmountZ));
                prevRotationAmountZ = newRotationAmount;
            }


        } */
    }
}
