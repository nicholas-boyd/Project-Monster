using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InitBattleState : BattleState
{
    public override void Enter()
    {
        base.Enter();
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        board.Load(levelData);
        Point p = new Point((int)levelData.tiles[1].x, (int)levelData.tiles[1].y);
        SelectTile(p);
        SpawnTestUnits();
        owner.battleClockController.Activate();
        yield return null;
        owner.ChangeState<CombatBattleState>();
    }
    void SpawnTestUnits()
    {
        string[] enemies = new string[]
        {
            "TestDummyGod",
            "TestDummyWeak",
            "TestDummyEqual"
        };
        List<Tile> locations = new List<Tile>(board.tiles.Values);
        for (int i = 0; i < locations.Count; ++i)
        {
            if (locations[i].pos.x != 5 || locations[i] == null)
            {
                locations.Remove(locations[i]);
                if (i >= 0)
                    i--;
            }
        }
        for (int i = 0; i < enemies.Length; ++i)
        {
            GameObject instance = UnitFactory.Create(enemies[i]);
            instance.transform.SetParent(owner.transform.Find("NPCs"));
            int random = UnityEngine.Random.Range(0, locations.Count);
            Tile randomTile = locations[random];
            locations.RemoveAt(random);
            Unit unit = instance.GetComponent<Unit>();
            unit.Place(randomTile);
            unit.dir = Directions.North;
            unit.Match();
            enemyUnits.Add(unit);
        }
        GameObject heroInstance = UnitFactory.Create("Hero");
        heroInstance.transform.SetParent(owner.transform.Find("Player"));
        Unit heroUnit = heroInstance.GetComponent<Unit>();
        heroUnit.Place(board.GetTile(new Point(0, 1)));
        heroUnit.Match();
        heroUnit.transform.rotation = Quaternion.Euler(0,180,0);
        heroUnit.GetComponent<Mana>().MMP = 1000;
        heroUnit.GetComponent<Mana>().MP = 1000;
        owner.playerUnit = heroUnit;
        SelectTile(heroUnit.tile.pos);
        heroUnit.GetComponent<Health>().HP -= 50;
        Debug.Log("Player Mana for testing: " + owner.playerUnit.GetComponent<Mana>().MP);
    }
}