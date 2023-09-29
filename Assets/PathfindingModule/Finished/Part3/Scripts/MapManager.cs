using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace finished3
{
    public class MapManager : MonoBehaviour
    {
        private static MapManager _instance;
        public static MapManager Instance { get { return _instance; } }

        public OverlayTile overlayPrefab;
        public GameObject overlayContainer;

        public Dictionary<Vector2Int, OverlayTile> map;
        public bool ignoreBottomTiles;

        public List<Tilemap> FloorMap;
        public List<Tilemap> WallMap;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        void Start()
        {
            var tileMaps = FloorMap;// gameObject.transform.GetComponentsInChildren<Tilemap>().OrderByDescending(x => x.GetComponent<TilemapRenderer>().sortingOrder);
            map = new Dictionary<Vector2Int, OverlayTile>();
            int counterName = 0;
            foreach (var tm in tileMaps)
            {
                BoundsInt bounds = tm.cellBounds;

                for (int z = bounds.max.z; z >= bounds.min.z; z--)
                {
                    if (z == 0 && ignoreBottomTiles)
                        break;

                    for (int y = bounds.min.y; y < bounds.max.y; y++)
                    {
                        for (int x = bounds.min.x; x < bounds.max.x; x++)
                        {
                            if (tm.HasTile(new Vector3Int(x, y, z)))
                            {
                                if (!map.ContainsKey(new Vector2Int(x, y)))
                                {
                                    OverlayTile overlayTile = Instantiate(overlayPrefab, overlayContainer.transform);
                                    var cellWorldPosition = tm.GetCellCenterWorld(new Vector3Int(x, y, z));
                                    overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);
                                    overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tm.GetComponent<TilemapRenderer>().sortingOrder;
                                    overlayTile.gameObject.GetComponent<OverlayTile>().gridLocation = new Vector3Int(x, y, z);
                                    overlayTile.gameObject.name = overlayTile.gameObject.name + counterName;
                                    overlayTile.ShowShadow();
                                    counterName = counterName + 1;
                                    map.Add(new Vector2Int(x, y), overlayTile.gameObject.GetComponent<OverlayTile>());
                                }
                            }
                        }
                    }
                }
            }

            // Keep track of walls
            foreach (var tm in WallMap)
            {
                BoundsInt bounds = tm.cellBounds;

                for (int z = bounds.max.z; z >= bounds.min.z; z--)
                {
                    if (z == 0 && ignoreBottomTiles)
                        break;

                    for (int y = bounds.min.y; y < bounds.max.y; y++)
                    {
                        for (int x = bounds.min.x; x < bounds.max.x; x++)
                        {
                            if (tm.HasTile(new Vector3Int(x, y, z)))
                            {
                                if (map.ContainsKey(new Vector2Int(x, y)))
                                {
                                    map[new Vector2Int(x, y)].isBlocked = true;
                                    map[new Vector2Int(x, y)].HideShadow();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Update()
        {
            // Make walls less visible
            if (Input.GetKey(KeyCode.Space))
            {
                foreach(var map in WallMap)
                {
                    map.color = new Color(map.color.r, map.color.g, map.color.b, 0.4f);
                }
            }
            else
            {
                foreach (var map in WallMap)
                {
                    map.color = new Color(map.color.r, map.color.g, map.color.b, 1f);
                }
            }
        }

        public List<OverlayTile> GetSurroundingTiles(Vector2Int originTile)
        {
            var surroundingTiles = new List<OverlayTile>();


            Vector2Int TileToCheck = new Vector2Int(originTile.x + 1, originTile.y);
            if (map.ContainsKey(TileToCheck))
            {
                if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1)
                    surroundingTiles.Add(map[TileToCheck]);
            }

            TileToCheck = new Vector2Int(originTile.x - 1, originTile.y);
            if (map.ContainsKey(TileToCheck))
            {
                if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1)
                    surroundingTiles.Add(map[TileToCheck]);
            }

            TileToCheck = new Vector2Int(originTile.x, originTile.y + 1);
            if (map.ContainsKey(TileToCheck))
            {
                if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1)
                    surroundingTiles.Add(map[TileToCheck]);
            }

            TileToCheck = new Vector2Int(originTile.x, originTile.y - 1);
            if (map.ContainsKey(TileToCheck))
            {
                if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1)
                    surroundingTiles.Add(map[TileToCheck]);
            }

            return surroundingTiles;
        }
    }
}
