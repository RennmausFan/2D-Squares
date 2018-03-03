using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Path
{
    public Queue<Vector3Int> moves;
    public Vector3Int startpoint;

    public Path(Vector3Int pStartpoint)
    {
        startpoint = pStartpoint;
        moves = new Queue<Vector3Int>();
    }

    //Adds a move to queue
    public void Add(Vector3Int pNewMove)
    {
        moves.Enqueue(pNewMove);
    }

    //Returns the length of all vectors in queue
    public int GetLength()
    {
        int l = 0;
        foreach (Vector3Int v in moves)
        {
            l += Mathf.Abs(v.x);
            l += Mathf.Abs(v.y);
        }
        return l;
    }

    //Returns the length of a Vector3Int
    public int GetLength(Vector3Int pMove)
    {
        int l = 0;
        l += Mathf.Abs(pMove.x);
        l += Mathf.Abs(pMove.y);
        return l;
    }

    //Breaks down a Vector3Int in indivual subvectors with a magnitude of 1 and returns them as an array
    public Vector3Int[] BreakDownVector(Vector3Int pV)
    {
        int count = GetLength(pV);
        int index = 0;
        int sizeX = Mathf.Abs(pV.x);
        int sizeY = Mathf.Abs(pV.y);
        Vector3Int[] subVectors = new Vector3Int[count];
        Vector3Int temp = new Vector3Int(0, 0, 0);
        for (int x = 0; x < sizeX; x++)
        {
            if (pV.x < 0)
            {
                temp = new Vector3Int(-1, 0, 0);
            }
            else
            {
                temp = new Vector3Int(1, 0, 0);
            }
            subVectors[index] = temp;
            index++;
        }
        for (int y = 0; y < sizeY; y++)
        {
            if (pV.y < 0)
            {
                temp = new Vector3Int(0, -1, 0);
            }
            else
            {
                temp = new Vector3Int(0, 1, 0);
            }
            subVectors[index] = temp;
            index++;
        }
        return subVectors;
    }

    //Gets a tile position (cell-space) at a specific index in queue
    public Vector3Int GetPositionAtIndex(int pIndex)
    {
        Vector3Int tilePos = startpoint;
        int index = 0;
        foreach (Vector3Int v in moves)
        {
            Vector3Int[] subvectors = BreakDownVector(v);
            for (int i = 0; i < subvectors.Length; i++)
            {
                if (index > pIndex)
                {
                    return tilePos;
                }
                tilePos += subvectors[i];
                index++;
            }
        }
        return tilePos;
    }

    //Returns the position of all tiles in path
    public Vector3Int[] GetAllPositions()
    {
        Vector3Int[] positions = new Vector3Int[GetLength()];
        Vector3Int tilePos = startpoint;
        int index = 0;
        foreach (Vector3Int v in moves)
        {
            Vector3Int[] subvectors = BreakDownVector(v);
            for (int i = 0; i < subvectors.Length; i++)
            {
                tilePos += subvectors[i];
                positions[index] = tilePos;
                index++;
            }
        }
        return positions;
    }

    //Returns the position of all tiles in path
    public Vector3Int[] GetAllSubVectors()
    {
        Vector3Int[] positions = new Vector3Int[GetLength()];
        Vector3Int tilePos = new Vector3Int(0, 0, 0);
        int index = 0;
        foreach (Vector3Int v in moves)
        {
            Vector3Int[] subvectors = BreakDownVector(v);
            for (int i = 0; i < subvectors.Length; i++)
            {
                tilePos = subvectors[i];
                positions[index] = tilePos;
                index++;
            }
        }
        return positions;
    }

    //Prints the queue with its length
    public void Print()
    {
        string temp = "";
        foreach (Vector3Int v in moves)
        {
            temp +=  "( " + v.x + " / " + v.y + " )" + " ( " + GetLength(v) + " )\n";
        }
        temp += "Length: " + GetLength();
        MonoBehaviour.print(temp);
    }

    //Prints all subvectors with its length
    public void PrintPositions()
    {
        Vector3Int[] subvectors = GetAllSubVectors();
        Vector3Int[] positions = GetAllPositions();
        string temp = "";
        for (int i=0; i < positions.Length; i++)
        {
            Vector3Int v = positions[i];
            Vector3Int sub = subvectors[i];
            temp += "( " + v.x + " / " + v.y + " ) " + "(" + sub.x + " / " + sub.y + ")\n";
        }
        temp += "Length: " + GetLength();
        MonoBehaviour.print(temp);
    }
}

public class PathEngine : MonoBehaviour {

    [SerializeField]
    private TileManager tileManager;

    void Start()
    {
        Path path = new Path(RoundManager.currentUnit.GetPos());
        print(RoundManager.currentUnit.GetPos());
        path.Add(new Vector3Int(0, 1, 0));
        path.Add(new Vector3Int(-2, 0, 0));
        path.Add(new Vector3Int(0, -2, 0));
        path.Print();
        print(path.GetPositionAtIndex(5));
        CheckPath(path, RoundManager.currentUnit);
        PerformPath(path, RoundManager.currentUnit);
    }

    public bool GeneratePathToLocation(Unit pUnit, Vector3Int pLoc)
    {
        Vector3Int unitPos = pUnit.GetPos();
        Vector3Int move1 = pLoc - unitPos;
        Path path = new Path(unitPos);
        path.Add(move1);
        if (!CheckPath(path, pUnit))
        {
            return false;
        }
        return true;
    }

    public bool CheckPath(Path pPath, Unit pUnit)
    {
        pPath.PrintPositions();
        Vector3Int[] positions = pPath.GetAllPositions();
        foreach (Vector3Int pos in positions)
        {
            if (!pUnit.TileValidForUnit(pos))
            {
                print("Not-Valid");
                return false;
            }
        }
        print("Valid");
        return true;
    }

    public void PerformPath(Path pPath, Unit pUnit)
    {
        int turns = pUnit.turns;
        int length = pPath.GetLength();
        Vector3Int dest;
        if (turns < length)
        {
            dest = pPath.GetPositionAtIndex(turns-1);
            pUnit.turns = 0;
        }
        else
        {
            dest = pPath.GetPositionAtIndex(length-1);
            pUnit.turns = turns;
        }
        pUnit.transform.position = tileManager.baseMap.GetCellCenterWorld(dest);
    }

}
