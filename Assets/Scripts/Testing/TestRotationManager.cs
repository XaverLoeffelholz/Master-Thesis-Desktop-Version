using UnityEngine;
using System.Collections;

public class TestRotationManager : MonoBehaviour {

    public Transform p1;
    public Transform p2;

    private Quaternion previousRotation;

    // Use this for initialization
    void Start () {
        Vector3 pos1 = new Vector3(p1.transform.position.x, 0, p1.transform.position.z);
        Vector3 pos2 = new Vector3(p2.transform.position.x, 0, p2.transform.position.z);
       
        previousRotation = Quaternion.LookRotation(pos1 - pos2);
    }
	
	// Update is called once per frame
	void Update () {

        // test like this, with rastering

        //     Vector3 centerBetween = p1.transform.position * 0.5f + p2.transform.position * 0.5f;
        //     transform.position = centerBetween;

        Vector3 pos1 = new Vector3(p1.transform.position.x, 0, p1.transform.position.z);
        Vector3 pos2 = new Vector3(p2.transform.position.x, 0, p2.transform.position.z);

        Quaternion newRotation = Quaternion.LookRotation(pos1-pos2);
        Quaternion relative = Quaternion.Inverse(previousRotation) * newRotation;
        previousRotation = newRotation;

        transform.Rotate(relative.eulerAngles);

        // transform.rotation = newRotation;

        // transform.localScale = new Vector3((p1.transform.position - p2.transform.position).magnitude, (p1.transform.position - p2.transform.position).magnitude, (p1.transform.position - p2.transform.position).magnitude);
    }
}
