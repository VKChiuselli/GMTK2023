using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public int Mana = 3;
    public Abilities CurrentAbility = Abilities.Walk;
    public RectTransform ManaImage;
    public bool IgnoreWalls = false;

    public enum Abilities
    {
        Walk,
        FireBall,
        SpotLight,
        Teleport,
        Taunt
    }

    protected override void Update()
    {
        base.Update();
        ManaImage.sizeDelta = new Vector2(Mana * 100, 100);
    }

    public override bool Move(Vector2Int target)
    {
        bool didMove = false;
        switch(CurrentAbility)
        {
            case Abilities.Walk:
                didMove = base.Move(target);
                break;
            case Abilities.Teleport:
                didMove = Teleport(target);
                break;

        }


        if (didMove)
        {
            Mana -= 1;
            SetAbilityWalk();
        }

        return didMove;
    }

    public void SetAbilityWalk()
    {
        CurrentAbility = Abilities.Walk;
        IgnoreWalls = false;
    }

    public void SetAbilityFireBall()
    {
        CurrentAbility = Abilities.FireBall;
    }

    public void SetAbilitySpotlight()
    {
        CurrentAbility = Abilities.SpotLight;
    }

    public void SetAbilityTeleport()
    {
        IgnoreWalls = true;
        CurrentAbility = Abilities.Teleport;
    }

    public void SetAbilityTaunt()
    {
        CurrentAbility = Abilities.Taunt;
    }

    public void RegenerateMana()
    {
        Mana = 3;
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
                StartCoroutine(TeleportCoroutine(target));
                _isMoving = true;
                Mana -= 1;
            }
        }
        return _isMoving;
    }

    private IEnumerator TeleportCoroutine(Vector2Int target)
    {
        if (!_isMoving)
        {
            _isMoving = true;
            yield return new WaitForSeconds(1);
            transform.position = (Vector2)target;
            _isMoving = false;
        }
    }
}
