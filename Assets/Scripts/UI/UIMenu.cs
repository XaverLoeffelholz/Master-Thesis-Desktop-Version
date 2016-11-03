using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIMenu : MonoBehaviour {

    public enum menuType
    {
        MainMenu,
        Rotation,
        NonUniformScaling,
        Color,
        Shape,
        Object,
        Group,
        NewObject
    }

    public enum buttonType
    {
        Menu,
		CloseMenu,
        Delete,
        GroupStart,
		GroupBreak,
        GroupEnd,
        Duplicate,
        Color,
        ResetFrustum,
        ResetRotation,
        ResetColor,
        Extrude
    }

    public menuType TypeOfMenu;
    public Selection controller1;
    public Selection controller2;

    private UiCanvasGroup parentCanvas;

    // Use this for initialization
    void Start () {
        parentCanvas = transform.parent.GetComponent<UiCanvasGroup>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ActivateMenu()
    {
        // get all menus, deactivate them
        List<GameObject> UiMenus = new List<GameObject>();

        // only activate this one
        foreach (Transform menu in transform.parent)
        {
            if (menu != this.gameObject)
            {
                // deactivate all buttons in other menus
                foreach (Transform button in menu)
                {
                    button.gameObject.SetActive(false);
                }
            }
        }

        List<GameObject> buttons = new List<GameObject>();
        foreach (Transform child in transform)
        {
			if (child.parent == transform)
            {
				if (child.name == "UI Close"){
					child.gameObject.SetActive(true);
					child.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, 0f, 0f);
				} else {			
					if(!child.gameObject.CompareTag("UIElementNotInUse")){						
						// check if it is a group 
						if (UiCanvasGroup.Instance.currentModelingObject.group == null) {
							child.gameObject.SetActive(true);
							buttons.Add(child.gameObject);
						} else {
							if (!(child.gameObject.name == "UI Shape")){
								child.gameObject.SetActive(true);
								buttons.Add(child.gameObject);
							}
						}
					}
				}
            }
        }

        transform.parent.GetComponent<UiCanvasGroup>().ArrangeUIObjects(buttons);
        transform.parent.GetComponent<UiCanvasGroup>().Show();

        controller1.enableFaceSelection(false);
        controller2.enableFaceSelection(false);

		UiCanvasGroup.Instance.currentMenu = this;

		ActivateCurrentHandles ();
    }


	public void ActivateCurrentHandles(){
		// Show the right handles
		switch (TypeOfMenu)
		{
		case (menuType.Rotation):
			parentCanvas.currentModelingObject.handles.ShowRotationHandles();
			parentCanvas.currentModelingObject.ShowBoundingBox(true);
			break;
		case (menuType.Color):
			parentCanvas.currentModelingObject.handles.DisableHandles();
			break;
		case (menuType.Shape):
			parentCanvas.currentModelingObject.handles.ShowFrustumHandles();
			break;
		case (menuType.Object):
			parentCanvas.currentModelingObject.handles.DisableHandles();
			break;
		case (menuType.Group):
			parentCanvas.currentModelingObject.handles.DisableHandles();
			break;
		case (menuType.NewObject):
			parentCanvas.currentModelingObject.handles.DisableHandles ();
			break;
		case (menuType.MainMenu):
			if (UiCanvasGroup.Instance.currentModelingObject.group == null) {
				parentCanvas.currentModelingObject.handles.ShowNonUniformScalingHandles();
			} else {
				parentCanvas.currentModelingObject.handles.DisableHandles();
			}					
			break;
		}
	}

    public void PerformAction(UiElement button, Selection controller)
    {
        switch (button.typeOfButton)
        {
            case (buttonType.Menu):
				Logger.Instance.AddLine (Logger.typeOfLog.menuNavigationUI);
                break;
            case (buttonType.Color):
				Logger.Instance.AddLine (Logger.typeOfLog.colorUI);
                ChangeColor(button);
                break;
            case (buttonType.Delete):
				Logger.Instance.AddLine (Logger.typeOfLog.deleteObjectUI);
                Delete();				
                break;
			case (buttonType.Duplicate):				
				Logger.Instance.AddLine (Logger.typeOfLog.duplicateObjectUI);
				Duplicate ();
                break;
			case (buttonType.GroupStart):
				Logger.Instance.AddLine (Logger.typeOfLog.groupStartUI);
				if (parentCanvas.currentModelingObject.group == null) {
					StartGroup ();
				} else {
					ContinueGrouping ();
				}
                break;
			case (buttonType.GroupBreak):
				Logger.Instance.AddLine (Logger.typeOfLog.groupBreakUI);
				parentCanvas.currentModelingObject.group.BreakGroup (controller);
				EndGroup (controller);
				//parentCanvas.CloseMenu (controller);	
				//parentCanvas.currentModelingObject.Select (controller, controller.uiPositon.position);
				//parentCanvas.currentModelingObject.ShowBoundingBox (false);
				break;
            case (buttonType.GroupEnd):
				Logger.Instance.AddLine (Logger.typeOfLog.groupEndUI);
				EndGroup (controller);
                break;
            case (buttonType.ResetColor):
                break;
			case (buttonType.CloseMenu):
				Logger.Instance.AddLine (Logger.typeOfLog.closeMenuUI);
				EndGroup (controller);
				parentCanvas.CloseMenu(controller);
				break;
        }

    }


    // All functions by buttons
    public void Extrude()
    {
        controller1.enableFaceSelection(true);
        controller2.enableFaceSelection(true);
    }

	public void ContinueGrouping(){
		ObjectsManager.Instance.currentGroup = parentCanvas.currentModelingObject.group;
		ObjectsManager.Instance.EnableObjects ();

		// activate selection of item for group
		controller1.groupItemSelection = true;
		controller2.groupItemSelection = true;
	}

    public void StartGroup()
    {
        // create group
        Group newGroup;
        newGroup = ObjectsManager.Instance.CreateGroup();

		ObjectsManager.Instance.EnableObjects ();

        // add first element to group
        ObjectsManager.Instance.AddObjectToGroup(newGroup, parentCanvas.currentModelingObject);

        // activate selection of item for group
        controller1.groupItemSelection = true;
        controller2.groupItemSelection = true;

		newGroup.SelectGroup (parentCanvas.currentModelingObject);
    }



	public void EndGroup(Selection controller)
    {
		if (parentCanvas.currentModelingObject.group != null) {

			if (parentCanvas.currentModelingObject.group.objectList.Count == 1) {
				parentCanvas.currentModelingObject.group.BreakGroup (controller);
			} else {
				parentCanvas.currentModelingObject.group.DeFocusElements(controller);
			}
		}

        // deactivate selection of item for group
        controller1.groupItemSelection = false;
        controller2.groupItemSelection = false;
    }



    public void Delete()
    {
        parentCanvas.Hide();
		ObjectsManager.Instance.EnableObjects ();
        UiCanvasGroup.Instance.currentModelingObject.TrashObject(true);
    }

    public void Duplicate()
    {
		if (parentCanvas.currentModelingObject.group == null) {
			parentCanvas.currentModelingObject.CalculateBoundingBox ();
			Vector3 position = parentCanvas.currentModelingObject.transform.localPosition + (2f * parentCanvas.currentModelingObject.transform.InverseTransformVector(parentCanvas.currentModelingObject.GetBoundingBoxTopCenter () - parentCanvas.currentModelingObject.GetBoundingBoxCenter ()));

            // needs to be local position
			ObjectCreator.Instance.DuplicateObject (parentCanvas.currentModelingObject, null, position);
		} else {
			parentCanvas.currentModelingObject.group.UpdateBoundingBox ();
			Vector3 offset = 2 * parentCanvas.currentModelingObject.group.GetBoundingBoxTopCenter () - parentCanvas.currentModelingObject.group.GetBoundingBoxCenter ();
			ObjectCreator.Instance.DuplicateGroup (parentCanvas.currentModelingObject.group, parentCanvas.currentModelingObject.transform.InverseTransformVector(offset));
		}


    }

    public void ChangeColor(UiElement button)
    {
        UiCanvasGroup.Instance.currentModelingObject.ChangeColor(button.gameObject.GetComponent<Image>().color, true);
    }
}
