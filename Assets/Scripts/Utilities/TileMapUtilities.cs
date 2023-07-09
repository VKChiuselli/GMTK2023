using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Assets.Scripts;
using Tile = GameManager.Tile;

public static class TileMapUtilities
{
    public static Dictionary<Vector2Int, Tile> GetTilesInRange(this Dictionary<Vector2Int, Tile> tilemap, Vector2Int startTile, int range, bool inLineOfSight)
    {
        Dictionary<Vector2Int, Tile> tilesInRange = new();
        for (int x = startTile.x - range; x <= startTile.x + range; x++)
        {
            for (int y = startTile.y - range; y <= startTile.y + range; y++)
            {
                var coords = new Vector2Int(x, y);
                if (Utility.Distance(startTile, coords) <= range)
                {
                    if (inLineOfSight)
                    {
                        if (GameManager.Inst.LineOfSightBlocked(startTile, coords, true))
                        {
                            continue;
                        }
                    }
                    tilesInRange.Add(coords, GameManager.Inst.Grid[coords]);
                }
                else
                {
                    continue;
                }
            }
        }
        return tilesInRange;
    }


}
