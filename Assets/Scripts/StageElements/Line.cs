using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour {

    public GameObject p1;
    public GameObject p2;
    public GameObject line;
	private float initialscaleStage;
	private float initialscaleLine;

    void Start()
    {
		initialscaleStage = 1f; 
		initialscaleLine = line.transform.lossyScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 centerBetween = p1.transform.position * 0.5f + p2.transform.position * 0.5f;
        line.transform.position = centerBetween;

        Quaternion newRotation = Quaternion.LookRotation(p1.transform.position - p2.transform.position);
        line.transform.rotation = newRotation;
	
		float newScale = initialscaleLine / line.transform.lossyScale.x;

		if (ObjectsManager.Instance.transform.parent.parent.transform.localScale.x > 0.9f) {
			newScale = Mathf.Min (1.5f, newScale);
		}
		//float newScale = Mathf.Min((ObjectsManager.Instance.transform.parent.parent.transform.localScale.x / initialscaleStage), 2f);

		line.transform.localScale = new Vector3(initialscaleLine * newScale, initialscaleLine * newScale, (p1.transform.localPosition - p2.transform.localPosition).magnitude + initialscaleLine * newScale);
    }

    public void DrawLineWorldCoord(Vector3 pos1, Vector3 pos2)
    {
        p1.transform.position = pos1;
        p2.transform.position = pos2;
    }

    public void DrawLineLocalCoord(Vector3 pos1, Vector3 pos2)
    {
        p1.transform.localPosition = pos1;
        p2.transform.localPosition = pos2;
    }

	public void ChangeColor(Color color){
	//	line.GetComponent<Renderer> ().material.shader = Shader.Find("Albedo");
		line.GetComponent<Renderer> ().material.color = color;
	}
}
