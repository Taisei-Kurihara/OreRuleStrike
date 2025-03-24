using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 自分以外
/// </summary>
public class Card_Red_Other_than : ICard, ICard_Red
{
    public PlayerSessionData PlayerData { get; set; } = null;

    public float? ProbabilityNum => 30;
    Card_Pattern ICard.card_pattern => Card_Pattern.Red;

    /// <summary>
    /// カード名
    /// </summary>
    string ICard.CardName => "自分以外の";

    /// <summary>
    /// カードBlueの時のみの実装となる。
    /// </summary>
    public List<int> EffectMember => _effectMember;

    private List<int> _effectMember = new List<int>();
    Sprite ICard.cardUI { get; set; }
    /// <summary>
    /// 青は全て返り値で効果を行う
    /// </summary>
    void ICard.CardNum()
    {
        GameSessionManager gameManager = GameSessionManager.Instance();
        foreach (var i in gameManager.Session_Data)
        {
            if (i.Key != PlayerData.PlayerId)
            {
                //
                EffectMember.Add(i.Key);
            }
        }

    }
}
