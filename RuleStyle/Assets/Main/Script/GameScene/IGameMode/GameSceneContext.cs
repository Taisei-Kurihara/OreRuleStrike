using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneContext
{
    private IGameMode _beforeMode;
    public IGameMode _currentgameMode;
    public void Mode_Change(IGameMode mode)
    {
        _beforeMode = _currentgameMode;
        _beforeMode?.Exit();

        _currentgameMode = mode;
        _currentgameMode.Init();
    }
}
