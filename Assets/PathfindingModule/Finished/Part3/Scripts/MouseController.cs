using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static finished3.ArrowTranslator;

namespace finished3
{
    public class MouseController : MonoBehaviour
    {
        public GameObject cursor;
        public float speed;
        public GameObject characterPrefab;
        private CharacterInfo character;

        private PathFinder pathFinder;
        private RangeFinder rangeFinder;
        private ArrowTranslator arrowTranslator;
        private List<OverlayTile> path;
        private List<OverlayTile> rangeFinderTiles;
        public bool isMoving;
        OverlayTile currentTileCharacter;
        public OverlayTile currentTileMouse;

        private void Start()
        {
            pathFinder = new PathFinder();
            rangeFinder = new RangeFinder();
            arrowTranslator = new ArrowTranslator();

            path = new List<OverlayTile>();
            isMoving = false;
            rangeFinderTiles = new List<OverlayTile>();
        }

        void LateUpdate()
        {
            RaycastHit2D? hit = GetFocusedOnTile();

            if (hit.HasValue)
            {
                OverlayTile tile = hit.Value.collider.gameObject.GetComponent<OverlayTile>();
                cursor.transform.position = tile.transform.position;
                cursor.gameObject.GetComponent<SpriteRenderer>().sortingOrder = tile.transform.GetComponent<SpriteRenderer>().sortingOrder + 1;

                if (character != null)
                {
                    currentTileCharacter = tile;
                }


                if (rangeFinderTiles.Contains(tile) && !isMoving)
                {
                    path = pathFinder.FindPath(character.standingOnTile, tile, rangeFinderTiles);

                    foreach (var item in rangeFinderTiles)
                    {
                        MapManager.Instance.map[item.grid2DLocation].SetSprite(ArrowDirection.None);
                    }

                    for (int i = 0; i < path.Count; i++)
                    {
                        var previousTile = i > 0 ? path[i - 1] : character.standingOnTile;
                        var futureTile = i < path.Count - 1 ? path[i + 1] : null;

                        var arrow = arrowTranslator.TranslateDirection(previousTile, path[i], futureTile);
                        path[i].SetSprite(arrow);
                    }
                }

                if (path.Count > 0)
                {
                    currentTileMouse = tile;
                }

                if (Input.GetMouseButtonDown(0))
                {

                    if (character != null)
                    {
                        if (currentTileCharacter == character.standingOnTile)
                        {
                            //We do this because the player stop to click
                            GetInRangeTiles();
                            return;

                        }

                    }

                    if (character == null)
                    {
                        character = Instantiate(characterPrefab).GetComponent<CharacterInfo>();
                        character.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        PositionCharacterOnLine(tile);
                        GetInRangeTiles();
                    }
                    else
                    {
                        if (CheckIfClickedOnTheTile(path, tile))
                        {
                            isMoving = true;
                            tile.gameObject.GetComponent<OverlayTile>().HideTile();
                        }

                    }
                }
            }

            if (path.Count > 0 && isMoving)
            {
                MoveAlongPath();
            }
        }

        private bool CheckIfClickedOnTheTile(List<OverlayTile> path, OverlayTile tile)
        {
            foreach (var item in path)
            {
                MapManager.Instance.map[item.grid2DLocation].SetSprite(ArrowDirection.None);
            }
            foreach (var item in rangeFinderTiles)
            {
                MapManager.Instance.map[item.grid2DLocation].SetSprite(ArrowDirection.None);
            }
            if (currentTileMouse == path[path.Count - 1])
            {
                return true;
            }
            tile.HideTile();

            GetInRangeTiles();

            return false;
        }

        private void MoveAlongPath()
        {
            var step = speed * Time.deltaTime;

            float zIndex = path[0].transform.position.z;
            character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position, step);
            character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zIndex);

            if (Vector2.Distance(character.transform.position, path[0].transform.position) < 0.00001f)
            {
                PositionCharacterOnLine(path[0]);
                path.RemoveAt(0);

                // Set character sorting order
                RaycastHit2D? hit = GetFocusedOnTile();
                if (hit.HasValue)
                {
                    OverlayTile tile = hit.Value.collider.gameObject.GetComponent<OverlayTile>();
                    character.SetSortingOrder(tile.transform.GetComponent<SpriteRenderer>().sortingOrder);
                }
            }

            if (path.Count == 0)
            {
                GetInRangeTiles();
                isMoving = false;
            }

        }

        private void PositionCharacterOnLine(OverlayTile tile)
        {
            character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
            character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
            character.standingOnTile = tile;
        }

        private static RaycastHit2D? GetFocusedOnTile()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);

            if (hits.Length > 0)
            {
                return hits.OrderByDescending(i => i.collider.transform.position.z).First();
            }

            return null;
        }

        public int howMuchPlayerMove = 3;

        private void GetInRangeTiles()
        {
            UnHideShadowsInRange();

            Vector2Int startPos = new Vector2Int(character.standingOnTile.gridLocation.x, character.standingOnTile.gridLocation.y);
            rangeFinderTiles = rangeFinder.GetTilesInRange(startPos, howMuchPlayerMove);

            // Remove all tiles not in range due to obsticals
            List<OverlayTile> finalTiles = new List<OverlayTile>();
            foreach (var tile in rangeFinderTiles)
            {
                var length = pathFinder.FindPath(character.standingOnTile, tile, rangeFinderTiles).Count;
                if (length > 0 && length <= howMuchPlayerMove)
                    finalTiles.Add(tile);
            }
            rangeFinderTiles = finalTiles;
            foreach (var item in rangeFinderTiles)
            {
                if (item.grid2DLocation != startPos)
                    item.ShowTile();
            }
        }

        public int howMuchVision = 5;
        private void UnHideShadowsInRange()
        {
            Vector2Int startPos = new Vector2Int(character.standingOnTile.gridLocation.x, character.standingOnTile.gridLocation.y);
            var visibleTiles = rangeFinder.GetTilesInRange(startPos, howMuchVision);

            // Remove tiles blocked by walls
            List<OverlayTile> finalTiles = new List<OverlayTile>();
            foreach (var tile in visibleTiles)
            {
                var hits = Physics2D.LinecastAll(character.standingOnTile.transform.position, tile.transform.position);
                bool hitWall = false;
                foreach( var hit in hits)
                {
                    var grid = hit.collider.gameObject.GetComponent<OverlayTile>();
                    if (grid != null && grid.isBlocked)
                    {
                        hitWall = true;
                    }
                }

                if (!hitWall)
                    finalTiles.Add(tile);
            }
            visibleTiles = finalTiles;

            foreach (var item in visibleTiles)
            {
                    item.HideShadow();
            }
        }
    }
}
