using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GPSData
{
    public string gpsStatus;
    public float latitudeVal;
    public float longitudeVal;
    public float altitudeVal;
    public float horizontalAccuracyVal;
    public double timestampVal;

    public GPSData(string gpsStatus, float latitudeVal, float longitudeVal,
        float altitudeVal, float horizontalAccuracyVal, double timestampVal)
    {
        this.gpsStatus = gpsStatus;
        this.latitudeVal = latitudeVal;
        this.longitudeVal = longitudeVal;
        this.altitudeVal = altitudeVal;
        this.horizontalAccuracyVal = horizontalAccuracyVal;
        this.timestampVal = timestampVal;
    }

    public GPSData(string gpsStatus)
    {
        this.gpsStatus = gpsStatus;
        this.latitudeVal = 0f;
        this.longitudeVal = 0f;
        this.altitudeVal = 0f;
        this.horizontalAccuracyVal = 0f;
        this.timestampVal = 0d;
    }
}
