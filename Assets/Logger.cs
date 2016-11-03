using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;

public class Logger : Singleton<Logger> {

	public enum typeOfLog { triggerOnObject, triggerNoTarget, touchpadScaleStage, touchpadRotateStage, touchpadMoveObject, touchpadRotateObject, nonUniformScaleHandle, RotationHandle, FrustumHandle, stage, 
		uiElement, gestureNavigation, duplicateObject, duplicateObjectUI, grabObjectFromLibrary, deleteObject, deleteObjectUI, uniformScale, closeMenuUI, groupEndUI, groupBreakUI, groupStartUI, colorUI, menuNavigationUI };
    public enum generalType { navigation, triggerInteraction, ui };

	private string filePathLogger = @"C:\Users\user\Documents\Playable Master thesis\Study Version\Log files\";
	// private string filePathLogger = @"L:\Master Thesis\New Git\";
	private string filePathSave = @"C:\Users\user\Documents\Playable Master thesis\Study Version\Safe files\";
	private string filePathImport = @"C:\Users\user\Documents\Playable Master thesis\Study Version\Safe files\CastleSave2_session1_0.xml";
    public string UserID;
    private FileStream fs;

    private float countTimeFrom;

    public StageFreeMovement stageMovement;
    public StageController stageController;
	public SeatedOrStandingManager seatedorStandingmanager;
    public Selection selection;
	public int sessionNumber;
    public int saveNumber = 0;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveCurrentState();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            ObjectCreator.Instance.ImportFromXML(filePathImport);
        }
    }

    public void RestartTimer()
    {
        countTimeFrom = Time.time;
    }

    public void CreateTextFile()
    {
		filePathLogger += "LogUser" + UserID + "_session" + sessionNumber + "_" + seatedorStandingmanager.currentMode + ".csv";

		RestartTimer ();
		fs = File.Create(filePathLogger);  
		fs.Dispose ();
	 	
		String header = "UserID, General Type of Interaction, Time, Specific Type of Interaction, Seated or Standing Mode, Stage movement, stage rotation and scaling, session number \n";
		File.AppendAllText(filePathLogger, header);
    }

    public void AddLine(Logger.typeOfLog logtype)
    {
        generalType currentType;

		if (logtype == typeOfLog.touchpadRotateStage || logtype == typeOfLog.touchpadScaleStage  || logtype == typeOfLog.gestureNavigation)
        {
			currentType = generalType.navigation; 
		} else if (logtype == typeOfLog.closeMenuUI || logtype == typeOfLog.groupEndUI || logtype == typeOfLog.groupBreakUI || logtype == typeOfLog.groupStartUI || logtype == typeOfLog.duplicateObjectUI ||
			logtype == typeOfLog.deleteObjectUI || logtype == typeOfLog.colorUI || logtype == typeOfLog.menuNavigationUI){
			currentType = generalType.ui;
		} else
        {
            currentType = generalType.triggerInteraction;
        }

		string text = UserID + "," + currentType + "," + (Time.time - countTimeFrom) + "," + logtype + "," + seatedorStandingmanager.currentMode + "," + stageMovement.currentStageMovement + "," + stageController.currentRotationScalingTechnique + "," + sessionNumber + "\n";
        
		File.AppendAllText(filePathLogger, text);
    }


    public void SaveCurrentState()
    {
        Debug.Log("Save castle");
        String fullPathForSave = filePathSave + "CastleSave" + UserID + "_session" + sessionNumber + "_" + saveNumber + ".xml";
        saveNumber++;

        fs = File.Create(fullPathForSave);
        fs.Dispose();

        // create the string 

        //String xmlRepresentation = "<?xml version="1.0" encoding="UTF - 8"?>\n";

        String xmlRepresentation = "";
        xmlRepresentation += "<castle>\n";
        xmlRepresentation += "<userID>" + UserID + "</userID>\n";
        xmlRepresentation += "<session>" + sessionNumber + "</session>\n";
        xmlRepresentation += "<creation>\n";

        foreach (Transform child in ObjectsManager.Instance.transform)
        {       
            if (child.CompareTag("ModelingObject"))
            {
                ModelingObject currentModelingobject = child.GetComponent<ModelingObject>();
                xmlRepresentation += CreateObjectXML(currentModelingobject);

            } else
            {
                // group
                foreach (Transform groupChild in child)
                {
					if (groupChild.CompareTag ("ModelingObject")) {
						ModelingObject currentModelingobject = groupChild.GetComponent<ModelingObject> ();
						xmlRepresentation += CreateObjectXML (currentModelingobject);
					}
                }
            }
        }
        xmlRepresentation += "</creation>\n";

        xmlRepresentation += "</castle>\n";

        File.AppendAllText(fullPathForSave, xmlRepresentation);
    }


    public String CreateObjectXML(ModelingObject currentModelingobject)
    {
        String xmlRepresentationTemp = "";

        // single object
		xmlRepresentationTemp += "<object>\n";

		xmlRepresentationTemp += "<objectId>" + currentModelingobject.ObjectID + "</objectId>\n";
		xmlRepresentationTemp += "<objectType>" + currentModelingobject.typeOfObject + "</objectType>\n";
		xmlRepresentationTemp += "<position>";

        // we need to change it to local position, or safe stage size
		xmlRepresentationTemp += "<x>" + currentModelingobject.transform.localPosition.x + "</x> \n";
		xmlRepresentationTemp += "<y>" + currentModelingobject.transform.localPosition.y + "</y> \n";
		xmlRepresentationTemp += "<z>" + currentModelingobject.transform.localPosition.z + "</z> \n";
		xmlRepresentationTemp += "</position>\n";
		xmlRepresentationTemp += "<color>";
		xmlRepresentationTemp += "<r>" + currentModelingobject.currentColor.r + "</r> \n";
		xmlRepresentationTemp += "<g>" + currentModelingobject.currentColor.g + "</g> \n";
		xmlRepresentationTemp += "<b>" + currentModelingobject.currentColor.b + "</b> \n";
		xmlRepresentationTemp += "<a>" + currentModelingobject.currentColor.a + "</a> \n";
		xmlRepresentationTemp += "</color>";

		xmlRepresentationTemp += "<topfacecenter>\n";
		xmlRepresentationTemp += "<x>" + currentModelingobject.topFace.center.coordinates.x + "</x> \n";
		xmlRepresentationTemp += "<y>" + currentModelingobject.topFace.center.coordinates.y + "</y> \n";
		xmlRepresentationTemp += "<z>" + currentModelingobject.topFace.center.coordinates.z + "</z> \n";
		xmlRepresentationTemp += "</topfacecenter>\n";

		xmlRepresentationTemp += "<bottomfacecenter>\n";
		xmlRepresentationTemp += "<x>" + currentModelingobject.bottomFace.center.coordinates.x + "</x> \n";
		xmlRepresentationTemp += "<y>" + currentModelingobject.bottomFace.center.coordinates.y + "</y> \n";
		xmlRepresentationTemp += "<z>" + currentModelingobject.bottomFace.center.coordinates.z + "</z> \n";
		xmlRepresentationTemp += "</bottomfacecenter>\n";

		xmlRepresentationTemp += "<faces>\n";
		xmlRepresentationTemp += "<topface>\n";
		xmlRepresentationTemp += "<vertices>\n";
        for (int i = 0; i < currentModelingobject.topFace.vertexBundles.Length; i++)
        {
			xmlRepresentationTemp += "<vertex> \n";
			xmlRepresentationTemp += "<x>" + currentModelingobject.topFace.vertexBundles[i].coordinates.x + "</x> \n";
			xmlRepresentationTemp += "<y>" + currentModelingobject.topFace.vertexBundles[i].coordinates.y + "</y> \n";
			xmlRepresentationTemp += "<z>" + currentModelingobject.topFace.vertexBundles[i].coordinates.z + "</z> \n";
			xmlRepresentationTemp += "</vertex> \n";
        }

		xmlRepresentationTemp += "</vertices>\n";
		xmlRepresentationTemp += "</topface> \n";

		xmlRepresentationTemp += "<bottomface>\n";
		xmlRepresentationTemp += "<vertices>\n";
        for (int i = 0; i < currentModelingobject.bottomFace.vertexBundles.Length; i++)
        {
			xmlRepresentationTemp += "<vertex> \n";
			xmlRepresentationTemp += "<x>" + currentModelingobject.bottomFace.vertexBundles[i].coordinates.x + "</x> \n";
			xmlRepresentationTemp += "<y>" + currentModelingobject.bottomFace.vertexBundles[i].coordinates.y + "</y> \n";
			xmlRepresentationTemp += "<z>" + currentModelingobject.bottomFace.vertexBundles[i].coordinates.z + "</z> \n";
			xmlRepresentationTemp += "</vertex> \n";
        }
		xmlRepresentationTemp += "</vertices>\n";
		xmlRepresentationTemp += "</bottomface>\n";
		xmlRepresentationTemp += "</faces>\n";
		xmlRepresentationTemp += "</object>\n";

		return xmlRepresentationTemp;
    }
}
