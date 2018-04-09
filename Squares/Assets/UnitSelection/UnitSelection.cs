using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UnitSelection : MonoBehaviour {

    [SerializeField]
    private GameObject[] unitPrefabs;

    [SerializeField]
    private Text headline;

    [SerializeField]
    private GameObject previewUnits;

    [SerializeField]
    private GameObject previewSelection;

    public static List<GameObject> allySelection = new List<GameObject>();
    public static List<GameObject> enemySelection = new List<GameObject>();

    public List<GameObject> currentSelection;

    public static UnitSelectionContainer selectedContainer;

    public int alliesCap, enemiesCap;

    public int leftAllies, leftEnemies;

    public CharClasses selectedUnit;

    public static int toRemoveIndex = -1;

    public Vector3 origin, offset;

    // Use this for initialization
    void Awake ()
    {
        alliesCap = MapManager.currentMap.alliesTeamSize;
        enemiesCap = MapManager.currentMap.enemiesTeamSize;

        currentSelection = allySelection;
	}
	
	// Update is called once per frame
	void Update ()
    {
        leftAllies = alliesCap - allySelection.Count;
        leftEnemies = enemiesCap - enemySelection.Count;
        if (currentSelection == allySelection)
        {
            headline.text = "Player 1: Select " + leftAllies + " units!";
        }
        else
        {
            headline.text = "Player 2: Select " + leftEnemies + " units!";
        }
	}

    public void Add()
    {
        if (selectedContainer == null)
        {
            return;
        }
        AddToSelection(selectedContainer.unit.unitClass, currentSelection);
        //PrintTeam();
        PreviewTeam();
    }

    public void PrintTeam()
    {
        foreach(GameObject obj in currentSelection)
        {
            print(obj.GetComponent<Unit>().unitClass);
        }
    }

    public void RemoveAll()
    {
        currentSelection.Clear();
        PreviewTeam();
    }

    public void Remove()
    {
        if (currentSelection.Count != 0 && toRemoveIndex >= 0)
        {
            currentSelection.RemoveAt(toRemoveIndex);
            PreviewTeam();
            toRemoveIndex = -1;
        }
    }

    public void Done()
    {
        if (currentSelection == allySelection)
        {
            if (currentSelection.Count == alliesCap)
            {
                //Player 2 can choose Units
                currentSelection = enemySelection;
                PreviewTeam();
            }
        }
        else
        {
            if (currentSelection.Count == enemiesCap)
            {
                //End selection and load level
                SceneManager.LoadScene(MapManager.currentMap.sceneName);
            }
        }
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
    
    public void PreviewTeam()
    {
        //Destroy all child objects of previewUnits
        for (int i=0; i<previewUnits.transform.childCount; i++)
        {
            Destroy(previewUnits.transform.GetChild(i).gameObject);
        }
        //Spawn all units of currentSelection to preview
        for (int p=0; p<currentSelection.Count; p++)
        {
            Vector3 position = origin + p * offset;
            GameObject go = Instantiate(currentSelection[p], position, Quaternion.identity);
            go.transform.localScale = new Vector3(1.25f, 1.25f, 0);
            go.transform.parent = previewUnits.transform;
            go.GetComponentInChildren<Healthbar>().gameObject.SetActive(false);
            GameObject sel = Instantiate(previewSelection, Vector3.zero, Quaternion.identity);
            sel.transform.SetParent(go.transform, false);
            sel.GetComponentInChildren<RemoveSelection>().index = p;
        }
    }
}
