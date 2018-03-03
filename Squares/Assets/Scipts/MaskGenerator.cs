using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskGenerator: MonoBehaviour{

    [SerializeField]
    private TileManager tileManager;

    [SerializeField]
    private PathEngine pathEngine;

    Unit unit;

    private int unitTurns;

    int[,] maskGreen;

    public void GenerateMask(Unit pUnit)
    {
        unit = pUnit;

        //Setup array containing only 0
        unitTurns = unit.turns;
        int range = unitTurns * 2 + 1;
        print(range);
        maskGreen = new int[range, range];
        for (int x=0; x<range; x++)
        {
            for (int y = 0; y < range; y++)
            {
                maskGreen[x,y] = 1;
            }
        }

        //Get pivot point and set it to 0
        maskGreen[unit.turns, unit.turns] = 0;

        //Set points to zero where no tile is
        for (int x = 0; x < range; x++)
        {
            for (int y = 0; y < range; y++)
            {
                if (tileManager.CheckForTile(CalTile(x,y),null))
                {
                    maskGreen[x, y] = 0;
                }
            }
        }

        //Check if points are no 'null'
        for (int x = 0; x < range; x++)
        {
            for (int y = 0; y < range; y++)
            {
                if (!unit.TileValidForUnit(CalTile(x, y)))
                {
                    maskGreen[x, y] = 0;
                }
            }
        }

        //Check paths --> PathEngine
        for (int x = 0; x < range; x++)
        {
            for (int y = 0; y < range; y++)
            {
                if (!pathEngine.GeneratePathToLocation(pUnit,CalTile(x,y)))
                {
                    maskGreen[x, y] = 0;
                }
            }
        }

        printMask(maskGreen);

        applyMask(maskGreen);
    }

    //Converts tile position to its actual position in cell-space
    public Vector3Int CalTile(int pX, int pY)
    {
        Vector3Int unitPos = unit.GetPos();
        Vector3Int center = new Vector3Int(unitTurns, unitTurns, 0);
        Vector3Int newPos = new Vector3Int(pX, pY, 0);
        Vector3Int dir = newPos - center;

        Vector3Int pos = unitPos + dir;
        return pos;
    }

    //Prints a mask in console
    public void printMask(int[,] pMask)
    {
        string temp = "";
        for (int x = 0; x < pMask.GetLength(0); x++)
        {
            for (int y = 0; y < pMask.GetLength(1); y++)
            {
                temp += pMask[x, y] + ", ";
            }
            temp += "\n";
        }
        print(temp);
    }

    //Applys a mask
    public void applyMask(int[,] pMask)
    {
        tileManager.masksMap.ClearAllTiles();
        for (int x = 0; x < pMask.GetLength(0); x++)
        {
            for (int y = 0; y < pMask.GetLength(1); y++)
            {
                if (pMask[x,y] == 1)
                {
                    tileManager.masksMap.SetTile(CalTile(x,y), tileManager.greenMask);
                }
             }
        }
    }
}
