using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// カード変更シーンの設定。
/// </summary>
public class GameMode_ExchangeMode : IGameMode
{
    private GameSessionManager sessionManager;

    ExChangeComponent ExChange;

    private ReactiveProperty<int> Cost = new ReactiveProperty<int>(3);

    /// <summary>
    /// 現在の変更プレイヤ-を格納する。
    /// </summary>
    private PlayerSessionData Change_CurrentPlayer;

    private ChangeData Changes = null;

    /// <summary>
    /// 今
    /// </summary>
    private ICard HandCards = null;

    private int HandCardSelect = 0;

    private IDisposable CostEvent;


    /// <summary>
    /// カードの情報とイベント付けを全て行う。
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    void IGameMode.Init()
    {
        sessionManager = GameSessionManager.Instance();
        if (sessionManager.ExchangeMember.Count == 0)
        {
            sessionManager.sceneContext.Mode_Change(new GameMode_TurnChange(sessionManager));
            Debug.Log("メインモードに戻る");
            Debug.Log("デバッグ用：残り" + sessionManager.ExchangeMember.Count + "人");
        }
        else if (sessionManager.ExchangeMember.Count > 0)
        {
            ExChange = sessionManager.gameObject.GetComponent<ExChangeComponent>();

            int current = sessionManager.ExchangeMember.First.Value;
            sessionManager.ExchangeMember.RemoveFirst();
            Change_CurrentPlayer = sessionManager.Session_Data[current];

            sessionManager.ChangeScene.SetActive(true);

            AddOnClick(Change_CurrentPlayer);

            AllLoadUI();

            //現在プレイヤーの表示処理
            sessionManager.gameObject.GetComponent<ExChangeComponent>().CurrentPlayerImage.sprite = sessionManager.card_Access["P" + Change_CurrentPlayer.PlayerId.ToString() + "の"].cardUI;
            HandLoad();


            //カードに対するイベント付け開始。
            HandCardDataOnClick();
            AllLoadCardDataOnClick();

            CostUIExChange();
        }
    }
    void IGameMode.Exit()
    {
        CostEvent?.Dispose();
    }
    void IGameMode.FixUpdate()
    {
    }
    void IGameMode.Update()
    {
    }

    /// <summary>
    /// コストイベント処理を発生させる関数
    /// </summary>
    void CostUIExChange()
    {
        //コスト関係の処理
        CostEvent = Cost.Subscribe(_ =>
        {
            ExChangeComponent ex = sessionManager.gameObject.GetComponent<ExChangeComponent>();
            switch (_)
            {
                case 0:
                    ex.Cost_One.gameObject.SetActive(false);
                    ex.Cost_Two.gameObject.SetActive(false);
                    ex.Cost_Three.gameObject.SetActive(false);
                    break;
                case 1:
                    ex.Cost_One.gameObject.SetActive(true);
                    ex.Cost_Two.gameObject.SetActive(false);
                    ex.Cost_Three.gameObject.SetActive(false);
                    break;
                case 2:
                    ex.Cost_One.gameObject.SetActive(true);
                    ex.Cost_Two.gameObject.SetActive(true);
                    ex.Cost_Three.gameObject.SetActive(false);
                    break;
                case 3:
                    ex.Cost_One.gameObject.SetActive(true);
                    ex.Cost_Two.gameObject.SetActive(true);
                    ex.Cost_Three.gameObject.SetActive(true);
                    break;
            }
        });
    }

