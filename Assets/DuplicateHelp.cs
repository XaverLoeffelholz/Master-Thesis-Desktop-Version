using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DuplicateHelp : MonoBehaviour {
	public Selection controller1;
	public Selection controller2;
	public StageController stageControl;

	public RectTransform textMain;
	public RectTransform textSub;
	public RectTransform icon;

	private Color standardColor;

	private bool active = true;

	public CanvasGroup NavHelp;

	// Use this for initialization
	void Start () {
		standardColor = textMain.GetComponent<Text> ().color;
	}

	// Update is called once per frame
	void Update () {
		transform.position =  controller2.transform.position;
		transform.localRotation = controller2.transform.localRotation;
	//	transform.LookAt (controller2.transform);
	}

	public void DuplicateActive(){
		if (active) {
			// color of duplicate text to blue
			LeanTween.textColor(textMain, UiCanvasGroup.Instance.hoverColor, 0.2f);

			// replace text with check
			LeanTween.textAlpha(textSub, 0f, 0.2f);
			LeanTween.alpha(icon, 1f, 0.2f);
		}
	}

	public void DuplicateNotActive(){
		if (active) {
			// color of duplicate text to grey
			LeanTween.textColor (textMain, standardColor, 0.2f);

			// replace check with text
			LeanTween.textAlpha (textSub, 1f, 0.2f);
			LeanTween.alpha (icon, 0f, 0.2f);
		}
	}

	public void Hide(bool value){
		if (value && active) {
			LeanTween.alphaCanvas (transform.GetComponent<CanvasGroup>(), 1f, 0.2f);

			if (stageControl.currentRotationScalingTechnique == StageController.RotationScalingTechnique.gesture) {
				LeanTween.alphaCanvas (NavHelp, 1f, 0.2f);
			}

		} else {
			LeanTween.alphaCanvas (transform.GetComponent<CanvasGroup>(), 0f, 0.2f);

			if (stageControl.currentRotationScalingTechnique == StageController.RotationScalingTechnique.gesture) {				
				LeanTween.alphaCanvas (NavHelp, 0f, 0.2f);
			}
		}
	}

	public void ToggleOnOff(){
		active = !active;
		Hide (active);
	}
}
