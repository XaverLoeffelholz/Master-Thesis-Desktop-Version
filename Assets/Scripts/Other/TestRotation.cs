using UnityEngine;
using System.Collections;

public class TestRotation : MonoBehaviour {

    public Vector3 normal;

	// Use this for initialization
	void Start () {


    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, normal);
        }


        // transform.LookAt(Vector3.zero + normal*2f - new Vector3(0f,1f,0f));

        //float angleX = Vector2.Angle(new Vector2(normal.y,normal.z), new Vector2(standard.y,standard.z));
        //float angleY = Vector2.Angle(new Vector2(normal.x, normal.z), new Vector2(standard.x, standard.z));
        //float angleZ = Vector2.Angle(new Vector2(normal.x, normal.y), new Vector2(standard.x, standard.y));

        //transform.RotateAround(Vector3.zero, new Vector3(1f,0f,0f),angleX);
    }
}
