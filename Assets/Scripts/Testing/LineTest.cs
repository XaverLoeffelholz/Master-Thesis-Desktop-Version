using UnityEngine;
using System.Collections;

public class LineTest : MonoBehaviour {

    public GameObject linePrefab;
    public GameObject linesPrefab;
    public Vector3[] coordinates;

    // Use this for initialization
    void Start () {
        LinesCreation();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LineCreation()
    {
        GameObject line = Instantiate(linePrefab);
        line.GetComponent<Line>().DrawLineWorldCoord(new Vector3(1f, 3f, 6f), new Vector3(-2f, -1f, 1f));
    }

    public void LinesCreation()
    {
        GameObject lines = Instantiate(linesPrefab);
        lines.GetComponent<Lines>().DrawLinesWorldCoordinate(coordinates, 0);
    }

}
