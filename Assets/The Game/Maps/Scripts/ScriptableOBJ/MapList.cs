using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapListSO", menuName = "MapGeneration/MapList", order = 0)]
public class MapListSO : ScriptableObject
{
    public List<MapData> Maps;
}