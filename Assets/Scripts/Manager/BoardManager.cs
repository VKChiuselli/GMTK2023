using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Inst;

    public Dictionary<Vector2Int, Tile> Grid = new();

    private GridMovementController _gridController;
    private ShadowGridController _shadowController;

    // TODO check tile map for actual size so we don't need to set a size each time
    // TopLeft < BottomRight
    public Vector2Int TopLeftBounds = new(-10, -10);
    public Vector2Int BottomRightBounds = new(10, 10);
    void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Destroy(this);
            return;
        }
        Inst = this;
    }

    // Start is called before the first frame update
    public void Start()
    {
        SetUpGrid(TopLeftBounds, BottomRightBounds);
        _gridController = GetComponent<GridMovementController>();
        _shadowController = GetComponent<ShadowGridController>();
    }

    // Update is called once per frame
    void Update()
    {
        ShadowGridUpdate();
    }

    void ShadowGridUpdate()
    {
        _shadowController.DrawShadow();
        _shadowController.DrawVisibility(new List<LightRange>(FindObjectsOfType<LightRange>()));
    }

    public void MovementGridUpdate(Player player, Vector2Int cursorPosition)
    {
        _gridController.DrawWalkablePath(Utility.Round(player.transform.position), player.MaxMovement, player.IgnoreWalls, player.NeedLineOfSight);
        if (player.CurrentAbility == Player.Abilities.Walk)
        {
            List<Vector2Int> path = FindPath(player.transform.position, cursorPosition, player.MaxMovement);
            _gridController.DrawMovementPath(path, Utility.Round(player.transform.position));
        }
    }

    // TODO Rename this class as it conflicts with Unity.Tilemap Tile class
    public class Tile
    {
        public Vector2Int Position;
        public TileStatus Walkable;
        public Tile Parent;
        public Visibility IsVisible = Visibility.Visible;

        public enum TileStatus
        {
            Walkable = 00,
            HasUnit = 10,
            Blocked = 01
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

                Grid.Add(position, new Tile() { Position = position, Walkable = TileIsWalkable(position) });
            }
        }
    }

    public void ResetGrid()
    {
        // Use an enumerator to speed up the process 2x
        var enumerator = Grid.GetEnumerator();

        while (enumerator.MoveNext())
        {
            enumerator.Current.Value.Walkable = TileIsWalkable(enumerator.Current.Key);
            enumerator.Current.Value.Parent = null;
        }
    }
    #region Utility
    Tile.TileStatus TileIsWalkable(Vector2Int position)
    {
        Tile.TileStatus status = Tile.TileStatus.Walkable;
        var collision = Physics2D.OverlapPoint(position);
        if (collision != null)
        {
            if (collision.GetComponent<Unit>() != null)
                status |= Tile.TileStatus.HasUnit;
            if (collision.GetComponent<InteractableEntity>() != null)
                status |= Tile.TileStatus.HasUnit;

            status |= Tile.TileStatus.Blocked;
        }
        return status;
    }

    public void UpdateHeroSight(Vector2Int pos)
    {
        _gridController.DrawHeroVisibility(pos);
    }

    public bool LineOfSightBlocked(Vector2Int start, Vector2Int end, bool includeEntities = false)
    {
        bool isBlocked = false;
        // Disable start and end

        var collision = Physics2D.LinecastAll(start, end);
        foreach (var c in collision)
        {
            if (c.collider != null && (Vector2)c.transform.position != end && (Vector2)c.transform.position != start)
            {
                isBlocked = c.collider.CompareTag("Wall");
                if (includeEntities && Utility.Distance(start, c.point) > 1.5f)
                    isBlocked |= c.collider.CompareTag("Entity");
                if (isBlocked)
                {
                    break;
                }
            }
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

        while (cur != start && cur != null)
        {
            path.Add(cur.Position);
            cur = cur.Parent;
        }

        //there is no path
        if (cur != start)
            return null;

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
        foreach (Vector2Int pos in positions)
        {
            if (Grid.ContainsKey(pos))
                neighbours.Add(Grid[pos]);
        }
        return neighbours;
    }

    public List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int endPos, int maxDist)
    {
        if (startPos == endPos)
            return null;

        if (!Grid.ContainsKey(endPos) || !Grid.ContainsKey(startPos))
            return null;

        Tile start = Grid[startPos];
        Tile end = Grid[endPos];

        if (end == null || start == null)
        {
            Debug.LogWarning("No Path found");
            return null;
        }

        ResetGrid();

        var isEndWalkable = end.Walkable;
        end.Walkable = Tile.TileStatus.Walkable;

        BFS(startPos, maxDist, IsNotWalkable, null, null);

        while (isEndWalkable.HasFlag(Tile.TileStatus.Blocked) && end != null && end != start)
        {
            end = end.Parent;
            if (end != null)
                isEndWalkable = end.Walkable;
        }

        if (isEndWalkable == Tile.TileStatus.Blocked)
            return null;
        if (end == start || end == null)
            return null;

        return RetracePath(start, end);
    }

    public List<Vector2Int> FindPath(Vector2 startPos, Vector2 endPos, int maxDist)
    {
        return FindPath(Utility.Round(startPos), Utility.Round(endPos), maxDist);
    }

    #endregion

    /*
     * Not sure if this is the best way to go with handeling multiple cases for a BFS search. Probably need to rename to something simpler
     * The problem with this is if we ever update one of these delegates, its going to be a pain to update every implemented delegate, though
     * it does show an error when we do change it.
     * - Chris
     **/
    public delegate bool CannotTravelThrough(Tile cur, Tile neighbour, Vector2Int startPos);
    public delegate bool ShouldCheckNeighbour(Tile tile);
    public delegate void OnCheckTile(Tile tile);

    /// <summary>
    /// Pathfinding Function
    /// Always remember to call reset grid first. Its like this because there are few functions who after resetting the grid, removes the current tile or something.
    /// Does a pathfinding search from a given position spread out to a fix distance.
    /// Each tile's parent can be combine to create a path to the starting position.
    /// You can pass 3 functions to modify how the pathfinding works.
    /// Treat this function as a black box if you get too confused :)
    /// </summary>
    /// <param name="caller">The game object that called this function.</param>
    /// <param name="startPos">The start position of the search on the grid.</param>
    /// <param name="maxDist">How far to do the search.</param>
    /// <param name="canTravel">Checks the tile if you can pass through it or not.</param>
    /// <param name="checkNeighbour">A function to determine whether to check the neighbour tiles.</param>
    /// <param name="onCheckTile">A function called when the tile is added to the BFS.</param>
    /// <returns></returns>
    public void BFS(Vector2Int startPos, int maxDist, CannotTravelThrough canTravel, ShouldCheckNeighbour shouldCheckNeighbour, OnCheckTile onCheckTile)
    {
        Tile start = Grid[startPos];

        List<Tile> queue = new();
        HashSet<Tile> visited = new();
        queue.Add(start);

        // Fail safe for the loop at the end
        while (queue.Count > 0 && queue.Count < maxDist * maxDist * 4 + 6)
        {
            Tile cur = queue[0];
            queue.RemoveAt(0);
            visited.Add(cur);

            onCheckTile?.Invoke(cur);
            if (shouldCheckNeighbour != null && !shouldCheckNeighbour(cur)) continue;

            List<Tile> neighbours = GetNeighbours(cur);
            foreach (Tile neighbour in neighbours)
            {
                if (visited.Contains(neighbour)) continue;
                if (queue.Contains(neighbour)) continue;
                if (GetDistance(start, cur) >= maxDist) continue;

                if (canTravel != null && canTravel(cur, neighbour, startPos)) continue;

                neighbour.Parent = cur;
                queue.Add(neighbour);
            }
        }
        if (queue.Count > maxDist * maxDist && maxDist != 1)
            Debug.LogError("BFS Search reached maxed. Max dist is " + maxDist.ToString());
    }

    // Need to rename these methods to something better to know which method is for which delegate if we are keeping this
    #region Delegates Methods
    public bool IsNotWalkable(Tile cur, Tile neighbour, Vector2Int startPos)
    {
        return neighbour.Walkable.HasFlag(Tile.TileStatus.Blocked);
    }

    public bool TileNotInSight(Tile cur, Tile neighbour, Vector2Int startPos)
    {
        return Inst.LineOfSightBlocked(startPos, neighbour.Position);
    }

    public bool TileNotVisible(Tile cur, Tile neighbour, Vector2Int startPos)
    {
        return neighbour.IsVisible != Tile.Visibility.Visible;
    }
    #endregion
}
