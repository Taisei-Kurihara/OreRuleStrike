using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ターンが変更する時の処理。クリア判定の役割も持つ。
/// </summary>
public class GameMode_TurnChange : IGameMode
{
    private GameSessionManager GameSceneManager;
    public GameMode_TurnChange(GameSessionManager gameSceneManager)
    {
        GameSceneManager = gameSceneManager;
    }

    /// <summary>
    /// クリア判定後にターン変更の処理だけを行う。その時の行動
    /// </summary>
    void IGameMode.Init()
    {;

        GameSceneManager = GameSessionManager.Instance();
        GameSceneManager.CurrentTurnNum++;
        if (GameSceneManager.CurrentTurnNum >= GameSceneManager.TurnList.Count)
        {
            GameSceneManager.CurrentTurnNum = 0;
        }

        //モードをメインシステムに
        GameSceneManager.sceneContext.Mode_Change(new GameMode_MainMode(GameSceneManager));
    }

    void IGameMode.Update()
    {

    }


    void IGameMode.FixUpdate()
    {
    }

    void IGameMode.Exit()
    {

    }
    /// <summary>
    /// クリア判定
    /// </summary>
    private void Clear()
    {
        GameManager manager = GameManager.Instance();
        Debug.Log("クリア判定開始");

        //クリアしたプレイヤーを一時的に保存するものとする。
        List<PlayerSessionData> clear_player = new List<PlayerSessionData>();
        List<PlayerSessionData> normal_player = new List<PlayerSessionData>();
        //クリア判定
        foreach (var i in GameSceneManager.Session_Data)
        {
            if (i.Value.PlayerPoint >= manager.ClearPoint)
            {
                clear_player.Add(i.Value);
            }
            else if (i.Value.PlayerPoint < manager.ClearPoint)
            {
                normal_player.Add(i.Value);
            }
        }
        //クリアプレイヤーがいる場合、集計を行い、リザルトモードへ移行するようにする
        if (clear_player.Count > 0)
        {
            //点数の順番付け
            clear_player = clear_player.OrderBy(player => player.PlayerPoint).ToList();
            normal_player = normal_player.OrderBy(player => player.PlayerPoint).ToList();

            //ここで順位付けを先に行ってしまう。
            int num = 1;
            PlayerSessionData beforeplayer = null;
            int PaddingNum = 0;
            //PaddingPlayerへ変数を格納することで滞納判定
            List<PlayerSessionData> PaddingPlayer = new List<PlayerSessionData>();

            //クリア者から順位付けをしていく
            foreach (var i in clear_player)
            {
                // 同じ点数のプレイヤーがいるかどうかを確認
                if (beforeplayer != null && i.PlayerPoint == beforeplayer.PlayerPoint)
                {
                    PaddingPlayer.Add(i);  // 同率順位に追加
                    PaddingNum++;//同率順位であれば。
                }
                else
                {
                    // 同率順位のプレイヤーがいた場合、順位を更新
                    if (PaddingPlayer.Count > 0)
                    {
                        foreach (var x in PaddingPlayer)
                        {
                            GameManager.Variable_Data[x.PlayerId].Ranking = num;
                        }
                        PaddingPlayer.Clear();
                        num += PaddingNum;
                        PaddingNum = 0;
                    }
                    //同率順位もおらず、前回とは点数が違う場合（通常）
                    else
                    {
                        // 現在のプレイヤーを順位付け
                        GameManager.Variable_Data[i.PlayerId].Ranking = num;
                        num++; // 次の順位に進む
                    }
                }
                beforeplayer = i;
            }

            // 最後の同率順位の処理
            if (PaddingPlayer.Count > 0)
            {
                foreach (var x in PaddingPlayer)
                {
                    GameManager.Variable_Data[x.PlayerId].Ranking = num - 1;
                }
            }


            //クリアできなかった者も順位付け
            foreach (var i in normal_player)
            {
                // 同率順位処理を行う
                if (i.PlayerPoint == beforeplayer.PlayerPoint)
                {
                    PaddingPlayer.Add(i);  // 同率順位に追加
                    PaddingNum++;//同率順位であれば。
                }
                else
                {
                    // 同率順位のプレイヤーがいた場合、順位を更新
                    if (PaddingPlayer.Count > 0)
                    {
                        foreach (var x in PaddingPlayer)
                        {
                            GameManager.Variable_Data[x.PlayerId].Ranking = num;
                        }
                        PaddingPlayer.Clear();
                        num += PaddingNum;
                        PaddingNum = 0;
                    }
                    //同率順位もおらず、前回とは点数が違う場合（通常）
                    else
                    {
                        // 現在のプレイヤーを順位付け
                        GameManager.Variable_Data[i.PlayerId].Ranking = num;
                        num++; // 次の順位に進む
                    }
                }
                beforeplayer = i;
            }

            // 最後まで残った同率順位の処理
            if (PaddingPlayer.Count > 0)
            {
                foreach (var x in PaddingPlayer)
                {
                    GameManager.Variable_Data[x.PlayerId].Ranking = num - 1;
                }
            }

            //リザルトへ移行する
            GameSceneManager.sceneContext.Mode_Change(new GameMode_Result());
        }
        else
        {
            //ゲームモードを変更し、カード改変モードへ移行する。
            GameSceneManager.sceneContext.Mode_Change(new GameMode_ExchangeMode());
        }

    }
}
