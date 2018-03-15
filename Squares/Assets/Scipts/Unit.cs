using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public enum CharClasses
{
    Archer,
    Samurai,
    Shogun,
    Tank,
    Horse,
    Horseman,
    Hunter,
    Swordsman
};

public class Unit : MonoBehaviour {

    public GameObject healthBar;

    [SerializeField]
    private TileManager tileManager;

    [SerializeField]
    private MaskGenerator maskGen;

    [SerializeField]
    private GameObject selection;

    [Header("Class Attributes")]
    public TileBase[] blockingTiles;

    //Attack positions as Vector3Int relative to this unit
    public Vector3Int[] attackPositions;

    public List<Unit> team = new List<Unit>();

    public bool canAct;
    public bool isDead;

    public float animWalkSpeed;

    [Header("Stats")]
    public CharClasses unitClass;

    public int health, maxHealth;

    public int turns, maxTurns;

    public int attacks, maxAttacks;

    public int atk, atkBase;

    public int def, defBase;

    [Range(-3, 3)]
    public int moral;

    // Use this for initialization
    void Start () {
        //Assign this unit to team
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

        //Check health
        if (health <= 0 && !isDead)
        {
            isDead = true;
            Die();
        }
        
        //Set can act
        if (turns <= 0 && attacks <= 0)
        {
            canAct = false;
        }
        else
        {
            canAct = true;
        }
    }

    #region Movement

    public void Move(int pX, int pY)
    {
        Path path = new Path(GetPos());
        path.Add(new Vector3Int(pX, pY, 0));
        WalkPath(path);
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
        maskGen.GenerateMask(this);
        tileManager.arrowMap.ClearAllTiles();
    }

    public void WalkPath(Path pPath)
    {
        List<Vector3Int> moves = pPath.moves;
        foreach (Vector3Int v in moves)
        {
            Vector3 desiredPos = transform.position + v;
            StartCoroutine(MoveObject(transform.position, desiredPos, animWalkSpeed));
        }
        turns -= pPath.GetLength();
        tileManager.arrowMap.ClearAllTiles();
    }

    #endregion

    #region Util

    //Move this unit over time
    IEnumerator MoveObject(Vector3 start, Vector3 target, float overTime)
    {
        TileManager.unitIsMoving = true;
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            transform.position = Vector3.Lerp(start, target, (Time.time - startTime) / overTime);
            yield return null;
        }
        transform.position = target;
        TileManager.unitIsMoving = false;
        //Generate new mask after unit is has moved
        maskGen.GenerateMask(RoundManager.currentUnit);
    }

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

    //Triggered if health equals zero
    public void Die()
    {
        healthBar.SetActive(false);
        UnitManager.allUnits.Remove(this);
        team.Remove(this);
        if (GetComponent<Animator>() != null)
        {
            GetComponent<Animator>().Play("DeathFade");
        }
    }

    //Triggered if health equals zero
    public void DieAfterAnim()
    {
        Destroy(gameObject);
    }

    //Attack other unit (calculate and apply damage)
    public void Attack(Unit pTarget)
    {
        if(attacks <= 0)
        {
            return;
        }
        int damage = atk - pTarget.def;
        if (damage <= 0)
        {
            damage = 1;
        }

        if (pTarget.GetComponent<Animator>() != null)
        {
            pTarget.GetComponent<Animator>().Play("TakeDamage");
        }

        pTarget.health -= damage;
        attacks -= 1;
        maskGen.GenerateMask(RoundManager.currentUnit);
    }

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
