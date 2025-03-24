using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System;
using UnityEngine.UI;

/// <summary>
/// ゴール行ったときの判定カード
/// </summary>
public class Card_Blue_Goal : ICard
{
    public PlayerSessionData PlayerData { get; set; } = null;

    public float? ProbabilityNum => null;
    Card_Pattern ICard.card_pattern => Card_Pattern.Blue;

    Sprite ICard.cardUI { get; set; }
    /// <summary>
    /// カード名
    /// </summary>
    string ICard.CardName => "ゴールで";

    void ICard.CardNum()
    {
        if (PlayerData != null)
        {
            //ショットイベントの念のための初期化
            PlayerData.BlueTrigger?.Dispose();

            List<GameObject> EffectObjects = new List<GameObject>();

            //効果対象のGameObjectList作成
            foreach (var effect in PlayerData.EffectPiecePlayer_Id)
            {
                //オブジェクトが存在しない場合、判定は行われない。
                if (GameSessionManager.Instance().Session_Data[effect].Player_GamePiece != null)
                {
                    EffectObjects.Add(GameSessionManager.Instance().Session_Data[effect].Player_GamePiece);
                }
                
            }

            

            PlayerData.BlueTrigger = EffectObjects.ConvertAll(obj => obj.GetComponent<Collider>()
            .OnTriggerEnterAsObservable())
                .Merge()
                .Where(collision => collision.gameObject.GetComponent<GoalObject>() != null)//プレイヤーのみ
                .Take(1)
                .Subscribe(_ =>
                {
                    PlayerData.Success_Local();
                });

            /*
            //ショットイベント登録
            PlayerData.BlueTrigger = PlayerData.Player_GamePiece.OnTriggerEnterAsObservable()
                .Take(1)//一回で自然にDisposeするようにする。
                .Subscribe(collider =>
            {
                if (collider.gameObject.GetComponent<GoalObject>() != null)
                {
                    PlayerData.Success_Local();
                }
            }).AddTo(PlayerData.Player_GamePiece);
            */
        }
        else
        {
            
        }
    }
}
