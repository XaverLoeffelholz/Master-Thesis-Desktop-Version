using UnityEngine;
using System.Collections;
using System;

public class Face : MonoBehaviour {

	public enum faceType {
		TopFace = 0,
		BottomFace = 1,
		SideFace = 2
	}

    public ModelingObject parentModelingObject;

	public faceType typeOfFace;

    public VertexBundle[] vertexBundles;
    public VertexBundle center;
    public VertexBundle scaler;
    public Vector3 scalerPosition;
    public Vector3 centerPosition;
	public Vector3 normal;
    public handle scaleHandle;
    public handle centerHandle;
    public handle heightHandle;

	public GameObject VertexBundlePrefab;
	public GameObject VertexPrefab;

    private Vector3 lastScalerToCenterbottom;
    private GameObject FaceSelectionVisual;

    // Use this for initialization
    void Start () {
		
    }
	
	// Update is called once per frame
	void Update () {

    }

	public void InitializeFace(int numberOfVertices) {
		vertexBundles = new VertexBundle[numberOfVertices];
        parentModelingObject = transform.parent.parent.GetComponent<ModelingObject>();
    }

    public void ScaleFace(float amount, Vector3 relativeTo)
    {
        for (int i = 0; i < vertexBundles.Length; i++)
        {
            if (vertexBundles[i] != null)
            {
                Vector3 VertexToPoint = vertexBundles[i].coordinates - relativeTo;
                vertexBundles[i].coordinates = RasterManager.Instance.Raster(vertexBundles[i].coordinates + VertexToPoint * amount);
                vertexBundles[i].coordinates = vertexBundles[i].coordinates + VertexToPoint * amount;
            }

        }
    }

    public void UpdateScaleFromCorner()
    {
        float lengthScalerToCenter = (scaler.coordinates - centerPosition).magnitude;

        if (Vector3.Dot((scalerPosition - centerPosition), (scaler.coordinates - centerPosition)) < 0f)
        {
            lengthScalerToCenter = (-1f) * lengthScalerToCenter;
        }

		// get amount of rescaling
		float amountRescaling = lengthScalerToCenter/(scalerPosition - centerPosition).magnitude;
        scalerPosition = scaler.coordinates;

        for (int i = 0; i < vertexBundles.Length; i++)
        {
            if (vertexBundles[i] != null && (vertexBundles[i] != scaler) && (vertexBundles[i] != center))
            {
                Vector3 VertexToCenter = vertexBundles[i].coordinates - centerPosition;
				vertexBundles[i].coordinates = centerPosition + VertexToCenter * amountRescaling;
            }

        }
    }

    public void UpdateFaceFromCenter()
    {
        Vector3 difference = center.coordinates - centerPosition;
        centerPosition = center.coordinates;

        // go through each vertex and apply a similar translation
        for (int i=0; i < vertexBundles.Length; i++)
        {
            if (vertexBundles[i] != center)
            {
                vertexBundles[i].coordinates += difference;
            }
        }

        scaleHandle.transform.localPosition += difference;

        centerHandle.transform.localPosition = centerPosition;
        if (heightHandle!= null)
        {
            heightHandle.transform.localPosition = centerPosition;
        }

        scalerPosition = scaler.coordinates;

		parentModelingObject.CalculateBoundingBox ();
		parentModelingObject.PositionHandles (false);
		parentModelingObject.RotateHandles ();
    }

	public void AddVertexBundle(VertexBundle vBundle){
		for (int i=0; i< vertexBundles.Length; i++){
			if (vertexBundles [i] == null) {
				vertexBundles [i] = vBundle;
				break;
			}
		}

	}

	public void RepositionScaler(){
		Vector3[] vertexBundlesCoordinates = new Vector3[vertexBundles.Length];
		int idOfVertexbundle;

		for (int i=0; i< vertexBundles.Length; i++){
			vertexBundlesCoordinates [i] = parentModelingObject.transform.TransformPoint(vertexBundles [i].coordinates);
		}

		scalerPosition = parentModelingObject.transform.InverseTransformPoint(parentModelingObject.GetPosOfClosestVertex(Camera.main.transform.position, vertexBundlesCoordinates));

		for (int i=0; i< vertexBundles.Length; i++){
			if (vertexBundles [i].coordinates == scalerPosition) {
				scaler = vertexBundles [i];
				parentModelingObject.scalerObject = vertexBundles [i];
				break;
			}
		}
	}


