using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * <summary>
 * An (x, y) cartesian style point representing a GPS location where x is
 * longitude [-180, 180] and y is latitude [-90, 90].
 * </summary>
 */
public class Point
{
    public float x;
    public float y;

    public Point() { }

    public Point(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    //compare this point to another point
    public bool Equals(Point p)
    {
        return this.x == p.x && this.y == p.y;
    }
}

