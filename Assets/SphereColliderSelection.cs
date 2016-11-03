using UnityEngine;
using System.Collections;

public class SphereColliderSelection : MonoBehaviour {

	// we have to check for all collisions

	public GameObject currentCollider;
	public MeshRenderer renderer;
	public CapsuleCollider collider;
	public Transform positionOfMostInterest;

	private int numberOfContactPoints = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision){
		float distanceToPosOfInterest = 99999f;

		// we can also compare which collider has more points and choose that one
		foreach (ContactPoint contact in collision.contacts) {
			float currentDistance = (contact.point - positionOfMostInterest.position).magnitude;

			if (currentDistance < distanceToPosOfInterest) {
				distanceToPosOfInterest = currentDistance;
				currentCollider = contact.otherCollider.gameObject;
			}
		}

		/*
		 * 
		if (currentCollider == null) {
			currentCollider = collision.collider.gameObject;
			numberOfContactPoints = collision.contacts.Length;
		} else {
			if (collision.contacts.Length > numberOfContactPoints) {
				currentCollider = collision.collider.gameObject;
				numberOfContactPoints = collision.contacts.Length;
			}
		}

		*/

	}

	void OnCollisionStay(Collision collision){
		
		float distanceToPosOfInterest = 99999f;

		// we can also compare which collider has more points and choose that one
		foreach (ContactPoint contact in collision.contacts) {
			float currentDistance = (contact.point - positionOfMostInterest.position).magnitude;

			if (currentDistance < distanceToPosOfInterest) {
				distanceToPosOfInterest = currentDistance;
				currentCollider = contact.otherCollider.gameObject;
			}
		}

		/*
		if (currentCollider != null) {
			if (collision.gameObject == currentCollider.gameObject) {
				numberOfContactPoints = collision.contacts.Length;
			} else {
				if (collision.contacts.Length > numberOfContactPoints) {
					currentCollider = collision.collider.gameObject;
					numberOfContactPoints = collision.contacts.Length;
				} 
			}
		}	
		*/

	}

	void OnCollisionExit(Collision collision){
		if (currentCollider != null && collision.gameObject == currentCollider.gameObject) {
			currentCollider = null;
			numberOfContactPoints = 0;
		}
	}

	public void ActivateCollider(){
		collider.enabled = true;
		//renderer.enabled = true;
		currentCollider = null;
	}

	public void DeActivateCollider(){
		collider.enabled = false;
		//renderer.enabled = false;
		currentCollider = null;
	}
}
