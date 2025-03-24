using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 変更する為に『どのプレイヤーのどのカードを選択しているか』
/// を指す
/// </summary>
public class ChangeData
{
    /// <summary>
    /// どのプレイヤーか
    /// </summary>
    public PlayerSessionData data;
    /// <summary>
    /// ルールのどの部分を選択しているかを指す
    /// </summary>
    public int Select_CardNum;

}