using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class GameState : State
{
    protected GameController owner;

    protected virtual void Awake()
    {
        owner = GetComponent<GameController>();
    }
}