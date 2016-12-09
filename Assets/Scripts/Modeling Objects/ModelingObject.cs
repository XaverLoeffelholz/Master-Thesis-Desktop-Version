using UnityEngine;
using System.Collections;
using System;

public class ModelingObject : MonoBehaviour
{
    public int ObjectID;

    public enum ObjectType
    {
        triangle = 3,
        square = 4,
        hexagon = 6,
        octagon = 8
    }

    public ObjectType typeOfObject;

    public handles handles;
    [HideInInspector]
    public bool selected = false;

    public Mesh mesh;
    private MeshCollider meshCollider;
    public Vector3[] MeshCordinatesVertices;
    private Vector2[] MeshUV;
    private int[] MeshTriangles;

    public Vertex[] vertices;
    public Face[] faces;
    public Face topFace;
    public Face bottomFace;
    public Color color;

    public GameObject VertexPrefab;
    public GameObject VertexBundlePrefab;

    public GameObject NormalPrefab;
    public GameObject Vertex2Prefab;
    public GameObject FacePrefab;

    private Selection controllerForMovement;
    private Vector3 lastPositionController;

    public bool moving = false;
    bool focused = false;

    public Vector3 PositionOnMovementStart;

    public GameObject linesPrefab;
    public GameObject DistanceVisualPrefab;
    public GameObject CenterVisualPrefab;
    public GameObject GroundVisualPrefab;
    public GameObject GroundVisualOnStartMoving;
	public GameObject GroundVisualOnStartMovingTop;
	public GameObject GroundVisualOnStartMovingBottom;
    public Transform DistanceVisualisation;
	public Vector3[] initialCoordinatesBoundingBox;

    private Vector3 lastPositionX;
    private Vector3 lastPositionY;
	private Vector3 lastPositionZ;
    private bool firstTimeMoving = true;

    private bool snapped = false;
    private Vector3 initialDistancceCenterBottomScaler;

	public float smoothTime = 0.1f;
	private Vector3 velocity = Vector3.zero;

    public VertexBundle scalerObject;

    public bool inTrashArea = false;
    public GameObject coordinateSystem;
    public Group group;

    public ObjectSelecter objectSelector;

    private Vector3 relativeTo;
	private Vector3[] vectorsTopOnScalingBegin;
	private Vector3[] vectorsBottomOnScalingBegin;
	private Vector3 boundingBoxTopToCenterOnScalingBegin;
	private Vector3 boundingBoxCenterToRightOnScalingBegin;

	private Transform player;
	public GameObject trashIcon;
	public Color currentColor;

	private bool initialBlocking = false;
	private Vector3 initialPositionController;
	private Vector3 initialDistanceCollisionGOModelingObject;

    public BoundingBox boundingBox;
	public BoundingBox boundingBoxWorld;
	public bool useBoundingBoxWorld = false;

	private bool trashed = false;
	public Transform rotationObject;
	private bool onRaster = true;

	public ColliderSphere colliderSphere;

	public float timeOnMovementStart;
	bool outOfLibrary = false;

	GameObject xAxis;
	GameObject yAxis;
	GameObject zAxis;

	public Lines connectingLinesHandles;

	private Selection selection;

	// vector used for bb calculation
	private Vector3 current;

	bool cameraMoving;

    // Use this for initialization
    void Start()
    {
        handles.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        DistanceVisualisation = ObjectsManager.Instance.DistanceVisualisation;
		player = Camera.main.transform;
		selection = GameObject.Find ("SelectionManager").GetComponent<Selection> ();
	}


	public void ToggleLocalGlobalBB(){

		useBoundingBoxWorld = !useBoundingBoxWorld;

		if (useBoundingBoxWorld) {
			SwitchToGlobalBoundingBox ();
		} else {
			SwitchToLocalBoundingBox ();
		}
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		if (selected && Input.GetKeyDown(KeyCode.P)){
			//Debug.Log("switch world local");
			ToggleLocalGlobalBB();
		}

		if (Input.GetMouseButtonDown (1)  || (selection.currentFocus == null && Input.GetMouseButton(0))) {
			cameraMoving = true;

			handles.HideRotationHandlesExcept (null);
			handles.HideScalingHandlesExcept (null);
			//connectingLinesHandles.ClearLines ();
		}

		if ((Input.GetMouseButtonUp (1) && selected) ||  (Input.GetMouseButtonUp (0) && selected)) {
			cameraMoving = false;

			PositionHandles (true);
			RotateHandles ();
			handles.ShowNonUniformScalingHandles ();
			handles.ShowRotationHandles ();

		}

        if (!moving && transform.parent.CompareTag("Library"))
        {
            transform.Rotate(0, 10f * Time.deltaTime, 0);
        }

		if (selected && !moving && !cameraMoving) {
			DrawConnectingLines ();
		}

		if (moving) {
			onRaster = false;

			if (inTrashArea) {
				trashIcon.transform.parent.transform.LookAt (player);
			}

			Vector3 prevPosition = transform.position;

			// destroy previous distance vis
			foreach (Transform visualObject in DistanceVisualisation) {
				if (visualObject.gameObject != GroundVisualOnStartMoving) {
					Destroy (visualObject.gameObject);
				}
			}

			// if the user takes an object from the library, delete other objects in the library
			// we could also just create a new version of the taken object

			if (transform.parent.CompareTag ("Library")) {
				outOfLibrary = true;
				transform.SetParent (ObjectsManager.Instance.transform);

				library.Instance.ClearLibrary (typeOfObject);                

				LeanTween.scale (this.gameObject, Vector3.one, 0.3f).setEase (LeanTweenType.easeInOutExpo);
				LeanTween.rotateLocal (this.gameObject, Vector3.zero, 0.3f).setEase (LeanTweenType.easeInOutExpo);

				Logger.Instance.AddLine (Logger.typeOfLog.grabObjectFromLibrary);
			}

			Vector3 newPositionCollider = controllerForMovement.pointOfCollisionGO.transform.position;
			Vector3 SmoothedNewPositionCollider; 

			float distanceNewMovement = (newPositionCollider - lastPositionController).sqrMagnitude;

			float smoothing = Mathf.Min((1f / distanceNewMovement * 0.03f), 0.12f);

			if (distanceNewMovement > transform.lossyScale.x * 2f) {
				smoothing = 0;
			}

			// smooth for fine movement
			SmoothedNewPositionCollider = Vector3.SmoothDamp (lastPositionController, newPositionCollider, ref velocity, smoothing);

			Vector3 newPositionWorld = newPositionCollider - initialDistanceCollisionGOModelingObject;

			lastPositionController = newPositionCollider;

			if ((newPositionWorld-transform.position).magnitude > 0.4f * transform.lossyScale.x ){
				snapped = false;

				if (colliderSphere.possibleSnapping != null) {				
					colliderSphere.possibleSnapping.colliderSphere.SnappedToThis = null;
					colliderSphere.SnappedToThis = null;
					colliderSphere.possibleSnapping = null;
				}
			}

			if (!snapped) {
				transform.position = newPositionWorld;

				KeepAboveZero (prevPosition);

				//transform.localPosition = transform.localPosition + RasterManager.Instance.Raster (transform.InverseTransformVector(newPositionWorld - prevPosition));
				//KeepAboveZero (prevPosition);

				transform.localPosition = RasterManager.Instance.Raster (transform.localPosition);

				// here check for possible snappings
				if (colliderSphere.possibleSnapping != null) {
					if (!snapped) {
						snapped = true;
						Vector3 distanceCurrentBottomSnap = colliderSphere.possibleSnapping.GetBoundingBoxTopCenter () - GetBoundingBoxBottomCenter ();
						this.transform.position = this.transform.position + distanceCurrentBottomSnap;
					} 
				} else {
					snapped = false;

					if (colliderSphere.possibleSnapping != null) {				
						colliderSphere.possibleSnapping.colliderSphere.SnappedToThis = null;
						colliderSphere.SnappedToThis = null;
						colliderSphere.possibleSnapping = null;
					}
				}
			}


			if (group != null) {
				Vector3 distance = transform.position - prevPosition;
				//group.DrawBoundingBox ();

				group.Move (distance, this);
				//group.DrawBoundingBox ();
			}

			lastPositionX = PositionOnMovementStart;
			lastPositionY = PositionOnMovementStart;
			lastPositionZ = PositionOnMovementStart;

			if ((transform.position - prevPosition).magnitude != null){
				//CalculateBoundingBox ();
			}


			if (!outOfLibrary && !inTrashArea) {
				VisualizeMovement ();
			}
		}		

    }


