using UnityEngine;
using System.Collections;

public enum Layer{
    terrain,
    river,
        
    filterAverage,
    filterMedian,
    filterSpike,
    filterGaussian,    
    filterMinThreshold,
    filterMaxThreshold,

    erosionHydraulicWater,
    erosionHydraulic,
    erosionThermal
}
