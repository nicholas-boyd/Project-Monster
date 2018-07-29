using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class BattleState : State
{
    protected BattleController owner;
    public CameraDrone cameraDrone { get { return owner.cameraDrone; } }
    public Board board { get { return owner.board; } }
    public LevelData levelData { get { return owner.levelData; } }
    public Point pos { get { return owner.pos; } set { owner.pos = value; } }
    public AbilityHandController abilityHandController { get { return owner.abilityHandController; } }
    public Unit playerUnit { get { return owner.playerUnit; } }
    public List<Unit> enemyUnits { get { return owner.enemyUnits; } }
    public StatPanelController statPanelController { get { return owner.statPanelController; } }

    //TESTING
    private void Update()
    {
        /*if (cycletime < Time.time)
        {
            cycletime += 1;
            RefreshStatPanel(owner.playerUnit.tile.pos, 0);
            for (int i = 0; i < enemyUnits.Count; i++)
            {
                RefreshStatPanel(owner.enemyUnits[i].tile.pos, i+1);
            }
        }*/
    }

    protected virtual void Awake()
    {
        owner = GetComponent<BattleController>();
    }

    protected override void AddListeners()
    {
        this.AddObserver(OnMove, InputController.Movement);
        this.AddObserver(OnFire, InputController.MouseUpNotification);
    }

    protected override void RemoveListeners()
    {
        this.RemoveObserver(OnMove, InputController.Movement);
        this.RemoveObserver(OnFire, InputController.MouseUpNotification);
    }

    protected virtual void OnMove(object sender, object args)
    {

    }

    protected virtual void OnFire(object sender, object args)
    {

    }

    protected virtual Unit GetUnit(Point p)
    {
        Tile t = board.GetTile(p);
        GameObject content = t != null ? t.content : null;
        return content != null ? content.GetComponent<Unit>() : null;
    }

    protected virtual void RefreshStatPanel(Point p, int index)
    {
        Unit target = GetUnit(p);
        if (target != null)
            statPanelController.ShowInPanel(target.gameObject, index);
        else
            statPanelController.HidePanel(index);
    }

    protected virtual int GetStatPanelIndex(Unit target)
    {
        for(int i = 0; i < statPanelController.Count; i++)
        {
            if (enemyUnits[i] == target)
                return i+1;
        }
        return statPanelController.Count-1;
    }

    /*protected virtual void RefreshPrimaryStatPanel(Unit target)
    {
        if (target != null)
            statPanelController.ShowPrimary(target.gameObject);
        else
            statPanelController.HidePrimary();
    }

    protected virtual void RefreshSecondaryStatPanel(Point p)
    {
        Unit target = GetUnit(p);
        if (target != null)
            statPanelController.ShowSecondary(target.gameObject);
        else
            statPanelController.HideSecondary();
    }*/

    protected virtual void SelectTile(Point p)
    {
        if (pos == p || !board.tiles.ContainsKey(p))
            return;
        pos = p;
    }
}