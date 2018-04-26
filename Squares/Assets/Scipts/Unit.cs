using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public enum CharClasses
{
    Archer,
    Samurai,
    Spearmen,
    Shogun,
    Tank,
    Horse,
    Horseman,
    Hunter,
    Swordsman
};

[System.Serializable]
public class AttackPattern
{
    public List<Vector3Int> positions = new List<Vector3Int>();
}

public class Unit : MonoBehaviour {

    public BuffManager buffs;

    public GameObject healthBar;

    [SerializeField]
    private TileManager tileManager;

    [SerializeField]
    private MaskGenerator maskGen;

    [SerializeField]
    private GameObject selection;

    [SerializeField]
    private SpriteRenderer spriteRend;

    [SerializeField]
    private Animator anim;

    [HideInInspector]
    public Team teamName;
    [HideInInspector]
    public List<Unit> team = new List<Unit>();

    [Header("General")]

    public Color cantActColor;

    public bool canAct = true;
    public bool isDead;

    public float animWalkSpeed;

    [Header("Class Attributes")]

    public CharClasses unitClass;

    public TileBase[] blockingTiles;
    public AttackPattern[] attackPattern;

    public bool canMoveAfterAttack = false;

    public string describtion;

    [Header("Stats")]
    public int health;
    public int maxHealth;

    public int turns, maxTurns;

    public int walked, attacked;

    public int attacks, maxAttacks;

    public int atk;

    public int def;

    [Range(-3, 3)]
    public int moral;

    void Awake()
    {
        buffs = new BuffManager(this);
    }

    // Use this for initialization
    void Start () {
        tileManager = TileManager.Instance;
        maskGen = MaskGenerator.Instance;
    }
	
	// Update is called once per frame
	void Update () {

        //Check health
        if (health <= 0 && !isDead)
        {
            isDead = true;
            Die();
        }

        //Cant act --> color change
        if (!canAct && team == RoundManager.currentTeam)
        {
            if (anim != null)
            {
                anim.enabled = false;
                spriteRend.color = cantActColor;
            }
            else
            {
                spriteRend.color = cantActColor;
            }
        }
        else
        {
            if (anim != null)
            {
                anim.enabled = true;
            }
            spriteRend.color = Color.white;
        }

        //Moral De-/Buffs
        switch (moral)
        {
            //Huge debuff
            case -3:
                buffs.AddBuff(Buff.HugeMoralDebuff);
                buffs.RemoveBuff(Buff.SmallMoralBuff);
                buffs.RemoveBuff(Buff.HugeMoralBuff);
                buffs.RemoveBuff(Buff.SmallMoralDebuff);
                break;
            
            //Small debuff
            case -2:
                buffs.AddBuff(Buff.SmallMoralDebuff);
                buffs.RemoveBuff(Buff.SmallMoralBuff);
                buffs.RemoveBuff(Buff.HugeMoralBuff);
                buffs.RemoveBuff(Buff.HugeMoralDebuff);
                break;
           
            //Small buff
            case 2:
                buffs.AddBuff(Buff.SmallMoralBuff);
                buffs.RemoveBuff(Buff.SmallMoralDebuff);
                buffs.RemoveBuff(Buff.HugeMoralBuff);
                buffs.RemoveBuff(Buff.HugeMoralDebuff);
                break;

            //Huge buff 
            case 3:
                buffs.AddBuff(Buff.HugeMoralBuff);
                buffs.RemoveBuff(Buff.SmallMoralDebuff);
                buffs.RemoveBuff(Buff.SmallMoralBuff);
                buffs.RemoveBuff(Buff.HugeMoralDebuff);
                break;

            //Nothing happens    
            default:
                buffs.RemoveBuff(Buff.SmallMoralBuff);
                buffs.RemoveBuff(Buff.HugeMoralBuff);
                buffs.RemoveBuff(Buff.SmallMoralDebuff);
                buffs.RemoveBuff(Buff.HugeMoralDebuff);
                break;
        }
    }

    public void SetCanAct()
    {
        if (turns == 0)
        {
            if (!MaskGenerator.Instance.CanAttack(this))
            {
                canAct = false;
                return;
            }
        }
        canAct = true;
    }

    public void WalkPath(Path pPath)
    {
        StartCoroutine(MoveObjectAlongPath(pPath, animWalkSpeed));
        turns -= pPath.GetLength();
        walked += pPath.GetLength();
    }

    #region Util

    //Move this unit over time
    IEnumerator MoveObject(Vector3 start, Vector3 target, float overTime)
    {
        PlayerActions.unitIsMoving = true;
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            transform.position = Vector3.Lerp(start, target, (Time.time - startTime) / overTime);
            yield return null;
        }
        transform.position = target;
        PlayerActions.unitIsMoving = false;
        //Generate new mask after unit is has moved
        maskGen.GenerateMasks(RoundManager.currentUnit);
    }

    //Move this unit over time
    IEnumerator MoveObjectAlongPath(Path pPath, float overTime)
    {
        PlayerActions.unitIsMoving = true;
        //Move on every subPath
        foreach(Vector3Int v in pPath.moves)
        {
            Vector3 start = transform.position;
            Vector3 destination = start + v;
            float startTime = Time.time;
            while (Time.time < startTime + overTime)
            {
                transform.position = Vector3.Lerp(start, destination, (Time.time - startTime) / overTime);
                yield return null;
            }
            transform.position = destination;
        }
        PlayerActions.unitIsMoving = false;
        tileManager.arrowMap.ClearAllTiles();
        //Generate new mask after unit is has moved
        maskGen.GenerateMasks(RoundManager.currentUnit);
        SetCanAct();
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
        else
        {
            Destroy(gameObject);
        }
    }

    //Triggered if health equals zero
    public void DieAfterAnim()
    {
        Destroy(gameObject);
    }

    public void Attack(Vector3Int pos)
    {
        List<Vector3Int> pattern = GetPartnerAP(pos);
        foreach (Vector3Int attackDir in pattern)
        {
            Vector3Int checkPos = GetPos() + attackDir;
            if (tileManager.CheckForUnit(checkPos))
            {
                Attack(tileManager.GetUnitAtPosition(checkPos));
            }
        }
        attacks -= 1;
        attacked += 1;
        if (!canMoveAfterAttack)
        {
            turns = 0;
            walked = maxTurns;
        }
        MaskGenerator.Instance.GenerateMasks(RoundManager.currentUnit);
        TileManager.Instance.arrowMap.ClearAllTiles();
        SetCanAct();
    }

    //Attack other unit (calculate and apply damage)
    void Attack(Unit pTarget)
    {
        if(attacks <= 0)
        {
            return;
        }
        if (pTarget.GetComponent<Animator>() != null)
        {
            pTarget.GetComponent<Animator>().Play("TakeDamage");
        }
        pTarget.health -= atk;
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

    //Gets all attackable positions
    public List<Vector3Int> GetAttackPositions()
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        foreach (AttackPattern ap in attackPattern)
        {
            foreach (Vector3Int v in ap.positions)
            {
                positions.Add(v);
            }
        }
        return positions;
    }

    public List<Vector3Int> GetPartnerAP(Vector3Int pos)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        foreach (AttackPattern ap in attackPattern)
        {
            foreach (Vector3Int v in ap.positions)
            {
                Vector3Int x = pos - GetPos();
                if (v == x)
                {
                    return ap.positions;
                }
            }
        }
        return null;
    }
}