    public void SetScaler()
    {
		if (parentModelingObject.typeOfObject != ModelingObject.ObjectType.octagon) {
			if (typeOfFace == faceType.TopFace) {   
				scaler = vertexBundles [1];
				scalerPosition = scaler.coordinates;

				parentModelingObject.scalerObject = vertexBundles [1];

			} else if (typeOfFace == faceType.BottomFace) {   
				scaler = vertexBundles [vertexBundles.Length - 1];
				scalerPosition = scaler.coordinates;

				parentModelingObject.scalerObject = vertexBundles [vertexBundles.Length - 1];
			}
		} else {
			if (typeOfFace == faceType.TopFace) {   
				scaler = vertexBundles [1];
				scalerPosition = scaler.coordinates;

				parentModelingObject.scalerObject = vertexBundles [1];

			} else if (typeOfFace == faceType.BottomFace) {   
				scaler = vertexBundles [vertexBundles.Length - 2];
				scalerPosition = scaler.coordinates;

				parentModelingObject.scalerObject = vertexBundles [vertexBundles.Length - 2];
			}
		}


    }

	public void CalculateCenter(){
		
		Vector3 centerPositionNew = new Vector3 (0, 0, 0);

		for (int i = 0; i < vertexBundles.Length; i++) {
			centerPositionNew += vertexBundles [i].coordinates;
		}

		centerPositionNew = centerPositionNew / vertexBundles.Length;

		// check if there is a bundle with the center already
		foreach (Transform child in transform)
		{
			if (child.gameObject.CompareTag("VertexBundle")){
				if (child.gameObject.GetComponent<VertexBundle>().coordinates == centerPositionNew) {
					center = child.gameObject.GetComponent<VertexBundle>();
				}
			}
		}

		if (center == null) {
			GameObject centerGameObject;
			GameObject centerVertexGameObject;

			centerGameObject = Instantiate (VertexBundlePrefab);
			centerVertexGameObject = Instantiate (VertexPrefab);
			centerVertexGameObject.transform.localPosition = centerPositionNew;
			centerVertexGameObject.transform.SetParent (centerGameObject.transform);
			centerGameObject.transform.SetParent (this.transform);
			center = centerGameObject.GetComponent<VertexBundle> ();
			center.coordinates = centerPositionNew;
		}

		centerPosition = centerPositionNew;

		if (typeOfFace == faceType.TopFace) {
			center.name = "Center Top";
            center.centerVertex = true;
			OrderVertexBundlesClockwise ();
            SetScaler();

        } else if (typeOfFace == faceType.BottomFace) {
			center.name = "Center Bottom";
            center.centerVertex = true;
            OrderVertexBundlesClockwise ();
            SetScaler();

        } else {
			center.name = "Center Side";
            center.centerVertex = true;
			center.DeactivateCollisionDetection ();
        }

		center.Initialize ();
	}

	public void OrderVertexBundlesClockwise(){

		// Quick fix for Octagon
		if (parentModelingObject.typeOfObject == ModelingObject.ObjectType.octagon) {

			if (typeOfFace == faceType.TopFace) {
				VertexBundle[] OrderedVertexBundles = new VertexBundle[vertexBundles.Length];

				for (int i = 0; i < OrderedVertexBundles.Length; i++) {
					OrderedVertexBundles [i] = vertexBundles [vertexBundles.Length - i - 1];
				}

				vertexBundles = OrderedVertexBundles;
			} 
		} else {
			VertexBundle[] OrderedVertexBundles = new VertexBundle[vertexBundles.Length];
			VertexBundle[] LeftVertexBundles = vertexBundles;

			OrderedVertexBundles [0] = vertexBundles [0];

			for (int i = 1; i < OrderedVertexBundles.Length; i++) {

				float distance = 0f;
				int idOfnextBundle = 0;

				// check which vertex bundle is the closes clockwise of the current one
				for (int j = 1; j < LeftVertexBundles.Length; j++) {
					if (LeftVertexBundles[j] != null) {
						float newDistance = Vector3.Dot (normal, Vector3.Cross (OrderedVertexBundles [i-1].coordinates - centerPosition, LeftVertexBundles [j].coordinates - centerPosition));
						if (newDistance > distance) {
							distance = newDistance;
							idOfnextBundle = j;
						}
					}

				}

				distance = 0f;
				OrderedVertexBundles [i] = LeftVertexBundles [idOfnextBundle];
				LeftVertexBundles [idOfnextBundle] = null;

			}

			vertexBundles = OrderedVertexBundles;
		}
		


	}

	public void UpdateCenter(){
		Vector3 centerPositionNew = new Vector3 (0, 0, 0);

		for (int i = 0; i < vertexBundles.Length; i++) {
			centerPositionNew += vertexBundles [i].coordinates;
		}

		centerPositionNew = centerPositionNew / vertexBundles.Length;
		center.coordinates = centerPositionNew;
		centerPosition = centerPositionNew;
	}

