using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
/// <summary>
/// 駒がステージ外に出たときに発動する
/// </summary>
public class Card_Blue_OverField : ICard
{

    public PlayerSessionData PlayerData { get; set; } = null;

    public float? ProbabilityNum => 45;

    Card_Pattern ICard.card_pattern => Card_Pattern.Blue;
    
    /// <summary>
    /// カード名
    /// </summary>
    string ICard.CardName => "場外で";

    Sprite ICard.cardUI { get; set; }

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

            //ショットイベント登録
            PlayerData.BlueTrigger = PlayerData.Player_GamePiece.OnTriggerEnterAsObservable()
                .Take(1)//一回で自然にDisposeするようにする。
                .Subscribe(collider =>
                {
                    if (collider.gameObject.GetComponent<OutPosition>() != null)
                    {
                        Debug.Log("場外です");
                        PlayerData.Success_Local();
                    }
                }).AddTo(PlayerData.Player_GamePiece);
        }

    }
}
