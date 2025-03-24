using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 計算方法：引く
/// </summary>
public class Card_Green_Minus : ICard
{
    public PlayerSessionData PlayerData { get; set; } = null;

    public float? ProbabilityNum => 40;
    Card_Pattern ICard.card_pattern => Card_Pattern.Green;

    /// <summary>
    /// カード名
    /// </summary>
    string ICard.CardName => "-";

    Sprite ICard.cardUI { get; set; }

    /// <summary>
    /// 
    /// </summary>
    void ICard.CardNum()
    {
    }
}
