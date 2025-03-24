using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 計算方法:掛け算(a版では実装しない。
/// </summary>
public class Card_Green_Multiplication : ICard
{
    public PlayerSessionData PlayerData { get; set; } = null;

    public float? ProbabilityNum => 3;
    Card_Pattern ICard.card_pattern => Card_Pattern.Yellow;

    /// <summary>
    /// カード名
    /// </summary>
    string ICard.CardName => "×";

    Sprite ICard.cardUI { get; set; }

    /// <summary>
    /// 
    /// </summary>
    void ICard.CardNum()
    {

    }
}
