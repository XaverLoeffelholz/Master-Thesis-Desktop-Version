using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UiCanvasGroup : Singleton<UiCanvasGroup>{
    public bool visible;
    public Transform player;
	public Transform headset;
    private CanvasGroup canvGroup;

    public float positioningWidth;
    public float positioningHeight;
    public ModelingObject currentModelingObject;

	public Selection controller1;
	public Selection controller2;

    public GameObject MainMenu;
    private float distanceUserObject;

    public UIMenu mainMenu;
    public UIMenu rotationMenu;
    public UIMenu colorMenu;
    public UIMenu shapeMenu;
    public UIMenu objectMenu;
    public UIMenu newObjectMenu;

	public UIMenu currentMenu;

	public Color normalColor;
	public Color hoverColor;
	public Color clickColor;

	public CanvasGroup MenuBG;

	public Transform uiPos;

    // Use this for initialization
    void Start () {
        visible = false;
        canvGroup = this.GetComponent<CanvasGroup>();
    }
	
	// Update is called once per frame
	void Update () {
        if (visible)
        {
			transform.LookAt(Camera.main.transform);
			//transform.position = controller2.transform.position + controller2.LaserPointer.transform.forward * 0.1f;
			transform.position = uiPos.position;
			//transform.localRotation = controller2.transform.localRotation;
        }

    }

    public void DeactivateMenus()
    {
		if (!visible) {
			foreach (Transform menu in transform)
			{
				// deactivate all buttons in other menus
				foreach (Transform button in menu)
				{
					button.gameObject.SetActive(false);
				}
			}
		}     
    }

    public void PositionUI()
    {
        if (currentModelingObject)
        {
            Vector3 pos = transform.position;
            distanceUserObject = Vector3.Distance(pos, player.transform.position);
            transform.localScale = new Vector3(Mathf.Max(distanceUserObject / 6f, 0.2f), Mathf.Max(distanceUserObject / 6f, 0.2f), Mathf.Max(distanceUserObject / 6f, 0.2f));

            pos = new Vector3(pos.x, pos.y + 0.8f + (distanceUserObject * 0.1f), pos.z);
            transform.position = pos;
        }
    }

    public void OpenMainMenu(ModelingObject modelingObject, Selection controller)
    {
        currentModelingObject = modelingObject;
        // let menu always face controller that selected object
		player = controller.transform;
        MainMenu.GetComponent<UIMenu>().ActivateMenu();
    }

    public void Show()
    {
        LeanTween.alphaCanvas(canvGroup, 1f, 0.3f);
		LeanTween.alphaCanvas(MenuBG, 1f, 0.3f);
        visible = true;
    }

	public void CloseMenu(Selection controller){
		
		controller1.groupItemSelection = false;
		controller2.groupItemSelection = false;

		Hide();

		ObjectsManager.Instance.EnableObjects ();

		if (currentModelingObject != null)
		{
			currentModelingObject.DeSelect(controller);
		}
	}


    public void Hide()
    {
        LeanTween.alphaCanvas(canvGroup, 0f, 0.3f).setOnComplete(DeactivateMenus);
		LeanTween.alphaCanvas (MenuBG, 0f, 0.3f);
		//ObjectsManager.Instance.HideAllHandles();

        visible = false;
    }

	public void TemporarilyHide(){
	//	LeanTween.alphaCanvas(canvGroup, 0f, 0.3f).setOnComplete(DeactivateMenus);

		LeanTween.alphaCanvas(canvGroup, 0f, 0.3f).setOnComplete(DeactivateMenus);
		LeanTween.alphaCanvas (MenuBG, 0f, 0.3f);

		visible = false;
	}

	public void ShowAgain(Vector3 uiPosition){
		if (!visible) {
			LeanTween.alphaCanvas(canvGroup, 1f, 0.3f);
			LeanTween.alphaCanvas (MenuBG, 1f, 0.3f);

			if (currentMenu != null) {
				currentMenu.ActivateMenu ();
			}

			UiCanvasGroup.Instance.transform.position = uiPosition;
		}
	}

    private void deactivateCanvGroup()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ArrangeUIObjects(List<GameObject> elements)
    {
        // define lenght based on number of elements
        float width = elements.Count * positioningWidth;

        // define height based on number of elements
        // float height = elements.Count * positioningHeight;
		float height = 4f * positioningHeight;

        for (int i=0; i < elements.Count; i++)
        {
			float y = ((i+1) * positioningHeight) + 20;

          //  float y = - (((Mathf.Sin((float) (i+1) / (float) elements.Count * Mathf.PI)) * positioningHeight) - height / 2);

			elements[i].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f,1f,-0.5f) * y;
        }
    }
}
