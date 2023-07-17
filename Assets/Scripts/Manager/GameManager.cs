using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoardManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager Inst;
    public PlayerInput inputActions;

    public Player PlayerUnit;
    public GameObject Cursor;

    //TODO an automated way to populate this
    public List<Unit> Heroes;
    public List<Unit> Enemies;

    public float MovementSpeed = 10;
    public TurnId CurrentTurn = TurnId.Player;

    private bool _isMovingUnits = false;
    private bool _isNextTurn = false;

    public Material HitMat;
    public GameObject SurprisedEffect;

    public GameOverScreen GameOverScreen;

    public enum TurnId
    {
        Player = 0,
        Hero = 1,
        Enemy = 2,
        Traps = 3
    }

    void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Destroy(this);
            return;
        }
        Inst = this;

        inputActions = new PlayerInput();
    }

    private void Start()
    {
        if (PlayerUnit == null)
            PlayerUnit = FindObjectOfType<Player>();


        // Auto populate enemy
        var objs = FindObjectsOfType<Unit>();
        foreach(var obj in objs)
        {
            if (obj is not Player && obj is not Hero && !Enemies.Contains(obj))
            {
                Enemies.Add(obj);
            }
        }
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        if (GameOverScreen.isActiveAndEnabled) return;
        MovementGridUpdate();
        HoverInfo();
        UnitController();
        CursorController();

        if (Keyboard.current.rKey.isPressed)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        if (Keyboard.current.escapeKey.isPressed)
        {
            //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }


    }

    // TODO if we update the persepctive, this might need to change.
    Vector2Int MousePositionToWorld()
    {
        Vector2 mousePosition = inputActions.Player.MouseLocation.ReadValue<Vector2>();
        return Utility.Round(Camera.main.ScreenToWorldPoint(mousePosition));
    }

    void MovementGridUpdate()
    {
        Vector2Int cursorPosition = MousePositionToWorld();
        BoardManager.Inst.MovementGridUpdate(PlayerUnit, cursorPosition);
    }


    void HoverInfo()
    {
        Vector2Int cursorPosition = MousePositionToWorld();

        var collision = Physics2D.OverlapPoint(cursorPosition);
        if (collision != null)
        {
            var unit = collision.GetComponent<Unit>();
            if (unit != null)
                unit.HoverInfo();
        }
    }

    void CursorController()
    {
        Vector2 position = inputActions.Player.MouseLocation.ReadValue<Vector2>();
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(position);
        Cursor.transform.position = (Vector2)Utility.Round(mousePosition);
    }

    void UnitController()
    {
        switch (CurrentTurn)
        {
            case TurnId.Player:
                PlayerControls();
                break;
            case TurnId.Hero:
                AIControls(Heroes);
                break;
            case TurnId.Enemy:
                AIControls(Enemies);
                break;
            case TurnId.Traps:
                TrapUpdates();
                break;
            default:
                break;
        }

        //Temp
        if (Input.GetKeyDown(KeyCode.Q))
            NextTurn();
    }

    void PlayerControls()
    {
        
        if (!_isMovingUnits)
        {
            if (Keyboard.current.spaceKey.isPressed)
            {
                _isNextTurn = true;
                PlayerUnit.Mana = 0;
                NextTurn();
                return;
            }
            bool click = inputActions.Player.Click.ReadValue<float>() > 0;
            Vector2 position = inputActions.Player.MouseLocation.ReadValue<Vector2>();
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(position);

            if (click)
            {
                _isMovingUnits = true;
                if (PlayerUnit.Move(Utility.Round(mousePosition)))
                {
                    _isNextTurn = true;
                }
            }
        }
        else if (!PlayerUnit._isMoving)
        {
            _isMovingUnits = false;
            if (PlayerUnit.Mana <= 0)
            {
                NextTurn();
            }
        }
    }

    void AIControls(List<Unit> units)
    {
        if (units != null)
        {
            if (!_isMovingUnits)
            {
                StartCoroutine(MoveUnits(units));
            }
        }
        else
            Debug.LogError("Units are null");
    }

    void TrapUpdates()
    {
        List<Trap> traps = new List<Trap>(FindObjectsOfType<Trap>());

        StartCoroutine(TrapCoroutine(traps));
    }

    IEnumerator TrapCoroutine(List<Trap> traps)
    {
        if (!_isMovingUnits)
        {
            _isMovingUnits = true;
            foreach (var trap in traps)
            {
                trap.Duration--;
                if (trap.Duration <= 0)
                    Destroy(trap.gameObject);
                yield return new WaitForSeconds(0.05f);
            }
            _isMovingUnits = true;
            yield return new WaitForSeconds(0.2f);
            _isMovingUnits = false;
            _isNextTurn = true;
            NextTurn();
        }
    }

    IEnumerator MoveUnits(List<Unit> units)
    {
        if (!_isMovingUnits)
        {
            _isMovingUnits = true;
            foreach (var unit in units)
            {
                if (unit != null)
                {
                    unit.AILogic();
                    yield return new WaitForSeconds(0.05f);
                }
            }

            while (_isMovingUnits)
            {
                yield return null;
                _isMovingUnits = false;
                foreach (var unit in units)
                {
                    if (unit != null && unit._isMoving)
                    {
                        _isMovingUnits = true;
                    }
                }
            }
            _isMovingUnits = true;
            yield return new WaitForSeconds(0.2f);
            _isMovingUnits = false;
            _isNextTurn = true;
            NextTurn();
        }
    }

    void NextTurn()
    {
        if (_isNextTurn)
        {
            CurrentTurn++;
            if (CurrentTurn > TurnId.Traps)
                CurrentTurn = TurnId.Player;
            _isNextTurn = false;

            if (CurrentTurn == TurnId.Player)
            {
                PlayerUnit.RegenerateMana();
                PlayerUnit.SetAbilityWalk();
            }
        }
    }

    public void SkipTurn()
    {
        if (CurrentTurn == TurnId.Player)
        {
            _isNextTurn = true;
            NextTurn();
        }
    }

    public void GameOverHeroDeath()
    {
        string reason = "My son died!";
        // if turn is player
        if (CurrentTurn == TurnId.Player)
        {
            reason = "How could you make me kill him?!";
        }
        GameOverScreen.SetGameOverReason(reason);
        GameOverScreen.gameObject.SetActive(true);
    }

    public void GameOverHeroSawParent()
    {
        GameOverScreen.SetGameOverReason("He saw me!");
        GameOverScreen.gameObject.SetActive(true);
    }
}
