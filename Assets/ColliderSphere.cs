using UnityEngine;
using System.Collections;

public class ColliderSphere : MonoBehaviour {

	public ModelingObject possibleSnapping;
	public ModelingObject SnappedToThis;
	public bool parentMoving;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if (parentMoving && other.gameObject.CompareTag ("ColliderSphere")) {	
			if (other.gameObject.GetComponent<ColliderSphere> ().SnappedToThis == null) {
				possibleSnapping = other.transform.parent.GetComponent<ModelingObject>();
				other.gameObject.GetComponent<ColliderSphere> ().SnappedToThis = transform.parent.GetComponent<ModelingObject> ();
			}
		}
	}

	void OnTriggerExit(Collider other){
		if (possibleSnapping != null && parentMoving && other.gameObject.CompareTag ("ColliderSphere")) {	
			if (other.transform.parent.gameObject == possibleSnapping.gameObject) {
				other.gameObject.GetComponent<ColliderSphere> ().SnappedToThis = null;
				possibleSnapping = null;
			}
		}
	}
}
