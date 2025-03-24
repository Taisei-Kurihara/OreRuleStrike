using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Card_Red_EffectOne : ICard, ICard_Red
{
    public PlayerSessionData PlayerData { get; set; } = null;

    public float? ProbabilityNum => 50;
    Card_Pattern ICard.card_pattern => Card_Pattern.Red;

    /// <summary>
    /// カード名
    /// </summary>
    string ICard.CardName => "P1の";
    Sprite ICard.cardUI { get; set; }
    /// <summary>
    /// カードBlueの時のみの実装となる。
    /// </summary>
    public List<int> EffectMember => new List<int> { 1 };

    void ICard.CardNum()
    {
    }
}
