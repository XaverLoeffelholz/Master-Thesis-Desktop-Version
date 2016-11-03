using UnityEngine;
using System.Collections;

public class CircleAnimaton : MonoBehaviour {

	public GameObject innerCircle;
	public GameObject outterCircle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartAnimation(){
		// replace
	//	innerCircle.position = position;
	//	outterCircle.position = position;

		innerCircle.transform.localScale = new Vector3 (0.06f, 0.06f, 0.06f);
		outterCircle.transform.localScale = new Vector3 (0.01f, 0.01f, 0.01f);

		// restart animation
		LeanTween.alpha (innerCircle, 0.6f, 0.1f);
		LeanTween.alpha (outterCircle, 0.8f, 0.1f);

		LeanTween.scale(innerCircle, new Vector3(0.3f,0.3f,0.3f), 0.2f);
		LeanTween.scale(outterCircle, new Vector3(0.8f,0.8f,0.8f), 0.4f).setEase(LeanTweenType.easeInCubic);

		LeanTween.alpha (innerCircle, 0f, 0.2f).setDelay(0.1f);
		LeanTween.alpha (outterCircle, 0f, 0.4f).setDelay(0.1f).setEase(LeanTweenType.easeOutCubic);
	}

}
