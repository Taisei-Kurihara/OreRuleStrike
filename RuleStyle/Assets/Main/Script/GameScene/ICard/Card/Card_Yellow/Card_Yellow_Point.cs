using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 得点を取得する。
/// </summary>
public class Card_Yellow_Point : ICard
{
    public PlayerSessionData PlayerData { get; set; } = null;

    public float? ProbabilityNum => null;
    Card_Pattern ICard.card_pattern => Card_Pattern.Yellow;

    string ICard.CardName => "得点";
    Sprite ICard.cardUI { get; set; }
    /// <summary>
    /// 個人ルール成功時実行。得点を変更する
    /// </summary>
    void ICard.CardNum()
    {
        if (PlayerData != null)
        {
            GameSessionManager manage = GameSessionManager.Instance();
            if (PlayerData.RuleSuccessCalculation == "+")
            {
                //EffectAward報酬対象全てにプレイヤーの点数を変更する
                foreach (var a in PlayerData.EffectAwardPlayer_Id)
                {
                    manage.Session_Data[a].PlayerPoint += PlayerData.RuleSuccessNum;
                }
            }
            else if (PlayerData.RuleSuccessCalculation == "-")
            {
                foreach (var a in PlayerData.EffectAwardPlayer_Id)
                {
                    manage.Session_Data[a].PlayerPoint -= PlayerData.RuleSuccessNum;
                    //0を下回った場合即座に0に変更する。
                    if (manage.Session_Data[a].PlayerPoint < 0) { manage.Session_Data[a].PlayerPoint = 0; }
                }


            }
        }
    }
}
