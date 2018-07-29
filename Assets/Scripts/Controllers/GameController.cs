using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : StateMachine {
    public MainMenuController mainMenuController;
    public BattleController battleController;
    public GameMenuController gameMenuController;

    void Start()
    {
        ChangeState<InitGameState>();
    }
}
