using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Drawing;

/**
 * <summary>
 * For anything having to do with "zones". A zone is a section of terrain that
 * has the same musical cue (song) associated with it. While the music may
 * change within a zone it is still part of the same cue.
 * </summary>
 */
public class ZoneManager : MonoBehaviour
{
    public static ZoneManager instance;
    public Zone[] zones;

    private void Awake()
    {
        instance = this;
    }

    public void Init()
    {
        ReadZoneConfig();
    }

    /**
     * <summary>
     * Reads the zones from the XML configuration file where zones can be
     * manually set.
     * 
     * NOTE: This method currently uses hardcoded values but this is temporary.
     * In the final version of the app the values will be read from the XML
     * configuration file.
     * </summary>
     */
    public void ReadZoneConfig()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Zone));

        //temporary
        zones = new Zone[1];
        
        Point[] quadPerimeter = {
            new Point(-123.31352650130322f, 48.46368388698954f), 
            new Point(-123.31013082482218f, 48.46390087064562f), 
            new Point(-123.30999134995882f, 48.46292977264291f), 
            new Point(-123.31316474553648f, 48.46264770411893f)
        };
        

        
        float[] quadLongitudeThresholds = {
            -123.31282532178712f,
            -123.31221293921188f,
            -123.31146394821602f,
            -123.31079588294928f
        };
        

        /*
         * FOR TESTING IN MY NEIGHBOURHOOD SO I DIDN'T NEED TO BE ON CAMPUS
        Point[] quadPerimeter = {
            new Point(-123.33366002972619f, 48.45613245198574f), 
            new Point(-123.32884833504535f, 48.45607465742319f), 
            new Point(-123.32877986332284f, 48.45483618670184f), 
            new Point(-123.33382187197934f, 48.45485269984354f)
        };
        

        
        float[] quadLongitudeThresholds = {
            -123.33229681997831f,
            -123.33138178877769f,
            -123.33086513850793f,
            -123.32998123081755f
        };
        */

        zones[0] = new Zone("quad", quadPerimeter, longitudeThresholds: quadLongitudeThresholds);
    }

    /**
     * <summary>
     * Returns the zone that the device is currently in.
     * </summary>
     * 
     * <param name="p">
     * The device's current GPS location.
     * </param>
     */
    public Zone GetCurrZone(Point p)
    {
        //loop through all the possible zones
        foreach(Zone curr in zones)
        {
            //if the device's GPS location falls within a zone return that zone
            if (curr.Contains(p))
                return curr;
        }

        //device was not in any zone
        return null;
    }
}