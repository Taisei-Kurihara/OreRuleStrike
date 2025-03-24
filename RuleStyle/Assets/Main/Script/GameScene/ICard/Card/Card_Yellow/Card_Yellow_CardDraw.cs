using UnityEngine.UI;

using UnityEngine;
/// <summary>
/// 改変カードを参照する
/// </summary>
public class Card_Yellow_CardDraw : ICard
{
    public PlayerSessionData PlayerData { get; set; } = null;

    public float? ProbabilityNum => 25;
    Card_Pattern ICard.card_pattern => Card_Pattern.Yellow;

    /// <summary>
    /// カード名
    /// </summary>
    string ICard.CardName => "カードを引く";
    Sprite ICard.cardUI { get; set; }
    void ICard.CardNum()
    {
        GameSessionManager manage = GameSessionManager.Instance();
        foreach (var data in PlayerData.EffectAwardPlayer_Id)
        {
            manage.DeckDraw(manage.Session_Data[data], PlayerData.RuleSuccessNum);
        }
    }
}
