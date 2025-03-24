using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 駒同士がぶつかった時に発動する
/// </summary>
public class Card_Blue_Attack : ICard
{
    public PlayerSessionData PlayerData { get; set; } = null;

    public float? ProbabilityNum => 35;

    Card_Pattern ICard.card_pattern => Card_Pattern.Blue;

    /// <summary>
    /// カード名
    /// </summary>
    string ICard.CardName => "駒が相手の駒に当たることで";
    Sprite ICard.cardUI { get; set; }
    /// <summary>
    /// 
    /// </summary>
    void ICard.CardNum()
    {
        if (PlayerData != null)
        {
            Debug.Log("アタック判定作成");
            //ショットイベントの念のための初期化
            PlayerData.BlueTrigger?.Dispose();

            List<GameObject> EffectObjects = new List<GameObject>();
            //効果対象のGameObjectList作成
            foreach (var effect in PlayerData.EffectPiecePlayer_Id)
            {
                //オブジェクトが存在しない場合、判定は行われない。
                if(GameSessionManager.Instance().Session_Data[effect].Player_GamePiece != null)
                {
                    EffectObjects.Add(GameSessionManager.Instance().Session_Data[effect].Player_GamePiece);
                }
                
            }

            PlayerData.BlueTrigger=EffectObjects.ConvertAll(obj => obj.GetComponent<Collider>()
            .OnCollisionEnterAsObservable())
                .Merge()
                .Where(collision => collision.gameObject.GetComponent<Player_Attach>()!=null)//プレイヤーのみ
                .Take(1)
                .Subscribe(_ => 
                {
                    Debug.Log("判定成功");
                    PlayerData.Success_Local();
                });

            /*
            //ショットイベント登録
            if (PlayerData.Player_GamePiece != null) 
            { 
                
                PlayerData.BlueTrigger = PlayerData?.Player_GamePiece
                .OnTriggerEnterAsObservable()
                .Take(1)//一回で自然にDisposeするようにする。
                .Subscribe(x =>
                {
                    if (x.gameObject != null)
                    {
                        Debug.Log("アタック判定");
                        PlayerData.Success_Local();
                    }
                }).AddTo(PlayerData.Player_GamePiece);
            }
            */

            //PlayerData.BlueTrigger = 
               // PlayerData.
        }
    }
}
