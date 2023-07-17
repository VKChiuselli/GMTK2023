using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShadowGridController : MonoBehaviour
{
    public Tilemap ShadowMap;
    public Tile Shadow;
    [SerializeField]
    private bool _switch = false;
    public void DrawShadow()
    {
        Vector2Int topLeft = BoardManager.Inst.TopLeftBounds;
        Vector2Int bottomRight = BoardManager.Inst.BottomRightBounds;

        for (int x = topLeft.x; x < bottomRight.x; x++)
        {
            for (int y = topLeft.y; y < bottomRight.y; y++)
            {
                ShadowMap.SetTile(new Vector3Int(x, y), Shadow);
            }
        }

        foreach(var key in BoardManager.Inst.Grid.Keys)
        {
            BoardManager.Inst.Grid[key].IsVisible = BoardManager.Tile.Visibility.Invisible;
        }
    }

    public void DrawVisibility(List<LightRange> lights)
    {
        foreach (var light in lights)
        {
            Vector2Int startPos = Utility.Round(light.transform.position);
            int maxDist = light.Range;

            if (_switch)
            {
                Dictionary<Vector2Int, BoardManager.Tile> Grid2 = BoardManager.Inst.Grid.GetTilesInRange(startPos, maxDist, true);
                foreach (var tile in Grid2.Values)
                {
                    ShadowMap.SetTile((Vector3Int)tile.Position, null);
                }
                continue;
            }
            BoardManager.Inst.ResetGrid();

            BoardManager.Tile start = BoardManager.Inst.Grid[startPos];
            start.Walkable = BoardManager.Tile.TileStatus.Walkable;

            BoardManager.Inst.BFS(startPos, maxDist, NotInLineOfSight, VisionNotBlocked, UpdateShadowMap);
        }
        
    }

    #region Delegates

    private bool VisionNotBlocked(BoardManager.Tile tile)
    {
        return (tile.Walkable != BoardManager.Tile.TileStatus.Blocked);
    }

    private void UpdateShadowMap(BoardManager.Tile tile)
    {
        ShadowMap.SetTile((Vector3Int)tile.Position, null);
        tile.IsVisible = BoardManager.Tile.Visibility.Visible;
    }

    private bool NotInLineOfSight(BoardManager.Tile cur, BoardManager.Tile neighbour, Vector2Int startPos)
    {
        // Show ranges that you can walk on but contains a unit on it
        return (cur.Walkable.HasFlag(BoardManager.Tile.TileStatus.HasUnit) && !neighbour.Walkable.HasFlag(BoardManager.Tile.TileStatus.HasUnit)) ||
               (BoardManager.Inst.LineOfSightBlocked(startPos, neighbour.Position));
    }

    #endregion
}
