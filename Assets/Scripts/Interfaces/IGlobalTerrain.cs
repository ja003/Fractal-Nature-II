using UnityEngine;
using System.Collections;

public interface IGlobalTerrain {

    void SetHeight(int x, int z, float height);

    float GetHeight(int x, int z);
}
