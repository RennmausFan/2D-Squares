using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour {

    [SerializeField]
    private GameObject[] unitPrefabs;

    public List<GameObject> allySelection = new List<GameObject>();
    public List<GameObject> enemySelection = new List<GameObject>();

    public List<GameObject> currentSelection;

    public static UnitSelectionContainer selectedContainer;

    public int alliesCap, enemiesCap;

    public CharClasses selectedUnit;

    public Vector3 origin, offset;

    // Use this for initialization
    void Awake ()
    {
        alliesCap = MapManager.currentMap.alliesTeamSize;
        enemiesCap = MapManager.currentMap.enemiesTeamSize;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RemoveAllFromSelection(List<GameObject> selection)
    {
        selection.Clear();
    }

    public void RemoveFromSelection(int index, List<GameObject> selection)
    {
        selection.RemoveAt(index);
    }

    public void AddToSelection(CharClasses type, List<GameObject> selection)
    {
        GameObject selectedUnit = null;
        foreach (GameObject unit in unitPrefabs)
        {
            if (unit.GetComponent<Unit>().unitClass == type)
            {
                selectedUnit = unit;
                break;
            }
        }
        if (selection == allySelection)
        {
            if (allySelection.Count < alliesCap)
            {
                selection.Add(selectedUnit);
            }
        }
        else
        {
            if (enemySelection.Count < enemiesCap)
            {
                selection.Add(selectedUnit);
            }
        }

    }


}
