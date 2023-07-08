using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int MaxMovement = 3;
    private bool _isMoving = false;
    private Transform _spriteObject;

    // Start is called before the first frame update
    protected void Awake()
    {
        _spriteObject = transform.GetChild(0);
    }

    // Update is called once per frame
    protected void Update()
    {
        
    }

    public virtual void Move(Vector2 target)
    {
        if (!_isMoving)
        {
            Utility.Round(target);
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
    }

    // Moves the sprite child instead
    public IEnumerator MoveCoroutine(List<Vector2Int> path)
    {
        if (!_isMoving)
        {
            // Pathfinding

            if (path != null)
            {
                _isMoving = true;

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
                            Animator animator = gameObject.transform.GetChild(0).GetComponent<Animator>();

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

                    _spriteObject.position = (Vector2)pos;
                }
                _isMoving = false; 
            }
        }
    }
}
