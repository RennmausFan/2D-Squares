using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

//Stores the information for each map
[System.Serializable]
public class MapInformation
{
    public string mapName = "No Map name given";
    public string sceneName = "Map_X";
    public int alliesTeamSize = 0;
    public int enemiesTeamSize = 0;

    public MapInformation(string pMapName, string pSceneName, int pAlliesTeamSize, int pEnemiesTeamSize)
    {
        mapName = pMapName;
        sceneName = pSceneName;
        enemiesTeamSize = pEnemiesTeamSize;
        alliesTeamSize = pAlliesTeamSize;
    }
}

//Contains a list with all maps
public static class MapManager{

    public static List<MapInformation> maps = new List<MapInformation>();

    public static MapInformation currentMap;

    static MapManager()
    {
        //Maps
        maps.Add(new MapInformation("Empty Map", "map_0", 0, 0));
        maps.Add(new MapInformation("River Islands", "map_1", 4, 3));
        maps.Add(new MapInformation("BattleField", "map_2", 6, 6));
        maps.Add(new MapInformation("Rocky Valley", "map_3", 6, 6));
        //Default-map
        currentMap = maps[0];
    }
}