    /// <summary>
    ///　保持カードUI（手札以外）を全て変更
    /// </summary>
    void AllLoadUI()
    {
        foreach (var x in sessionManager.Session_Data)
        {
            LoadUI(ExChange.Player_UI[x.Value.PlayerId], x.Value);
        }
    }
    /// <summary>
    /// プレイヤー単位のUI変更
    /// </summary>
    /// <param name="UICompo"></param>
    void LoadUI(Rule_UI_RuleComponent UIComponent, PlayerSessionData playerdata)
    {
        GameSessionManager manager = GameSessionManager.Instance();
        //プレイヤーUI変更（もうすでに変更したの用意した方がいいかもしれない
        UIComponent.PlayerImage.sprite = manager.card_Access["P" + playerdata.PlayerId.ToString() + "の"].cardUI;

        //カードのUI変更(セッションデータで既にロードされているデータの為、基本的にカードはアタッチするだけで問題ない
        UIComponent.Red_Card_EffectPiece.image.sprite = playerdata.Card_Red_EffectPiece.Value.cardUI;
        UIComponent.Blue_Card.image.sprite = playerdata.Card_Blue.Value.cardUI;
        UIComponent.Red_Card_EffectAward.image.sprite = playerdata.Card_Red_EffectAward.Value.cardUI;
        UIComponent.Yellow_Card.image.sprite = playerdata.Card_Yellow.Value.cardUI;
        UIComponent.Green_Card.image.sprite = playerdata.Card_Green.Value.cardUI;
        UIComponent.Purple_Card.image.sprite = playerdata.Card_Purple.Value.cardUI;

        //ルール文変更
        UIComponent.RuleText.text = playerdata.RuleText_Exchange();
    }

    /// <summary>
    /// 手札以外のプレイヤーのイベント付け(一通りのみ）
    /// </summary>
    void LoadCardDataOnClick(Rule_UI_RuleComponent UIComponent, int num)
    {
        UIComponent.Red_Card_EffectPiece.onClick.RemoveAllListeners();
        UIComponent.Blue_Card.onClick.RemoveAllListeners();
        UIComponent.Red_Card_EffectAward.onClick.RemoveAllListeners();
        UIComponent.Yellow_Card.onClick.RemoveAllListeners();
        UIComponent.Green_Card.onClick.RemoveAllListeners();
        UIComponent.Purple_Card.onClick.RemoveAllListeners();

        UIComponent.Red_Card_EffectPiece.onClick.AddListener(() =>
        {
            if (Changes == null)
            {
                Changes = new ChangeData
                {
                    data = sessionManager.Session_Data[num],
                    Select_CardNum = 1
                };
            }
            else if (Changes.data == sessionManager.Session_Data[num] && Changes.Select_CardNum == 1)
            {
                Changes = null;
            }
            else
            {
                Changes = new ChangeData
                {
                    data = sessionManager.Session_Data[num],
                    Select_CardNum = 1
                };
            }
        });
        UIComponent.Blue_Card.onClick.AddListener(() =>
        {
            if (Changes == null)
            {
                Changes = new ChangeData
                {
                    data = sessionManager.Session_Data[num],
                    Select_CardNum = 2
                };
            }
            else if (Changes.data == sessionManager.Session_Data[num] && Changes.Select_CardNum == 2)
            {
                Changes = null;
            }
            else
            {
                Changes = new ChangeData
                {
                    data = sessionManager.Session_Data[num],
                    Select_CardNum = 2
                };
            }
        });
        UIComponent.Red_Card_EffectAward.onClick.AddListener(() =>
        {
            if (Changes == null)
            {
                Changes = new ChangeData
                {
                    data = sessionManager.Session_Data[num],
                    Select_CardNum = 3
                };
            }
            else if (Changes.data == sessionManager.Session_Data[num] && Changes.Select_CardNum == 3)
            {
                Changes = null;
            }
            else
            {
                Changes = new ChangeData
                {
                    data = sessionManager.Session_Data[num],
                    Select_CardNum = 3
                };
            }
        });
        UIComponent.Yellow_Card.onClick.AddListener(() =>
        {
            if (Changes == null)
            {
                Changes = new ChangeData
                {
                    data = sessionManager.Session_Data[num],
                    Select_CardNum = 4
                };
            }
            else if (Changes.data == sessionManager.Session_Data[num] && Changes.Select_CardNum == 4)
            {
                Changes = null;
            }
            else
            {
                Changes = new ChangeData
                {
                    data = sessionManager.Session_Data[num],
                    Select_CardNum = 4
                };
            }
        });
        UIComponent.Green_Card.onClick.AddListener(() =>
        {
            if (Changes == null)
            {
                Changes = new ChangeData
                {
                    data = sessionManager.Session_Data[num],
                    Select_CardNum = 5
                };
            }
            else if (Changes.data == sessionManager.Session_Data[num] && Changes.Select_CardNum == 5)
            {
                Changes = null;
            }
            else
            {
                Changes = new ChangeData
                {
                    data = sessionManager.Session_Data[num],
                    Select_CardNum = 5
                };
            }
        });
        UIComponent.Purple_Card.onClick.AddListener(() =>
        {
            if (Changes == null)
            {
                Changes = new ChangeData
                {
                    data = sessionManager.Session_Data[num],
                    Select_CardNum = 6
                };
            }
            else if (Changes.data == sessionManager.Session_Data[num] && Changes.Select_CardNum == 6)
            {
                Changes = null;
            }
            else
            {
                Changes = new ChangeData
                {
                    data = sessionManager.Session_Data[num],
                    Select_CardNum = 6
                };
            }

        });
    }

