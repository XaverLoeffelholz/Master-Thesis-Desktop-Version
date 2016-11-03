using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Group : MonoBehaviour {

    public List<ModelingObject> objectList = new List<ModelingObject>();
    public Vector3 ScalingCenter;
    public Vector3 RotationCenter;

	public BoundingBox boundingBox;
	public bool focused;
	public handles handles;
	public bool selected;

	public ModelingObject lowestModObject;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void InitiateGroup(){

	}

	public void BreakGroup (Selection controller) {
		boundingBox.ClearBoundingBox ();

		for (int i = 0; i < objectList.Count; i++) {  
			objectList [i].DeSelect (controller);
			objectList [i].transform.SetParent (ObjectsManager.Instance.transform);
			objectList [i].group = null;
		}

		ObjectsManager.Instance.DeleteGroup (this);
	}

	public void UpdateBoundingBox(){
		boundingBox.coordinates = new Vector3[8];

		// get highest and lowest values for x,y,z
		Vector3 minima = GetBoundingBoxMinima();
		Vector3 maxima = GetBoundingBoxMaxima();

		// set all points
		boundingBox.coordinates[0] = transform.TransformPoint(new Vector3(maxima.x,maxima.y,maxima.z));
		boundingBox.coordinates[1] = transform.TransformPoint(new Vector3(maxima.x,maxima.y,minima.z));
		boundingBox.coordinates[2] = transform.TransformPoint(new Vector3(minima.x,maxima.y,minima.z));
		boundingBox.coordinates[3] = transform.TransformPoint(new Vector3(minima.x,maxima.y,maxima.z));

		boundingBox.coordinates[4] = transform.TransformPoint(new Vector3(maxima.x,minima.y,maxima.z));
		boundingBox.coordinates[5] = transform.TransformPoint(new Vector3(maxima.x,minima.y,minima.z));
		boundingBox.coordinates[6] = transform.TransformPoint(new Vector3(minima.x,minima.y,minima.z));
		boundingBox.coordinates[7] = transform.TransformPoint(new Vector3(minima.x,minima.y,maxima.z));
	}

	public void DrawBoundingBox(){
		for (int i = 0; i < objectList.Count; i++)
		{
			objectList [i].CalculateBoundingBox ();
		}

		UpdateBoundingBox ();
		boundingBox.DrawBoundingBox ();
	}

	public void HideBoundingBox(){
		boundingBox.ClearBoundingBox ();

		for (int i = 0; i < objectList.Count; i++) {  
			objectList [i].HideBoundingBox (false);
		}
	}

	public Vector3 GetBoundingBoxTopCenter()
	{ 
		UpdateBoundingBox ();
		Vector3 boundingBoxBottomCenter = 0.25f * boundingBox.coordinates [0] + 0.25f * boundingBox.coordinates [1] + 0.25f * boundingBox.coordinates [2] + 0.25f * boundingBox.coordinates [3];
		return boundingBoxBottomCenter;
	}

    public Vector3 GetBoundingBoxBottomCenter()
    {
		UpdateBoundingBox ();
		Vector3 boundingBoxBottomCenter = 0.25f * boundingBox.coordinates [4] + 0.25f * boundingBox.coordinates [5] + 0.25f * boundingBox.coordinates [6] + 0.25f * boundingBox.coordinates [7];

		return boundingBoxBottomCenter;     
    }

	public Vector3 GetBoundingBoxCenter()
	{
		Vector3 boundingBoxCenter = 0.5f * GetBoundingBoxBottomCenter () + 0.5f * GetBoundingBoxTopCenter ();
		return boundingBoxCenter;
	}

	public Vector3 GetBoundingBoxMinima()
	{
		Vector3 minima = new Vector3 (9999f, 9999f, 9999f);

		for (int i = 0; i < objectList.Count; i++)
		{
			Vector3 currentMinima = transform.InverseTransformPoint(objectList [i].transform.TransformPoint(objectList [i].GetBoundingBoxMinima ()));

			if (currentMinima.x < minima.x) {
				minima.x = currentMinima.x;
			}

			if (currentMinima.y < minima.y) {
				minima.y = currentMinima.y;
			}

			if (currentMinima.z < minima.z) {
				minima.z = currentMinima.z;
			}

		}

		return minima;
	}

	public Vector3 GetBoundingBoxMaxima()
	{
		Vector3 maxima = new Vector3 (-9999f, -9999f, -9999f);

		for (int i = 0; i < objectList.Count; i++)
		{
			Vector3 currentMaxima = transform.InverseTransformPoint(objectList [i].transform.TransformPoint(objectList [i].GetBoundingBoxMaxima ()));

			if (currentMaxima.x > maxima.x) {
				maxima.x = currentMaxima.x;
			}

			if (currentMaxima.y > maxima.y) {
				maxima.y = currentMaxima.y;
			}

			if (currentMaxima.z > maxima.z) {
				maxima.z = currentMaxima.z;
			}

		}

		return maxima;
	}


	public void FocusGroup(ModelingObject initiater, Selection controller)
    {
		if (!focused) {
			focused = true;

			DrawBoundingBox ();

			/*
			for (int i = 0; i < objectList.Count; i++) {         
				if (objectList[i] != initiater)
				{
					objectList [i].Highlight ();
					objectList [i].ShowBoundingBox (false);
				}
			}*/ 
		}
    }

	public void UnFocusGroup(ModelingObject initiater, Selection controller)
    {
		if (!selected) {
			focused = false;

			boundingBox.ClearBoundingBox ();

			for (int i = 0; i < objectList.Count; i++)
			{
				objectList [i].UnHighlight ();
				objectList [i].HideBoundingBox (false);
			}
		}
    }

	public void DeFocusElements(Selection controller){
		
		for (int i = 0; i < objectList.Count; i++)
		{
			//objectList [i].UnHighlight ();
			objectList [i].HideBoundingBox (false);
		}

	}

    public void RotateGroup(ModelingObject initiater, Vector3 angleAxis, float angle)
    {
        // use
        RotationCenter = GetBoundingBoxCenter();

        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
             //   objectList[i].RotateAround(angleAxis, angle);
            }
        }

    }

    public void DuplicateGroup()
    {
        // create new group
        Group newGroup;
        newGroup = ObjectsManager.Instance.CreateGroup();

        // Go through all objects, duplicate each
        for (int i = 0; i < objectList.Count; i++)
        {
          //  ObjectCreator.Instance.DuplicateObject(objectList[i], newGroup);
        }
    }

    public void SelectGroup(ModelingObject initiater)
    {
		selected = true;
		DrawBoundingBox ();
    }

	public void DeSelectGroup(ModelingObject initiator, Selection controller)
    {		
		selected = false;
		boundingBox.ClearBoundingBox ();
		UnFocusGroup (initiator, controller);
    }

    public void Move(Vector3 distance, ModelingObject initiater)
    {
		boundingBox.ClearBoundingBox ();

		for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
                // apply a similar movement
                objectList[i].MoveModelingObject(distance);
            }
        }
    }

    public void StopMoving(Selection controller, ModelingObject initiater)
    {
		DrawBoundingBox ();

        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
                objectList[i].StopMoving(controller, initiater);
            }
        }
    }

    public void StartScalingGroup(ModelingObject initiater)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
                objectList[i].StartScaling(false);
            }
        }
    }

    public void ScaleBy(float newScale, ModelingObject initiater)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
                objectList[i].ScaleBy(newScale, false);
            }
        }
    }

    public void TrashGroup(ModelingObject initiater)
    {
		boundingBox.ClearBoundingBox ();

        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
                objectList[i].TrashObject(false);
            }
        }

		ObjectsManager.Instance.DeleteGroup (this);
    }

	public void ColorGroup(ModelingObject initiater, Color color){
		for (int i = 0; i < objectList.Count; i++)
		{
			if (objectList[i] != initiater)
			{
				objectList[i].ChangeColor(color, false);
			}
		}
	}
  
	public Vector3 GetPosOfClosestVertex(Vector3 position, Vector3[] coordinates){
		
		Vector3 closestVertex = coordinates[0];
		float shortestDistance = 999999f;

		for (int i = 0; i < coordinates.Length; i++)
		{
			Vector3 newCoordinate = coordinates[i];

			if (Vector3.Distance(position, newCoordinate) < shortestDistance)
			{
				closestVertex = newCoordinate;
				shortestDistance = Vector3.Distance(position, newCoordinate);
			}
		}

		return closestVertex;
	}
		

	public void DeActivateCollider (){
		for (int i = 0; i < objectList.Count; i++)
		{
			objectList[i].DeActivateCollider();
		}
	}

	public void DarkenColorObject (){
		for (int i = 0; i < objectList.Count; i++)
		{
			objectList[i].DarkenColorObject();
		}
	}


	public void ActivateCollider (){
		for (int i = 0; i < objectList.Count; i++)
		{
			objectList[i].ActivateCollider();
		}
	}

	public void NormalColorObject (){
		for (int i = 0; i < objectList.Count; i++)
		{
			objectList[i].NormalColorObject();
		}
	}

	public void SetLowestObjectOfGroup(){
		lowestModObject = objectList [0];

		for (int i = 0; i < objectList.Count; i++)
		{
			if (objectList [i].transform.position.y < lowestModObject.transform.position.y) {
				lowestModObject = objectList[i];
			}
		}
	}
		
}
