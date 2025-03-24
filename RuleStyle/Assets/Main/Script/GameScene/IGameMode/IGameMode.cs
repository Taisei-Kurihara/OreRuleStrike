using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーが行う行為のState
/// </summary>
public interface IGameMode
{
    void Init();
    void Update();
    void FixUpdate();
    void Exit();
}
