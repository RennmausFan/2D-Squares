using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System.Linq;

public class MaskGenerator: MonoBehaviour{

    [SerializeField]
    private TileManager tileManager;

    public static MaskGenerator Instance;

    public TileBase[] toIgnorePurple;

    Unit unit;

    void Awake()
    {
        Instance = this;
    }

    //Generates mask and applies them (for a single unit)
    public void GenerateMasks(Unit pUnit)
    {
        unit = pUnit;

        //Clear maskmaps
        tileManager.masksMap.ClearAllTiles();
        tileManager.pinkMaskMap.ClearAllTiles();

        List<Vector3Int> maskGreen = GenerateMaskGreen();

        if (UIManager.showAttackPattern == true)
        {
            List<Vector3Int> maskPink = GenerateMaskPink();
            ApplyMask(maskPink, tileManager.pinkMask, tileManager.pinkMaskMap);
        }

        List<Vector3Int> maskRed = GenerateMaskRed(maskGreen);

        //Apply masks
        ApplyMask(maskGreen, tileManager.greenMask, tileManager.masksMap);
        ApplyMask(maskRed, tileManager.redMask, tileManager.masksMap);
    }

    //Generates the danger zone for the current team
    public void GenerateDangerZone()
    {
        List<Unit> units;
        if (RoundManager.currentTeam == UnitManager.allies)
        {
            units = UnitManager.enemies;
        }
        else
        {
            units = UnitManager.allies;
        }
        tileManager.purpleMaskMap.ClearAllTiles();
        foreach (Unit u in units)
        {
            GenerateMaskPurple(u);
        }
    }

    //Displays walkable tiles
    List<Vector3Int> GenerateMaskGreen()
    {
        List<Vector3Int> maskGreen = GetDiamondPattern(unit.GetPos(), unit.turns, true);
        List<Vector3Int> toRemove = new List<Vector3Int>();

        #region Remove all positions that the unit isnt allowed to step on
        for (int i = 0; i < maskGreen.Count; i++)
        {
            if (!unit.TileValidForUnit(maskGreen[i]))
            {
                toRemove.Add(maskGreen[i]);
            }
        }
        foreach (Vector3Int v in toRemove)
        {
            maskGreen.Remove(v);
        }
        #endregion

        #region Check paths --> PathEngine
        toRemove.Clear();
        for (int i = 0; i < maskGreen.Count; i++)
        {
            Path path = PathEngine.Instance.GetPath(unit, maskGreen[i]);
            if (path == null)
            {
                toRemove.Add(maskGreen[i]);
            }
            else if (path.GetLength() > unit.turns)
            {
                toRemove.Add(maskGreen[i]);
            }
        }
        foreach (Vector3Int v in toRemove)
        {
            maskGreen.Remove(v);
        }
        #endregion

        return maskGreen;
    }

    //Calculate mask green with the max turns of each unit
    List<Vector3Int> GenerateMaskGreen_MaxTurns()
    {
        int turns = unit.turns;
        unit.turns = unit.maxTurns;
        List<Vector3Int> greenMask = GenerateMaskGreen();
        unit.turns = turns;
        return greenMask;
    }

    //Displays the attack pattern of the unit at its current position
    List<Vector3Int> GenerateMaskPink()
    {
        List<Vector3Int> maskPink = new List<Vector3Int>();
        List<Vector3Int> attackPositions = unit.GetAttackPositions();
        Vector3Int unitPos = unit.GetPos();
        foreach (Vector3Int v in attackPositions)
        {
            Vector3Int pos = unitPos + v;
            maskPink.Add(pos);
        }
        return maskPink;
    }

    //Calulcates actual/possible attackpositions (also the ones requirering moving)
    List<Vector3Int> GenerateMaskRed(List<Vector3Int> maskGreen)
    {
        List<Vector3Int> maskRed = new List<Vector3Int>();
        if (unit.attacks > 0)
        {
            List<Vector3Int> attackPositions;
            //Attack positions in maskGreen
            foreach (Vector3Int v in maskGreen)
            {
                attackPositions = GetAttackPositions(unit, v);
                if (attackPositions != null)
                {
                    foreach (Vector3Int pos in attackPositions)
                    {
                        maskRed.Add(pos);
                    }
                }
            }
            //Attack positions at unit position
            attackPositions = GetAttackPositions(unit, unit.GetPos());
            if (attackPositions != null)
            {
                foreach (Vector3Int pos in attackPositions)
                {
                    maskRed.Add(pos);
                }
            }
        }
        return maskRed;
    }