	public void VisualizeMovement(){

		if (RasterManager.Instance.rasterLevel >= 0.05f && !ProModeMananager.Instance.beginnersMode) {
			// maybe check local positon
			int countX = RasterManager.Instance.getNumberOfGridUnits(transform.InverseTransformPoint(GetBoundingBoxBottomCenterWorld()).x, transform.InverseTransformPoint(PositionOnMovementStart).x);

			for (int i = 0; i <= Mathf.Abs(countX); i++)
			{
				if (i == 0)
				{
					// Display center of object before moving
					GameObject CenterVisual = Instantiate(CenterVisualPrefab);
					CenterVisual.transform.SetParent(DistanceVisualisation);
					CenterVisual.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

					// change size depending on scale of stage
					CenterVisual.transform.localScale = new Vector3(1f, 1f, 1f);
					CenterVisual.transform.position = PositionOnMovementStart;

					// Display center of object after moving
					GameObject CenterVisual2 = Instantiate(CenterVisualPrefab);
					CenterVisual2.transform.SetParent(DistanceVisualisation);
					CenterVisual2.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
					CenterVisual2.transform.localScale = new Vector3(1f, 1f, 1f);

					// not very efficient
					CenterVisual2.transform.position = 0.25f * boundingBoxWorld.coordinates[4] + 0.25f * boundingBoxWorld.coordinates[5] + 0.25f * boundingBoxWorld.coordinates[6] + 0.25f * boundingBoxWorld.coordinates[7];

					// prev position
					GameObject lines = Instantiate(linesPrefab);
					lines.transform.SetParent(DistanceVisualisation);
					lines.GetComponent<Lines>().DrawLinesWorldCoordinate(new Vector3[] { boundingBoxWorld.coordinates[4], boundingBoxWorld.coordinates[5], boundingBoxWorld.coordinates[6], boundingBoxWorld.coordinates[7]}, 0);

					// new position
					GameObject lines2 = Instantiate(linesPrefab);
					lines2.transform.SetParent(DistanceVisualisation);
					lines2.GetComponent<Lines>().DrawLinesWorldCoordinate(initialCoordinatesBoundingBox, 0);

				}

				GameObject DistanceVisualX = Instantiate(DistanceVisualPrefab);
				DistanceVisualX.transform.SetParent(DistanceVisualisation);
				DistanceVisualX.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
				DistanceVisualX.transform.localScale = new Vector3(1f, 1f, 1f);
				DistanceVisualX.transform.position = PositionOnMovementStart;

				if (bottomFace.center.coordinates.x > transform.InverseTransformPoint(PositionOnMovementStart).x)
				{
					DistanceVisualX.transform.localPosition += new Vector3(i * RasterManager.Instance.rasterLevel, 0f, 0f);
				}
				else
				{
					DistanceVisualX.transform.localPosition += new Vector3(i * RasterManager.Instance.rasterLevel * (-1.0f), 0f, 0f);
				}

				lastPositionX = DistanceVisualX.transform.position;
				lastPositionZ = DistanceVisualX.transform.position;
			}

			// show amount of movement on z
			if (transform.InverseTransformPoint(GetBoundingBoxBottomCenterWorld()).z != transform.InverseTransformPoint(PositionOnMovementStart).z)
			{
				// use raster manager
				int countZ = RasterManager.Instance.getNumberOfGridUnits(transform.InverseTransformPoint(GetBoundingBoxBottomCenterWorld()).z, transform.InverseTransformPoint(PositionOnMovementStart).z);

				for (int i = 0; i <= Mathf.Abs(countZ); i++)
				{
					GameObject DistanceVisualZ = Instantiate(DistanceVisualPrefab);
					DistanceVisualZ.transform.SetParent(DistanceVisualisation);
					DistanceVisualZ.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
					DistanceVisualZ.transform.localScale = new Vector3(1f, 1f, 1f);
					DistanceVisualZ.transform.GetChild(0).localEulerAngles = new Vector3(0f, 90f, 0f);
					DistanceVisualZ.transform.position = lastPositionX;

					if (bottomFace.center.coordinates.z > transform.InverseTransformPoint(PositionOnMovementStart).z)
					{
						DistanceVisualZ.transform.localPosition += new Vector3(0f, 0f, i * RasterManager.Instance.rasterLevel);
					}
					else
					{
						DistanceVisualZ.transform.localPosition += new Vector3(0f, 0f, i * RasterManager.Instance.rasterLevel * (-1.0f));
					}

					lastPositionZ = DistanceVisualZ.transform.position;
				}

			}

			// show amount of movement on y
			if (transform.InverseTransformPoint(GetBoundingBoxBottomCenterWorld()).y != transform.InverseTransformPoint(PositionOnMovementStart).y)
			{
				// use raster manager
				int countY = RasterManager.Instance.getNumberOfGridUnits(transform.InverseTransformPoint(GetBoundingBoxBottomCenterWorld()).y, transform.InverseTransformPoint(PositionOnMovementStart).y);

				for (int i = 0; i <= Mathf.Abs(countY); i++)
				{
					GameObject DistanceVisualY = Instantiate(DistanceVisualPrefab);
					DistanceVisualY.transform.SetParent(DistanceVisualisation);
					DistanceVisualY.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
					DistanceVisualY.transform.localScale = new Vector3(1f, 1f, 1f);
					DistanceVisualY.transform.position = lastPositionZ;

					if (bottomFace.center.coordinates.y > transform.InverseTransformPoint(PositionOnMovementStart).y)
					{
						DistanceVisualY.transform.localPosition += new Vector3(0f, i * RasterManager.Instance.rasterLevel, 0f);
					}
					else
					{
						DistanceVisualY.transform.localPosition += new Vector3(0f, i * RasterManager.Instance.rasterLevel * (-1.0f), 0f);
					}

					lastPositionY = DistanceVisualY.transform.position;
				}

			}
		}


	}

