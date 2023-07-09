using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int MaxMovement = 3;
    public bool _isMoving = false;

    [SerializeField]
    private Vector2Int _currentLookDirection = Vector2Int.down;
    private Transform _spriteObject;

    private SpriteRenderer _spriteRenderer;
    private Material _defaultMat;
    private Material HitMat;


    // Start is called before the first frame update
    protected virtual void Awake()
    {
        _spriteObject = transform.GetChild(0);
        _spriteRenderer = _spriteObject.GetComponent<SpriteRenderer>();
        _defaultMat = _spriteRenderer.material;
    }
    
    protected virtual void Start()
    {
        HitMat = GameManager.Inst.HitMat;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public virtual void Hit(Unit other)
    {
        _spriteRenderer.material = HitMat;
        Death(other);
        CancelInvoke(nameof(ResetMaterial));
        Invoke(nameof(ResetMaterial), 0.2f);
    }

    private void ResetMaterial()
    {
        _spriteRenderer.material = _defaultMat;
        Destroy(gameObject);
    }

    public virtual bool Move(Vector2Int target)
    {
        if (!_isMoving)
        {
            List<Vector2Int> path = GameManager.Inst.FindPath(_spriteObject.position, target, MaxMovement);

            if (path != null)
            {
                Vector3 pos = transform.position;
                // Get the last node in the path as the last node may not be the target pos
                transform.position = (Vector2)path[^1];
                _spriteObject.position = pos;

                StartCoroutine(MoveCoroutine(path));
            }
        }

        return _isMoving;
    }

    // Similar to move, but will try to move if there is a path regardless of distance

    /// <summary>
    /// Returns true if the unit can move to the target
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool MoveTowards(Vector2Int target)
    {
        bool canMoveToTarget = false;
        if (!_isMoving)
        {
            
            List<Vector2Int> path = GameManager.Inst.FindPath(_spriteObject.position, target, 1000);

            if (path != null)
            {
                Vector3 pos = transform.position;
                // Move as much as your max Movement
                if (path.Count > MaxMovement)
                {
                    path.RemoveRange(MaxMovement, path.Count - MaxMovement);
                    canMoveToTarget = false;
                }
                else
                {
                    canMoveToTarget = true;
                }

                // Get the last node in the path as the last node may not be the target pos
                transform.position = (Vector2)path[^1];
                _spriteObject.position = pos;
                StartCoroutine(MoveCoroutine(path));
            }
        }
        return canMoveToTarget;
    }

    // Moves the sprite child instead
    public IEnumerator MoveCoroutine(List<Vector2Int> path)
    {
        if (!_isMoving)
        {
            // Pathfinding

            _isMoving = true;

           
            Animator animator = transform.GetChild(0).GetComponent<Animator>();
            foreach (var pos in path)
            {
                Vector2 difference = pos - (Vector2)_spriteObject.position;
                while (difference.sqrMagnitude > Vector2.kEpsilon)
                {

                    _spriteObject.position = Vector3.MoveTowards(_spriteObject.position, (Vector2)pos, Time.deltaTime * GameManager.Inst.MovementSpeed);
                    yield return null;
                    difference = pos - (Vector2)_spriteObject.position;

                    if (gameObject.transform.GetChild(0).GetComponent<Animator>())
                    {

                        _spriteObject.position = Vector3.MoveTowards(_spriteObject.position, (Vector2)pos, Time.deltaTime * GameManager.Inst.MovementSpeed);
                        yield return null;
                        difference = pos - (Vector2)_spriteObject.position;
                        _currentLookDirection = Utility.Round(difference);

                        if (animator)
                        {
                            
                            if (difference.x < 0 && difference.y == 0)
                            {

                                animator.SetTrigger("Left"); 
                                //Debug.Log("TODO left animation");
                          
                            }
                            else
                            if (difference.x > 0 && difference.y == 0)
                            {
                                animator.SetTrigger("Right"); 
                                //Debug.Log("TODO right animation");
                        
                            }
                            else
                            if (difference.x == 0 && difference.y > 0)
                            {
                                animator.SetTrigger("Back"); 
                                //Debug.Log("TODO back animation");
                            
                            }
                            else
                            if (difference.x == 0 && difference.y < 0)
                            {
                                animator.SetTrigger("Front");
                                    
                                //we are going left
                            }

                        }
                    }  
                }

                _spriteObject.position = (Vector2)pos;
            }
            if (GetComponent<SFX>() != null)
            {
                GetComponent<SFX>().PlayThirdEffect();//the third effect is always a movevement sound
            }
            _isMoving = false; 
            
        }
    }


    public virtual void Death(Unit causeOfDeath)
    { }
    
    public virtual void AILogic()
    { }

    public virtual void HoverInfo()
    { }

    public virtual void Attack(Unit other)
    {
        StartCoroutine(AttackCoroutine(other.transform.position));
        other.Hit(this);
    }

    protected IEnumerator AttackCoroutine(Vector2 target)
    {
        Vector2 difference = target - (Vector2)_spriteObject.position;
        while (difference.sqrMagnitude > Vector2.kEpsilon)
        {
            _spriteObject.position = Vector3.MoveTowards(_spriteObject.position, target, Time.deltaTime * GameManager.Inst.MovementSpeed);
            yield return null;
            difference = target - (Vector2)_spriteObject.position;
        }

        target = transform.position;
        difference = target - (Vector2)_spriteObject.position;
        while (difference.sqrMagnitude > Vector2.kEpsilon)
        {
            _spriteObject.position = Vector3.MoveTowards(_spriteObject.position, target, Time.deltaTime * GameManager.Inst.MovementSpeed);
            yield return null;
            difference = target - (Vector2)_spriteObject.position;
        }

        _spriteObject.position = transform.position;
    }


    protected bool TargetClosest<T>(List<GameObject> interactable)
    {
        GameObject closest = GetClosestObject(interactable.FindAll(x => x.GetComponent<T>() != null));
        if (closest != null)
        {
            if (Utility.Distance(transform.position, closest.transform.position) <= 1)
            {
                T component = closest.GetComponent<T>();
                InteractableEntity item = component as InteractableEntity;
                if (item != null)
                {
                    item.InteractWith(this);
                }
                Unit u = component as Unit;
                if (u != null && u!= this)
                {
                    Attack(u);
                }
            }
            else
                MoveTowards(Utility.Round(closest.transform.position));
            return true;
        }
        return false;
    }

    public List<GameObject> GetObjectsInRange(bool showRange, int maxDist, bool needVisibility)
    {
        List<GameObject> obj = new List<GameObject>();
        Vector2Int startPos = Utility.Round(transform.position);


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
                if (neighbour.IsVisible != GameManager.Tile.Visibility.Visible && needVisibility) continue;
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

    protected List<GameObject> GetObjectsAtPosition(Vector2Int position)
    {
        List<GameObject> objs = new();
        var collisions = Physics2D.OverlapPointAll(position);
        foreach (var collision in collisions)
        {
            if (collision != null)
                objs.Add(collision.gameObject);
        }

        return objs;
    }

    protected Unit GetUnitAtPoint(Vector2Int position)
    {
        Unit unit = null;
        var collisions = Physics2D.OverlapPointAll(position);
        foreach (var collision in collisions)
        {

            if (collision != null)
            {
                var u = collision.gameObject.GetComponent<Unit>();
                if (u != null)
                {
                    unit = u;
                    break;
                }
            }
                
        }

        return unit;
    }

    protected GameObject GetClosestObject(List<GameObject> objs)
    {
        if (objs.Count <= 0)
            return null;
        int dist = Utility.Distance(transform.position, objs[0].transform.position);
        GameObject closest = objs[0];

        foreach (var obj in objs)
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
}
