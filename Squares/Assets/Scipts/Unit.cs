using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Unit : MonoBehaviour {

    [SerializeField]
    private TileManager tileManager;

    [SerializeField]
    private MaskGenerator maskGen;

    [SerializeField]
    private GameObject selection;

    public TileBase[] blockingTiles;
    
    public int health, maxHealth;

    public int turns, maxTurns;

    public bool canAct;

    public int atk, def;

    public LinkedList<Unit> team = new LinkedList<Unit>();

    // Use this for initialization
    void Start () {
		if (tag == "Ally")
        {
            team = UnitManager.allies;
        }
        else if (tag == "Enemy")
        {
            team = UnitManager.enemies;
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (health <= 0)
        {
            UnitManager.allUnits.Remove(this);
            team.Remove(this);
            Destroy(gameObject);
        }

        if (turns <= 0)
        {
            canAct = false;
        }
        else
        {
            canAct = true;
        }

        if (RoundManager.currentUnit != this)
        {
            ResetHighlight();
            return;
        }
        Highlight();
		if (Input.GetKeyDown(KeyCode.A))
        {
            Move(-1, 0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Move(1, 0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Move(0, -1);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Move(0, 1);
        }
    }

    public void Move(int pX, int pY)
    {
        Vector3Int move = new Vector3Int(pX, pY, 0);
        Vector3Int unitPos = tileManager.baseMap.WorldToCell(transform.position);
        Vector3 dest = tileManager.baseMap.GetCellCenterWorld(unitPos + move);

        Vector3Int checkTile = unitPos + move;

        int distance = CalDistance(unitPos, checkTile);
        //Returns if not enough turns
        if (distance > turns)
        {
            return;
        }
        if (!TileValidForUnit(checkTile))
        {
            return;
        }

        transform.position = dest;
        turns -= distance;
        maskGen.GenerateMask(this);
    }

    public void MoveTo(int pX, int pY)
    {
        Vector3Int moveTo = new Vector3Int(pX, pY, 0);
        Vector3Int unitPos = tileManager.baseMap.WorldToCell(transform.position);

        int distance = CalDistance(unitPos, moveTo);
        //Returns if not enough turns
        if (distance > turns)
        {
            return;
        }
        if (!TileValidForUnit(moveTo))
        {
            return;
        }

        Vector3 dest = tileManager.baseMap.GetCellCenterWorld(moveTo);
        transform.position = dest;
        turns -= distance;
    }

    #region Util
    //Return true if Unit is allowed to stand on the tile at the given position
    public bool TileValidForUnit(Vector3Int pPos)
    {
        //Returns if blocking tile
        if (tileManager.CheckForTiles(pPos, blockingTiles))
        {
            return false;
        }
        //Returns if there is no tile
        if (tileManager.CheckForTile(pPos, null))
        {
            return false;
        }
        //Returns if there is already a unit
        if (tileManager.CheckForUnit(pPos))
        {
            return false;
        }
        return true;
    }

    //Calculates the turns needed to move from 'A' to 'B'
    public static int CalDistance (Vector3Int A, Vector3Int B)
    {
        int d = 0;
        d += Mathf.Abs(B.x - A.x);
        d += Mathf.Abs(B.y - A.y);
        d += Mathf.Abs(B.z - A.z);
        return d;
    }

    //Gets the position of the unit in cell-space
    public Vector3Int GetPos()
    {
        Vector3Int pos = tileManager.baseMap.WorldToCell(transform.position);
        return pos;
    }
    #endregion

    //Activate selection
    public void Highlight()
    {
        selection.SetActive(true);    
    }

    //Deaktivate selection
    public void ResetHighlight()
    {
        selection.SetActive(false);
    }
}
