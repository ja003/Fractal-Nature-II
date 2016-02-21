using UnityEngine;
using System.Collections;
using System;

public class FunctionMathCalculator {

    public bool IsInRange(Vertex vert, Vertex center, int radius)
    {
        if (GetDistance(vert, center) < radius)
            return true;
        return false;
    }

    public float GetDistance(Vertex v1, Vertex v2)
    {
        return (float)Math.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.z - v2.z) * (v1.z - v2.z));
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
}
