using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class GPSManager : MonoBehaviour
{
    public static GPSManager instance;

    public Text GPSStatus;
    //public Text latitudeValue;
    //public Text longitudeValue;
    //public Text horizontalAccuracyValue;
    //public Text timestampValue;

    private void Awake()
    {
        instance = this;
    }

    public void Init()
    {
        StartCoroutine(GPSLoc());
    }

//CITATION: YouTube. (2020, September 4). GPS location in unity 2020. YouTube.
//Retrieved November 2, 2021, from https://www.youtube.com/watch?v=JWccDbm69Cg.
    private IEnumerator GPSLoc()
    {
        Debug.Log("running coroutine");

        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Debug.Log("before permissions");
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
            Debug.Log("after permissions");
        }

        Debug.Log("waiting");
        if (!Input.location.isEnabledByUser)
            yield return new WaitForSeconds(10);
        Debug.Log("done waiting");
        //check that the user has enabled GPS location
        if (!Input.location.isEnabledByUser)
            yield break;

        Debug.Log("tests complete");
        //begin initializing GPS service
        Input.location.Start();

        int maximumSeconds = 20;
        int currentSeconds = 0;

        //wait until the GPS service is finished trying to initialize or it times out
        while (Input.location.status == LocationServiceStatus.Initializing && currentSeconds < maximumSeconds)
        {
            yield return new WaitForSeconds(1);
            currentSeconds++;
        }

        //break if the initialization takes too long
        if (currentSeconds >= maximumSeconds)
        {
            Debug.Log("GPS initialization timed out");
            //GPSStatus.text = "GPS initialization timed out";
            yield break;
        }

        //break if the GPS service was not initialized
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Failed to initialize GPS service.");
            //GPSStatus.text = "Failed to initialize GPS service.";
            yield break;
        }
        else
        {
            //the service was initialized properly
            Debug.Log("GPS Running");
            //GPSStatus.text = "GPS Running";

            //call UpdateGPSData after 0.5 seconds and then once every second
            InvokeRepeating("UpdateGPSData", 0.5f, 1f);
        }
    }

    /**
     * <summary>
     * Collects the GPS data from the mobile device if it's available and then
     * tells the WwiseMusicManager to update the music if needed based on this
     * new GPS data. Also updates the GPS data shown on the UI.
     * </summary>
     */
    private void UpdateGPSData()
    {
        GPSData gpsReport;

        if (Input.location.status == LocationServiceStatus.Running)
        {
            //GPS service is still running
            Debug.Log("GPS Running");
            GPSStatus.text = "GPS Running";
            //latitudeValue.text = Input.location.lastData.latitude.ToString();
            //longitudeValue.text = Input.location.lastData.longitude.ToString();
            //horizontalAccuracyValue.text = Input.location.lastData.horizontalAccuracy.ToString();
            //timestampValue.text = Input.location.lastData.timestamp.ToString();

//END OF CITATION

            //collect the GPS data from the mobile device
            string gpsStatus = "GPS Running";
            float latitudeVal = Input.location.lastData.latitude;
            float longitudeVal = Input.location.lastData.longitude;
            float altitudeVal = Input.location.lastData.altitude;
            float horizontalAccuracyVal = Input.location.lastData.horizontalAccuracy;
            double timestampVal = Input.location.lastData.timestamp;

            //store the GPS data in a struct for later use
            gpsReport = new GPSData(gpsStatus, latitudeVal,
                longitudeVal, altitudeVal, horizontalAccuracyVal, timestampVal);
        }
        else
        {
            //GPS service stopped
            Debug.Log("GPS Stopped");
            //GPSStatus.text = "GPS Stopped";

            string gpsStatus = "GPS Stopped";

            /*
             * store the GPS status in a struct for later use (no point in
             * storing other GPS data since it won't be accurate if the GPS
             * isn't running
             */
            gpsReport = new GPSData(gpsStatus);
        }

        //make the wwise music change based on the new GPS data if needed
        MainManager.instance.RespondToGPSReport(gpsReport);
    }
}
