using UnityEngine;
using System.Collections;

public class ProModeMananager : Singleton<ProModeMananager> {

	public bool beginnersMode = false;
	public Selection selection;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ActivateBeginnersMode(){
		beginnersMode = true;

		if (selection.currentSelection != null) {
			selection.currentSelection.GetComponent<ModelingObject> ().UpdateVisibleHandles ();
		}
	}

	public void DeActivateBeginnersMode(){
		beginnersMode = false; 

		if (selection.currentSelection != null) {
			selection.currentSelection.GetComponent<ModelingObject> ().UpdateVisibleHandles ();
		}
	}
}
