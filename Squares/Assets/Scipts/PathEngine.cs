using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;


//Allows to sort Nodes by their f value --> Needed for NodeList
public class NodeSorter : IComparer<Node>
{
    public int Compare(Node pA, Node pB)
    {
        if (pA.f > pB.f)
        {
            return 1;
        }
        else if (pB.f > pA.f)
        {
            return -1;
        }
        else
        {
            if (pA.h > pB.h)
            {
                return 1;
            }
            else if (pB.h > pA.h)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}

public class Path
{

    public List<Vector3Int> moves = new List<Vector3Int>();
    public Vector3Int startpoint;

    public Path(Vector3Int pStartpoint)
    {
        startpoint = pStartpoint;
    }

    //Adds a move to queue
    public void Add(Vector3Int pNewMove)
    {
        moves.Add(pNewMove);
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

    public Path MergeSubVectors()
    {
        Vector3Int lastV = Vector3Int.zero;
        Vector3Int combinedV = Vector3Int.zero;
        Path path = new Path(startpoint);
        bool isFirst = true;
        foreach (Vector3Int v in moves)
        {
            if (lastV == v)
            {
                combinedV += v;
            }
            else
            {
                if (isFirst)
                {
                    combinedV = v;
                    isFirst = false;
                }
                else
                {
                    path.Add(combinedV);
                    combinedV = v;
                }
            }
            lastV = v;
        }
        path.Add(combinedV);
        return path;
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
            subvectors = BreakDownVector(v);
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
            subvectors = BreakDownVector(v);
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
            subvectors = BreakDownVector(v);
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

    public void AddFirst(Vector3Int pMove)
    {
        moves.Insert(0, pMove);
    }
}

public class Node
{
    public Vector3Int pos;

    //Connection to the node before in this path
    public Node lastNode;

    //g represents the distance traveled so far (start --> this)
    public int g;

    //h represents the distance to the destination (approximately)
    public int h;

    //optinal costs or credit friction for a field
    public int optional;

    //The sum of g, h and optional
    public int f;

    //Is Node still relevant?
    public bool isClosed;

    public Node(Vector3Int pPos, Node pLastNode, Vector3Int pDestination, int pG)
    {
        pos = pPos;
        lastNode = pLastNode;

        g = pG;

        Vector3Int distance = pDestination - pos;
        h = Mathf.Abs(distance.x) + Mathf.Abs(distance.y);

        optional = 0;
         
        f = g + h + optional;
    }

    public Node(int pF, int pH)
    {
        f = pF;
        h = pH;
    }

    //Prints all values from this node
    public void Print()
    {
        string temp;
        if (lastNode == null)
        {
            temp = "None";
        }
        else
        {
            temp = lastNode.pos.ToString();
        }
        string text = "Pos: " + pos + ", g: " + g + ", last node pos: " + temp + ", h: " + h + ", optional: " + optional + ", f : " + f;
        MonoBehaviour.print(text);
    }

    public string GetText()
    {
        string temp;
        if (lastNode == null)
        {
            temp = "None";
        }
        else
        {
            temp = lastNode.pos.ToString();
        }
        string text = "Pos: " + pos + ", g: " + g + ", last node pos: " + temp + ", h: " + h + ", optional: " + optional + ", f : " + f;
        return text;
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

    List<Node> openList = new List<Node>();
    List<Node> closedList = new List<Node>();

    Node currentNode;

    Vector3Int destination;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Path path = GetPath(RoundManager.currentUnit, RoundManager.currentUnit.GetPos() + (new Vector3Int(3, 7, 0)));
        path.Print();
    }

    #region A* Algorithm

    //Get a path by calculating the shorthest path
    public Path GetPath(Unit pUnit, Vector3Int pTarget)
    {
        openList.Clear();
        closedList.Clear();
        destination = pTarget;

        Node a = new Node(10, 5);
        Node b = new Node(10, 6);
        print(new NodeSorter().Compare(a, b));

        print(pUnit.GetPos());
        print(destination);

        if (!pUnit.TileValidForUnit(destination))
        {
            print("PathEngine: Target position is not a valid tile for this unit!");
            return null;
        }

        Path path = new Path(pUnit.GetPos());

        //Create start and goal node
        Node goalNode = new Node(destination, null, destination, 0);
        Node startNode = new Node(pUnit.GetPos(), null, destination, 0);

        //Add the startNode to openlist and set it as currentNode
        openList.Add(startNode);
        currentNode = openList[0];

        int count = 0;
        while (currentNode.pos != destination)
        {
            count++;
            if (count >= 500)
            {
                openList.Sort(new NodeSorter());
                foreach (Node n in openList)
                {
                    print(n.pos + " " + n.f + " " + n.h + " " + n.g);
                }
                print("Infinity Loop!" + openList.Count);
                return null;
            }
            //Call NextNode() until the goalNode is the currentNode
            if (openList.Count == 0)
            {
                print("No Path foound --> Using Debug Path");
                return null;

            }
            NextNode(pUnit);
        }

        //Get path by revisiting shortest path
        while (currentNode.pos != startNode.pos)
        {
            Node node = currentNode;
            currentNode = currentNode.lastNode;
            Vector3Int walk = node.pos - currentNode.pos;
            path.AddFirst(walk);
        }
        print(count);
        return path.MergeSubVectors();
    }

    public void NextNode(Unit pUnit)
    {
        //Get Node with the smallest f value
        openList.Sort(new NodeSorter());
        currentNode = openList[0];
        print(currentNode.pos + " " + currentNode.f + " " + currentNode.h + " " + currentNode.g);
        int g = currentNode.g + 1;

        //Remove the currentNode and add it to the closedList
        openList.Remove(currentNode);
        currentNode.isClosed = true;
        closedList.Add(currentNode);

        //Neighbor tiles
        Node leftNode = new Node(currentNode.pos + Vector3Int.left, currentNode, destination, g);
        Node rightNode = new Node(currentNode.pos + Vector3Int.right, currentNode, destination, g);
        Node upNode = new Node(currentNode.pos + Vector3Int.up, currentNode, destination, g);
        Node downNode = new Node(currentNode.pos + Vector3Int.down, currentNode, destination, g);

        AddNote(leftNode, pUnit);
        AddNote(rightNode, pUnit);
        AddNote(upNode, pUnit);
        AddNote(downNode, pUnit);
    }

    public void AddNote(Node toAdd, Unit pUnit)
    {
        if (pUnit.TileValidForUnit(toAdd.pos))
        {
            foreach (Node n in closedList)
            {
                //If to nodes are the same
                if (n.pos == toAdd.pos)
                {
                    //Only add toAdd if the new node is better (smaller f / h value)
                    if (new NodeSorter().Compare(toAdd, n) == -1)
                    {
                        openList.Add(toAdd);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            openList.Add(toAdd);
        }
    }

    #endregion

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
        Vector3Int left = Vector3Int.left;
        Vector3Int right = Vector3Int.right;
        Vector3Int up = Vector3Int.up;
        Vector3Int down = Vector3Int.down;

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
