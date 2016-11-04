using UnityEngine;
using System.Collections;

public class Trash : Singleton<Trash> {
    private Color initialColor;
    public Selection controller1;
    public Selection controller2;

	// Use this for initialization
	void Start () {
        initialColor = transform.GetComponent<Renderer>().material.color;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Mesh"))
        {
            TrashAreaActive(true);
            other.transform.parent.GetComponent<ModelingObject>().inTrashArea = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Mesh"))
        {
            TrashAreaActive(false);
            other.transform.parent.GetComponent<ModelingObject>().inTrashArea = false;
        }
    }

    public void TrashAreaActive(bool value)
    {
        if (value)
        {
            transform.GetComponent<Renderer>().material.color = new Color(0.7f, 0.1f, 0.1f, 0.6f);
        } else
        {
            transform.GetComponent<Renderer>().material.color = initialColor;
        }

    }
}
