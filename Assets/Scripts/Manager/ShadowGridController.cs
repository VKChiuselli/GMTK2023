using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShadowGridController : MonoBehaviour
{
    public Tilemap ShadowMap;
    public Tile Shadow;

    public void DrawShadow()
    {
        Vector2Int topLeft = GameManager.Inst.TopLeftBounds;
        Vector2Int bottomRight = GameManager.Inst.BottomRightBounds;

        for (int x = topLeft.x; x < bottomRight.x; x++)
        {
            for (int y = topLeft.y; y < bottomRight.y; y++)
            {
                ShadowMap.SetTile(new Vector3Int(x, y), Shadow);
            }
        }

        foreach(var key in GameManager.Inst.Grid.Keys)
        {
            GameManager.Inst.Grid[key].IsVisible = false;
        }
    }

    public void DrawVisibility(List<LightRange> lights)
    {
        foreach (var light in lights)
        {
            Vector2Int startPos = Utility.Round(light.transform.position);
            int maxDist = light.Range;

            Dictionary <Vector2Int, GameManager.Tile> Grid = GameManager.Inst.Grid;
            GameManager.Inst.UpdateGrid();

            GameManager.Tile start = Grid[startPos];
            start.Walkable = GameManager.Tile.TileStatus.Walkable;


            List<GameManager.Tile> queue = new List<GameManager.Tile>();
            HashSet<GameManager.Tile> visited = new HashSet<GameManager.Tile>();
            queue.Add(start);

            // Fail safe for the loop at the end
            while (queue.Count > 0 && queue.Count < maxDist * maxDist * 4)
            {
                GameManager.Tile cur = queue[0];
                queue.RemoveAt(0);
                visited.Add(cur);
                ShadowMap.SetTile((Vector3Int)cur.Position, null);
                cur.IsVisible = true;

                List<GameManager.Tile> neighbours = GameManager.Inst.GetNeighbours(cur);
                foreach (GameManager.Tile neighbour in neighbours)
                {
                    // You can see the walls at least
                    if (cur.Walkable == GameManager.Tile.TileStatus.Blocked) continue;
                    //if (neighbour.Walkable.HasFlag(GameManager.Tile.TileStatus.Blocked) && !neighbour.Walkable.HasFlag(GameManager.Tile.TileStatus.HasUnit)) continue;
                    if (visited.Contains(neighbour)) continue;
                    if (GameManager.Inst.GetDistance(start, cur) >= maxDist) continue;
                    if (queue.Contains(neighbour)) continue;

                    // Show ranges that you can walk on but contains a unit on it
                    if (cur.Walkable.HasFlag(GameManager.Tile.TileStatus.HasUnit) && !neighbour.Walkable.HasFlag(GameManager.Tile.TileStatus.HasUnit)) continue;

                    // Check line of sight
                    if (LineOfSightBlocked(startPos, neighbour.Position)) continue;

                    neighbour.Parent = cur;
                    queue.Add(neighbour);
                }
            }
            if (queue.Count > maxDist * maxDist)
                Debug.LogError("BFS Search reached maxed. Max dist is " + maxDist.ToString());
        }
        
    }

    bool LineOfSightBlocked(Vector2Int start, Vector2Int end)
    {
        bool isBlocked = false;
        // Disable start and end
        var obj1 = DisableObjectsAtPos(start);
        var obj2 = DisableObjectsAtPos(end);

        var collision = Physics2D.Linecast(start, end);
        if (collision.collider != null)
            isBlocked = collision.collider.CompareTag("Wall");

        if (obj1 != null)
            obj1.enabled = true;
        if (obj2 != null)
            obj2.enabled = true;

        return isBlocked;
    }

    Collider2D DisableObjectsAtPos(Vector2Int pos)
    {
        Collider2D obj = null;
        var collision = Physics2D.OverlapPoint(pos);
        if (collision != null)
        {
            obj = collision;
            obj.enabled = true;
        }
        return obj;
    }
}
