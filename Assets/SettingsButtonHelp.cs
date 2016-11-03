using UnityEngine;
using System.Collections;

public class SettingsButtonHelp : MonoBehaviour {
	public Selection controller1;
	public Selection controller2;

	public GameObject SettingsButton;
	private CanvasGroup canvgroup;

	public Color normalColorButton;
	public Color inactiveColorButton;

	private bool active = true;

	// Use this for initialization
	void Start () {
		canvgroup = transform.GetComponent<CanvasGroup> ();
		Hide ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = controller1.transform.position;

		if (UiCanvasGroup.Instance.visible) {
			HideCompletely(false);
		}
	}

	public void Show(){
		if (!UiCanvasGroup.Instance.visible && active) {
			LeanTween.alphaCanvas (canvgroup, 1f, 0.3f);
			LeanTween.scale (transform.GetComponent<RectTransform> (), new Vector3 (1f, 1f, 1f), 0.3f);
			LeanTween.color (SettingsButton, normalColorButton, 0.3f);
		}
	}

	public void Hide(){
		if (active) {
			LeanTween.alphaCanvas (canvgroup, 0.4f, 0.3f);
			LeanTween.scale (transform.GetComponent<RectTransform>(), new Vector3(0.85f,0.85f,0.85f), 0.3f);
			LeanTween.color (SettingsButton, inactiveColorButton, 0.3f);
		}
	}

	public void HideCompletely(bool value){
		if (value) {
			LeanTween.alphaCanvas (canvgroup, 0.0f, 0.3f);
			LeanTween.scale (transform.GetComponent<RectTransform>(), new Vector3(0.4f,0.4f,0.4f), 0.3f);
		} else {
			LeanTween.alphaCanvas (canvgroup, 0.0f, 0.3f);
			LeanTween.scale (transform.GetComponent<RectTransform>(), new Vector3(0.4f,0.4f,0.4f), 0.3f);
		}
	}

	public void ToggleOnOff(){
		active = !active;
		HideCompletely (active);
	}
}