	public void RecalculateNormal(){
		// maybe we can get it from the mesh, otherwise calculate here
		Vector3 dir = Vector3.Cross(vertexBundles[1].coordinates - vertexBundles[0].coordinates, vertexBundles[2].coordinates - vertexBundles[0].coordinates);
		normal = Vector3.Normalize(dir);
	}

	public void SetType(faceType type){
		typeOfFace = type;
		this.gameObject.name = type.ToString ();
	}

	public void ReplaceFaceOnOtherFace(Face other, Vector3 distance, bool reverseOrder){

		if (!reverseOrder) {
			for (int i = 0; i < vertexBundles.Length; i++) {
                if (i == 0)
                {
                    Vector3 newPoint = other.vertexBundles[vertexBundles.Length - 1].transform.TransformPoint(other.vertexBundles[vertexBundles.Length - 1].coordinates);
                    newPoint += other.vertexBundles[vertexBundles.Length - 1].transform.TransformDirection(distance);
                    vertexBundles[i].coordinates = vertexBundles[i].transform.InverseTransformPoint(newPoint);


                    // old:
                    //  vertexBundles[i].coordinates = other.vertexBundles[vertexBundles.Length - 1].coordinates + distance;
                }
                else
                {
                    Vector3 newPoint = other.vertexBundles[i - 1].transform.TransformPoint(other.vertexBundles[i - 1].coordinates);
                    newPoint += other.vertexBundles[i - 1].transform.TransformDirection(distance);
                    vertexBundles[i].coordinates = vertexBundles[i].transform.InverseTransformPoint(newPoint);

                    // old:
                    // vertexBundles[i].coordinates = other.vertexBundles[i - 1].coordinates + distance;
                }
			}
		} else {
			for (int i = 0; i < vertexBundles.Length; i++) {
                Vector3 newPoint = other.vertexBundles[vertexBundles.Length - 1 - i].transform.TransformPoint(other.vertexBundles[vertexBundles.Length - 1 - i].coordinates);
                newPoint += other.vertexBundles[vertexBundles.Length - 1 - i].transform.TransformDirection(distance);
                vertexBundles[i].coordinates = vertexBundles[i].transform.InverseTransformPoint(newPoint);
                
                // old:
                // vertexBundles[i].coordinates = other.vertexBundles[vertexBundles.Length - 1 - i].coordinates + distance;
            }
        }

		UpdateCenter ();
		RecalculateNormal ();
	}

    public void CreateNewModelingObject(){
		ObjectCreator.Instance.createNewObjectOnFace (this);
	}


    public void Highlight()
    {
        for (int i = 0; i < vertexBundles.Length; i++)
        {
            vertexBundles[i].Show();
        }

        // Display outline of face
        FaceSelectionVisual = Instantiate(parentModelingObject.GroundVisualPrefab);
        LineRenderer lines = FaceSelectionVisual.GetComponent<LineRenderer>();
        lines.SetWidth(0.02f, 0.02f);
        lines.SetColors(Color.white, Color.white);
        lines.SetVertexCount(this.vertexBundles.Length + 1);

        for (int j = 0; j <= this.vertexBundles.Length; j++)
        {
            if (j == this.vertexBundles.Length)
            {
                Vector3 pos = this.vertexBundles[0].transform.GetChild(0).position;
                lines.SetPosition(j, pos);
            }
            else
            {
                Vector3 pos = this.vertexBundles[j].transform.GetChild(0).position;
                lines.SetPosition(j, pos);
            }

        }
    }

    public void UnHighlight()
    {
        Destroy(FaceSelectionVisual);        
            
        for (int i = 0; i < vertexBundles.Length; i++)
        {
            vertexBundles[i].Hide();
        }
    }

    public void ReplaceFacefromObjectScaler(Vector3 relativeTo, float amount)
    {

        for (int i = 0; i < vertexBundles.Length; i++)
        {
            if (vertexBundles[i] != null && (vertexBundles[i] != parentModelingObject.scalerObject))
            {
                Vector3 VertexToScaleCenter = vertexBundles[i].coordinates - relativeTo;
                vertexBundles[i].coordinates = relativeTo + VertexToScaleCenter * (amount);
            }

        }

        UpdateCenter();
    }

    public void UpdateSpecialVertexCoordinates()
    {
        if (center != null)
        {
            centerPosition = center.coordinates;
        }

        if (scaler != null)
        {
            scalerPosition = scaler.coordinates;
        }

    }
}
