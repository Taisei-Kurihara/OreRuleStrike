using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 計算方法：足し算
/// </summary>
public class Card_Green_Plus : ICard
{
    public PlayerSessionData PlayerData { get; set; } = null;

    /// <summary>
    /// 基準カードの為Null
    /// </summary>
    public float? ProbabilityNum => null;
    Card_Pattern ICard.card_pattern => Card_Pattern.Green;

    string ICard.CardName => "+";

    Sprite ICard.cardUI { get; set; }
    /// <summary>
    /// PlayerData
    /// </summary>
    void ICard.CardNum()
    {
        Debug.Log("+カード");
        if (PlayerData != null)
        {

        }
    }
}
