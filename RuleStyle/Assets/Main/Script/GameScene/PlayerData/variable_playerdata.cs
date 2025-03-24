using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可変可能なプレイヤーデータ(名前等）
/// 全シーンでの共通データを保存する為のクラスとして使用する（GameManagerで保管しておく）
/// </summary>
public class variable_playerdata
{
    /// <summary>
    /// 初期化数値
    /// </summary>
    /// <param name="NewName"></param>
    public variable_playerdata(int ID, string NewName)
    {
        Id = ID;
        PlayerName = NewName;
    }
    public int Id;
    public string PlayerName;
    /// <summary>
    /// 順位付けを保存する変数
    /// </summary>
    public int Ranking = 0;
}