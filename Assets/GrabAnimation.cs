using UnityEngine;
using System.Collections;

public class GrabAnimation : MonoBehaviour {

	public GameObject left;
	public GameObject right;

	public bool grabberactive = true;

	// Use this for initialization
	void Start () {


	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Open(){
		if (grabberactive) {
			LeanTween.rotateLocal (left, new Vector3 (0f, 45f, 0f), 0.08f).setEase (LeanTweenType.easeInOutCubic);
			LeanTween.rotateLocal (right, new Vector3 (0f, -45f, 0f), 0.08f).setEase (LeanTweenType.easeInOutCubic);
		}

	}

	public void Close(){
		if (grabberactive) {
			LeanTween.rotateLocal (left, new Vector3 (0f, 0f, 0f), 0.1f).setEase (LeanTweenType.easeInOutCubic);
			LeanTween.rotateLocal (right, new Vector3 (0f, 0f, 0f), 0.1f).setEase (LeanTweenType.easeInOutCubic);
		}
	}

	public void Hide(){
		grabberactive = false;

		left.SetActive (false);
		right.SetActive (false);
	}
}
