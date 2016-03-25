using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerManager {

    public GlobalCoordinates terrain;
    public GlobalCoordinates filterXXX;

    public void AssignLayers(GlobalCoordinates terrain)
    {
        this.terrain = terrain;
    }

    public float GetValueFromLayers(int x, int z, List<Layer> layers)
    {
        float value = 0;
        float v;
        if (layers.Contains(Layer.terrain))
        {
            v = terrain.GetValue(x, z);
            if(v != 666)
            {
                value += v;
            }
            else
            {
                value = 0;
            }
        }

        if (layers.Contains(Layer.river))
        {
            /*v = river.GetValue(x, z);
            if (v != 666)
            {
                value += v;
            }*/
        }
        return value;
    }
}