    /// <summary>
    /// 手札のデータ（イベント）を全て登録した
    /// </summary>
    void HandCardDataOnClick()
    {
        sessionManager._HandCard_Component.card_one.onClick.RemoveAllListeners();
        sessionManager._HandCard_Component.card_two.onClick.RemoveAllListeners();
        sessionManager._HandCard_Component.card_three.onClick.RemoveAllListeners();
        sessionManager._HandCard_Component.card_four.onClick.RemoveAllListeners();
        sessionManager._HandCard_Component.card_five.onClick.RemoveAllListeners();
        int num = 0;
        foreach (var x in Change_CurrentPlayer.HandCards)
        {
            switch (num)
            {
                case 0:
                    sessionManager._HandCard_Component.card_one.onClick.RemoveAllListeners();
                    sessionManager._HandCard_Component.card_one.onClick.AddListener(() =>
                    {
                        HandCards = x;
                        HandCardSelect = 0;
                    });
                    break;
                case 1:
                    sessionManager._HandCard_Component.card_two.onClick.RemoveAllListeners();
                    sessionManager._HandCard_Component.card_two.onClick.AddListener(() =>
                    {
                        HandCards = x;
                        HandCardSelect = 1;
                    });
                    break;
                case 2:
                    sessionManager._HandCard_Component.card_three.onClick.RemoveAllListeners();
                    sessionManager._HandCard_Component.card_three.onClick.AddListener(() =>
                    {
                        HandCards = x;
                        HandCardSelect = 2;
                    });
                    break;
                case 3:
                    sessionManager._HandCard_Component.card_four.onClick.RemoveAllListeners();
                    sessionManager._HandCard_Component.card_four.onClick.AddListener(() =>
                    {
                        HandCards = x;
                        HandCardSelect = 3;
                    });
                    break;
                case 4:
                    sessionManager._HandCard_Component.card_five.onClick.RemoveAllListeners();
                    sessionManager._HandCard_Component.card_five.onClick.AddListener(() =>
                    {
                        HandCards = x;
                        HandCardSelect = 4;
                    });
                    break;
            }
            num++;
        }

    }
    /// <summary>
    /// 保持カードのイベントを全て登録した。
    /// </summary>
    void AllLoadCardDataOnClick()
    {
        List<int> list = new List<int>() { 1, 2, 3, 4 };

        foreach (int i in list)
        {
            LoadCardDataOnClick(ExChange.Player_UI[i], i);
        }
    }

    /// <summary>
    /// 手札のUIを更新する。
    /// </summary>
    void HandLoad()
    {
        sessionManager._HandCard_Component.card_one.image.sprite = null;
        sessionManager._HandCard_Component.card_two.image.sprite = null;
        sessionManager._HandCard_Component.card_three.image.sprite = null;
        sessionManager._HandCard_Component.card_four.image.sprite = null;
        sessionManager._HandCard_Component.card_five.image.sprite = null;

        int num = 1;
        foreach (var a in Change_CurrentPlayer.HandCards)
        {
            switch (num)
            {
                case 1:
                    sessionManager._HandCard_Component.card_one.image.sprite = a.cardUI;
                    break;
                case 2:
                    sessionManager._HandCard_Component.card_two.image.sprite = a.cardUI;
                    break;
                case 3:
                    sessionManager._HandCard_Component.card_three.image.sprite = a.cardUI;
                    break;
                case 4:
                    sessionManager._HandCard_Component.card_four.image.sprite = a.cardUI;
                    break;
                case 5:
                    sessionManager._HandCard_Component.card_five.image.sprite = a.cardUI;
                    break;
            }

            num++;
        }

    }

