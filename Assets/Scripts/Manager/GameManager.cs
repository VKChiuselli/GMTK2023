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
    public GameObject Cursor;

    //TODO an automated way to populate this
    public List<Unit> Heroes;
    public List<Unit> Enemies;

    public Vector2Int TopLeftBounds;
    public Vector2Int BottomRightBounds;

    public Dictionary<Vector2Int, Tile> Grid = new();
    public float MovementSpeed = 10;
    public TurnId CurrentTurn = TurnId.Player;

    private GridMovementController _gridController;
    private ShadowGridController _shadowController;

    private bool _isMovingUnits = false;
    private bool _isNextTurn = false;

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
        SetUpGrid(TopLeftBounds, BottomRightBounds);
        _gridController = GetComponent<GridMovementController>();
        _shadowController = GetComponent<ShadowGridController>();
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
        MovementGridUpdate();
        ShadowGridUpdate();
        HoverInfo();

        UnitController();
        CursorController();
    }

    void HoverInfo()
    {
        Vector2 mousePosition = inputActions.Player.MouseLocation.ReadValue<Vector2>();
        Vector2Int position = Utility.Round(Camera.main.ScreenToWorldPoint(mousePosition));

        var collision = Physics2D.OverlapPoint(position);
        if (collision != null)
        {
            var unit = collision.GetComponent<Unit>();
            if (unit != null)
                unit.HoverInfo();
        }
    }

    void ShadowGridUpdate()
    {
        _shadowController.DrawShadow();
        _shadowController.DrawVisibility(new List<LightRange>(FindObjectsOfType<LightRange>()));
    }

    void MovementGridUpdate()
    {
        Vector2 mousePosition = inputActions.Player.MouseLocation.ReadValue<Vector2>();
        Vector2Int cursorPosition = Utility.Round(Camera.main.ScreenToWorldPoint(mousePosition));
        _gridController.ClearGrid();
        _gridController.DrawWalkablePath(Utility.Round(PlayerUnit.transform.position), PlayerUnit.MaxMovement);
        List<Vector2Int> path = FindPath(PlayerUnit.transform.position, cursorPosition, PlayerUnit.MaxMovement);
        _gridController.DrawMovementPath(path, Utility.Round(PlayerUnit.transform.position));
    }

    void CursorController()
    {
        Vector2 position = inputActions.Player.MouseLocation.ReadValue<Vector2>();
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(position);
        Cursor.transform.position = (Vector2)Utility.Round(mousePosition);
    }

    void UnitController()
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
        if (!_isMovingUnits)
        {
            bool click = inputActions.Player.Click.ReadValue<float>() > 0;
            Vector2 position = inputActions.Player.MouseLocation.ReadValue<Vector2>();
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(position);

            if (click)
            {
                _isMovingUnits = true;
                PlayerUnit.Move(mousePosition);
                _isNextTurn = true;
            }
        }
        else if (!PlayerUnit._isMoving)
        {
            _isMovingUnits = false;
            NextTurn();
        }
    }

    void AIControls(List<Unit> units)
    {
        if (units != null)
        {
            if (!_isMovingUnits)
            {
                StartCoroutine(MoveUnits(units));
            }
        }
        else
            Debug.LogError("Units are null");
    }

    IEnumerator MoveUnits(List<Unit> units)
    {
        if (!_isMovingUnits)
        {
            _isMovingUnits = true;
            foreach (var unit in units)
            {
                unit.AILogic();
                yield return new WaitForSeconds(0.05f);
            }

            while (_isMovingUnits)
            {
                _isMovingUnits = false;
                foreach (var unit in units)
                {
                    if (unit._isMoving)
                    {
                        _isMovingUnits = true;
                        yield return null;
                    }
                }
            }
            _isMovingUnits = true;
            //yield return new WaitForSeconds(1f);
            _isMovingUnits = false;
            _isNextTurn = true;
            NextTurn();
        }
    }

    void NextTurn()
    {
        if (_isNextTurn)
        {
            CurrentTurn++;
            if (CurrentTurn > TurnId.Traps)
                CurrentTurn = TurnId.Player;
            _isNextTurn = false;
        }
    }

    public class Tile
    {
        public Vector2Int Position;
        public TileStatus Walkable;
        public Tile Parent;
        public Visibility IsVisible = Visibility.Visible;

        public enum TileStatus
        {
            Walkable = 00,
            HasUnit  = 10,
            Blocked  = 01
        }

        public enum Visibility
        {
            Invisible,
            Visible,
            HeroVisible
        }
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

    public void UpdateGrid()
    {
        foreach(Vector2Int pos in Grid.Keys)
        {
            Grid[pos].Walkable = IsWalkable(pos);
        }
    }

    Tile.TileStatus IsWalkable(Vector2Int position)
    {
        Tile.TileStatus status = Tile.TileStatus.Walkable;
        var collision = Physics2D.OverlapPoint(position);
        if (collision != null)
        {
            if (collision.GetComponent<Unit>() != null)
                status |= Tile.TileStatus.HasUnit;
            
            status |= Tile.TileStatus.Blocked;
        }
        return status;
    }

    public void UpdateHeroSight(Vector2Int pos)
    {
        _gridController.DrawHeroVisibility(pos);
    }

    public bool LineOfSightBlocked(Vector2Int start, Vector2Int end)
    {
        bool isBlocked = false;
        // Disable start and end

        var collision = Physics2D.LinecastAll(start, end);
        foreach (var c in collision)
        {
            if (c.collider != null && (Vector2)c.transform.position != end && (Vector2)c.transform.position != start)
                isBlocked = c.collider.CompareTag("Wall");
        }

        return isBlocked;
    }

    public int GetDistance(Tile a, Tile b)
    {
        // Returns Manhattan Distance
        return Utility.Distance(a.Position, b.Position);
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

    public List<Tile> GetNeighbours(Tile cur)
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
        var isEndWalkable = end.Walkable;
        end.Walkable = Tile.TileStatus.Walkable;

        List<Tile> queue = new List<Tile>();
        HashSet<Tile> visited = new HashSet<Tile>();
        queue.Add(start);

        // Fail safe for the loop at the end
        while (queue.Count > 0 && queue.Count < maxDist * maxDist * 4 + 6)
        {
            Tile cur = queue[0];
            queue.RemoveAt(0);
            visited.Add(cur);

            if (cur.Equals(end))
            {
                // If you can't walk to the end, walk one tile before
                if (isEndWalkable.HasFlag(Tile.TileStatus.Blocked))
                    end = end.Parent;
                if (end == start)
                    return null;
                return RetracePath(start, end);
            }

            List<Tile> neighbours = GetNeighbours(cur);
            foreach (Tile neighbour in neighbours)
            {
                if (neighbour.Walkable.HasFlag(Tile.TileStatus.Blocked)) continue;
                if (visited.Contains(neighbour)) continue;
                if (GetDistance(start, cur) >= maxDist) continue;
                if (queue.Contains(neighbour)) continue;

                neighbour.Parent = cur;
                queue.Add(neighbour);
            }
        }
        if (queue.Count > maxDist * maxDist && maxDist != 1)
            Debug.LogError("BFS Search reached maxed. Max dist is " + maxDist.ToString());
        Debug.Log("No Path found with move dist of " + maxDist.ToString());
        return null;
    }
}
