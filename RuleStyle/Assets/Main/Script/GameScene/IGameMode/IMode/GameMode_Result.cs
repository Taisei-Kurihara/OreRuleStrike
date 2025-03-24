using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// リザルトへ移行する(こちらに全て処理を書いていただければ大体動きます。
/// </summary>
public class GameMode_Result : IGameMode
{
    void IGameMode.Exit()
    {
    }

    void IGameMode.FixUpdate()
    {
    }

    void IGameMode.Init()
    {
        UISceneManager uISceneManager = UISceneManager.Instance();
        uISceneManager.ChangeScene(Call.Result);
    }

    void IGameMode.Update()
    {
    }
}
