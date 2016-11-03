using UnityEngine;
using System.Collections;

public class RotationScalingVisualization : MonoBehaviour {

	public bool active = false;
	private CanvasGroup canvgroup;

	public Transform left;
	public Transform right;
	public Transform center;
	public Transform line;
	public CanvasGroup lineCanv;


	public Transform controller1;
	public Transform controller2;

	float initialscaleLine;
	float distanceForFullvisibility = 0.35f;


	// Use this for initialization
	void Start () {
		canvgroup = transform.GetComponent<CanvasGroup> ();
		initialscaleLine = line.transform.localScale.z;
		lineCanv = line.GetComponent<CanvasGroup> ();

		if (!active) {
			HideVisualization ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (active) {
			Vector3 centerBetween = controller1.transform.position * 0.5f + controller2.transform.position * 0.5f;
			center.transform.position = centerBetween;
			line.transform.position = centerBetween;

			Quaternion newRotation = Quaternion.LookRotation(controller1.transform.position - controller2.transform.position);
			line.transform.rotation = newRotation;
			center.transform.rotation = newRotation;

			line.transform.localScale = new Vector3(line.transform.localScale.x, line.transform.localScale.y, ((controller1.transform.position - controller2.transform.position).magnitude));

			lineCanv.alpha  = Mathf.Min(Mathf.Max(0f,((controller1.transform.position - controller2.transform.position).magnitude - 0.3f)) / 0.3f, 1f);

			left.transform.position = controller1.transform.position;
			left.transform.LookAt (controller2);
			right.transform.position = controller2.transform.position;
			right.transform.LookAt (controller1);
		}
	}

	public void ShowVisualization(){
		active = true;
		LeanTween.alphaCanvas (canvgroup, 1f, 0.1f);
	}

	public void HideVisualization(){
		LeanTween.alphaCanvas (canvgroup, 0f, 0.1f).setOnComplete(Deactivate);
	}

	public void Deactivate(){
		active = false;
	}

}
