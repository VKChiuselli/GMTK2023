using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMovementController : MonoBehaviour
{
    public Tilemap MovementMap;
    public Tile CanMoveTile;
    public Tile CantMoveTile;
    public Tile PathTile;
    public Tile PathTileCorner;
    public Tile EndTile;

    public void ClearGrid()
    {
        MovementMap.ClearAllTiles();
    }

    public void DrawWalkablePath(Vector2Int startPos, int maxDist)
    {
        Dictionary<Vector2Int, GameManager.Tile> Grid = GameManager.Inst.Grid;
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
            if (cur.Walkable.HasFlag(GameManager.Tile.TileStatus.HasUnit))
            {
                MovementMap.SetTile((Vector3Int)cur.Position, CantMoveTile);
            }
            else
                MovementMap.SetTile((Vector3Int)cur.Position, CanMoveTile);

            List<GameManager.Tile> neighbours = GameManager.Inst.GetNeighbours(cur);
            foreach (GameManager.Tile neighbour in neighbours)
            {
                if (neighbour.Walkable.HasFlag(GameManager.Tile.TileStatus.Blocked) && !neighbour.Walkable.HasFlag(GameManager.Tile.TileStatus.HasUnit)) continue;
                if (visited.Contains(neighbour)) continue;
                if (GameManager.Inst.GetDistance(start, cur) > maxDist) continue;
                if (queue.Contains(neighbour)) continue;

                // Show ranges that you can walk on but contains a unit on it
                if (cur.Walkable.HasFlag(GameManager.Tile.TileStatus.HasUnit) && !neighbour.Walkable.HasFlag(GameManager.Tile.TileStatus.HasUnit)) continue;

                neighbour.Parent = cur;
                queue.Add(neighbour);
            }
        }
        if (queue.Count > maxDist * maxDist)
            Debug.LogError("BFS Search reached maxed. Max dist is " + maxDist.ToString());
    }

    public void DrawMovementPath(List<Vector2Int> path, Vector2Int startPos)
    {
        if (path != null)
        {
            Vector2Int prev_pos = startPos;
            Vector2Int cur_pos = startPos;

            int rotation;
            Vector2Int direction;
            Vector2Int direction2;

            foreach (var next_pos in path)
            {
                if (cur_pos == startPos)
                {
                    // Do start tile
                    MovementMap.SetTile((Vector3Int)cur_pos, EndTile);

                    rotation = 0;
                    // Its the reverse of the end tile
                    direction = cur_pos - next_pos;
                    if (direction == Vector2Int.up)
                        rotation = 90;
                    else if (direction == Vector2Int.left)
                        rotation = 180;
                    else if (direction == Vector2Int.down)
                        rotation = 270;

                    MovementMap.SetTransformMatrix((Vector3Int)cur_pos, Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotation)));
                }
                else
                {
                    rotation = 0;
                    direction = next_pos - cur_pos;
                    direction2 = cur_pos - prev_pos; 
                    // Straight tile
                    if (direction == direction2)
                    {
                        MovementMap.SetTile((Vector3Int)cur_pos, PathTile);

                        if (direction == Vector2Int.up || direction == Vector2Int.down)
                            rotation = 90;
                    }
                    else // Corner Tle
                    {
                        MovementMap.SetTile((Vector3Int)cur_pos, PathTileCorner);

                        if (direction2 == Vector2Int.up)
                        {
                            if (direction == Vector2Int.right)
                                rotation = 90;
                            else if (direction == Vector2Int.left)
                                rotation = 0;
                        }
                        else if (direction2 == Vector2Int.right)
                        {
                            if (direction == Vector2Int.up)
                                rotation = 270;
                            else if (direction == Vector2Int.down)
                                rotation = 0;
                        }
                        else if (direction2 == Vector2Int.down)
                        {
                            if (direction == Vector2Int.left)
                                rotation = 270;
                            else if (direction == Vector2Int.right)
                                rotation = 180;
                        }
                        else if (direction2 == Vector2Int.left)
                        {
                            if (direction == Vector2Int.up)
                                rotation = 180;
                            else if (direction == Vector2Int.down)
                                rotation = 90;
                        }
                    }

                    MovementMap.SetTransformMatrix((Vector3Int)cur_pos, Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotation)));
                }
                if (cur_pos != startPos)
                    prev_pos = cur_pos;
                cur_pos = next_pos;
            }

            // Do end tile
            MovementMap.SetTile((Vector3Int)cur_pos, EndTile);

            rotation = 0;
            direction = cur_pos - prev_pos;
            if (direction == Vector2Int.up)
                rotation = 90;
            else if (direction == Vector2Int.left)
                rotation = 180;
            else if (direction == Vector2Int.down)
                rotation = 270;

            MovementMap.SetTransformMatrix((Vector3Int)cur_pos, Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotation)));
        }
    }
}
