using UnityEngine;
using System.Collections;
using System;

public class RasterManager : Singleton<RasterManager> {

    public float rasterLevel;
    public float rasterLevelAngles;
	public float smoothTime = 0.3F;

	public float maxValue;
	public float minValue;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public int getNumberOfGridUnits(float value1, float value2)
    {
        int count = (int) (Mathf.Round(value1 / rasterLevel) - Mathf.Round(value2 / rasterLevel));
        return count;
    }

    public float Raster(float input)
    {
        float count = Mathf.Round(input/rasterLevel);
		float rasteredFloat = Mathf.Max (Mathf.Min (count * rasterLevel, maxValue), minValue);


        return rasteredFloat;
    }

    // 
    public Vector3 Raster(Vector3 input)
    {
        float countx = Mathf.Round(input.x / rasterLevel);
        float county = Mathf.Round(input.y / rasterLevel);
        float countz  = Mathf.Round(input.z / rasterLevel);

        Vector3 rasteredVector = new Vector3(countx * rasterLevel, county * rasterLevel, countz * rasterLevel);

        return rasteredVector;
    }

    public float RasterAngle(float input)
    {
        float count = Mathf.Round(input / rasterLevelAngles);
        float rasteredFloat = count * rasterLevelAngles;
        return rasteredFloat;
    }
}
