using UnityEngine;
using System.Collections;

public class Infopanel : MonoBehaviour {

	public GameObject colliderInfoPanel;
	public CanvasGroup inside;
	private bool focused = false;

	public RectTransform closeText;
	public RectTransform closeButton;

	// Use this for initialization
	void Start () {
		CloseInfoPanel ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Focus(Selection controller){
		if (!focused) {
			controller.AssignCurrentFocus(transform.gameObject);
			LeanTween.alphaText(closeText, 1.0f, 0.2f);
			LeanTween.scale(closeButton, new Vector3(0.5f, 0.5f, 0.5f), 0.3f);

			focused = true;
		}
	}


	public void UnFocus(Selection controller){
		if (focused) {
			controller.DeAssignCurrentFocus(transform.gameObject);
			LeanTween.alphaText(closeText, 0.0f, 0.2f);
			LeanTween.scale(closeButton, new Vector3(0.4f, 0.4f, 0.4f), 0.3f);

			focused = false;
		}
	}

	public void CloseInfoPanel(){
		colliderInfoPanel.SetActive (false);
		LeanTween.alpha (gameObject.GetComponent<RectTransform> (), 0f, 0.4f).setEase (LeanTweenType.easeInOutCubic);
		LeanTween.alphaCanvas (inside, 0f, 0.4f).setEase (LeanTweenType.easeInOutCubic);

	}
}
