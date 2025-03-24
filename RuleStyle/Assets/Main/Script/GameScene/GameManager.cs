using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームマネージャー
/// </summary>
public class GameManager : SingletonMonoBehaviourBase<GameManager>
{
    [Header("プレイヤーの人数")]
    public int PlayerNum = 4;

    /// <summary>
    /// 永続データ
    /// </summary>
    public static Dictionary<int, variable_playerdata> Variable_Data { get; private set; }


    /// <summary>
    /// クリアする為の点数
    /// </summary>
    public int ClearPoint = 5;

    /// <summary>
    /// クリアしたプレイヤーを保存する。
    /// </summary>
    public List<variable_playerdata> ClearPlayer=new List<variable_playerdata>();
    /// <summary>
    /// 初期化
    [RuntimeInitializeOnLoadMethod()]
    public static void Init()
    {
        Instance().VariableDataInit();
    }

    /// <summary>
    /// プレイヤー人数の変更
    /// </summary>
    /// <param name="playernum"></param>
    public void PlayerNumChange(int playernum)
    {
        PlayerNum = playernum;
    }
    /// <summary>
    /// プレイヤーのデータを初期化
    /// </summary>
    public void VariableDataInit()
    {
        Variable_Data = new Dictionary<int, variable_playerdata>()
        {
            { 1, new variable_playerdata(1,"Player1") }
            ,
            { 2, new variable_playerdata(2,"Player2") }
            ,
            { 3, new variable_playerdata(3,"Player3") }
            ,
            { 4, new variable_playerdata(4,"Player4") }
        };
    }


}

public enum GameMode
{
    PlayerOnly
}