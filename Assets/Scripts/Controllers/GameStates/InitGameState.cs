using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitGameState : GameState {
    
    public override void Enter()
    {
        base.Enter();
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        while (!owner.mainMenuController.hidden)
        {
            yield return null;
        }
        owner.ChangeState<GameMenuState>();
    }

    
}
