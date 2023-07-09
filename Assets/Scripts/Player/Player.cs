using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : Unit
{
    public int MaxMana = 3;
    public int Mana = 5;
    public Abilities CurrentAbility = Abilities.Walk;
    //public RectTransform ManaImage;
    public bool IgnoreWalls = false;
    public bool NeedLineOfSight = false;

    public GameObject FireBallObj;
    public GameObject WindObj;
    public GameObject TeleportEffect;

    public TMP_Text Description;

    protected override void Start()
    {
        base.Start();
        Mana = MaxMana;
        SetAbilityWalk();
    }

    public enum Abilities
    {
        Walk,
        FireBall,
        SpotLight,
        Teleport,
        Taunt,
        Wind
    }

    protected override void Update()
    {
        base.Update();
     //   ManaImage.sizeDelta = new Vector2(Mana * 100, 100);
    }

    public override void Death(Unit causeOfDeath)
    {
        //Player shoulnt die
    }

    public override bool Move(Vector2Int target)
    {
        bool didMove = false;
        switch(CurrentAbility)
        {
            case Abilities.Walk:
                var prePos = transform.position;
                didMove = base.Move(target);
                if (didMove)
                    Mana -= Utility.Distance(target, prePos);
                break;
            case Abilities.Teleport:
                didMove = Teleport(target);
                if (didMove)
                    Mana -= 2;
                break;
            case Abilities.FireBall:
                didMove = Fireball(target);
                if (didMove)
                    Mana -= 3;
                break;
            case Abilities.Wind:
                didMove = WindGust(target);
                if (didMove)
                    Mana -= 1;
                break;
        }


        if (didMove)
        {
            SetAbilityWalk();
        }

        return didMove;
    }

    public void SetAbilityWalk()
    {
        CurrentAbility = Abilities.Walk;
        IgnoreWalls = false;
        NeedLineOfSight = false;
        MaxMovement = Mana;
        SetDescription();
    }

    public void SetAbilityFireBall()
    {
        if (CurrentAbility == Abilities.FireBall)
        {
            SetAbilityWalk();
        }
        else
        {
            IgnoreWalls = true;
            NeedLineOfSight = true;
            CurrentAbility = Abilities.FireBall;
            MaxMovement = 4;
            SetDescription();
        }
    }

    public void SetAbilitySpotlight()
    {
        if (CurrentAbility == Abilities.SpotLight)
        {
            SetAbilityWalk();
        }
        else
        {
            IgnoreWalls = true;
            NeedLineOfSight = false;
            CurrentAbility = Abilities.SpotLight;
            SetDescription();
        }
    }

    public void SetAbilityTeleport()
    {
        if (CurrentAbility == Abilities.Teleport)
        {
            SetAbilityWalk();
        }
        else
        {
            IgnoreWalls = true;
            NeedLineOfSight = false;
            CurrentAbility = Abilities.Teleport;
            MaxMovement = Mana;
            SetDescription();
        }
    }

    public void SetAbilityTaunt()
    {
        if (CurrentAbility == Abilities.Taunt)
        {
            SetAbilityWalk();
        }
        else
        {
            IgnoreWalls = false;
            NeedLineOfSight = true;
            CurrentAbility = Abilities.Taunt;
            MaxMovement = 5;
            SetDescription();
        }
    }

    public void SetAbilityWind()
    {
        if (CurrentAbility == Abilities.Wind)
        {
            SetAbilityWalk();
        }
        else
        {
            IgnoreWalls = true;
            NeedLineOfSight = true;
            CurrentAbility = Abilities.Wind;
            MaxMovement = 5;
            SetDescription();
        }
    }

    public void RegenerateMana()
    {
        Mana = MaxMana;
    }

    private bool Teleport(Vector2Int target)
    {
        if (!_isMoving)
        {
            if (GameManager.Inst.Grid.ContainsKey(target) &&
               !GameManager.Inst.Grid[target].Walkable.HasFlag(GameManager.Tile.TileStatus.Blocked) &&
                Utility.Distance(transform.position, target) <= MaxMovement
                )
            {
                PlayTeleportSFX();
                StartCoroutine(TeleportCoroutine(target));
                _isMoving = true;
            }
        }
        return _isMoving;
    }

    private IEnumerator TeleportCoroutine(Vector2Int target)
    {
        if (!_isMoving)
        {
            _isMoving = true;
            Instantiate(TeleportEffect, (Vector2)target, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            transform.position = (Vector2)target;
            _isMoving = false;
        }
    }

    private bool Fireball(Vector2Int target)
    {
        if (!_isMoving)
        {
            if (GameManager.Inst.Grid.ContainsKey(target) &&
                GameManager.Inst.Grid[target].Walkable != GameManager.Tile.TileStatus.Blocked &&
                Utility.Distance(transform.position, target) <= MaxMovement &&
                !GameManager.Inst.LineOfSightBlocked(Utility.Round(transform.position), target)
                )
            {
                Unit unit = GetUnitAtPoint(target);
                if (unit != null && unit!= this)
                {
                    unit.Hit(this);
                }
                PlayFireSFX();
                StartCoroutine(FireballCoroutine(target));
                _isMoving = true;
            }
        }
        return _isMoving;
    }

    private IEnumerator FireballCoroutine(Vector2Int target)
    {
        if (!_isMoving)
        {
            _isMoving = true;
            Instantiate(FireBallObj, (Vector2)target, Quaternion.identity);
            yield return new WaitForSeconds(1);
            _isMoving = false;
        }
    }

    private bool WindGust(Vector2Int target)
    {
        if (!_isMoving)
        {
            if (GameManager.Inst.Grid.ContainsKey(target) &&
               GameManager.Inst.Grid[target].Walkable != GameManager.Tile.TileStatus.Blocked &&
                Utility.Distance(transform.position, target) <= MaxMovement &&
                !GameManager.Inst.LineOfSightBlocked(Utility.Round(transform.position), target)
                )
            {
                PlayWindSFX();
                StartCoroutine(WindCoroutine(target));
                _isMoving = true;
            }
        }
        return _isMoving;
    }

    private IEnumerator WindCoroutine(Vector2Int target)
    {
        if (!_isMoving)
        {
            _isMoving = true;
            Instantiate(WindObj, (Vector2)target, Quaternion.identity);
            // Destroys any traps on tile
            var collisions = Physics2D.OverlapPointAll(target);
            foreach (var collision in collisions)
            {
                if (collision != null)
                {
                    if (collision.GetComponent<Trap>() != null)
                        Destroy(collision.gameObject);
                }
            }
            // Pushes units 1 tile away
            Vector2Int[] offset = new Vector2Int[]
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            foreach(var i in offset)
            {
                collisions = Physics2D.OverlapPointAll(target + i);
                foreach (var collision in collisions)
                {
                    if (collision != null)
                    {
                        var unit = collision.GetComponent<Unit>();
                        if (unit != null)
                        {
                            unit.Move(Utility.Round(unit.transform.position) + i);
                        }
                        if (collision.GetComponent<Trap>() != null)
                            Destroy(collision.gameObject);
                    }
                }
            }
            yield return new WaitForSeconds(1);
            _isMoving = false;
        }
    }

    private void SetDescription()
    {
        switch(CurrentAbility)
        {
            case Abilities.Walk:
                Description.text = "Move [x mana]: x = Number of spaces moved";
                break;
            case Abilities.FireBall:
                Description.text = "FireBall [3 mana]: Lights a Tile on fire";
                break;
            case Abilities.Teleport:
                Description.text = "Teleport [2 mana]: Teleports self to a tile";
                break;
            case Abilities.Wind:
                Description.text = "Wind Gust [1 mana]: Blows wind on a targeted tile";
                break;
        }
    }

    public void PlayFireSFX()
    {
        GetComponent<SFX>().PlayFirstEffect();
    }

    public void PlayWindSFX()
    {
        GetComponent<SFX>().PlaySecondEffect();
    }

    public void PlayTeleportSFX()
    {
        GetComponent<SFX>().PlayOneSpecific(0);
    }
}
