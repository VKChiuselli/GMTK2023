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


    // Start is called before the first frame update
    protected virtual void Awake()
    {
        _spriteObject = transform.GetChild(0);
    }
    
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
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
                _isMoving = true;
            }
        }

        return _isMoving;
    }

    // Similar to move, but will try to move if there is a path regardless of distance
    public virtual void MoveTowards(Vector2Int target)
    {
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
                }

                // Get the last node in the path as the last node may not be the target pos
                transform.position = (Vector2)path[^1];
                _spriteObject.position = pos;
                StartCoroutine(MoveCoroutine(path));
                _isMoving = true;
            }
        }
    }

    public virtual void Interact(Vector2 target)
    {
        if (!_isMoving)
        {
            Utility.Round(target);
            // todo limit with layers
            List<Collider2D> potentials = new List<Collider2D>(Physics2D.OverlapBoxAll(target, Vector2.one * 0.8f, 0f)); // the *0.8f is to avoid catching nearby objects
            if (potentials.Count > 0)
            {
                foreach (var potentialInteractible in potentials)
                {
                    if (potentialInteractible.TryGetComponent(out InteractItem interactible))
                    {
                        if (interactible)
                        {
                            interactible.InteractWith(this);
                        }
                    }
                }
            }
        }
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
                                Debug.Log("TODO left animation");
                          
                            }
                            else
                            if (difference.x > 0 && difference.y == 0)
                            {
                                animator.SetTrigger("Right"); 
                                Debug.Log("TODO right animation");
                        
                            }
                            else
                            if (difference.x == 0 && difference.y > 0)
                            {
                                animator.SetTrigger("Back"); 
                                Debug.Log("TODO back animation");
                            
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


    public virtual void AILogic()
    { }

    public virtual void HoverInfo()
    { }
}
