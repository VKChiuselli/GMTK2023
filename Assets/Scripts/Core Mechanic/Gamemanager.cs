using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst;
    public PlayerInput inputActions;

    public Unit PlayerUnit;

    //TODO an automated way to populate this
    public List<Unit> Heroes;
    public List<Unit> Enemies;

    public Dictionary<Vector2Int, Tile> Grid = new();
    public float MovementSpeed = 10;
    public TurnId CurrentTurn = TurnId.Player;

    public enum TurnId
    {
        Player = 0,
        Hero = 1,
        Enemy = 2,
        Traps = 3
    }

    void Awake()
    {
        if (Inst == null)
            Inst = this;
        else if (Inst != this)
            Destroy(gameObject);

        inputActions = new PlayerInput();
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetUpGrid(new Vector2Int(-5, -5) ,new Vector2Int(5, 5));
    }

    private void OnEnable()
    {
        if (Inst == null)
            Inst = this;
        else if (Inst != this)
            Destroy(gameObject);
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        switch (CurrentTurn)
        {
            case TurnId.Player:
                PlayerControls();
                break;
            case TurnId.Hero:
                AIControls(Heroes);
                break;
            case TurnId.Enemy:
                AIControls(Enemies);
                break;
            case TurnId.Traps:
                CurrentTurn = TurnId.Player;
                break;
            default:
                break;
        }

        //Temp
        if (Input.GetKeyDown(KeyCode.Q))
            NextTurn();
    }

    void PlayerControls()
    {
        bool click = inputActions.Player.Click.ReadValue<float>() > 0;
        Vector2 position = inputActions.Player.MouseLocation.ReadValue<Vector2>();

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(position);

        if (click)
        {
            PlayerUnit.Move(mousePosition);
        }
    }

    void AIControls(List<Unit> units)
    {
        // Get the properties that each AI unit has


        // Temp movement
        bool click = inputActions.Player.Click.ReadValue<float>() > 0;
        Vector2 position = inputActions.Player.MouseLocation.ReadValue<Vector2>();

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(position);

        if (click)
        {
            StartCoroutine(MoveUnits(units, mousePosition));
        }
    }

    IEnumerator MoveUnits(List<Unit> units, Vector2 position)
    {
        foreach (var unit in units)
        {
            unit.Move(position);
            yield return new WaitForSeconds(0.05f);
        }
    }

    void NextTurn()
    {
        CurrentTurn++;
        if (CurrentTurn > TurnId.Traps)
            CurrentTurn = TurnId.Player;
    }

    public class Tile
    {
        public Vector2Int Position;
        public bool Walkable;
        public Tile Parent;
    }

    void SetUpGrid(Vector2Int topLeft, Vector2Int bottomRight)
    {
        for (int x = topLeft.x; x < bottomRight.x; x++) 
        {
            for (int y = topLeft.y; y < bottomRight.y; y++)
            {
                Vector2Int position = new Vector2Int(x, y);

                Grid.Add(position, new Tile() { Position = position, Walkable = IsWalkable(position)});
            }
        }
    }

    void UpdateGrid()
    {
        foreach(Vector2Int pos in Grid.Keys)
        {
            Grid[pos].Walkable = IsWalkable(pos);
        }
    }

    bool IsWalkable(Vector2Int position)
    {
        bool walkable = true;
        var collision = Physics2D.OverlapPoint(position);
        if (collision != null)
            walkable = false;
        return walkable;
    }

    int GetDistance(Tile a, Tile b)
    {
        // Returns Manhattan Distance
        return Mathf.Abs(a.Position.x - b.Position.x) + Mathf.Abs(a.Position.y - b.Position.y);
    }

    List<Vector2Int> RetracePath(Tile start, Tile end)
    {
        List<Vector2Int> path = new();
        Tile cur = end;

        while(cur != start)
        {
            path.Add(cur.Position);
            cur = cur.Parent;
        }

        path.Reverse();
        return path;
    }

    List<Tile> GetNeighbours(Tile cur)
    {
        List<Tile> neighbours = new();
        List<Vector2Int> positions = new List<Vector2Int>()
        {
            cur.Position + Vector2Int.up,
            cur.Position + Vector2Int.down,
            cur.Position + Vector2Int.left,
            cur.Position + Vector2Int.right
        };
        foreach(Vector2Int pos in positions)
        {
            if (Grid.ContainsKey(pos))
                neighbours.Add(Grid[pos]);
        }
        return neighbours;
    }

    public List<Vector2Int> FindPath(Vector2 startPos, Vector2 endPos, int maxDist)
    {
        return FindPath(Utility.Round(startPos), Utility.Round(endPos), maxDist);
    }

    // Breath first search
    public List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int endPos, int maxDist)
    {
        if (startPos == endPos)
            return null;

        if (!Grid.ContainsKey(endPos) || !Grid.ContainsKey(startPos))
            return null;

        UpdateGrid();

        Tile start = Grid[startPos];
        Tile end = Grid[endPos];

        // end must be set to walkable even if not, so that you can walk towards it
        bool isEndWalkable = end.Walkable;
        end.Walkable = true;

        List<Tile> queue = new List<Tile>();
        HashSet<Tile> visited = new HashSet<Tile>();
        queue.Add(start);

        // Fail safe for the loop at the end
        while (queue.Count > 0 && queue.Count < maxDist * maxDist * 4)
        {
            Tile cur = queue[0];
            queue.RemoveAt(0);
            visited.Add(cur);

            if (cur.Equals(end))
            {
                // If you can't walk to the end, walk one tile before
                if (!isEndWalkable)
                    end = end.Parent;
                if (end == start)
                    return null;
                return RetracePath(start, end);
            }

            List<Tile> neighbours = GetNeighbours(cur);
            foreach (Tile neighbour in neighbours)
            {
                if (!neighbour.Walkable) continue;
                if (visited.Contains(neighbour)) continue;
                if (GetDistance(start, cur) > maxDist) continue;
                if (queue.Contains(neighbour)) continue;

                neighbour.Parent = cur;
                queue.Add(neighbour);
            }
        }
        if (queue.Count > maxDist * maxDist)
            Debug.LogError("BFS Search reached maxed. Max dist is " + maxDist.ToString());
        Debug.Log("No Path found with move dist of " + maxDist.ToString());
        return null;
    }
}
