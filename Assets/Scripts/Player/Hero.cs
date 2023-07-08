using System.Security.AccessControl;
using Assets.Scripts;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class Hero : Unit
{

    /*
         * Priorities------------
         * Spotlight
         * Chests
         * Keys
         * Doors
         * Enemies
         * Random
         */
    private LightRange _light;

    protected override void Start()
    {
        base.Start();
        _light = GetComponent<LightRange>();
    }

    public override void HoverInfo()
    {
        GetObjectsInRange(true);
    }

    public override void AILogic()
    {
        List<GameObject> objsTest = GetObjectsInRange(false);
        objsTest.OrderBy(x => Utility.Distance(transform.position, x.transform.position));
        // print names of all objsTest gameobjects
        


        List<PointOfInterest> objs = GetObjectsInRange(false, true);
        Debug.Log(objs.Count);
        Debug.Log(objsTest.Count);

        if (objsTest.Count > 0)
            MoveTowards(Utility.Round(objsTest[0].transform.position));

        /*// Find closeset chest
        GameObject closest = GetClosestObject(objs.FindAll(x => x.GetComponent<Chest>() != null));
        if (closest != null)
        {
            if (Utility.Distance(transform.position, closest.transform.position) <= 1)
            {
                Chest chest = closest.GetComponent<Chest>();
                if (chest !=null)
                {
                    chest.InteractWith(this);
                    // Interact with chest
                }
            }
            else
                MoveTowards(Utility.Round(closest.transform.position));
            return;
        }

        // Find keys

        // Find Doors

        // Find enemies

        */
        // Do something???
    }

    public List<PointOfInterest> GetObjectsInRange(bool showRange, bool diffthantheotherone)
    {
        List<PointOfInterest> visibleObjects = new List<PointOfInterest>();
        Vector2Int startPos = Utility.Round(transform.position);
        

        visibleObjects = Physics2D.OverlapCircleAll(transform.position, 999).ToList()
            .FindAll(x => !GameManager.Inst.LineOfSightBlocked(startPos, Utility.Round(x.transform.position)))
            .ConvertAll(x => x.GetComponent<PointOfInterest>())
            .FindAll(x => x != null);
        visibleObjects.OrderBy(x => Utility.Distance(transform.position, x.transform.position));
        return visibleObjects;
        // visibleObjects.Add(GetObjectsLitUpBySpotlight())
    }

    // TODO: Remove probably?
    public List<GameObject> GetObjectsInRange(bool showRange)
    {
        List<GameObject> obj = new List<GameObject>();
        Vector2Int startPos = Utility.Round(transform.position);
        int maxDist = 99;


        GameManager.Inst.UpdateGrid();
        Dictionary<Vector2Int, GameManager.Tile> Grid = GameManager.Inst.Grid;

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

            // 2 in one function... kind of bad to do...
            if (showRange)
                GameManager.Inst.UpdateHeroSight(cur.Position);
            else
                obj.AddRange(GetObjectsAtPosition(cur.Position));

            List<GameManager.Tile> neighbours = GameManager.Inst.GetNeighbours(cur);
            foreach (GameManager.Tile neighbour in neighbours)
            {
                // You can see the walls at least
                if (neighbour.IsVisible != GameManager.Tile.Visibility.Visible) continue;
                if (visited.Contains(neighbour)) continue;
                if (GameManager.Inst.GetDistance(start, cur) >= maxDist) continue;
                if (queue.Contains(neighbour)) continue;
                // Check line of sight
                if (GameManager.Inst.LineOfSightBlocked(startPos, neighbour.Position)) continue;

                neighbour.Parent = cur;
                queue.Add(neighbour);
            }
        }
        if (queue.Count > maxDist * maxDist)
            Debug.LogError("BFS Search reached maxed. Max dist is " + maxDist.ToString());

        return obj;
    }

    private List<GameObject> GetObjectsAtPosition(Vector2Int position)
    {
        List<GameObject> objs = new();
        var collisions = Physics2D.OverlapPointAll(position);
        foreach(var collision in collisions)
        {
            if (collision != null)
                objs.Add(collision.gameObject);
        }

        return objs;
    }

    private GameObject GetClosestObject(List<GameObject> objs)
    {
        if (objs.Count <= 0)
            return null;
        int dist = Utility.Distance(transform.position, objs[0].transform.position);
        GameObject closest = objs[0];

        foreach(var obj in objs)
        {
            int d = Utility.Distance(transform.position, obj.transform.position);
            if (d < dist)
            {
                dist = d;
                closest = obj;
            }
        }

        return closest;

    }

    public void PlayArrowSFX()
    {
        GetComponent<SFX>().PlayOneSpecific(0);
    } 

    public void PlayAttackSFX()
    {
        GetComponent<SFX>().PlayOneSpecific(1);
    } 

    public void PlayShieldBlockSFX()
    {
        GetComponent<SFX>().PlayOneSpecific(2);
    } 

    public void PlayTakingDamageSFX()
    {
        GetComponent<SFX>().PlayOneSpecific(3);
    } 

    public void PlayTorchSFX()
    {
        GetComponent<SFX>().PlayOneSpecific(4);
    } 



}
