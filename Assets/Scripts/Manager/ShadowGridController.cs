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
            GameManager.Inst.Grid[key].IsVisible = GameManager.Tile.Visibility.Invisible;
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
            while (queue.Count > 0 && queue.Count < maxDist * maxDist * 4 + 6)
            {
                GameManager.Tile cur = queue[0];
                queue.RemoveAt(0);
                visited.Add(cur);
                ShadowMap.SetTile((Vector3Int)cur.Position, null);
                cur.IsVisible = GameManager.Tile.Visibility.Visible;

                if (cur.Walkable == GameManager.Tile.TileStatus.Blocked) continue;

                List<GameManager.Tile> neighbours = GameManager.Inst.GetNeighbours(cur);
                foreach (GameManager.Tile neighbour in neighbours)
                {  
                    if (visited.Contains(neighbour)) continue;
                    if (queue.Contains(neighbour)) continue;
                    if (GameManager.Inst.GetDistance(start, cur) >= maxDist) continue;

                    // Show ranges that you can walk on but contains a unit on it
                    if (cur.Walkable.HasFlag(GameManager.Tile.TileStatus.HasUnit) && !neighbour.Walkable.HasFlag(GameManager.Tile.TileStatus.HasUnit)) continue;

                    // Check line of sight
                    if (GameManager.Inst.LineOfSightBlocked(startPos, neighbour.Position)) continue;
                    queue.Add(neighbour);
                }
            }
            if (queue.Count > maxDist * maxDist && maxDist > 1)
                Debug.LogError("BFS Search reached maxed. Max dist is " + maxDist.ToString());
        }
        
    }
}
