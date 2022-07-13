using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager instance;

    //other manager scripts in the app
    private ZoneManager zoneManager; //manages "zones" (areas with same music)
    private GPSManager gpsManager; //manages collecting GPS data from device
    private WwiseManager wwiseManager; //manages communications with Wwise

    //the last colelction of GPS data to be read from the mobile device
    private GPSData currGPSReport;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        zoneManager = ZoneManager.instance;
        gpsManager = GPSManager.instance;
        wwiseManager = WwiseManager.instance;

        //start the zone manager (must be done before starting gps manager)
        zoneManager.Init();

        //start the GPS manager
        gpsManager.Init();
    }

    /**
     * <summary>
     * Called from GPSManager when new GPS data is read from the mobile device.
     * Does everything that must be done when new GPS data comes in.
     * </summary>
     * 
     * <param name="gpsReport">
     * A struct containing the most recent GPS data from the mobile device.
     * </param>
     */
    public void RespondToGPSReport(GPSData gpsReport)
    {
        currGPSReport = gpsReport;

        //create a new Point object out of the latitude and longitude
        Point currLocation = new Point(gpsReport.longitudeVal,
            gpsReport.latitudeVal);

        //temporary
        //currLocation.x = -123.33190314499522f;
        //currLocation.y = 48.45529152064581f;
        Debug.Log("CURRENT LOCATION = (" + currLocation.x + ", " + currLocation.y + ")");

        //change the music based on the new GPS position
        wwiseManager.UpdateMusic(currLocation);
    }
}