using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card_Purple_One : ICard
{
    public PlayerSessionData PlayerData { get; set; } = null;

    /// <summary>
    /// 基準カードの為Null
    /// </summary>
    public float? ProbabilityNum => null;
    Card_Pattern ICard.card_pattern => Card_Pattern.Purple;

    /// <summary>
    /// カード名
    /// </summary>
    string ICard.CardName => "1";
    Sprite ICard.cardUI { get; set; }
    /// <summary>
    /// 
    /// </summary>
    void ICard.CardNum()
    {
        if (PlayerData != null)
        {
            PlayerData.RuleSuccessNum = 1;
        }
    }
}
