using UnityEngine;
using System.Collections;

public class SeatedOrStandingManager : MonoBehaviour {

	public enum seatedOrStandingMode { seated, standing };

	public seatedOrStandingMode currentMode = seatedOrStandingMode.seated;
	public GameObject chairPart1;
	public GameObject chairPart2;
	public GameObject chairPart3;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetSeatedMode(){
		currentMode =  seatedOrStandingMode.seated;

		chairPart1.SetActive (true);
		chairPart2.SetActive (true);
		chairPart3.SetActive (true);
	}

	public void SetStandingMode(){
		currentMode =  seatedOrStandingMode.standing;	

		chairPart1.SetActive (false);
		chairPart2.SetActive (false);
		chairPart3.SetActive (false);
	}
}
