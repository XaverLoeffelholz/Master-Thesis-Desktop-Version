using UnityEngine;
using System.Collections;

public class Lines : MonoBehaviour {

    public GameObject linePrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DrawLinesWorldCoordinate(Vector3[] coordinates, int startPoint)
    {
        if (coordinates.Length > 1)
        {
			for (int i = 0; i < coordinates.Length; i++)
            {
				GameObject line;

				if (i + startPoint < transform.childCount) {
					line = transform.GetChild (i + startPoint).gameObject;
				} else {
					line = Instantiate (linePrefab);
					line.transform.SetParent(transform);
				}               

                if (i < coordinates.Length - 1)
                {
                    line.GetComponent<Line>().DrawLineWorldCoord(coordinates[i], coordinates[i + 1]);
                }
                else
                {
                    line.GetComponent<Line>().DrawLineWorldCoord(coordinates[i], coordinates[0]);
                }
            }
        }
    }

	public void EnhancedDrawLinesWorldCoordinate(Vector3[] coordinates, int startPoint, Color color)
	{
		if (coordinates.Length > 1)
		{
			for (int i = 0; i < coordinates.Length; i++)
			{
				GameObject line;

				if (i + startPoint < transform.childCount) {
					line = transform.GetChild (i + startPoint).gameObject;
				} else {
					line = Instantiate (linePrefab);
					line.transform.SetParent(transform);
					line.GetComponent<Line> ().ChangeColor (color);
				}               

				if (i < coordinates.Length - 1)
				{
					line.GetComponent<Line>().DrawLineWorldCoord(coordinates[i], coordinates[i + 1]);
				}
				else
				{
					line.GetComponent<Line>().DrawLineWorldCoord(coordinates[i], coordinates[0]);
				}
			}
		}

	}

	public void ChangeColors(Color color){
		foreach (Transform child in transform) {
			child.GetComponent<Line> ().ChangeColor (color);
		}
	}

	public void ClearLines(){
		foreach (Transform child in transform) {
			Destroy (child.gameObject);
		}
	}
}
