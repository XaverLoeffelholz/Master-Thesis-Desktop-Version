using UnityEngine;
using System.Collections;

public class VertexBundle : MonoBehaviour {

    public Vector3 coordinates;
    public bool centerVertex = false;
	public VertexBundle possibleSnappingVertexBundle;
    public GameObject possibleLineSnapping;
    public GameObject possibleGroundSnapping;

	public bool usedForSnapping = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize() {
		// initiate all children that are of type Vertex
		Vertex[] vertices = transform.GetComponentsInChildren<Vertex>();

		foreach (Vertex vert in vertices) {
			vert.Initialize ();
            if (!centerVertex)
            {
				//Collider col = vert.GetComponent<BoxCollider>();
              //  col.enabled = false;

              //  Rigidbody rig = vert.GetComponent<Rigidbody>();
             //   rig.detectCollisions = false;
            }
		}
	}

    public void Show() {
        Vertex[] vertices = transform.GetComponentsInChildren<Vertex>();

        foreach (Vertex vert in vertices)
        {
            vert.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void Hide()
    {
        Vertex[] vertices = transform.GetComponentsInChildren<Vertex>();

        foreach (Vertex vert in vertices)
        {
            vert.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

	public void ActivateCollisionDetection(){
		Vertex[] vertices = transform.GetComponentsInChildren<Vertex>();

		foreach (Vertex vert in vertices)
		{
			vert.gameObject.GetComponent<BoxCollider>().enabled = true;
		}
	}

	public void DeactivateCollisionDetection(){
	/*	Vertex[] vertices = transform.GetComponentsInChildren<Vertex>();

		foreach (Vertex vert in vertices)
		{
			vert.gameObject.GetComponent<BoxCollider>().enabled = false;
		} */
	}
}
