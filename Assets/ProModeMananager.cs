using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProModeMananager : Singleton<ProModeMananager> {

	public bool beginnersMode = false;
	public Selection selection;

	public GameObject selectStateBeginner;
	public GameObject selectStatePro;

	public GameObject CreationVisuals;

	public Slider rasterSlider;

	// Use this for initialization
	void Start () {
		ActivateBeginnersMode ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ActivateBeginnersMode(){

		if (selection.currentSelection != null) {
			if (selection.currentSelection.GetComponent<ModelingObject> ().boundingBox.local) {
				WorldLocalToggle.Instance.ToggleWorldLocal ();
			}
		}

		beginnersMode = true;

		if (selection.currentSelection != null) {
			selection.currentSelection.GetComponent<ModelingObject> ().UpdateVisibleHandles ();
		}

		selectStateBeginner.SetActive (true);
		selectStatePro.SetActive (false);

		CreationVisuals.SetActive (false);

		rasterSlider.value = 0f;
		rasterSlider.gameObject.SetActive (false);


	}

	public void DeActivateBeginnersMode(){
		beginnersMode = false; 

		if (selection.currentSelection != null) {
			selection.currentSelection.GetComponent<ModelingObject> ().UpdateVisibleHandles ();
		}
	
		selectStateBeginner.SetActive (false);
		selectStatePro.SetActive (true);

		CreationVisuals.SetActive (true);

		rasterSlider.gameObject.SetActive (true);
		rasterSlider.value = 1f;

	}


}
