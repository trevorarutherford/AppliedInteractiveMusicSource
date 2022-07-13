using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WwiseManager : MonoBehaviour
{
    public static WwiseManager instance;

    //event posted when the device enters the quadrangle (turns on quad music)
    public AK.Wwise.Event enterQuad;

    //event posted when the device leaves the quadrangle (shuts off quad music)
    public AK.Wwise.Event exitQuad;

    //RTPC for north south position within quad (affects volume of tracks)
    public AK.Wwise.RTPC quadNorthSouth;

    //states for east west position within quad (affects percussion)
    public AK.Wwise.State noPerc;
    public AK.Wwise.State perc1;
    public AK.Wwise.State perc2;
    public AK.Wwise.State perc3;
    public AK.Wwise.State perc4;

    //debug UI
    public Text zoneText;
    public Text xText;
    public Text yText;


    private ZoneManager zoneManager; //manages zones (areas with the same music)
    private Point testPoint;

    //the point where the android device was last seen
    private Point pointOfLastGPSUpdate;
    private Zone zoneOfLastGPSUpdate; //the zone the device was last seen in

    private void Awake()
    {
        instance = this;
        zoneManager = ZoneManager.instance;
        //testPoint = new Point(-123.33177394500655f, 48.45507975499686f);
        /*
        //to test...
        enterQuad.Post(this.gameObject);//~~~
        perc4.SetValue();//~~~
        perc1.SetValue();//~~~
        quadNorthSouth.SetGlobalValue(0f);//~~~
        */

    }

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            testPoint.x += 0.0001f;
            Debug.Log("TEST POINT: " +  testPoint.x);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            testPoint.x -= 0.0001f;
            Debug.Log("TEST POINT: " + testPoint.x);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            testPoint.y += 0.0001f;
            Debug.Log("TEST POINT: " + testPoint.x);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            testPoint.y -= 0.0001f;
            Debug.Log("TEST POINT: " + testPoint.x);
        }

        //UpdateMusic(testPoint);
    }
    

    /**
     * <summary>
     * Updates the music based on the current location of the mobile device by
     * posting Wwise events and setting Wwise gamesyncs.
     * </summary>
     * 
     * <param name="currLoc">
     * The most up-to-date GPS location collected from the mobile device.
     * </param>
     */
    public void UpdateMusic(Point currLoc)
    {
        pointOfLastGPSUpdate = currLoc;

        //find out what zone the device is in right now
        Zone currZone = zoneManager.GetCurrZone(currLoc);

        if (currZone != null) Debug.Log("CURRENT ZONE = " + currZone.name);
        else Debug.Log("NOT IN A ZONE");

        //Debug.Log("CurrZone = " + currZone.name);

        
        

        //if it's a new zone from last time update the music
        checkForZoneChange(currZone);

        //adjust the music within the zone
        adjustMusicWithinZone();

        if (zoneOfLastGPSUpdate != null) zoneText.text = zoneOfLastGPSUpdate.name;
        else zoneText.text = "No Zone";
        xText.text = "" + pointOfLastGPSUpdate.x;
        yText.text = "" + pointOfLastGPSUpdate.y;
        Debug.Log("Music Updated");
    }

    /**
     * <summary>
     * If the player is in a new zone from last time the GPS was updated, stops
     * the music for the previous zone and starts the music for the new zone.
     * If they're still in the same zone, doesn't change the music.
     * </summary>
     * 
     * <param name="currZone">
     * The zone where the device is (could be the same as zoneOfLastGPSUpdate
     * or could be different)
     * </param>
     */
    private void checkForZoneChange(Zone currZone)
    {
        //if the device is in the same zone as last time do nothing
        if (currZone == zoneOfLastGPSUpdate) return;

        //the device has changed zones...

        /*
         * Turn off the music from the old zone (but only if there was an
         * old zone ie: they could have entered from outside of any zone)
         */
        if (zoneOfLastGPSUpdate != null)
        {
            switch (zoneOfLastGPSUpdate.name)
            {
                case GlobalConstants.ZONE_NAME_QUAD:
                    exitQuad.Post(this.gameObject);
                    break;

                    //...add any new zones here...
            }
        }

        /*
         * Turn on the music for the new zone (but only if there is a new zone
         * ie: they could have exited into territory that is not in any zone)
         */
        if (currZone != null)
        {
            switch (currZone.name)
            {
                case GlobalConstants.ZONE_NAME_QUAD:
                    enterQuad.Post(this.gameObject);
                    break;

                    //...add any new zones here...
            }
        }

        //update zoneOfLastGPSUpdate to reflect the new zone
        zoneOfLastGPSUpdate = currZone;
    }

    /**
     * <summary>
     * Makes changes to the music based on GPS changes within a zone. The
     * changes required depend on the zone since not all zones have the
     * same musical behaviour.
     * </summary>
     */
    private void adjustMusicWithinZone()
    {
        //if the device isn't in any zone do nothing
        if (zoneOfLastGPSUpdate == null) return;

        //different musical behaviour depending on zone
        switch (zoneOfLastGPSUpdate.name)
        {
            case GlobalConstants.ZONE_NAME_QUAD:
                adjustMusicWithinQuad();
                break;

                //...add any new zones here...
        }
    }

    /**
     * <summary>
     * Makes changes to the music specifically for the quad. These include
     * updating the QUAD_NORTH_SOUTH and QUAD_EAST_WEST game parameters as
     * well as posting the QUAD_FOUNTAIN trigger.
     * </summary>
     */
    private void adjustMusicWithinQuad()
    {
        //calculate the north-south position RTPC...

        float quadYMin = zoneOfLastGPSUpdate.FarthestSouthPoint().y;
        float quadYMax = zoneOfLastGPSUpdate.FarthestNorthPoint().y;
        float quadYRange = quadYMax - quadYMin;
        float deviceYPos = pointOfLastGPSUpdate.y;
        float deviceDistFromQuadYMin = deviceYPos - quadYMin;

       
        /*
         * The final value should be a percentage (how far north have they
         * traveled within the zone.
         */
        float quadNorthSouthNewVal = deviceDistFromQuadYMin / quadYRange * 100f;

        //set the north-south position RTPC
        quadNorthSouth.SetGlobalValue(quadNorthSouthNewVal);

        //Debug.Log("Quad DIST = " + deviceDistFromQuadYMin);




        //calculate the current state for the percussion state group

        float thresh1 = zoneOfLastGPSUpdate.longitudeThresholds[0];
        float thresh2 = zoneOfLastGPSUpdate.longitudeThresholds[1];
        float thresh3 = zoneOfLastGPSUpdate.longitudeThresholds[2];
        float thresh4 = zoneOfLastGPSUpdate.longitudeThresholds[3];


        if (pointOfLastGPSUpdate.x > thresh4)
        {
            perc4.SetValue();
        }
        else if(pointOfLastGPSUpdate.x > thresh3)
        {
            perc3.SetValue();
        }
        else if(pointOfLastGPSUpdate.x > thresh2)
        {
            perc2.SetValue();
        }
        else if(pointOfLastGPSUpdate.x > thresh1)
        {
            perc1.SetValue();
        }
        else
        {
            noPerc.SetValue();
        }
    }
}
