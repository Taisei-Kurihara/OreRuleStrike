using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ���U���g�ֈڍs����(������ɑS�ď����������Ă���������Α�̓����܂��B
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
