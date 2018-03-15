using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Path
{

    public List<Vector3Int> moves = new List<Vector3Int>();
    public List<bool> reversedMoves = new List<bool>();
    public Vector3Int startpoint;

    public Path(Vector3Int pStartpoint)
    {
        startpoint = pStartpoint;
    }

    //Adds a move to queue
    public void Add(Vector3Int pNewMove)
    {
        moves.Add(pNewMove);
        reversedMoves.Add(false);
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

    //Breaks down a Vector3Int in indivual subvectors with a magnitude of 1 and returns them as an array
    public Vector3Int[] BreakDownVectorReversed(Vector3Int pV)
    {
        int count = GetLength(pV);
        int index = 0;
        int sizeX = Mathf.Abs(pV.x);
        int sizeY = Mathf.Abs(pV.y);
        Vector3Int[] subVectors = new Vector3Int[count];
        Vector3Int temp = new Vector3Int(0, 0, 0);
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
        return subVectors;
    }

    //Gets a tile position (cell-space) at a specific index in queue
    public Vector3Int GetPositionAtIndex(int pIndex)
    {
        Vector3Int tilePos = startpoint;
        int index = 0;
        int moveIndex = 0;
        foreach (Vector3Int v in moves)
        {
            Vector3Int[] subvectors;
            if (reversedMoves.ToArray()[moveIndex])
            {
                subvectors = BreakDownVectorReversed(v);
            }
            else
            {
                subvectors = BreakDownVector(v);
            }
            for (int i = 0; i < subvectors.Length; i++)
            {
                if (index > pIndex)
                {
                    return tilePos;
                }
                tilePos += subvectors[i];
                index++;
            }
            moveIndex++;
        }
        return tilePos;
    }

    //Returns the position of all tiles in path
    public Vector3Int[] GetAllPositions()
    {
        Vector3Int[] positions = new Vector3Int[GetLength()];
        Vector3Int tilePos = startpoint;
        int index = 0;
        int moveIndex = 0;
        foreach (Vector3Int v in moves)
        {
            Vector3Int[] subvectors;
            if (reversedMoves.ToArray()[moveIndex])
            {
                subvectors = BreakDownVectorReversed(v);
            }
            else
            {
                subvectors = BreakDownVector(v);
            }
            for (int i = 0; i < subvectors.Length; i++)
            {
                tilePos += subvectors[i];
                positions[index] = tilePos;
                index++;
            }
            moveIndex++;
        }
        return positions;
    }

    //Returns the position of all tiles in path
    public Vector3Int[] GetAllSubVectors()
    {
        Vector3Int[] positions = new Vector3Int[GetLength()];
        Vector3Int tilePos = new Vector3Int(0, 0, 0);
        int index = 0;
        int moveIndex = 0;
        foreach (Vector3Int v in moves)
        {
            Vector3Int[] subvectors;
            if (reversedMoves.ToArray()[moveIndex])
            {
                subvectors = BreakDownVectorReversed(v);
            }
            else
            {
                subvectors = BreakDownVector(v);
            }
            for (int i = 0; i < subvectors.Length; i++)
            {
                tilePos = subvectors[i];
                positions[index] = tilePos;
                index++;
            }
            moveIndex++;
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

    public static PathEngine Instance;

    [SerializeField]
    private TileManager tileManager;

    [SerializeField]
    private TileBase[] arrowStraight;

    [SerializeField]
    private TileBase[] arrowCorner;

    [SerializeField]
    private TileBase[] arrowStraight_End;

    [SerializeField]
    private TileBase[] arrowCorner_End;

    void Awake()
    {
        Instance = this;   
    }

    void Start()
    { 
        /*  
        Path path = new Path(RoundManager.currentUnit.GetPos());
        path.Add(new Vector3Int(0, 1, 0));
        path.Add(new Vector3Int(-2, 0, 0));
        path.Add(new Vector3Int(0, -2, 0));
        DisplayPath(path, RoundManager.currentUnit);
        */
    }

    public Path GetPath(Unit pUnit, Vector3Int pLoc)
    {
        Path path = null;
        return path;
    }

    public bool GeneratePathToLocation(Unit pUnit, Vector3Int pLoc)
    {
        Vector3Int unitPos = pUnit.GetPos();
        Vector3Int move1 = pLoc - unitPos;
        Path path = new Path(unitPos);
        path.Add(move1);
        if (!CheckPath(path, pUnit))
        {
            path.reversedMoves[0] = true;
            if (!CheckPath(path, pUnit))
            {
                return false;
            }
        }
        return true;
    }

    public Path GetPathToLocation(Unit pUnit, Vector3Int pLoc)
    {
        Vector3Int unitPos = pUnit.GetPos();
        Vector3Int move1 = pLoc - unitPos;
        Path path = new Path(unitPos);
        path.Add(move1);
        if (!CheckPath(path, pUnit))
        {
            path.reversedMoves[0] = true;
        }
        return path;
    }

    public bool CheckPath(Path pPath, Unit pUnit)
    {
        Vector3Int[] positions = pPath.GetAllPositions();
        foreach (Vector3Int pos in positions)
        {
            if (!pUnit.TileValidForUnit(pos))
            {
                return false;
            }
        }
        int turns = pUnit.turns;
        int length = pPath.GetLength();
        if (turns < length)
        {
            return false;
        }
        return true;
    }

    #region Displaying a path with arrows   
    public void DisplayPath(Path pPath, Unit pUnit)
    {
        //Clear arrows
        tileManager.arrowMap.ClearAllTiles();

        Vector3Int[] subvectors = pPath.GetAllSubVectors();

        if (subvectors.Length == 0)
        {
            return;
        }
        
        Vector3Int currentMove;
        Vector3Int nextMove = subvectors[0];
        Vector3Int tilePos = pUnit.GetPos();
        bool isEnd = false;
        for (int i=0; i < subvectors.Length; i++)
        {
            currentMove = subvectors[i];
            if (i + 1 != subvectors.Length)
            {
                nextMove = subvectors[i+1];
            }
            else
            {
                isEnd = true;
            }
            //Set tile
            tilePos += subvectors[i];
            TileBase arrowTile = GetArrow(currentMove, nextMove, isEnd);
            tileManager.arrowMap.SetTile(tilePos, arrowTile);
        }
    }

    public TileBase GetArrow(Vector3Int pCurrent, Vector3Int pNext, bool isEnd)
    {
        TileBase tile = null;

        //Directional vectors
        Vector3Int left = new Vector3Int(-1, 0, 0);
        Vector3Int right = new Vector3Int(1, 0, 0);
        Vector3Int up = new Vector3Int(0, 1, 0);
        Vector3Int down = new Vector3Int(0, -1, 0);

        //Vectors are orthogonal --> Corner
        if (pCurrent * pNext == Vector3Int.zero)
        {
            if (pCurrent == left && pNext == up)
            {
                if (isEnd)
                {
                    tile = arrowCorner_End[0];
                }
                tile = arrowCorner[0];
            }
            else if (pCurrent == down && pNext == right)
            {
                if (isEnd)
                {
                    tile = arrowCorner_End[5];
                }
                tile = arrowCorner[0];
            }

            else if (pCurrent == up && pNext == right)
            {
                if (isEnd)
                {
                    tile = arrowCorner_End[1];
                }
                tile = arrowCorner[1];
            }
            else if (pCurrent == left && pNext == down)
            {
                if (isEnd)
                {
                    tile = arrowCorner_End[6];
                }
                tile = arrowCorner[1];
            }

            else if (pCurrent == right && pNext == down)
            {
                if (isEnd)
                {
                    tile = arrowCorner_End[2];
                }
                tile = arrowCorner[2];
            }
            else if (pCurrent == up && pNext == left)
            {
                if (isEnd)
                {
                    tile = arrowCorner_End[7];
                }
                tile = arrowCorner[2];
            }

            else if (pCurrent == right && pNext == up)
            {
                if (isEnd)
                {
                    tile = arrowCorner_End[4];
                }
                tile = arrowCorner[3];
            }
            else if (pCurrent == down && pNext == left)
            {
                if (isEnd)
                {
                    tile = arrowCorner_End[3];
                }
                tile = arrowCorner[3];
            }
        }
        //--> Straight
        else
        {
            if (isEnd)
            {
                if (pNext == up)
                {
                    tile = arrowStraight_End[0];
                }
                else if (pNext == right)
                {
                    tile = arrowStraight_End[1];
                }
                else if (pNext == down)
                {
                    tile = arrowStraight_End[2];
                }
                else if (pNext == left)
                {
                    tile = arrowStraight_End[3];
                }
            }
            else
            {
                //Horizontal
                if (pCurrent.x != 0)
                {
                    tile = arrowStraight[0];
                }
                //Vertical
                else if (pCurrent.y != 0)
                {
                    tile = arrowStraight[1];
                }
            }
        }

        return tile;
    }
    #endregion
}
