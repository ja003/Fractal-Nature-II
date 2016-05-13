using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class FunctionMathCalculator {

    public LocalTerrain lt;

    public FunctionMathCalculator()
    {
    }

    public void AssignFunctions(LocalTerrain localTerrain)
    {
        lt = localTerrain;
    }

    public bool IsInRange(Vertex vert, Vertex center, int radius)
    {
        if (GetDistance(vert, center) < radius)
            return true;
        return false;
    }

    

    /// <summary>
    /// negative sign: returns number1 < number2
    /// positive sign: returns number1 > number2
    /// 0: returns number1 == number2
    /// </summary>
    /// <param name="sign"></param>
    /// <param name="number1"></param>
    /// <param name="number2"></param>
    /// <returns></returns>
    public bool LesserEqual(int sign, float number1, float number2)
    {
        if(sign < 0)
        {
            return number1 < number2;
        }
        else if (sign > 0)
        {
            return number1 > number2;
        }
        else
            return number1 == number2;
    }


    /// <summary>
    /// determines which side of given area does [x,z] lies on
    /// </summary>
    public Direction GetReachedSide(
        int x, int z, int borderOffset,
        int x_min, int z_min, int x_max, int z_max)
    {
        if (IsCloseTo(z, z_max, borderOffset))
        {
            return Direction.top;
        }
        if (IsCloseTo(x, x_max, borderOffset))
        {
            return Direction.right;
        }
        if (IsCloseTo(z, z_min, borderOffset))
        {
            return Direction.bot;
        }
        if (IsCloseTo(x, x_min, borderOffset))
        {
            return Direction.left;
        }
        return Direction.none;
    }

    // determines if point is close to one of the available side
    public bool ReachedAvailableSide(
        int x, int z, List<Direction> reachedSides, int borderOffset,
        int x_min, int z_min, int x_max, int z_max)
    {
        bool reachedAvailableSide = true;
        foreach (Direction sides in reachedSides)
        {
            switch (sides)
            {
                case Direction.top:
                    if (IsCloseTo(z, z_max, borderOffset))
                    {
                        reachedAvailableSide = false;
                    }
                    break;
                case Direction.right:
                    if (IsCloseTo(x, x_max, borderOffset))
                    {
                        reachedAvailableSide = false;
                    }
                    break;
                case Direction.bot:
                    if (IsCloseTo(z, z_min, borderOffset))
                    {
                        reachedAvailableSide = false;
                    }
                    break;
                case Direction.left:
                    if (IsCloseTo(x, x_min, borderOffset))
                    {
                        reachedAvailableSide = false;
                    }
                    break;
            }
        }
        return reachedAvailableSide;
    }


    // returns projection of given vertex on side of defined border
    public Vertex GetVertexOnBorder(Vertex vertex, int borderOffset, Direction onSide,
        int x_min, int x_max, int z_min, int z_max)
    {
        int x = vertex.x;
        int z = vertex.z;

        Vertex borderVertex = vertex;

        switch (onSide)
        {
            case Direction.top:
                borderVertex = new Vertex(x, z_max, vertex.height);
                break;
            case Direction.right:
                borderVertex = new Vertex(x_max, z, vertex.height);
                break;
            case Direction.bot:
                borderVertex = new Vertex(x, z_min, vertex.height);
                break;
            case Direction.left:
                borderVertex = new Vertex(x_min, z, vertex.height);
                break;
            case Direction.none:
                return vertex;

        }
        borderVertex.side = vertex.side;
        vertex.side = Direction.none;
        return borderVertex;        
    }

    /// <summary>
    /// assignes values to boundaries
    /// boundaries starts on starting point and ends on opposite side of the reached side
    /// </summary>
    /// MAYBE OBSOLETE
    /*public void DetermineBoundaries(Vertex start, RiverInfo river,
         ref int x_min, ref int z_min, ref int x_max, ref int z_max)
    {
        //Debug.Log("FROM");
        //Debug.Log(x_min);
        //Debug.Log(z_min);
        //Debug.Log(x_max);
        //Debug.Log(z_max);

        if (river.reachTop)
        {
            z_max = start.z;
        }
        else if (river.reachRight)
        {
            x_max = start.x;
        }
        else if (river.reachBot)
        {
            z_min = start.z;
        }
        else if (river.reachBot)
        {
            x_min = start.x;
        }
        //Debug.Log("TO");
        //Debug.Log(x_min);
        //Debug.Log(z_min);
        //Debug.Log(x_max);
        //Debug.Log(z_max);
    }*/

    public Direction GetOppositeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.top:
                return Direction.bot;
            case Direction.right:
                return Direction.left;
            case Direction.bot:
                return Direction.top;
            case Direction.left:
                return Direction.right;
        }
        return Direction.none;
    }

    /// <summary>
    /// determines if [x,z] lies in rectangle defined by v1,v2 and width
    /// </summary>
    public bool BelongsToPath(int x, int z, Vertex v1, Vertex v2, float width)
    {
        Vector2 left;
        Vector2 right;
        if (v1.x < v2.x)
        {
            left = new Vector2(v1.x, v1.z);
            right = new Vector2(v2.x, v2.z);
        }
        else
        {
            left = new Vector2(v2.x, v2.z);
            right = new Vector2(v1.x, v1.z);
        }

        Vector2 dir = new Vector2(right.y - left.y, -(right.x - left.x));
        dir = dir.normalized;

        float widthMultiplier = 2.1f;
        Vector2 point1 = new Vector2(left.x + dir.x * widthMultiplier * width, left.y + dir.y * widthMultiplier * width);
        Vector2 point2 = new Vector2(left.x - dir.x * widthMultiplier * width, left.y - dir.y * widthMultiplier * width);
        //has to be correct order! 1-2-3-4
        Vector2 point3 = new Vector2(right.x - dir.x * widthMultiplier * width, right.y - dir.y * widthMultiplier * width);
        Vector2 point4 = new Vector2(right.x + dir.x * widthMultiplier * width, right.y + dir.y * widthMultiplier * width);

        if (v1.x == 30 && v1.z == 60 && x == 10)
        {
            //Debug.Log(point1);
            //Debug.Log(point2);
            //Debug.Log(point3);
            //Debug.Log(point4);
        }

        List<Vector2> rectangle = new List<Vector2>();
        rectangle.Add(point1);
        rectangle.Add(point2);
        rectangle.Add(point3);
        rectangle.Add(point4);

        bool isInSet = IsInSet(x, z, rectangle);
        return isInSet;
    }

    /// <summary>
    /// some..??? magic
    /// y = z (...used in Vector2)
    /// </summary>
    /// <returns></returns>
    public bool IsInSet(int x, int y, List<Vector2> set)
    {
        if (set.Count < 3)
            return false;
        Vector2 last = set[set.Count - 1];
        Vector2 first = set[0];
        //determine position of point
        int position = Math.Sign((first.x - last.x) * (y - last.y) - (first.y - last.y) * (x - last.x));
        if (position == 0)//not exactly correct!!!
        {
            Vector2 A = set[0];
            Vector2 B = set[1];
            position = Math.Sign((B.x - A.x) * (y - A.y) - (B.y - A.y) * (x - A.x));
        }
        //point has to have same position from each part of set
        for (int i = 0; i < set.Count - 1; i++)
        {
            Vector2 A = set[i];
            Vector2 B = set[i + 1];
            int pos = Math.Sign((B.x - A.x) * (y - A.y) - (B.y - A.y) * (x - A.x));

            if (pos != position)
                return false;
        }

        return true;
    }

    /// <summary>
    /// general line equation: ax + by + c = 0
    /// distance = |a*x_0 + b*y_0 + c| / sqrt(a^2 + b^2)
    /// </summary>
    public float GetDistanceFromLine(Vertex point, int a, int b, int c)
    {
        return (float)(Math.Abs(a * point.x + b * point.z + c) / (Math.Sqrt(a * a + b * b)));
    }

    /// <summary>
    /// general line equation: ax + by + c = 0
    /// line defined by v1 and v2
    /// </summary>
    public float GetDistanceFromLine(Vertex point, Vertex v1, Vertex v2)
    {
        //general line equation parameters

        int a = v1.z - v2.z;
        int b = -(v1.x - v2.x);
        int c = -(a * v1.x) - (b * v1.z);

        return GetDistanceFromLine(point, a, b, c);
    }


    public float GetDistanceBetweenPoints(Vertex point1, Vertex point2)
    {
        float a = point2.x - point1.x;
        float b = point2.z - point1.z;
        float distance = (float)Math.Sqrt(a * a + b * b);
        return distance;
    }

    /// <summary>
    /// calculates surrounding area of given visible area
    /// starts in center and moves by step = patchSize
    /// </summary>
    /// <param name="extraPatchCount">how many extra patches will be contained</param>
    /// <returns></returns>
    public Area GetSurroundingAreaOf(Vertex centerOnGrid,Area visibleArea,int patchSize, int extraPatchCount)
    {
        Vertex botLeft;
        Vertex topRight;

        if (!visibleArea.Contains(centerOnGrid) || patchSize <= 0)
        {
            Debug.Log("centerOnGrid not in visible area: " + centerOnGrid);
            botLeft = new Vertex(centerOnGrid.x - patchSize,  centerOnGrid.z - patchSize);
            topRight = new Vertex(centerOnGrid.x + patchSize,  centerOnGrid.z + patchSize);
            return new Area(botLeft, topRight);
        }

        //get most left coordinate
        Vertex pointLeft = centerOnGrid.Clone(); ;
        while (visibleArea.Contains(pointLeft)){
            pointLeft.x -= patchSize;
        }
        //get most bot coordinate
        Vertex pointBot = centerOnGrid.Clone(); ;
        while (visibleArea.Contains(pointBot))
        {
            pointBot.z -= patchSize;
        }

        //get most right coordinate
        Vertex pointRight = centerOnGrid.Clone(); ;
        while (visibleArea.Contains(pointRight))
        {
            pointRight.x += patchSize;
        }
        //get most top coordinate
        Vertex pointTop = centerOnGrid.Clone(); ;
        while (visibleArea.Contains(pointTop))
        {
            pointTop.z += patchSize;
        }

        botLeft = new Vertex(pointLeft.x - extraPatchCount * patchSize, 
            pointBot.z - extraPatchCount * patchSize);
        topRight = new Vertex(pointRight.x + extraPatchCount * patchSize,  
            pointTop.z + extraPatchCount * patchSize);

        return new Area(botLeft, topRight);
    }

    /// <summary>
    /// calculates botLeft point from given vertices
    /// used in river digging function
    /// </summary>
    public Vertex CalculateBotLeft(Vertex v1, Vertex v2, int width, float widthFactor)
    {
        Vertex botLeft = new Vertex(v1.x, v1.z);
        if (v1.x < v2.x)
        {
            botLeft.x = (int)(v1.x - widthFactor*width);
        }
        else
        {
            botLeft.x = (int)(v2.x - widthFactor*width);
        }
        if (v1.z < v2.z)
        {
            botLeft.z = (int)(v1.z - widthFactor * width);
        }
        else
        {
            botLeft.z = (int)(v2.z - widthFactor * width);
        }
        return botLeft;
    }

    /// <summary>
    /// calculates botLeft point from given vertices
    /// </summary>
    public Vertex CalculateTopRight(Vertex v1, Vertex v2, int width, float widthFactor)
    {
        Vertex topRight = new Vertex(v1.x, v1.z);
        if (v1.x < v2.x)
        {
            topRight.x = (int)(v2.x + widthFactor * width);
        }
        else
        {
            topRight.x = (int)(v1.x + widthFactor * width);
        }
        if (v1.z < v2.z)
        {
            topRight.z = (int)(v2.z + widthFactor * width);
        }
        else
        {
            topRight.z = (int)(v1.z + widthFactor * width);
        }
        return topRight;
    }

    // returns distance from corner of given area
    public float GetDistanceFromCorner(int x, int z,
        int x_min, int x_max, int z_min, int z_max)
    {
        List<float> distances = new List<float>();
        distances.Add(GetDistance(x, z, x_min, z_min));
        distances.Add(GetDistance(x, z, x_min, z_max));
        distances.Add(GetDistance(x, z, x_max, z_min));
        distances.Add(GetDistance(x, z, x_max, z_max));

        distances.Sort();

        return distances[0];
    }

    public float GetDistance(int x1, int z1, int x2, int z2)
    {
        return GetDistance(new Vertex(x1, z1), new Vertex(x2, z2));
    }

    public float GetDistance(Vertex v1, Vertex v2)
    {
        return (float)Math.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.z - v2.z) * (v1.z - v2.z));
    }


    public bool IsCloseTo(int value, int border, int offset)
    {
        return border - offset <= value && value <= border + offset;
    }


    /// <summary>
    /// determines the area on visible terrain based on given point 
    /// and side where point was previously located (before visible terrain moved)
    /// </summary>
    public Area CalculateRestrictedArea(Vertex point)
    {
        /*
        Vector3 botLeft = lt.GetBotLeft();
        Vector3 topRight= lt.GetTopRight();
        
        switch (point.side)
        {
            case Direction.top:
                botLeft = new Vector3(lt.GetBotLeft().x, 0, point.z);
                topRight = lt.GetTopRight();
                break;
            case Direction.right:
                botLeft = new Vector3(point.x, 0, lt.GetBotLeft().z);
                topRight = lt.GetTopRight();
                break;
            case Direction.bot:
                botLeft = lt.GetBotLeft();
                topRight = new Vector3(lt.GetTopRight().x, 0, point.z);
                break;
            case Direction.left:
                botLeft = lt.GetBotLeft();
                topRight = new Vector3(point.x, 0, lt.GetTopRight().z);
                break;
        }*/

        Vector3 botLeft = lt.globalTerrainC.definedArea.botLeft;
        Vector3 topRight = lt.globalTerrainC.definedArea.topRight;
        Area definedArea = lt.globalTerrainC.definedArea;

        switch (point.side)
        {
            case Direction.top:
                botLeft = new Vector3(definedArea.botLeft.x, 0, point.z);
                topRight = definedArea.topRight;
                break;
            case Direction.right:
                botLeft = new Vector3(point.x, 0, definedArea.botLeft.z);
                topRight = definedArea.topRight;
                break;
            case Direction.bot:
                botLeft = definedArea.botLeft;
                topRight = new Vector3(definedArea.topRight.x, 0, point.z);
                break;
            case Direction.left:
                botLeft = definedArea.botLeft;
                topRight = new Vector3(point.x, 0, definedArea.topRight.z);
                break;
        }


        return new Area(botLeft, topRight);
    }

}