	public void KeepAboveZero(Vector3 prevPosition){
		
		if (group != null && group.lowestModObject != null &&  group.lowestModObject != this) {
			// get lowest point of group
			Vector3 lowestPoint = group.lowestModObject.GetBoundingBoxBottomCenterWorld () + (transform.position - prevPosition);

			if ((transform.localPosition.y + transform.InverseTransformPoint(lowestPoint).y) < 0f) {	
				float belowZero = Mathf.Abs(transform.localPosition.y + transform.InverseTransformPoint(lowestPoint).y);
				transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y + belowZero, transform.localPosition.z);
			}

		} else {
			Vector3 lowestPoint = GetBoundingBoxBottomCenterWorld ();

			if ((transform.localPosition.y + transform.InverseTransformPoint(lowestPoint).y) < 0f) {	
				float belowZero = Mathf.Abs(transform.localPosition.y + transform.InverseTransformPoint(lowestPoint).y);
				transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y + belowZero, transform.localPosition.z);
			}
		}
	}

    public void MoveModelingObject(Vector3 distance)
    {
        transform.position += distance;
    }


    public void Initiate(Mesh initialShape)
    {
        this.transform.GetChild(0).GetComponent<MeshFilter>().mesh = initialShape;

        mesh = this.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
        meshCollider = this.transform.GetChild(0).GetComponent<MeshCollider>();
        meshCollider.sharedMesh = initialShape;

        MeshCordinatesVertices = mesh.vertices;
        MeshTriangles = mesh.triangles;
        mesh.RecalculateNormals();

        MeshUV = mesh.uv;

        vertices = new Vertex[MeshCordinatesVertices.Length];

        for (int i = 0; i < MeshCordinatesVertices.Length; i++)
        {
            GameObject vert = Instantiate(VertexPrefab);

            vert.transform.localPosition = MeshCordinatesVertices[i];
            vertices[i] = vert.GetComponent<Vertex>();

            // the normals in the mesh have the same Ids as the vertices, so we can safe a normal for every vertex
            vertices[i].normal = mesh.normals[i];
        }

        GetFacesBasedOnNormals();

        for (int i = 0; i < MeshCordinatesVertices.Length; i++)
        {
            if (MeshCordinatesVertices[i].y > 0.0f)
            {
                vertices[i].transform.SetParent(topFace.gameObject.transform);
            }
            else {
                vertices[i].transform.SetParent(bottomFace.gameObject.transform);
            }

            vertices[i].Initialize();
        }

        mesh.vertices = MeshCordinatesVertices;

        BundleSimilarVertices(topFace.gameObject.transform);
        BundleSimilarVertices(bottomFace.gameObject.transform);

        AssignVertexBundlesToFaces();

        for (int j = 0; j < faces.Length; j++)
        {
            faces[j].CalculateCenter();
        }

        InitializeVertexBundles();
        InitializeVertices();

	//	ShowOutline (false);
    }

    public void RecalculateNormals()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i].RecalculateNormal();
        }
    }

    public void RecalculateCenters()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i].UpdateCenter();
        }
    }

    public void RecalculateSideCenters()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            if (faces[i].typeOfFace == Face.faceType.SideFace)
            {
                faces[i].UpdateCenter();
            }
        }
    }


    public void InitializeVertices()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].Initialize();
        }
    }

    public void InitializeVertexBundles()
    {
        for (int i = 0; i < topFace.vertexBundles.Length; i++)
        {
            topFace.vertexBundles[i].Initialize();
        }

        for (int i = 0; i < bottomFace.vertexBundles.Length; i++)
        {
            bottomFace.vertexBundles[i].Initialize();
        }
    }


    public void ShowNormals()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            Debug.DrawLine(faces[i].center.transform.position, faces[i].center.transform.position + faces[i].normal * 3.0f, Color.red, 500f);
        }
    }

    public void BundleSimilarVertices(Transform face)
    {

        Transform[] allChildren = face.GetComponentsInChildren<Transform>();

        for (int i = 0; i < allChildren.Length; i++)
        {
            Vector3 position = allChildren[i].localPosition;

            // get all Vertex bundles
			foreach (Transform vertexBundle in face) {
				if (vertexBundle.CompareTag ("VertexBundle")) {

					// compare position of Vertex with position of vertex bundle, if similar, set vertex bundle as parent
					if (position == vertexBundle.GetComponent<VertexBundle> ().coordinates) {
						allChildren [i].SetParent (vertexBundle);
					}

				}
			}

            // if no similar found, create new vertex bundle
            if (!allChildren[i].parent.gameObject.CompareTag("VertexBundle"))
            {
                GameObject vertexBundle = Instantiate(VertexBundlePrefab);
                vertexBundle.transform.SetParent(face);
                vertexBundle.transform.localPosition = new Vector3(0, 0, 0);

                vertexBundle.GetComponent<VertexBundle>().coordinates = allChildren[i].transform.localPosition;

                allChildren[i].SetParent(vertexBundle.transform);

                if (vertexBundle.transform.childCount == 0)
                {
                    Destroy(vertexBundle);
                }
                else {
                    face.GetComponent<Face>().AddVertexBundle(vertexBundle.GetComponent<VertexBundle>());
                }
            }
        }
    }

    public void UpdateMesh()
    {
        for (int i = 0; i < MeshCordinatesVertices.Length; i++)
        {
            MeshCordinatesVertices[i] = vertices[i].transform.localPosition;
        }

        mesh.Clear();
        mesh.vertices = MeshCordinatesVertices;
        mesh.uv = MeshUV;
        mesh.triangles = MeshTriangles;
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
    }

	public void UpdateVisibleHandles(){
		handles.HideRotationHandlesExcept (null);
		handles.HideScalingHandlesExcept (null);

		PositionHandles (true);
		RotateHandles ();
		handles.ShowNonUniformScalingHandles ();
		handles.ShowRotationHandles ();
		DrawConnectingLines ();
	}

	public void SwitchToGlobalBoundingBox(){
		useBoundingBoxWorld = true;
		boundingBox.local = false;
		UpdateVisibleHandles ();
	}

	public void SwitchToLocalBoundingBox(){
		useBoundingBoxWorld = false;
		boundingBox.local = true;
		UpdateVisibleHandles ();
	}

	public void PositionHandles(bool showRotationHandles)
    {
		CalculateBoundingBox ();
		//Vector3 sizeHandles =  transform.InverseTransformVector (Vector3.one);

		handles.faceTopScale.transform.position = transform.TransformPoint(topFace.scaler.coordinates);
		handles.faceBottomScale.transform.position = transform.TransformPoint(bottomFace.scaler.coordinates);
		handles.CenterTopPosition.transform.localPosition = topFace.centerPosition;
        handles.CenterBottomPosition.transform.localPosition = bottomFace.centerPosition;
        handles.HeightTop.transform.localPosition = topFace.centerPosition;
        handles.HeightBottom.transform.localPosition = bottomFace.centerPosition;

		// get Closest bounding box coordinate 
		//Vector3 closesBBcorner = GetPosOfClosestVertex(Camera.main.transform.position, boundingBox.coordinates);

		if (showRotationHandles) {

			int idOfClosestVertToCamera = GetIdOfClosestVertex(Camera.main.transform.position, boundingBox.coordinates);

			/*
			int nextId = idOfClosestVertToCamera + 1;

			if (nextId > 3) {
				nextId = 0;
			}

			int preId = idOfClosestVertToCamera - 1;

			if (preId < 0) {
				preId = 3;
			}

			int idOfCornerOnGround = idOfClosestVertToCamera + 4;
		
			handles.RotateY.transform.position = boundingBox.coordinates [idOfCornerOnGround];

			Vector3 direction = boundingBox.coordinates [preId] - boundingBox.coordinates [idOfClosestVertToCamera];

			if (Mathf.Abs(Vector3.Dot (direction.normalized, Vector3.forward)) == 1f) {
				handles.RotateZ.transform.position = boundingBox.coordinates [preId];
				handles.RotateX.transform.position = boundingBox.coordinates [nextId];
			} else {
				handles.RotateX.transform.position = boundingBox.coordinates [preId];
				handles.RotateZ.transform.position = boundingBox.coordinates [nextId];
			}

			*/

			Vector3 bbCenter = GetBoundingBoxCenter();

			int[] xyz = GetThreeAdjacentCorners (idOfClosestVertToCamera,boundingBox.local);


			//Debug.Log ("Closest vert: " + idOfClosestVertToCamera);
			//Debug.Log ("X: " + xyz[0]);
			//Debug.Log ("Y: " + xyz[1]);
			//Debug.Log ("Z: " + xyz[2]);
			
			handles.RotateX.transform.position = boundingBox.coordinates [xyz[0]];
			handles.RotateY.transform.position = boundingBox.coordinates [xyz[1]];
			handles.RotateZ.transform.position = boundingBox.coordinates [xyz[2]];


			// RotateHandles 
			if (!boundingBox.local)
			{
				handles.RotateX.transform.rotation = Quaternion.LookRotation (new Vector3(1f,0f,0f), handles.RotateX.transform.position - bbCenter);
				handles.RotateY.transform.rotation = Quaternion.LookRotation (new Vector3(0f,1f,0f), handles.RotateY.transform.position - bbCenter);
				handles.RotateZ.transform.rotation = Quaternion.LookRotation (new Vector3(0f,0f,1f), handles.RotateZ.transform.position - bbCenter);
			} else {
				handles.RotateX.transform.rotation = Quaternion.LookRotation (coordinateSystem.transform.right, handles.RotateX.transform.position - bbCenter);
				handles.RotateY.transform.rotation = Quaternion.LookRotation (coordinateSystem.transform.up, handles.RotateY.transform.position - bbCenter);
				handles.RotateZ.transform.rotation = Quaternion.LookRotation (coordinateSystem.transform.forward, handles.RotateZ.transform.position - bbCenter);
			}

			if (!ProModeMananager.Instance.beginnersMode) {
				handles.RotateX.SetActive (true);
				handles.RotateZ.SetActive (true);
			}
			handles.RotateY.SetActive (true);
		}
	
		handles.NonUniformScaleTop.transform.position = GetBoundingBoxTopCenter () + 0.00f * (GetBoundingBoxTopCenter () - GetBoundingBoxBottomCenter ()).normalized;
		handles.NonUniformScaleBottom.transform.position = GetBoundingBoxBottomCenter () + 0.00f * (GetBoundingBoxBottomCenter () - GetBoundingBoxTopCenter ()).normalized;

		handles.NonUniformScaleFront.transform.position = GetBoundingBoxFrontCenter () + 0.00f * (GetBoundingBoxFrontCenter () - GetBoundingBoxBackCenter ()).normalized;
		handles.NonUniformScaleBack.transform.position = GetBoundingBoxBackCenter () + 0.00f * (GetBoundingBoxBackCenter () - GetBoundingBoxFrontCenter ()).normalized;

		handles.NonUniformScaleLeft.transform.position = GetBoundingBoxLeftCenter () + 0.00f * (GetBoundingBoxLeftCenter () - GetBoundingBoxRightCenter ()).normalized;
		handles.NonUniformScaleRight.transform.position = GetBoundingBoxRightCenter () + 0.00f * (GetBoundingBoxRightCenter () - GetBoundingBoxLeftCenter ()).normalized;

		//we always also need wolrd coordinates

		// verschiebung muss relativ sein
		handles.YMovement.transform.position = GetBoundingBoxTopCenterWorld () + 0.03f * (GetBoundingBoxTopCenterWorld () - GetBoundingBoxBottomCenterWorld ()).normalized;
		handles.UniformScale.transform.position = GetBoundingBoxTopCenterWorld () + 0.03f * (GetBoundingBoxTopCenterWorld () - GetBoundingBoxBottomCenterWorld ()).normalized;

		Vector3 bbFront = GetBoundingBoxFrontCenter ();

		handles.RotateAndTranslate.transform.position = new Vector3 (bbFront.x, GetBoundingBoxBottomCenter().y, bbFront.z) + 0.3f * (bbFront - GetBoundingBoxCenter ()).normalized;

		DrawConnectingLines ();
    }

	public void DrawConnectingLines(){

		Vector3 bbCenter = GetBoundingBoxCenter ();

		/*

		if (!ProModeMananager.Instance.beginnersMode) {		
			//connectingLinesHandles.ClearLines ();
				
			// draw new lines
			connectingLinesHandles.EnhancedDrawLinesWorldCoordinate (new Vector3[] {
				handles.NonUniformScaleFront.transform.position,
				handles.NonUniformScaleBack.transform.position
			}, 0, handles.NonUniformScaleFront.GetComponent<handle> ().normalColor);

			connectingLinesHandles.EnhancedDrawLinesWorldCoordinate (new Vector3[] {
				handles.NonUniformScaleLeft.transform.position,
				handles.NonUniformScaleRight.transform.position
			}, 2, handles.NonUniformScaleLeft.GetComponent<handle> ().normalColor);

			connectingLinesHandles.EnhancedDrawLinesWorldCoordinate (new Vector3[] {
				handles.NonUniformScaleTop.transform.position,
				handles.NonUniformScaleBottom.transform.position
			}, 4, handles.YMovement.GetComponent<handle> ().normalColor);

			connectingLinesHandles.EnhancedDrawLinesWorldCoordinate (new Vector3[] {
				handles.YMovement.transform.GetChild (0).position,
				bbCenter
			}, 6, Color.white);
		} else {
				connectingLinesHandles.EnhancedDrawLinesWorldCoordinate (new Vector3[] {
				handles.YMovement.transform.GetChild (0).position,
				bbCenter
			}, 0, Color.white);
		}

		*/

	}

    public void RotateHandles()
    {
		Vector3 currentBBCenter = GetBoundingBoxCenter ();

		// frustum
		handles.faceTopScale.transform.rotation = Quaternion.LookRotation (handles.faceTopScale.transform.position - transform.TransformPoint (topFace.center.coordinates));
		handles.HeightTop.transform.rotation = Quaternion.LookRotation (transform.TransformDirection(topFace.normal));

		handles.faceBottomScale.transform.rotation = Quaternion.LookRotation (handles.faceBottomScale.transform.position -  transform.TransformPoint (bottomFace.center.coordinates));
		handles.HeightBottom.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(bottomFace.normal));


		// rotate non-uniform scaling handles

		// global

		if (boundingBox.local == false) {
			handles.NonUniformScaleTop.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(Vector3.up));
			handles.NonUniformScaleBottom.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(Vector3.down));

			handles.NonUniformScaleFront.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(Vector3.back));
			handles.NonUniformScaleBack.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(Vector3.forward));

			handles.NonUniformScaleLeft.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(Vector3.left));
			handles.NonUniformScaleRight.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(Vector3.right));
		}

		// local
		if (boundingBox.local == true) {
			handles.NonUniformScaleTop.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(coordinateSystem.transform.up));
			handles.NonUniformScaleBottom.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(coordinateSystem.transform.up * (-1f)));

			handles.NonUniformScaleFront.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(coordinateSystem.transform.forward  * (-1f)));
			handles.NonUniformScaleBack.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(coordinateSystem.transform.forward));

			handles.NonUniformScaleLeft.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(coordinateSystem.transform.right  * (-1f)));
			handles.NonUniformScaleRight.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(coordinateSystem.transform.right));
		}


		handles.YMovement.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(Vector3.up));
		handles.UniformScale.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(Vector3.up));

		/*
		Destroy (xAxis);
		Destroy (yAxis);
		Destroy (zAxis);

		xAxis = Instantiate (handles.linesGO);
		yAxis = Instantiate (handles.linesGO);
		zAxis = Instantiate (handles.linesGO);

		xAxis.GetComponent<Lines> ().DrawLinesWorldCoordinate (new Vector3[] {
			handles.NonUniformScaleLeft.transform.position,
			handles.NonUniformScaleRight.transform.position 
		}, 0);

		yAxis.GetComponent<Lines> ().DrawLinesWorldCoordinate (new Vector3[] {
			handles.YMovement.transform.position,
			handles.NonUniformScaleBottom.transform.position 
		}, 0);

		zAxis.GetComponent<Lines> ().DrawLinesWorldCoordinate (new Vector3[] {
			handles.NonUniformScaleFront.transform.position,
			handles.NonUniformScaleBottom.transform.position
		}, 0);
		*/
	}

    public void InitiateHandles()
    {
        PositionHandles(false);
        RotateHandles();

        handles.faceTopScale.GetComponent<handle>().face = topFace;
        handles.faceBottomScale.GetComponent<handle>().face = bottomFace;

        handles.CenterTopPosition.GetComponent<handle>().face = topFace;
        handles.CenterBottomPosition.GetComponent<handle>().face = bottomFace;

        handles.HeightTop.GetComponent<handle>().face = topFace;
        handles.HeightBottom.GetComponent<handle>().face = bottomFace;

        topFace.centerHandle = handles.CenterTopPosition.GetComponent<handle>();
        topFace.heightHandle = handles.HeightTop.GetComponent<handle>();
        topFace.scaleHandle = handles.faceTopScale.GetComponent<handle>();

        bottomFace.centerHandle = handles.CenterBottomPosition.GetComponent<handle>();
        bottomFace.heightHandle = handles.HeightBottom.GetComponent<handle>();
        bottomFace.scaleHandle = handles.faceBottomScale.GetComponent<handle>();

		if (!moving && !selected) {
			//handles.DisableHandles();
		}
        

    }

    public void Focus(Selection controller)
    {
		if (!focused || (group != null && !group.focused))
        {
			if (group != null) {
				group.FocusGroup (this, controller);
			} else {
				Highlight();
				ShowBoundingBox (false);
			}

			if (controller.currentSettingSelectionMode == Selection.settingSelectionMode.SettingsButton && !transform.parent.CompareTag("Library") && !controller.groupItemSelection && !moving){
			 	objectSelector.ShowSelectionButton (controller);
			}

            controller.AssignCurrentFocus(transform.gameObject);
            focused = true;

        }
    }

	public void UnFocus(Selection controller)
    {
		if ((focused && !selected && !moving)  || (group != null && !group.focused))
        {
			if (group == null) {
				// ObjectSelector.SetActive(false);
				UnHighlight();
				HideBoundingBox (false);

				if (controller.currentSettingSelectionMode == Selection.settingSelectionMode.SettingsButton) {
					objectSelector.HideSelectionButton ();
				}

				focused = false;

			} else {
				if (group.focused)
				{
					if (!controller.groupItemSelection && !group.selected) {
						UnHighlight();
						HideBoundingBox (false);
						focused = false;
						group.UnFocusGroup (this, controller);
					}
				} 
			}           
        }
    }

    public void Highlight()
    {
      //  Color newColor = transform.GetChild(0).GetComponent<Renderer>().material.color;
		transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(currentColor.r * 1.3f, currentColor.g * 1.3f, currentColor.b * 1.3f, 1f);
    }

    public void UnHighlight()
    {
     //   Color newColor = transform.GetChild(0).GetComponent<Renderer>().material.color;
		transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
    }

    public void Select(Selection controller, Vector3 uiPosition)
    {		
		if (!selected) {
			//WorldLocalToggle.Instance.Show ();

			if (group != null)
			{
				group.SelectGroup(this);
			} 

			// move selection button
			if (controller.currentSettingSelectionMode == Selection.settingSelectionMode.SettingsButton) {
				objectSelector.active = false;
				objectSelector.MoveAndFace (uiPosition);
			}

			controller.AssignCurrentSelection(transform.gameObject);
			handles.gameObject.transform.GetChild(0).gameObject.SetActive(true);

			UiCanvasGroup.Instance.transform.position = uiPosition;
			UiCanvasGroup.Instance.OpenMainMenu(this, controller);

			if (WorldLocalToggle.Instance.local == useBoundingBoxWorld) {
				ToggleLocalGlobalBB ();
			}

			//ShowOutline(true);
			ShowBoundingBox (true);

		//	Camera.main.gameObject.GetComponent<maxCamera> ().target = transform;
		}     

		selected = true;
    }
		
    public void DeSelect(Selection controller)
    {	
		handles.DisableHandles();

		if (selected) {			
			if (group != null)
			{
				group.DeSelectGroup(this, controller);
			}
			
			selected = false;
			//ShowOutline(false);

			objectSelector.DeSelect (controller);

			controller.DeAssignCurrentSelection(transform.gameObject);

		//	Camera.main.gameObject.GetComponent<maxCamera> ().target = GameObject.Find("StageScaler").transform;
		}

		selected = false;
		UnFocus (controller);
    }
	  

    public void CalculateBoundingBox()
    {
		boundingBox.coordinates = new Vector3[8];

        // get highest and lowest values for x,y,z
		Vector3 minima = GetBoundingBoxMinima(boundingBox.local);
		Vector3 maxima = GetBoundingBoxMaxima(boundingBox.local);

        // set all points
		if (!boundingBox.local) {
			boundingBox.coordinates [0] = transform.TransformPoint (new Vector3 (maxima.x, maxima.y, maxima.z));
			boundingBox.coordinates [1] = transform.TransformPoint (new Vector3 (maxima.x, maxima.y, minima.z));
			boundingBox.coordinates [2] = transform.TransformPoint (new Vector3 (minima.x, maxima.y, minima.z));
			boundingBox.coordinates [3] = transform.TransformPoint (new Vector3 (minima.x, maxima.y, maxima.z));

			boundingBox.coordinates [4] = transform.TransformPoint (new Vector3 (maxima.x, minima.y, maxima.z));
			boundingBox.coordinates [5] = transform.TransformPoint (new Vector3 (maxima.x, minima.y, minima.z));
			boundingBox.coordinates [6] = transform.TransformPoint (new Vector3 (minima.x, minima.y, minima.z));
			boundingBox.coordinates [7] = transform.TransformPoint (new Vector3 (minima.x, minima.y, maxima.z));
		} else {
			boundingBox.coordinates [0] = coordinateSystem.transform.TransformPoint (new Vector3 (maxima.x, maxima.y, maxima.z));
			boundingBox.coordinates [1] = coordinateSystem.transform.TransformPoint (new Vector3 (maxima.x, maxima.y, minima.z));
			boundingBox.coordinates [2] = coordinateSystem.transform.TransformPoint (new Vector3 (minima.x, maxima.y, minima.z));
			boundingBox.coordinates [3] = coordinateSystem.transform.TransformPoint (new Vector3 (minima.x, maxima.y, maxima.z));

			boundingBox.coordinates [4] = coordinateSystem.transform.TransformPoint (new Vector3 (maxima.x, minima.y, maxima.z));
			boundingBox.coordinates [5] = coordinateSystem.transform.TransformPoint (new Vector3 (maxima.x, minima.y, minima.z));
			boundingBox.coordinates [6] = coordinateSystem.transform.TransformPoint (new Vector3 (minima.x, minima.y, minima.z));
			boundingBox.coordinates [7] = coordinateSystem.transform.TransformPoint (new Vector3 (minima.x, minima.y, maxima.z));
		}
    }

	public void CalculateBoundingBoxWorld()
	{
		boundingBoxWorld.coordinates = new Vector3[8];

		// get highest and lowest values for x,y,z
		Vector3 minima = GetBoundingBoxMinima(boundingBoxWorld.local);
		Vector3 maxima = GetBoundingBoxMaxima(boundingBoxWorld.local);

		boundingBoxWorld.coordinates [0] = transform.TransformPoint (new Vector3 (maxima.x, maxima.y, maxima.z));
		boundingBoxWorld.coordinates [1] = transform.TransformPoint (new Vector3 (maxima.x, maxima.y, minima.z));
		boundingBoxWorld.coordinates [2] = transform.TransformPoint (new Vector3 (minima.x, maxima.y, minima.z));
		boundingBoxWorld.coordinates [3] = transform.TransformPoint (new Vector3 (minima.x, maxima.y, maxima.z));

		boundingBoxWorld.coordinates [4] = transform.TransformPoint (new Vector3 (maxima.x, minima.y, maxima.z));
		boundingBoxWorld.coordinates [5] = transform.TransformPoint (new Vector3 (maxima.x, minima.y, minima.z));
		boundingBoxWorld.coordinates [6] = transform.TransformPoint (new Vector3 (minima.x, minima.y, minima.z));
		boundingBoxWorld.coordinates [7] = transform.TransformPoint (new Vector3 (minima.x, minima.y, maxima.z));
	}



	public void ShowBoundingBox(bool checkForGroup)
    {
		CalculateBoundingBox ();
		boundingBox.DrawBoundingBox ();

		if (group != null && checkForGroup) {
			group.DrawBoundingBox ();
		}

    }

	public void HideBoundingBox(bool checkForGroup)
	{
		boundingBox.ClearBoundingBox ();

		if (group != null && checkForGroup) {
			group.boundingBox.ClearBoundingBox ();
		}
	}

    public void AssignVertexBundlesToFaces()
    {

        // go through all vertices
        for (int i = 0; i < vertices.Length; i++)
        {

            // check normal of vertice and compare with normal of face
            for (int j = 0; j < faces.Length; j++)
            {

                // if normals are similar, linke parent vertex bundle to face
                if (vertices[i].normal == faces[j].normal)
                {
                    if (faces[j].typeOfFace == Face.faceType.SideFace)
                    {
                        faces[j].AddVertexBundle(vertices[i].transform.GetComponentInParent<VertexBundle>());
                    }
                }
            }
        }
    }

	public void UseAsPossibleSnap(){
		meshCollider.enabled = false;
		colliderSphere.transform.GetComponent<SphereCollider> ().enabled = true;
		colliderSphere.transform.position = GetBoundingBoxTopCenter ();
		colliderSphere.transform.localScale = Vector3.one * RasterManager.Instance.rasterLevel * 3f;
	}

	public void StopUseAsPossibleSnap(){
		meshCollider.enabled = true;
		colliderSphere.transform.GetComponent<SphereCollider> ().enabled = false;
	}
			

    public void StartMoving(Selection controller, ModelingObject initiater)
    {
		if (!moving) {
			UiCanvasGroup.Instance.TemporarilyHide ();
			ObjectsManager.Instance.StartMovingObject (this);

			timeOnMovementStart = Time.time;

			CalculateBoundingBox ();
			moving = true;
			//initialBlocking = true;
			controllerForMovement = controller;
			lastPositionController = controllerForMovement.pointOfCollisionGO.transform.position;
			initialPositionController = controller.pointOfCollisionGO.transform.position;
			initialDistanceCollisionGOModelingObject = initialPositionController - transform.position;
			PositionOnMovementStart = 0.25f * boundingBoxWorld.coordinates[4] + 0.25f * boundingBoxWorld.coordinates[5] + 0.25f * boundingBoxWorld.coordinates[6] + 0.25f * boundingBoxWorld.coordinates[7];

			bottomFace.center.possibleSnappingVertexBundle = null;
			objectSelector.HideSelectionButton ();
			//DisplayOutlineOfGroundFace ();

			initialCoordinatesBoundingBox = new Vector3[4];

			for (int j = 0; j < 4; j++)
			{
				initialCoordinatesBoundingBox[j] = boundingBoxWorld.coordinates[j+4];
			}

			// disable collider of object when moving
			//meshCollider.enabled = false;

			colliderSphere.transform.GetComponent<SphereCollider> ().enabled = true;
			colliderSphere.parentMoving = true;
			colliderSphere.transform.position = GetBoundingBoxBottomCenter ();
			colliderSphere.transform.localScale = Vector3.one * RasterManager.Instance.rasterLevel * 3f;

			if (colliderSphere.SnappedToThis != null) {				
				colliderSphere.SnappedToThis.colliderSphere.possibleSnapping = null;
				colliderSphere.SnappedToThis.snapped = false;
				colliderSphere.SnappedToThis = null;
			}

			//handles.DisableHandles ();

			if (group != null) {
				group.SetLowestObjectOfGroup ();
				//group.HideBoundingBox ();
			}

			//handles.ShowNonUniformScalingHandles ();
			handles.DisableHandles();
			ShowBoundingBox (false);
		}
    }

	public void StopMoving(Selection controller, ModelingObject initiater)
	{
		ObjectsManager.Instance.StopMovingObject (this);

		if (group != null && initiater == this)
		{
			group.StopMoving(controller, initiater);
		}

		firstTimeMoving = false;
		moving = false;
		controllerForMovement = null;

		// destroz previous distance vis
		foreach (Transform visualObject in DistanceVisualisation)
		{
			Destroy(visualObject.gameObject);
		}

		CalculateBoundingBox ();

		colliderSphere.parentMoving = false;
		colliderSphere.transform.GetComponent<SphereCollider> ().enabled = false;
		meshCollider.enabled = true;
		outOfLibrary = false;

		if (controller.currentSettingSelectionMode == Selection.settingSelectionMode.SettingsButton) {
			if (!selected) {
				handles.DisableHandles();
			}
		}
	}

	public void DisplayOutlineOfGroundFace(){
		// Display outline of groundface

        GameObject lines = Instantiate(linesPrefab);
        lines.transform.SetParent(DistanceVisualisation);
		lines.GetComponent<Lines>().DrawLinesWorldCoordinate(initialCoordinatesBoundingBox, 0);

        /*
        GroundVisualOnStartMoving = Instantiate(GroundVisualPrefab);
		GroundVisualOnStartMoving.transform.SetParent(DistanceVisualisation);
		LineRenderer lines = GroundVisualOnStartMoving.GetComponent<LineRenderer>();
		lines.SetVertexCount(bottomFace.vertexBundles.Length + 1);
		lines.SetWidth (Mathf.Min(0.025f * transform.lossyScale.x, 0.03f), Mathf.Min(0.025f * transform.lossyScale.x, 0.03f));

		for (int j = 0; j <= bottomFace.vertexBundles.Length; j++) {
			if (j == bottomFace.vertexBundles.Length) {
				Vector3 pos = bottomFace.vertexBundles [0].transform.GetChild (0).position;
				lines.SetPosition (j, pos);
			} else {
				Vector3 pos = bottomFace.vertexBundles [j].transform.GetChild (0).position;
				lines.SetPosition (j, pos);
			}
		} */
    }

  



    public void GetFacesBasedOnNormals()
    {

        //currently we have top and bottom face 2 times
        int arrayLength = 0;

        switch (typeOfObject)
        {
            case ObjectType.triangle:
                arrayLength = 5;
                break;
            case ObjectType.square:
                arrayLength = 6;
                break;
            case ObjectType.hexagon:
                arrayLength = 8;
                break;
            case ObjectType.octagon:
                arrayLength = 10;
                break;
        }

        faces = new Face[arrayLength];

        int faceCount = 0;
        Face faceFound;

        // go trough all vertices
        for (int i = 0; i < vertices.Length; i++)
        {
            faceFound = null;

            // check for each vertex every face and 
            for (int j = 0; j < faces.Length; j++)
            {

                if (faces[j] != null && vertices[i].normal == faces[j].normal)
                {
                    faceFound = faces[j];
                }

            }

            if (faceFound == null)
            {
                GameObject newFace = Instantiate(FacePrefab);
                newFace.transform.SetParent(transform.GetChild(0));
                faces[faceCount] = newFace.GetComponent<Face>();

                // Check if it is top face? Or not create new face if it is the top/bottom face
                if (vertices[i].normal.x == 0 && vertices[i].normal.z == 0)
                {

                    switch (typeOfObject)
                    {
                        case ObjectType.triangle:
                            faces[faceCount].InitializeFace(3);
                            break;
                        case ObjectType.square:
                            faces[faceCount].InitializeFace(4);
                            break;
                        case ObjectType.hexagon:
                            faces[faceCount].InitializeFace(6);
                            break;
                        case ObjectType.octagon:
                            faces[faceCount].InitializeFace(8);
                            break;
                    }

                    //if direction is up, it is the top face (number of vertices depends on type)
                    if (vertices[i].normal.y > 0)
                    {
                        faces[faceCount].SetType(Face.faceType.TopFace);
                        topFace = newFace.GetComponent<Face>();

                        //if direction is down, it is the bottom face (number of vertices depends on type)
                    }
                    else {
                        faces[faceCount].SetType(Face.faceType.BottomFace);
                        bottomFace = newFace.GetComponent<Face>();
                    }

                    // others are side faces (4 vertices)
                }
                else {
                    faces[faceCount].InitializeFace(4);
                    faces[faceCount].SetType(Face.faceType.SideFace);
                }

                faces[faceCount].normal = vertices[i].normal;

                faceCount++;
            }
        }
    }

    public void CorrectOffset()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i].transform.localPosition = new Vector3(0f, transform.localPosition.y * (-1f), 0f);
        }
    }

    public Face GetFaceFromCollisionCoordinate(Vector3 pointOfCollision)
    {

        // use dot product to check if point lies on a face
        int idOfSmallest = -1;

        for (int i = 0; i < faces.Length; i++)
        {

            if (Mathf.Abs(Vector3.Dot((faces[i].center.transform.GetChild(0).transform.position - pointOfCollision), transform.TransformDirection(faces[i].normal))) <= 0.01)
            {
                idOfSmallest = i;
            }
        }

        if (idOfSmallest != -1)
        {
            return faces[idOfSmallest];
        }
        else
        {
            return null;
        }

    }

    public void StartScaling(bool initiater)
    {
		if (group == null)
        {
            relativeTo = GetBoundingBoxBottomCenter();
            initialDistancceCenterBottomScaler = scalerObject.coordinates - relativeTo;
			
			boundingBoxCenterToRightOnScalingBegin = GetBoundingBoxRightCenter () - GetBoundingBoxCenter (); 

			vectorsTopOnScalingBegin = new Vector3[topFace.vertexBundles.Length];
			vectorsBottomOnScalingBegin = new Vector3[bottomFace.vertexBundles.Length];
			Vector3 boundingBoxBottomLocal = transform.InverseTransformPoint (GetBoundingBoxBottomCenter ());

			for (int i = 0; i < topFace.vertexBundles.Length; i++) {
				vectorsTopOnScalingBegin[i] = topFace.vertexBundles [i].coordinates - boundingBoxBottomLocal;
			}

			for (int i = 0; i < bottomFace.vertexBundles.Length; i++) {
				vectorsBottomOnScalingBegin[i] = bottomFace.vertexBundles [i].coordinates - boundingBoxBottomLocal;
			}

        } else
        {
            if (initiater)
            {
                group.StartScalingGroup(this);
            }

            relativeTo = group.GetBoundingBoxBottomCenter();
            initialDistancceCenterBottomScaler = scalerObject.coordinates - relativeTo;
        }
    }

	public void ScaleNonUniform(float newScale, Vector3 direction, handle.handleType typeOfHandle, Vector3 centerOfScaling){		
		
		// here we need to adjust scaling so that the outcome is in grid

		// this is the hack why it does not work the other way
		Vector3 scaling = (direction * (newScale)) + Vector3.one - direction;

		Matrix4x4 scalingMatrix = Matrix4x4.TRS (Vector3.zero, Quaternion.Euler(0f,0f,0f), scaling);

		if (boundingBox.local) {
			
			// go through all points
			for (int i = 0; i < topFace.vertexBundles.Length; i++) {
				Vector3 newPoint = scalingMatrix.MultiplyPoint3x4 (coordinateSystem.transform.InverseTransformPoint (transform.TransformPoint (topFace.vertexBundles [i].coordinates - centerOfScaling)));
				topFace.vertexBundles [i].coordinates = transform.InverseTransformPoint (coordinateSystem.transform.TransformPoint (newPoint)) + centerOfScaling;
			}

			for (int i = 0; i < bottomFace.vertexBundles.Length; i++) {
				Vector3 newPoint = scalingMatrix.MultiplyPoint3x4 (coordinateSystem.transform.InverseTransformPoint (transform.TransformPoint (bottomFace.vertexBundles [i].coordinates - centerOfScaling)));
				bottomFace.vertexBundles [i].coordinates = transform.InverseTransformPoint (coordinateSystem.transform.TransformPoint (newPoint)) + centerOfScaling;
			}

		} else {
			
			for (int i = 0; i < topFace.vertexBundles.Length; i++) {
				Vector3 newPoint = scalingMatrix.MultiplyPoint3x4 (topFace.vertexBundles [i].coordinates - centerOfScaling);
				topFace.vertexBundles [i].coordinates = newPoint + centerOfScaling;
			}

			for (int i = 0; i < bottomFace.vertexBundles.Length; i++) {
				Vector3 newPoint = scalingMatrix.MultiplyPoint3x4 (bottomFace.vertexBundles [i].coordinates - centerOfScaling);
				bottomFace.vertexBundles [i].coordinates = newPoint + centerOfScaling;
			}
		}

		for (int i = 0; i < faces.Length; i++) {
			faces [i].UpdateCenter ();
		}

		topFace.UpdateSpecialVertexCoordinates ();
		bottomFace.UpdateSpecialVertexCoordinates ();

		CalculateBoundingBox ();
		PositionHandles (true);
		RotateHandles ();
	}



    public void ScaleBy(float newScale, bool initiater)
    {     

		Debug.Log ("Scale it!!!");

		// first calculate scaling with top center
		Vector3 newBoundingBoxCenterRight = GetBoundingBoxCenter() + newScale * (boundingBoxCenterToRightOnScalingBegin);

		if ((transform.InverseTransformVector (newBoundingBoxCenterRight-GetBoundingBoxCenter ())).magnitude >= RasterManager.Instance.rasterLevel * 2) {

			// raster that value
			newBoundingBoxCenterRight = transform.TransformPoint(RasterManager.Instance.Raster (transform.InverseTransformPoint(newBoundingBoxCenterRight)));
			float rasteredScalingValue = (newBoundingBoxCenterRight - GetBoundingBoxCenter ()).magnitude / boundingBoxCenterToRightOnScalingBegin.magnitude;

			Vector3 boundingBoxBottomLocal = transform.InverseTransformPoint (GetBoundingBoxBottomCenter ());

			for (int i = 0; i < topFace.vertexBundles.Length; i++) {
				topFace.vertexBundles [i].coordinates = boundingBoxBottomLocal + rasteredScalingValue * (vectorsTopOnScalingBegin[i]);
			}

			for (int i = 0; i < bottomFace.vertexBundles.Length; i++) {
				bottomFace.vertexBundles [i].coordinates = boundingBoxBottomLocal + rasteredScalingValue * (vectorsBottomOnScalingBegin[i]);
			}

			for (int i = 0; i < faces.Length; i++) {
				faces [i].UpdateCenter ();
			}

			topFace.UpdateSpecialVertexCoordinates ();
			bottomFace.UpdateSpecialVertexCoordinates ();

			ShowBoundingBox(false);
			PositionHandles (false);
			RotateHandles ();
		}



		/*

		if (group != null && initiater)
        { 
            group.ScaleBy(newScale, this);
        }

        Vector3 positionScalerObject = RasterManager.Instance.Raster(relativeTo + ((newScale) * initialDistancceCenterBottomScaler));

        float amount = (positionScalerObject - relativeTo).magnitude / (scalerObject.coordinates - relativeTo).magnitude;

        scalerObject.coordinates = positionScalerObject;

        bottomFace.ReplaceFacefromObjectScaler(relativeTo, amount);
        topFace.ReplaceFacefromObjectScaler(relativeTo, amount);

		*/
    }

    public void UpDateObjectFromCorner()
    {
        // Get distance from scaler to  ground
        float lengthScalerToCenterBottomFace = (scalerObject.coordinates - bottomFace.centerPosition).magnitude / initialDistancceCenterBottomScaler.magnitude;

        if (Vector3.Dot((scalerObject.coordinates - bottomFace.centerPosition), (scalerObject.coordinates - bottomFace.centerPosition)) < 0f)
        {
            lengthScalerToCenterBottomFace = (-1f) * lengthScalerToCenterBottomFace;
        }


    }

    public void TrashObject(bool initiator)
    {
		if (!trashed) {
			trashed = true;

			if (group != null && initiator)
			{
				group.TrashGroup(this);
			}


			transform.gameObject.SetActive(false);

			Logger.Instance.AddLine (Logger.typeOfLog.deleteObject);

			//Trash.Instance.TrashAreaActive(false);
		}
		

    }

	public void ChangeColor(Color color, bool initiator)
    {
		if (group != null && initiator)
		{
			group.ColorGroup(this, color);
		}

        transform.GetChild(0).GetComponent<Renderer>().material.color = color;
		currentColor = color;
    }



	public void RotateAround(Vector3 angleAxis, float angle, Vector3 bbCenterBeforeRotation)
    {
        for (int i = 0; i < topFace.vertexBundles.Length; i++)
        {
            // rotate coordinates of every vertexbundle
			topFace.vertexBundles[i].coordinates = (Quaternion.AngleAxis(angle, angleAxis) * (topFace.vertexBundles[i].coordinates - bbCenterBeforeRotation)) + bbCenterBeforeRotation;
        }

		// rotate coordinates of every vertexbundle
		topFace.center.coordinates = (Quaternion.AngleAxis(angle, angleAxis) * (topFace.center.coordinates - bbCenterBeforeRotation)) + bbCenterBeforeRotation;
									
        for (int i = 0; i < bottomFace.vertexBundles.Length; i++)
        {
            // rotate coordinates of every vertexbundle
			bottomFace.vertexBundles[i].coordinates = (Quaternion.AngleAxis(angle, angleAxis) * (bottomFace.vertexBundles[i].coordinates - bbCenterBeforeRotation)) + bbCenterBeforeRotation;
        }

		bottomFace.center.coordinates = (Quaternion.AngleAxis(angle, angleAxis) * (bottomFace.center.coordinates - bbCenterBeforeRotation)) + bbCenterBeforeRotation;

        // update centers and recalculate normals of side faces
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i].UpdateCenter();
            faces[i].RecalculateNormal();
            faces[i].UpdateSpecialVertexCoordinates();
        }

		coordinateSystem.transform.RotateAround (coordinateSystem.transform.position, angleAxis, angle);

        CalculateBoundingBox();
		PositionHandles (false);
		RotateHandles();
    }

    public void SetVertexBundlePositions(Vector3[] topFaceCoordinates, Vector3[] bottomFaceCoordinates, Vector3 topFaceCenter, Vector3 bottomFaceCenter)
    {
        for (int i = 0; i < topFace.vertexBundles.Length; i++)
        {
            topFace.vertexBundles[i].coordinates = topFaceCoordinates[i];
        }

        topFace.center.coordinates = topFaceCenter;

        for (int i = 0; i < bottomFace.vertexBundles.Length; i++)
        {
            // rotate coordinates of every vertexbundle
            bottomFace.vertexBundles[i].coordinates = bottomFaceCoordinates[i];

        }

        bottomFace.center.coordinates = bottomFaceCenter;

        // update centers and recalculate normals of side faces

        for (int i = 0; i < faces.Length; i++)
        {
            faces[i].UpdateCenter();
            faces[i].RecalculateNormal();
            faces[i].UpdateSpecialVertexCoordinates();
        }

		CalculateBoundingBox ();
		PositionHandles (false);
		RotateHandles();		
    }


	public Vector3 GetBoundingBoxBottomCenterWorld()
	{ 
		CalculateBoundingBoxWorld ();
		Vector3 boundingBoxBottomCenter = 0.25f * boundingBoxWorld.coordinates [4] + 0.25f * boundingBoxWorld.coordinates [5] + 0.25f * boundingBoxWorld.coordinates [6] + 0.25f * boundingBoxWorld.coordinates [7];
		return boundingBoxBottomCenter;
	}


	public Vector3 GetBoundingBoxTopCenterWorld()
	{ 
		CalculateBoundingBoxWorld ();
		Vector3 boundingBoxBottomCenter = 0.25f * boundingBoxWorld.coordinates [0] + 0.25f * boundingBoxWorld.coordinates [1] + 0.25f * boundingBoxWorld.coordinates [2] + 0.25f * boundingBoxWorld.coordinates [3];
		return boundingBoxBottomCenter;
	}




	public Vector3 GetBoundingBoxBottomCenter()
	{ 
		CalculateBoundingBox ();
		Vector3 boundingBoxBottomCenter = 0.25f * boundingBox.coordinates [4] + 0.25f * boundingBox.coordinates [5] + 0.25f * boundingBox.coordinates [6] + 0.25f * boundingBox.coordinates [7];
		return boundingBoxBottomCenter;
	}


	public Vector3 GetBoundingBoxTopCenter()
	{ 
		CalculateBoundingBox ();
		Vector3 boundingBoxBottomCenter = 0.25f * boundingBox.coordinates [0] + 0.25f * boundingBox.coordinates [1] + 0.25f * boundingBox.coordinates [2] + 0.25f * boundingBox.coordinates [3];
		return boundingBoxBottomCenter;
	}


	public Vector3 GetBoundingBoxFrontCenter()
	{ 
		CalculateBoundingBox ();
		Vector3 pos = 0.25f * boundingBox.coordinates [1] + 0.25f * boundingBox.coordinates [2] + 0.25f * boundingBox.coordinates [5] + 0.25f * boundingBox.coordinates [6];
		return pos;
	}

	public Vector3 GetBoundingBoxBackCenter()
	{ 
		CalculateBoundingBox ();
		Vector3 pos = 0.25f * boundingBox.coordinates [0] + 0.25f * boundingBox.coordinates [3] + 0.25f * boundingBox.coordinates [4] + 0.25f * boundingBox.coordinates [7];
		return pos;
	}

	public Vector3 GetBoundingBoxLeftCenter()
	{ 
		CalculateBoundingBox ();
		Vector3 pos = 0.25f * boundingBox.coordinates [2] + 0.25f * boundingBox.coordinates [3] + 0.25f * boundingBox.coordinates [6] + 0.25f * boundingBox.coordinates [7];
		return pos;
	}

	public Vector3 GetBoundingBoxRightCenter()
	{ 
		CalculateBoundingBox ();
		Vector3 pos = 0.25f * boundingBox.coordinates [0] + 0.25f * boundingBox.coordinates [1] + 0.25f * boundingBox.coordinates [4] + 0.25f * boundingBox.coordinates [5];
		return pos;
	}

	public Vector3 GetBoundingBoxCenter()
	{
		Vector3 boundingBoxCenter = 0.5f * GetBoundingBoxBottomCenter () + 0.5f * GetBoundingBoxTopCenter ();
		return boundingBoxCenter;
	}


	public int[] GetThreeAdjacentCorners(int id, bool local){

		int[] xyz = new int[3];

		Vector3 corner = coordinateSystem.transform.InverseTransformPoint(boundingBox.coordinates [id]);

		if (!local) {
			corner = boundingBox.coordinates [id];
		}

		for (int i=0; i < boundingBox.coordinates.Length; i++){
			
			if (i != id) {
				
				Vector3 current = coordinateSystem.transform.InverseTransformPoint(boundingBox.coordinates [i]);

				if (!local) {
					current = boundingBox.coordinates [i];
				}

				if (Mathf.Abs(current.y-corner.y) < 0.01f && Mathf.Abs(current.z-corner.z) < 0.01f) {
		
					xyz [0] = i;
				}

				if (Mathf.Abs(current.x-corner.x) < 0.01f && Mathf.Abs(current.z-corner.z) < 0.01f) {

					xyz [1] = i;
				}

				if (Mathf.Abs(current.x-corner.x) < 0.01f && Mathf.Abs(current.y-corner.y) < 0.01f) {
		
					xyz [2] = i;
				}
			}
		}

		return xyz;
	}
  
	public Vector3 GetBoundingBoxMinima(bool local)
	{
		Vector3 minima = new Vector3 (9999f, 9999f, 9999f);

		for (int i = 0; i < topFace.vertexBundles.Length; i++)
		{
			if (!local) {
				current = topFace.vertexBundles [i].coordinates;
			} else {
				current = coordinateSystem.transform.InverseTransformPoint(transform.TransformPoint(topFace.vertexBundles [i].coordinates));					
			}

			if (current.x < minima.x) {
				minima.x = current.x;
			}

			if (current.y < minima.y) {
				minima.y = current.y;
			}

			if (current.z < minima.z) {
				minima.z = current.z;
			}

		}

		for (int i = 0; i < bottomFace.vertexBundles.Length; i++)
		{
			if (!local) {
				current = bottomFace.vertexBundles [i].coordinates;
			} else {
				current = coordinateSystem.transform.InverseTransformPoint(transform.TransformPoint(bottomFace.vertexBundles [i].coordinates));					
			}

			if (current.x < minima.x) {
				minima.x = current.x;
			}

			if (current.y < minima.y) {
				minima.y = current.y;
			}

			if (current.z < minima.z) {
				minima.z = current.z;
			}

		}

		return minima;
	}

	public Vector3 GetBoundingBoxMaxima(bool local)
	{
		Vector3 maxima = new Vector3 (-9999f, -9999f, -9999f);

		for (int i = 0; i < topFace.vertexBundles.Length; i++)
		{
			if (!local) {
				current = topFace.vertexBundles [i].coordinates;
			} else {
				current = coordinateSystem.transform.InverseTransformPoint(transform.TransformPoint(topFace.vertexBundles [i].coordinates));					
			}

			if (current.x > maxima.x) {
				maxima.x = current.x;
			}

			if (current.y > maxima.y) {
				maxima.y = current.y;
			}

			if (current.z > maxima.z) {
				maxima.z = current.z;
			}

		}

		for (int i = 0; i < bottomFace.vertexBundles.Length; i++)
		{
			if (!local) {
				current = bottomFace.vertexBundles [i].coordinates;
			} else {
				current = coordinateSystem.transform.InverseTransformPoint(transform.TransformPoint(bottomFace.vertexBundles [i].coordinates));					
			}

			if (current.x > maxima.x) {
				maxima.x = current.x;
			}

			if (current.y > maxima.y) {
				maxima.y = current.y;
			}

			if (current.z > maxima.z) {
				maxima.z = current.z;
			}

		}

		return maxima;
	}



	public Vector3 GetPosOfClosestVertex(Vector3 position, Vector3[] coordinates){

        Vector3 closestVertex = bottomFace.vertexBundles[0].coordinates;
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

	public int GetIdOfClosestVertex(Vector3 position, Vector3[] coordinates){

		Vector3 closestVertex = coordinates[0];

		float shortestDistance = 999999f;
		int idOfClosestVertex = 0;

		for (int i = 0; i < coordinates.Length; i++)
		{
			Vector3 newCoordinate = coordinates[i];

			if (Vector3.Distance(position, newCoordinate) < shortestDistance)
			{
				closestVertex = newCoordinate;
				shortestDistance = Vector3.Distance(position, newCoordinate);
				idOfClosestVertex = i;
			}
		}

		return idOfClosestVertex;

	}

	public void RepositionScalers(){
		topFace.RepositionScaler ();
		bottomFace.RepositionScaler ();
	}

	public void EnterTrashArea(){		
		handles.DisableHandles ();
		HideBoundingBox (true);

		inTrashArea = true;

		Color newColor = transform.GetChild(0).GetComponent<Renderer>().material.color;
		transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(newColor.r - 0.3f, newColor.g - 0.3f, newColor.b - 0.3f, 1f);

		// display trash icon
		LeanTween.alpha(trashIcon, 1f, 0.3f);
	}

	public void ExitTrashArea(){
		inTrashArea = false;

		Color newColor = transform.GetChild(0).GetComponent<Renderer>().material.color;
		transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(newColor.r + 0.3f, newColor.g + 0.3f, newColor.b + 0.3f, 1f);

		// display trash icon
		LeanTween.alpha(trashIcon, 0f, 0.3f);
	}

	public void DeActivateCollider (){
		meshCollider.enabled = false;
	}

	public void ActivateCollider (){
		meshCollider.enabled = true;
	}

	public void DarkenColorObject (){
		Color darkColor = new Color (currentColor.r * 0.5f, currentColor.g * 0.5f, currentColor.b * 0.5f);  
		transform.GetChild(0).GetComponent<Renderer>().material.color = darkColor;
	}

	public void NormalColorObject (){
		transform.GetChild(0).GetComponent<Renderer>().material.color = currentColor;
	}


}
		

 
 