    /// <summary>
    /// 外す場合。
    /// </summary>
    private async void CardRemove()
    {
        //コスト処理
        if (Changes.data == Change_CurrentPlayer)
        {
            if (Cost.Value - 1 >= 0)
            {
                Cost.Value -= 1;
            }
            else
            {
                return;
            }
        }
        else
        {
            if (Cost.Value - 2 >= 0)
            {
                Cost.Value -= 2;
            }
            else
            {
                return;
            }
        }

        //番号次第でリムーブするカードを変える。
        switch (Changes.Select_CardNum)
        {
            case 1:
                await Changes.data.Remove_Red_EffectPiece();
                break;
            case 2:
                await Changes.data.Remove_Blue();
                break;
            case 3:
                await Changes.data.Remove_Red_EffectAward();
                break;
            case 4:
                await Changes.data.Remove_Yellow();
                break;
            case 5:
                await Changes.data.Remove_Green();
                break;
            case 6:
                await Changes.data.Remove_Purple();
                break;
        }
        Changes = null;

        AllLoadUI();
    }
    /// <summary>
    /// カードが変更される時。
    /// </summary>
    void CardChange()
    {
        Debug.Log("第一条件");
        switch (Changes.Select_CardNum)
        {
            case 1:
                if (HandCards.card_pattern == Card_Pattern.Red)
                {
                    Changes.data.Card_Red_EffectPiece.Value = HandCards;

                    HandCards = null;
                }
                break;
            case 2:
                if (HandCards.card_pattern == Card_Pattern.Blue)
                {
                    Changes.data.Card_Blue.Value = HandCards;

                    HandCards = null;
                }
                break;
            case 3:
                if (HandCards.card_pattern == Card_Pattern.Red)
                {

                    Changes.data.Card_Red_EffectAward.Value = HandCards;

                    HandCards = null;
                }
                break;
            case 4:
                if (HandCards.card_pattern == Card_Pattern.Yellow)
                {

                    Changes.data.Card_Yellow.Value = HandCards;

                    HandCards = null;
                }
                break;
            case 5:
                if (HandCards.card_pattern == Card_Pattern.Green)
                {
                    Changes.data.Card_Green.Value = HandCards;

                    HandCards = null;
                }

                break;
            case 6:
                if (HandCards.card_pattern == Card_Pattern.Purple)
                {
                    Debug.Log("テストしています（パープル");
                    Changes.data.Card_Purple.Value = HandCards;

                    HandCards = null;
                }

                break;
        }
        Change_CurrentPlayer.HandCards.RemoveAt(HandCardSelect);
        AllLoadUI();
        HandLoad();
        HandCardDataOnClick();
    }
    /// <summary>
    /// プレイヤーデータを参照にUIにイベントを付けて行く作業
    /// </summary>
    public void AddOnClick(PlayerSessionData playerdata)
    {
        ExChange.ExChange_or_Remove_StartButton.onClick.RemoveAllListeners();
        ExChange.ExChange_or_Remove_StartButton.onClick.AddListener(() =>
        {
            if (HandCards != null && Changes != null)
            {
                CardChange();
                Debug.Log("データ変更完了");
            }
            else if (HandCards == null && Changes != null)
            {
                CardRemove();
                Debug.Log("データ削除完了");
            }
            else
            {
                Debug.Log("何も選択されていない");
            }
        });

        ExChange.ExChangeEndButton.onClick.RemoveAllListeners();
        ExChange.ExChangeEndButton.onClick.AddListener(() =>
        {
            sessionManager.ChangeScene.SetActive(false);
            //再度変更モードを読み込むことで全て消化する。
            sessionManager.sceneContext.Mode_Change(new GameMode_ExchangeMode());
        });

        //.Player_One.
    }
}

