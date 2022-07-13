using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * <summary>
 * A representation of a "zone" (a section of terrain associated with one
 * musical cue)
 * </summary>
 */
public class Zone
{
    //the name of this zone
    public string name;

    //(x, y) coordinates of all vertices of the perimeter of this zone
    public Point[] perimeter;

    //longitude thresholds for changes in the music
    public float[] longitudeThresholds;

    public Zone() { }

    public Zone(string name, Point[] perimeter, float[] longitudeThresholds = null)
    {
        this.name = name;
        this.perimeter = perimeter;
        this.longitudeThresholds = longitudeThresholds;
    }

//CITATION: How to check if a given point lies inside or outside a polygon?
//GeeksforGeeks. (2021, November 24). Retrieved December 8, 2021,
//from https://www.geeksforgeeks.org/how-to-check-if-a-given-point-lies-inside-a-polygon/.
//----- NOTE: originally in C++ -----
    #region GEOMETRY_FUNCITONS
    // Given three collinear points p, q, r, the function checks if 
    // point q lies on line segment 'pr' 
    bool onSegment(Point p, Point q, Point r)
    {
        if (q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) &&
                q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y))
            return true;
        return false;
    }

    // To find orientation of ordered triplet (p, q, r). 
    // The function returns following values 
    // 0 --> p, q and r are collinear 
    // 1 --> Clockwise 
    // 2 --> Counterclockwise 
    int orientation(Point p, Point q, Point r)
    {
        float val = ((q.y - p.y) * (r.x - q.x) -
                (q.x - p.x) * (r.y - q.y));

        if (val == 0f) return 0; // collinear 
        return (val > 0f) ? 1 : 2; // clock or counterclock wise 
    }

    // The function that returns true if line segment 'p1q1' 
    // and 'p2q2' intersect. 
    bool doIntersect(Point p1, Point q1, Point p2, Point q2)
    {
        // Find the four orientations needed for general and 
        // special cases 
        int o1 = orientation(p1, q1, p2);
        int o2 = orientation(p1, q1, q2);
        int o3 = orientation(p2, q2, p1);
        int o4 = orientation(p2, q2, q1);

        // General case 
        if (o1 != o2 && o3 != o4)
            return true;

        // Special Cases 
        // p1, q1 and p2 are collinear and p2 lies on segment p1q1 
        if (o1 == 0 && onSegment(p1, p2, q1)) return true;

        // p1, q1 and p2 are collinear and q2 lies on segment p1q1 
        if (o2 == 0 && onSegment(p1, q2, q1)) return true;

        // p2, q2 and p1 are collinear and p1 lies on segment p2q2 
        if (o3 == 0 && onSegment(p2, p1, q2)) return true;

        // p2, q2 and q1 are collinear and q1 lies on segment p2q2 
        if (o4 == 0 && onSegment(p2, q1, q2)) return true;

        return false; // Doesn't fall in any of the above cases 
    }

    /**
     * <summary>
     * Checks if the given point is within this zone.
     * </summary>
     * 
     * <param name="p"> The point to check. </param>
     */
    public bool Contains(Point p)
    {
        //the number of points in the perimeter of this zone
        int n = perimeter.Length;

        // There must be at least 3 vertices in polygon[] 
        if (n < 3) return false;

        // Create a point for line segment from p to infinite 
        Point extreme = new Point(GlobalConstants.MAX_LONGITUDE, p.y);

        // Count intersections of the above line with sides of polygon 
        int count = 0, i = 0;
        do
        {
            int next = (i + 1) % n;



            // Check if the line segment from 'p' to 'extreme' intersects 
            // with the line segment from 'polygon[i]' to 'polygon[next]' 
            if (doIntersect(perimeter[i], perimeter[next], p, extreme))
            {
                // If the point 'p' is collinear with line segment 'i-next', 
                // then check if it lies on segment. If it lies, return true, 
                // otherwise false 
                if (orientation(perimeter[i], p, perimeter[next]) == 0)
                    return onSegment(perimeter[i], p, perimeter[next]);

                count++;
            }
            i = next;
        } while (i != 0);

        // Return true if count is odd, false otherwise 
        return count % 2 == 1; // Same as (count%2 == 1) 
    }
    #endregion
//END OF CITATION

    /**
     * <summary>
     * Returns the farthest north point in the zone.
     * </summary>
     */
    public Point FarthestNorthPoint()
    {
        Point farthestNorthPoint = perimeter[0];

        foreach (Point p in perimeter)
        {
            if (p.y > farthestNorthPoint.y)
                farthestNorthPoint = p;
        }

        return farthestNorthPoint;
    }

    /**
     * <summary>
     * Returns the farthest south point in the zone.
     * </summary>
     */
    public Point FarthestSouthPoint()
    {
        Point farthestSouthPoint = perimeter[0];

        foreach (Point p in perimeter)
        {
            if (p.y < farthestSouthPoint.y)
                farthestSouthPoint = p;
        }

        return farthestSouthPoint;
    }

    /**
     * <summary>
     * Returns the farthest east point in the zone.
     * </summary>
     */
    public Point FarthestEastPoint()
    {
        Point farthestEastPoint = perimeter[0];

        foreach (Point p in perimeter)
        {
            if (p.y < farthestEastPoint.y)
                farthestEastPoint = p;
        }

        return farthestEastPoint;
    }

    /**
     * <summary>
     * Returns the farthest west point in the zone.
     * </summary>
     */
    public Point FarthestWestPoint()
    {
        Point farthestWestPoint = perimeter[0];

        foreach (Point p in perimeter)
        {
            if (p.y > farthestWestPoint.y)
                farthestWestPoint = p;
        }

        return farthestWestPoint;
    }


}
