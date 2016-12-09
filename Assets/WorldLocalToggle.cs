using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorldLocalToggle : Singleton<WorldLocalToggle> {

	public bool local = true;
	public Selection selectionManager;

	private bool visible = false;
	public CanvasGroup bubble;

	public GameObject iconWorld;
	public GameObject iconLocal;

	//public 


	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		if (selectionManager.currentSelection != null) {
			if (selectionManager.currentSelection.GetComponent<ModelingObject> ().handles.YMovement.activeSelf) {
				transform.position = Camera.main.WorldToScreenPoint(selectionManager.currentSelection.GetComponent<ModelingObject> ().handles.YMovement.transform.GetChild(0).position) + new Vector3(0f,50f,0f);
			}
		}
	}

	public void ToggleWorldLocal(){

		if (!ProModeMananager.Instance.beginnersMode) {
			local = !local;
			selectionManager.currentSelection.GetComponent<ModelingObject> ().ToggleLocalGlobalBB ();

			if (local) {
				iconWorld.SetActive (false);
				iconLocal.SetActive (true);
			} else {
				iconWorld.SetActive (true);
				iconLocal.SetActive (false);
			}

			Focus ();
		}
	}

	public void Focus(){
		if (bubble.alpha >= 0.5f) {
			LeanTween.alphaCanvas (bubble, 1f, 0.05f).setEase (LeanTweenType.easeInOutCubic);
		}
	}

	public void UnFocus(){
		LeanTween.alphaCanvas (bubble, 0.5f, 0.05f).setEase (LeanTweenType.easeInOutCubic);
	}

	public void Show (){
		visible = true;
		bubble.interactable = true;
		bubble.blocksRaycasts = true;

		LeanTween.alphaCanvas (bubble, 0.5f, 0.1f).setEase (LeanTweenType.easeInOutCubic);
	}

	public void Hide (){
		LeanTween.alphaCanvas (bubble, 0f, 0.01f).setOnComplete(DeActivateBubble);
	}


	public void DeActivateBubble(){
		//bubble.interactable = false;
		//bubble.blocksRaycasts = false;
		visible = false;

		//transform.JumpHigh ();
	}
}
