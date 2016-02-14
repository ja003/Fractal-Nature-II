using UnityEngine;
using System.Collections;

public interface ITerrainGenerator {

    void GenerateTerrainOn(float[,] heightmap);
}