    //Displays danger zone for a given unit
    public void GenerateMaskPurple(Unit pUnit)
    {
        unit = pUnit;
        //Based on Mask Green
        List<Vector3Int> maskGreen = GenerateMaskGreen_MaxTurns();
        maskGreen.Add(unit.GetPos());
        List<Vector3Int> maskPurple = new List<Vector3Int>();
        List<Vector3Int> attackPositions = unit.GetAttackPositions();
        foreach (Vector3Int v in maskGreen)
        {
            foreach (Vector3Int ap in attackPositions)
            {
                Vector3Int pos = v + ap;
                TileBase tile = tileManager.baseMap.GetTile(pos);
                if (!maskPurple.Contains(pos) && tileManager.baseMap.GetTile(pos) != null)
                {
                    if (!TileManager.ArrayContainsTile(tile, toIgnorePurple))
                    { 
                        maskPurple.Add(pos);
                    }
                }
            }
        }
        ApplyMask(maskPurple, tileManager.purpleMask, tileManager.purpleMaskMap);
    }

    //Generates and displays a fog mask for a given team
    public void GenerateFog(List<Unit> pTeam)
    {
        //FogMaskReverse contains every position where no fog should be displayed
        List<Vector3Int> fogMaskReverse = new List<Vector3Int>();
        Tilemap fogMap;
        //Combine the diamond pattern of each unit in given team
        foreach (Unit u in pTeam)
        {
            List<Vector3Int> positions = GetDiamondPattern(u.GetPos(), u.sightRange, false);
            fogMaskReverse = fogMaskReverse.Union(positions).ToList<Vector3Int>();
        }
        //Depending on the team choose a fog-TileMap and set them up (turn all tiles to fog)
        if (pTeam == UnitManager.allies)
        {
            fogMap = tileManager.fogAllies;
            tileManager.SetupFogBaseLayer(Team.Allies);
            tileManager.fogEnemies.gameObject.SetActive(false);
            tileManager.fogAllies.gameObject.SetActive(true);
        }
        else
        {
            fogMap = tileManager.fogEnemies;
            tileManager.SetupFogBaseLayer(Team.Enemies);
            tileManager.fogAllies.gameObject.SetActive(false);
            tileManager.fogEnemies.gameObject.SetActive(true);
        }
        //Remove where no fog should be
        ApplyMask(fogMaskReverse, null, fogMap);
        UnitManager.SetIsVisibleAllUnits();
    }

    #region Util

    //Gets available attack positions at a given positition for a given unit
    public List<Vector3Int> GetAttackPositions(Unit pUnit, Vector3Int pPos)
    {
        List<Vector3Int> maskPositions = new List<Vector3Int>();
        foreach (Vector3Int v in pUnit.GetAttackPositions())
        {
            Vector3Int pos = pPos + v;
            if (tileManager.CheckForUnit(pos))
            {
                Unit unit = tileManager.GetUnitAtPosition(pos);
                if (pUnit.team != unit.team && unit.isVisible)
                {
                    maskPositions.Add(pos);
                }
            }
        }
        return maskPositions;
    }

    //Calculates if the given unit can attack --> RedMask
    public bool CanAttack(Unit pUnit)
    {
        unit = pUnit;
        List<Vector3Int> maskRed = GenerateMaskRed(GenerateMaskGreen());
        if (maskRed.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Generates diamond shapes positions pattern for the given unut
    private List<Vector3Int> GetDiamondPattern(Vector3Int pCenter, int pRange, bool pRemoveCenter)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        Vector3Int center = pCenter;
        for (int x = 0; x <= pRange; x++)
        {
            for (int y = 0; y <= pRange - x; y++)
            {
                Vector3Int v1 = center + new Vector3Int(x, y, 0);
                Vector3Int v2 = center + new Vector3Int(x, -y, 0);
                Vector3Int v3 = center + new Vector3Int(-y, x, 0);
                Vector3Int v4 = center + new Vector3Int(-y, -x, 0);
                if (!positions.Contains(v1))
                {
                    positions.Add(v1);
                }
                if (!positions.Contains(v2))
                {
                    positions.Add(v2);
                }
                if (!positions.Contains(v3))
                {
                    positions.Add(v3);
                }
                if (!positions.Contains(v4))
                {
                    positions.Add(v4);
                }
            }
        }
        if (pRemoveCenter)
        {
            positions.Remove(center);
        }
        return positions;
    }

    //Applys a mask
    private void ApplyMask(List<Vector3Int> pMask, TileBase pMasktile, Tilemap pLayer)
    {
        foreach (Vector3Int v in pMask)
        {
            pLayer.SetTile(v, pMasktile);
        }  
    }

    #endregion
}
