using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    private List<MapInformation> maps = new List<MapInformation>();

    [SerializeField]
    private Dropdown mapSelection;

	// Use this for initialization
	void Start () {
        maps = MapManager.maps;
        CreateSelection(maps.Count);
    }

    //Sets up the dictonary and the list
    public void CreateSelection(int count)
    {
        List<string> mapNames = new List<string>();
        foreach (MapInformation i in maps)
        {
            mapNames.Add(i.mapName);
        }
        mapSelection.AddOptions(mapNames);
    }

    public void StartGame()
    {
        MapManager.currentMap = maps[mapSelection.value];
        print(MapManager.currentMap.mapName);
        SceneManager.LoadScene("unitSelection");
    }
}
