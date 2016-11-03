using UnityEngine;
using System.Collections;

public class Teleportation : Singleton<Teleportation> {

    public Transform View1;
    public Transform View2;
    public Transform View3;
    public Transform View4;
	public Transform MovingObject;

	public GameObject maskLeft;
	public GameObject maskRight;

    int i = 1;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
     
    }

    public void JumpToPos(int pos)
    {
        switch (pos)
        {
            case 1:
                transform.position = View1.position;
                transform.localRotation = Quaternion.Euler(new Vector3(0f,0f,0f));
                break;
            case 2:
                transform.position = View2.position;
                transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                break;
            case 3:
                transform.position = View3.position;
                transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
                break;
            case 4:
                transform.position = View4.position;
                transform.localRotation = Quaternion.Euler(new Vector3(0f, 270f, 0f));
                break;
		case 5:
				JumpToPosOflittleGuy ();
				break;
        }
    }


	public void JumpToPosOflittleGuy(){

		// get distance
		float distance = Vector3.Distance(MovingObject.position, transform.position);

		// get angle between
		//float angle = Quaternion.Angle(MovingObject.rotation, transform.rotation);

		// define time variable
		float time = distance * 0.3f;
        //float amountOfscaling = Mathf.Max (0.05f, (1f / (Mathf.Pow(angle*0.5f,2) + Mathf.Pow(distance,2))));

        float smallFieldOfView = 0.1f;
        float bigFieldOfView = 0.1732f;

        // fade in masks	
        LeanTween.scale(maskLeft, new Vector3(smallFieldOfView, smallFieldOfView, smallFieldOfView), time * 0.2f);
		LeanTween.scale(maskRight, new Vector3(smallFieldOfView, smallFieldOfView, smallFieldOfView), time *  0.2f);

		LeanTween.move (gameObject, MovingObject.position, time * 0.4f).setDelay(time * 0.05f);
		LeanTween.rotate (gameObject, MovingObject.rotation.eulerAngles, time * 0.4f).setDelay(time * 0.05f);

		LeanTween.scale(maskLeft, new Vector3(bigFieldOfView, bigFieldOfView, bigFieldOfView), time * 0.2f).setDelay(time * 0.4f);
		LeanTween.scale(maskRight, new Vector3(bigFieldOfView, bigFieldOfView, bigFieldOfView), time * 0.2f).setDelay(time * 0.4f);

		// here we have to adapt the position of scaling center and scaling library
	}
}


