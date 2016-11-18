using UnityEngine;
using System.Collections;

public class BoundingBox : MonoBehaviour {

	public Vector3[] coordinates;
    public GameObject linesPrefab;
	public GameObject boundingBoxCollider;

	private bool active = false;

	private GameObject linesGO;

	public bool local = true;

	// Use this for initialization
	void Start () {

	}
	
	void FixedUpdate () {
		if (active) {
			boundingBoxCollider.transform.localScale = new Vector3 ((coordinates [0] - coordinates [1]).magnitude, (coordinates [0] - coordinates [4]).magnitude, (coordinates [0] - coordinates [3]).magnitude) / boundingBoxCollider.transform.lossyScale.x;
		}
	}


	public void DrawBoundingBox(){
		if (linesGO == null) {
			linesGO = Instantiate(linesPrefab);
			linesGO.transform.SetParent(transform.GetChild(0));
		}        

		Lines lines = linesGO.GetComponent<Lines> ();

		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinates[0],coordinates[1],coordinates[2],coordinates[3]}, 0);
		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinates[4],coordinates[5],coordinates[6],coordinates[7]}, 4);

		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinates[0],coordinates[4]},8);
		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinates[1],coordinates[5]},10);
		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinates[2],coordinates[6]},12);
		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinates[3],coordinates[7]},14);

		// needs to be changed for local stuff

		/*
		if (coordinates [0].y > 0) {
			Vector3 groundedA = new Vector3 (coordinates [0].x, 0f, coordinates [0].z);
			Vector3 groundedB = new Vector3 (coordinates [1].x, 0f, coordinates [1].z);
			Vector3 groundedC = new Vector3 (coordinates [2].x, 0f, coordinates [2].z);
			Vector3 groundedD = new Vector3 (coordinates [3].x, 0f, coordinates [3].z);

			lines.EnhancedDrawLinesWorldCoordinate(new Vector3[] {groundedA,groundedB,groundedC,groundedD}, 16, new Color(0.8f,0.8f,0.8f, 0.4f));
		}*/
    }

    public void ClearBoundingBox()
    {
        // delete inside 
        foreach (Transform lineObject in transform.GetChild(0))
        {
            Destroy(lineObject.gameObject);
        }
    }

}
