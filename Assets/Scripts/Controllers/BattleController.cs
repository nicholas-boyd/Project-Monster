using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BattleController : StateMachine {
	public CameraDrone cameraDrone;
	public Board board;
	public LevelData levelData;
	public Point pos;
	public Unit playerUnit;
	public Tile currentTile { get { return board.GetTile(pos); }}
	public AbilityHandController abilityHandController;
	public List<Unit> enemyUnits = new List<Unit>();
    public StatPanelController statPanelController;
    public InventoryPanelController inventoryPanelController;
    public BattleMessageController battleMessageController;
    public BattleClockController battleClockController;

    public void Activate () {
		ChangeState<InitBattleState>();
	}
}