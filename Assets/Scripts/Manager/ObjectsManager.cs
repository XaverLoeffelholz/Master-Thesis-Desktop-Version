using UnityEngine;
using System.Collections;

public class ObjectsManager : Singleton<ObjectsManager>
{
    public Transform DistanceVisualisation;
    public GameObject groupPrefab;
    public Group currentGroup;
    public GameObject user;
	public Transform stage;
	public Transform stageScaler;

    // Use this for initialization
    void Start () {
        user = ObjectsManager.Instance.user;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
	public void DeleteGroup (Group group){
		Destroy (group.gameObject);
	}

    public void HideAllHandles()
    {
		Debug.Log ("Hide all handls");
		
        foreach(Transform model in this.transform)
        {
            if (model.CompareTag("ModelingObject"))
            {
                model.GetComponent<ModelingObject>().handles.DisableHandles();
            }

        }
    }

    public ModelingObject GetlatestObject()
    {
        int i = 0;

        foreach (Transform model in this.transform)
        {
            if (model.CompareTag("ModelingObject"))
            {
              if (model.GetComponent<ModelingObject>().ObjectID > i)
                {
                    i = model.GetComponent<ModelingObject>().ObjectID;
                }
            }
        }

        ModelingObject m = GameObject.Find("Object " + i).GetComponent<ModelingObject>();

        return m;
    }

    public Group CreateGroup()
    {
        GameObject newgroup = Instantiate(groupPrefab);
        newgroup.transform.SetParent(transform);

        newgroup.transform.localPosition = Vector3.zero;
        newgroup.transform.localEulerAngles = Vector3.zero;
        newgroup.transform.localScale = Vector3.one;

        currentGroup = newgroup.GetComponent<Group>();
        return currentGroup;
    }

	public void AddAllObjectsOfGroupToGroup(Group newGroup, Group oldGroup){
		for (int i = 0; i < oldGroup.objectList.Count; i++) {
			AddObjectToGroup (newGroup, oldGroup.objectList [i]);
		}

		DeleteGroup (oldGroup);
	}

    public void AddObjectToGroup(Group group, ModelingObject modelingObject)
    {
        modelingObject.transform.SetParent(group.transform);
        modelingObject.group = group;
        group.objectList.Add(modelingObject);
		group.DrawBoundingBox ();
    }

    public void TakeObjectOutOfGroup(Group group, ModelingObject modelingObject)
    {
        modelingObject.transform.SetParent(group.transform.parent);
        modelingObject.group = null;
        group.objectList.Remove(modelingObject);
		group.DrawBoundingBox ();
    }

    public void ReOpenGroup(Group group)
    {
        currentGroup = group.GetComponent<Group>();
    }

	public void DisableObjectsExcept(ModelingObject selectedObject){
		foreach(Transform model in this.transform)
		{
			if (model.CompareTag ("ModelingObject")) {
				ModelingObject currentModelingObject = model.GetComponent<ModelingObject> ();

				if (currentModelingObject != selectedObject) {
					currentModelingObject.DeActivateCollider ();
					currentModelingObject.DarkenColorObject ();
				}
			} else if (model.CompareTag ("Group")) {
				Group currentGroup =  model.GetComponent<Group> ();

				if (currentGroup != selectedObject.group) {
					currentGroup.DeActivateCollider ();
					currentGroup.DarkenColorObject ();
				}
			}
		}
	}

	public void DisableObjectsExcept(Group selectedGroup){
		// disable all objects except group
	}


	public void EnableObjects(){
		foreach(Transform model in this.transform)
		{
			if (model.CompareTag("ModelingObject"))
			{
				ModelingObject currentModelingObject = model.GetComponent<ModelingObject> ();

				currentModelingObject.ActivateCollider ();
				currentModelingObject.NormalColorObject ();
			} else if (model.CompareTag ("Group")) {
				Group currentGroup =  model.GetComponent<Group> ();

				currentGroup.ActivateCollider ();
				currentGroup.NormalColorObject ();
			}
		}
	}

	public void StartMovingObject(ModelingObject movedObject){		
		foreach(Transform unit in this.transform)
		{
			if (unit.CompareTag("ModelingObject"))
			{
				ModelingObject currentModelingObject = unit.GetComponent<ModelingObject> ();

				if (currentModelingObject != movedObject) {
					currentModelingObject.UseAsPossibleSnap ();
				}
			}

			if (unit.CompareTag ("Group")) {
				if (unit.transform != movedObject.group) {
					foreach(Transform groupElement in unit.transform)
					{
						if (groupElement.CompareTag("ModelingObject"))
						{
							ModelingObject currentModelingObject = groupElement.GetComponent<ModelingObject> ();

							if (currentModelingObject != movedObject) {
								currentModelingObject.UseAsPossibleSnap ();
							}
						}
					}
				}
			}
		}
	}

	public void StopMovingObject(ModelingObject movedObject){	

		foreach(Transform unit in this.transform)
		{
			if (unit.CompareTag("ModelingObject"))
			{
				ModelingObject currentModelingObject = unit.GetComponent<ModelingObject> ();

				if (currentModelingObject != movedObject) {
					currentModelingObject.StopUseAsPossibleSnap ();
				}
			}

			if (unit.CompareTag ("Group")) {
				if (unit.transform != movedObject.group) {
					foreach(Transform groupElement in unit.transform)
					{
						if (groupElement.CompareTag("ModelingObject"))
						{
							ModelingObject currentModelingObject = groupElement.GetComponent<ModelingObject> ();

							if (currentModelingObject != movedObject) {
								currentModelingObject.StopUseAsPossibleSnap ();
							}
						}
					}
				}
			}
		}
	}



}
