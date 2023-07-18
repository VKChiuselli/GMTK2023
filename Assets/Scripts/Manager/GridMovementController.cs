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
    public Tile Visibility;

    private bool _ignoreWalls = false;
    private bool _needLineOfSite = false;

    private void ClearGrid()
    {
        MovementMap.ClearAllTiles();
    }

    public void DrawWalkablePath(Vector2Int startPos, int maxDist, bool ignoreWalls, bool needLineOfSite)
    {
        ClearGrid();
        _ignoreWalls = ignoreWalls;
        _needLineOfSite = needLineOfSite;
        BoardManager.Inst.ResetGrid();
        BoardManager.Tile start = BoardManager.Inst.Grid[startPos];
        start.Walkable = BoardManager.Tile.TileStatus.Walkable;
        BoardManager.Inst.BFS(startPos, maxDist, CannotWalkOn, null, UpdateMovementMap);
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

    public void DrawHeroVisibility(Vector2Int pos)
    {
        BoardManager.Inst.Grid[pos].IsVisible = BoardManager.Tile.Visibility.HeroVisible;
        MovementMap.SetTile((Vector3Int)pos, Visibility);
    }

    #region Delegate Functions
    private void UpdateMovementMap(BoardManager.Tile tile)
    {
        if (tile.Walkable.HasFlag(BoardManager.Tile.TileStatus.HasUnit) || tile.Walkable.HasFlag(BoardManager.Tile.TileStatus.Blocked))
            MovementMap.SetTile((Vector3Int)tile.Position, CantMoveTile);
        else
            MovementMap.SetTile((Vector3Int)tile.Position, CanMoveTile);
    }

    private bool CannotWalkOn(BoardManager.Tile cur, BoardManager.Tile neighbour, Vector2Int startPos)
    {
        // Show ranges that you can walk on but contains a unit on it
        return (!_ignoreWalls && neighbour.Walkable == BoardManager.Tile.TileStatus.Blocked) ||
               (!_ignoreWalls && cur.Walkable.HasFlag(BoardManager.Tile.TileStatus.HasUnit) && !neighbour.Walkable.HasFlag(BoardManager.Tile.TileStatus.HasUnit)) ||
               (_needLineOfSite && BoardManager.Inst.LineOfSightBlocked(startPos, neighbour.Position));
    }
    #endregion
}
