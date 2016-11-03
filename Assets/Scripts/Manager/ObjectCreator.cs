using UnityEngine;
using System.Xml;
using System.Collections;

public class ObjectCreator : Singleton<ObjectCreator> {
	
    public GameObject modelingObject;
    public Mesh triangle;
    public Mesh square;
    public Mesh hexagon;
    public Mesh octagon;

    public Transform objects;
    public Transform library;

	public Color standardColor;

    private int objectIDcount = 0;

	public ModelingObject latestModelingObject;

	public GameObject duplicateAnimationPrefab;
	private GameObject duplicateAnimation;


    // Use this for initialization
    void Start () {
        //	createNewObject(ModelingObject.ObjectType.triangle, null, null, new Vector3(-2, 1.3f, 4f), true, null, standardColor);
        createNewObject(ModelingObject.ObjectType.square, null, null, new Vector3(0f, 0.3f, 0f), true, null, standardColor);
        //	createNewObject(ModelingObject.ObjectType.octagon, null, null, new Vector3(2, 1.3f, 4f), true, null, standardColor);

        ObjectCreator.Instance.createSetofObjects();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

	public void createObjectInLibrary(ModelingObject.ObjectType objectType){
		if (objectType == ModelingObject.ObjectType.triangle) {
			createNewObject(ModelingObject.ObjectType.triangle, null, null, library.GetComponent<library>().pos1.localPosition, false, null, standardColor);       
		} else if (objectType == ModelingObject.ObjectType.square) {
			createNewObject(ModelingObject.ObjectType.square, null, null, library.GetComponent<library>().pos2.localPosition, false, null, standardColor);
		} else if (objectType == ModelingObject.ObjectType.octagon) {
			createNewObject(ModelingObject.ObjectType.octagon, null, null, library.GetComponent<library>().pos3.localPosition, false, null, standardColor);
		}	
	}



    public void createSetofObjects()
    {
		createNewObject(ModelingObject.ObjectType.triangle, null, null, library.GetComponent<library>().pos1.localPosition, false, null, standardColor);       
		createNewObject(ModelingObject.ObjectType.square, null, null, library.GetComponent<library>().pos2.localPosition, false, null, standardColor);
		createNewObject(ModelingObject.ObjectType.octagon, null, null, library.GetComponent<library>().pos3.localPosition, false, null, standardColor);

        //createNewObject(hexagon, ModelingObject.ObjectType.hexagon, null, null, library.GetComponent<library>().pos3.localPosition, false, null);		
    }

	public void createNewObject(ModelingObject.ObjectType type, Face groundface, ModelingObject original, Vector3 offSet, bool insideStage, Group group, Color color)
    {
        Mesh mesh = new Mesh();

        if (type == ModelingObject.ObjectType.triangle)
        {
            mesh = triangle;
        }
        else if (type == ModelingObject.ObjectType.square)
        {
            mesh = square;
        }
        else if (type == ModelingObject.ObjectType.octagon)
        {
            mesh = octagon;
        }

		GameObject newObject = Instantiate(modelingObject);

        if (insideStage)
        {
            newObject.transform.SetParent(objects);
        } else
        {
            newObject.transform.SetParent(library);
        }

        ModelingObject newModelingObject = newObject.GetComponent<ModelingObject>();

        newObject.name = "Object " + objectIDcount;
        newModelingObject.ObjectID = objectIDcount;
        objectIDcount++;

        newModelingObject.typeOfObject = type;

        newObject.GetComponent<ModelingObject>().Initiate(mesh);

        if (groundface != null) {

            newObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            newObject.transform.position = groundface.center.transform.TransformPoint(groundface.center.coordinates + groundface.normal*0.4f);
            newObject.transform.localScale = groundface.transform.parent.parent.transform.localScale;

            newModelingObject.bottomFace.ReplaceFaceOnOtherFace (groundface, new Vector3 (0f, 0f, 0f), true);
            newModelingObject.topFace.ReplaceFaceOnOtherFace (groundface, groundface.normal.normalized*0.1f, false);

            // recalculate centers
            newModelingObject.RecalculateCenters();

            // recalculate normals
            newModelingObject.RecalculateNormals();
        }
        else {
            if (insideStage)
            {
                newObject.transform.localScale = new Vector3(1f, 1f, 1f);
                newObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                newObject.transform.localPosition = new Vector3(0f, 0f, 0f);
				newObject.transform.localPosition = offSet;
            } else
            {
				newObject.transform.localPosition = offSet;
                newObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }

        }

        if (original != null)
        {
            // SetVertexBundlePositions(Vector3[] topFaceCoordinates, Vector3[] bottomFaceCoordinates, Vector3 topFaceCenter, Vector3 bottomFaceCenter)

            Vector3[] topFaceCoordinates = new Vector3[original.topFace.vertexBundles.Length];
            Vector3[] bottomFaceCoordinates = new Vector3[original.bottomFace.vertexBundles.Length];

            for (int i=0; i < topFaceCoordinates.Length; i++)
            {
                topFaceCoordinates[i] = original.topFace.vertexBundles[i].coordinates;
            }

            for (int i = 0; i < bottomFaceCoordinates.Length; i++)
            {
                bottomFaceCoordinates[i] = original.bottomFace.vertexBundles[i].coordinates;
            }

            newModelingObject.SetVertexBundlePositions(topFaceCoordinates, bottomFaceCoordinates, original.topFace.centerPosition, original.bottomFace.centerPosition);
			newObject.transform.localPosition = offSet;
        }

        // newModelingObject.CorrectOffset();
        newModelingObject.CalculateBoundingBox();
        newModelingObject.InitiateHandles();

        if (group != null) {
            ObjectsManager.Instance.AddObjectToGroup(group, newModelingObject);
			//newObject.transform.position = offSet;
        }

        newModelingObject.ChangeColor(color, false);
		latestModelingObject = newModelingObject;
    }


	public void createNewObjectOnFace(Face groundface) {

		// get number of vertices
		int numberOfVertices = groundface.vertexBundles.Length;
		Vector3 offset = new Vector3(0f,0f,0f);

		switch (numberOfVertices) 
		{
		case 3:
			createNewObject (ModelingObject.ObjectType.triangle, groundface, null, offset, true, null, standardColor);
                break;
		case 4:
			createNewObject (ModelingObject.ObjectType.square, groundface, null, offset, true, null, standardColor);
                break;
		case 6:
			createNewObject (ModelingObject.ObjectType.hexagon, groundface, null, offset, true, null, standardColor);
                break;
		case 8:
			createNewObject (ModelingObject.ObjectType.octagon, groundface, null, offset, true, null, standardColor);
                break;
		}
	}

	public void DuplicateObject(ModelingObject original, Group group, Vector3 objectPosition)
    {
		//Debug.Log ("before: " + objectPosition);
		//Debug.Log ("after inverse transform point: " + original.transform.InverseTransformPoint (objectPosition));

		createNewObject(original.typeOfObject, null, original, objectPosition, true, group, original.currentColor);

		if (group == null) {
			DuplicateAnimaton (original.GetBoundingBoxCenter());
		}
    }

	public void DuplicateAnimaton(Vector3 position){
		duplicateAnimation = Instantiate (duplicateAnimationPrefab);
		duplicateAnimation.transform.SetParent (DuplicateAnimationContainer.Instance.transform);

		// parent it in a container and empty container 

		duplicateAnimation.transform.position = position;
		duplicateAnimation.GetComponent<CircleAnimaton> ().StartAnimation ();

		Invoke ("DeleteAnimationObject", 0.6f);
	}

	private void DeleteAnimationObject(){
		DuplicateAnimationContainer.Instance.ClearContainer ();
	}

	public void DuplicateGroup(Group group, Vector3 offset)
	{
		// create new group
		Group NewGroup = ObjectsManager.Instance.CreateGroup();

		for (int i = 0; i < group.objectList.Count; i++) {
            // we need to change this to local position
			DuplicateObject (group.objectList [i], NewGroup, (group.objectList [i].transform.localPosition + offset)); 
		}

		NewGroup.UpdateBoundingBox ();
		DuplicateAnimaton (NewGroup.GetBoundingBoxCenter());
	}

    public void ImportFromXML(string pathToXMl)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(pathToXMl);
        XmlElement baseNode = doc.DocumentElement;

        // we could do something with id and session

        XmlNode creation = baseNode.SelectSingleNode("creation");

        foreach (XmlNode modelingObject in creation.ChildNodes)
        {
            ModelingObject.ObjectType typeofObject = (ModelingObject.ObjectType)System.Enum.Parse(typeof(ModelingObject.ObjectType), modelingObject.SelectSingleNode("objectType").InnerText);
            Vector3 objectPosition = new Vector3(float.Parse(modelingObject.SelectSingleNode("position").SelectSingleNode("x").InnerText),
                float.Parse(modelingObject.SelectSingleNode("position").SelectSingleNode("y").InnerText),
                float.Parse(modelingObject.SelectSingleNode("position").SelectSingleNode("z").InnerText));

            Color color = new Vector4(float.Parse(modelingObject.SelectSingleNode("color").SelectSingleNode("r").InnerText),
                float.Parse(modelingObject.SelectSingleNode("color").SelectSingleNode("g").InnerText),
                float.Parse(modelingObject.SelectSingleNode("color").SelectSingleNode("b").InnerText),
                float.Parse(modelingObject.SelectSingleNode("color").SelectSingleNode("a").InnerText));

            createNewObject(typeofObject, null, null, objectPosition, true, null, color);

            // replace vertices based on positions
            Vector3 topFaceCenter = new Vector3(float.Parse(modelingObject.SelectSingleNode("topfacecenter").SelectSingleNode("x").InnerText),
                float.Parse(modelingObject.SelectSingleNode("topfacecenter").SelectSingleNode("y").InnerText),
                float.Parse(modelingObject.SelectSingleNode("topfacecenter").SelectSingleNode("z").InnerText));
            Vector3 bottomFaceCenter = new Vector3(float.Parse(modelingObject.SelectSingleNode("topfacecenter").SelectSingleNode("x").InnerText),
                float.Parse(modelingObject.SelectSingleNode("topfacecenter").SelectSingleNode("y").InnerText),
                float.Parse(modelingObject.SelectSingleNode("topfacecenter").SelectSingleNode("z").InnerText));

            Vector3[] topFaceCoordinates = new Vector3[modelingObject.SelectSingleNode("faces").SelectSingleNode("topface").SelectSingleNode("vertices").ChildNodes.Count];
            for (int i = 0; i < topFaceCoordinates.Length; i++)
            {
                XmlNode vertex = modelingObject.SelectSingleNode("faces").SelectSingleNode("topface").SelectSingleNode("vertices").ChildNodes[i];
                topFaceCoordinates[i] = new Vector3(float.Parse(vertex.SelectSingleNode("x").InnerText), float.Parse(vertex.SelectSingleNode("y").InnerText), float.Parse(vertex.SelectSingleNode("z").InnerText));
            }

            Vector3[] bottomFaceCoordinates = new Vector3[modelingObject.SelectSingleNode("faces").SelectSingleNode("bottomface").SelectSingleNode("vertices").ChildNodes.Count];

            for (int i = 0; i < bottomFaceCoordinates.Length; i++)
            {
                XmlNode vertex = modelingObject.SelectSingleNode("faces").SelectSingleNode("bottomface").SelectSingleNode("vertices").ChildNodes[i];
                bottomFaceCoordinates[i] = new Vector3(float.Parse(vertex.SelectSingleNode("x").InnerText), float.Parse(vertex.SelectSingleNode("y").InnerText), float.Parse(vertex.SelectSingleNode("z").InnerText));
            }

            latestModelingObject.SetVertexBundlePositions(topFaceCoordinates, bottomFaceCoordinates, topFaceCenter, bottomFaceCenter);
        }
    }
}
