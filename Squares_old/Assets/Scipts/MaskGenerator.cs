using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class MaskGenerator: MonoBehaviour{

    [SerializeField]
    private TileManager tileManager;

    public static MaskGenerator Instance;

    private int unitTurns;

    List<Vector3Int> maskGreen = new List<Vector3Int>();
    List<Vector3Int> maskRed = new List<Vector3Int>();
    List<Vector3Int> toRemove = new List<Vector3Int>();

    void Awake()
    {
        Instance = this;
    }

    //Generates a green and red mask for pUnit and applies them to the mask layer
    public void GenerateMask(Unit pUnit)
    {
        maskGreen.Clear();
        maskRed.Clear();
        int turns = pUnit.turns;
        Vector3Int center = pUnit.GetPos();

        #region MaskGreen - Setup
        //Setup list containing positions relative to center
        for (int x = 0; x <= turns; x++)
        {
            for (int y = 0; y <= turns - x; y++)
            {
                Vector3Int v1 = center + new Vector3Int(x, y, 0);
                Vector3Int v2 = center + new Vector3Int(x, -y, 0);
                Vector3Int v3 = center + new Vector3Int(-y, x, 0);
                Vector3Int v4 = center + new Vector3Int(-y, -x, 0);

                if (!maskGreen.Contains(v1))
                {
                    maskGreen.Add(v1);
                }
                if (!maskGreen.Contains(v2))
                {
                    maskGreen.Add(v2);
                }
                if (!maskGreen.Contains(v3))
                {
                    maskGreen.Add(v3);
                }
                if (!maskGreen.Contains(v4))
                {
                    maskGreen.Add(v4);
                }
            }
        }
        maskGreen.Remove(center);
        #endregion

        #region Filter MaskGreen
        //Remove all positions that pUnit isnt allowed to step on
        toRemove.Clear();
        for (int i = 0; i < maskGreen.Count; i++)
        {
            if (!pUnit.TileValidForUnit(maskGreen[i]))
            {
                toRemove.Add(maskGreen[i]);
            }
        }
        foreach (Vector3Int v in toRemove)
        {
            maskGreen.Remove(v);
        }
        #endregion

        #region CheckPaths for MaskGreen
        //Check paths --> PathEngine
        toRemove.Clear();
        for (int i = 0; i<maskGreen.Count; i++)
        {
            Path path = PathEngine.Instance.GetPath(pUnit, maskGreen[i]);
            if (path == null)
            {
                toRemove.Add(maskGreen[i]);
            }
            else if(path.GetLength() > pUnit.turns)
            {
                toRemove.Add(maskGreen[i]);
            }
        }
        foreach (Vector3Int v in toRemove)
        {
            maskGreen.Remove(v);
        }
        #endregion

        #region Calculate RedMask
        if (pUnit.attacks > 0)
        {
            //Get attack positions
            List<Vector3Int> attackPositions;
            //Attack positions in maskGreen
            foreach (Vector3Int v in maskGreen)
            {
                attackPositions = GetAttackPositions(pUnit, v);
                if (attackPositions != null)
                {
                    foreach (Vector3Int pos in attackPositions)
                    {
                        maskRed.Add(pos);
                    }
                }
            }
            //Attack positions at unit position
            attackPositions = GetAttackPositions(pUnit, pUnit.GetPos());
            if (attackPositions != null)
            {
                foreach (Vector3Int pos in attackPositions)
                {
                    maskRed.Add(pos);
                }
            }
        }
        #endregion

        //Apply masks
        tileManager.masksMap.ClearAllTiles();
        ApplyMask(maskGreen, tileManager.greenMask);
        ApplyMask(maskRed, tileManager.redMask);
    }

    //Returns the possible positions as a list
    public List<Vector3Int> GetAttackPositions(Unit pUnit, Vector3Int pPos)
    {
        List<Vector3Int> maskPositions = new List<Vector3Int>();
        Vector3Int[] attackPositions = pUnit.attackPositions;
        foreach (Vector3Int v in attackPositions)
        {
            Vector3Int pos = pPos + v;
            if (tileManager.CheckForUnit(pos))
            {
                Unit unit = tileManager.GetUnitAtPosition(pos);
                if (pUnit.team != unit.team)
                {
                    maskPositions.Add(pos);
                }
            }
        }
        return maskPositions;
    }

    //Applys a mask
    public void ApplyMask(List<Vector3Int> pMask, TileBase pMasktile)
    {
        foreach (Vector3Int v in pMask)
        {
            tileManager.masksMap.SetTile(v, pMasktile);
        }  
    }
}
