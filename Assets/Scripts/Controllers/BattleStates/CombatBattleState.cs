using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class CombatBattleState : BattleState
{
    public const string CheckSelectionTargetNotification = "CHECK_SELECTION";
    
    AbilityHandController hand;
    bool canMove = true;
    bool canFire = true;


    public override void Enter()
    {
        base.Enter();
        RefreshStatPanel(owner.playerUnit.tile.pos, 0);
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            RefreshStatPanel(enemyUnits[i].tile.pos, i + 1);
        }
        hand = owner.abilityHandController;
        SelectTile(owner.playerUnit.tile.pos);
        hand.ActivatePanel();
    }

    public override void Exit()
    {
        base.Exit();
    }

    protected override void AddListeners()
    {
        base.AddListeners();
        this.AddObserver(RefreshPanel, Stats.UnitUpdateNotification);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        this.RemoveObserver(RefreshPanel, Stats.UnitUpdateNotification);
    }

    protected override void OnMove(object sender, object args)
    {
        if (canMove)
        {
            Point tilePos = owner.playerUnit.tile.pos + (Point)args;
            List<Tile> tiles = playerUnit.GetComponent<Movement>().GetTilesInRange(board);
            foreach (Tile t in tiles)
            {
                if (tilePos == t.pos)
                {
                    try
                    {
                        if (board.tiles[tilePos] != null && board.tiles[tilePos].pos.x < board.width / 2)
                        {
                            canMove = false;
                            canFire = false;
                            SelectTile(tilePos);
                            StartCoroutine("MoveSequence");
                            hand.RefreshMarkers();
                        }
                    }
                    catch (KeyNotFoundException k)
                    {
                        Debug.Log("Player could not move to point " + tilePos.ToString() + " because it's outside the playable area. " + k.Message);
                    }
                }
            }
        }
    }

    protected override void OnFire(object sender, object args)
    {
        List<int> activePosition = new List<int>() { owner.abilityHandController.selection };
        this.PostNotification(CheckSelectionTargetNotification, activePosition);
        if (canFire && !owner.inventoryPanelController.Show)
        {
            if (args.Equals(1))
            {
                canFire = false;
                canMove = false;
                if (activePosition[activePosition.Count - 1] < hand.handSize)
                    hand.SetSelection(activePosition[activePosition.Count - 1]);
                hand.ActivateSelection();
                canFire = true;
                canMove = true;
            }
        }
    }

    private void RefreshPanel(object sender, object args)
    {
        Unit target = (Unit)args;
        if (target == playerUnit)
            RefreshStatPanel(target.tile.pos, 0);
        else
            RefreshStatPanel(target.tile.pos, GetStatPanelIndex(target));
    }

    IEnumerator MoveSequence()
    {
        yield return StartCoroutine(playerUnit.GetComponent<Movement>().Traverse(owner.currentTile));
        canMove = true;
        canFire = true;
    }
}
