using UnityEngine;
using System.Collections;

public class DuplicateAnimationContainer : Singleton<DuplicateAnimationContainer> {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ClearContainer(){
		foreach (Transform child in transform) {
			Destroy (child.gameObject);
		}
	}
}
