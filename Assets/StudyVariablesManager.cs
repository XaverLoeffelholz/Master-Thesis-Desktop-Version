using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StudyVariablesManager : MonoBehaviour {

	public CanvasGroup studySettings;
	public Dropdown variableNav1;
	public Dropdown variableNav2;
	public Dropdown variableSelection3;
	public Dropdown session;
	public InputField id;

	public Selection contoller1_sel;
	public Selection contoller2_sel;
	public StageFreeMovement stage;
	public Logger logger;

	public SeatedOrStandingManager seatMode;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ChooseSettings(){
		
		logger.UserID = id.text;

		if (variableNav1.value == 0) {
			stage.currentStageMovement = StageFreeMovement.stageMovement.free;
		} else {
			stage.currentStageMovement = StageFreeMovement.stageMovement.handles;
		}

		stage.UpdateStageMovement ();

		if (variableSelection3.value == 0) {
			seatMode.SetSeatedMode ();
		} else {
			seatMode.SetStandingMode ();
		}

		if (session.value == 0) {
			logger.sessionNumber = 1;
		} else {
			logger.sessionNumber = 2;
		}
			
		logger.CreateTextFile ();

		studySettings.gameObject.SetActive (false);

	}
}
