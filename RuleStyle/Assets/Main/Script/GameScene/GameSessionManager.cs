using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;


/// <summary>
/// ゲームシーン時、アタッチされている事を想定しています。
/// </summary>
public class GameSessionManager : MonoBehaviour
{
    /// <summary>
    /// ゲームマネージャーにプレイヤー情報を持たせておく必要があるのか？
    /// 解決策。ゲーム的情報はこちらで取得させておくことで
    ///　プレイヤ―ネーム等の情報はゲームマネージャーで設定させておく
    /// </summary>
    public GameManager gameManager;

    #region Static化
    protected static GameSessionManager instance;
    /// <summary>
    /// ゲームシーンマネージャーに接続する為の関数
    /// </summary>
    /// <returns></returns>
    public static GameSessionManager Instance()
    {
        return instance;
    }
    #endregion
    /// <summary>
    /// 現在のモード
    /// </summary>%
    public IGameMode gamemode;

    public Dictionary<int, PlayerSessionData> Session_Data = new Dictionary<int, PlayerSessionData>();


    /// <summary>
    /// モード変更自に呼ぶ
    /// </summary>
    public GameSceneContext sceneContext = new GameSceneContext();

    #region プレイヤーの駒の変数。
    public GameObject PlayerGameObject_One;

    public GameObject PlayerGameObject_Two;

    public GameObject PlayerGameObject_Three;

    public GameObject PlayerGameObject_Four;
    #endregion

    /// <summary>
    /// MainのUI
    /// </summary>
    public Main_UI_Component Main_UI_Component;

    /// <summary>
    /// 順番
    /// </summary>
    public List<int> TurnList = new List<int>();
    
    //現在のプレイヤーの順番
    public int CurrentTurnNum = 0;

    [Header("駒の生成場所")]
    /// <summary>
    /// 駒の生成場所
    /// </summary>
    public Vector3 PieceStartPoint=new Vector3(0,10,0);

    [Header("カメラ")]
    public Transform CameraPosition;

    /// <summary>
    /// カードのデッキ
    /// </summary>
    public List<ICard> cards =new List<ICard>();

    /// <summary>
    /// カードのデータを検索する為の物（cardsのデータをどちらかに限定した方が良いかもしれない。)
    /// </summary>
    public Dictionary<string,ICard> card_Access = new Dictionary<string,ICard>();

    /// <summary>
    /// 改変カード待ち
    /// </summary>
    public LinkedList<int> ExchangeMember = new LinkedList<int>();


    /// <summary>
    /// 基本的にこのラインレンダラーしか使用しない。
    /// </summary>
    public LineRenderer Arrowline;

    [Header("変更シーン")]
    /// <summary>
    /// 変更する時のシーン
    /// </summary>
    public GameObject ChangeScene;

    public HandCard_Component _HandCard_Component;

    public RotationUI rotation;
    public SetPlayerUINum setPlayerUINum;

    public TextMeshProUGUI pointNumText;

    void Start()
    {
        OnPointDsp();

        //もう既にアタッチされていることが想定されている為
        instance = this;
        //
        gameManager = GameManager.Instance();
        
        sceneContext.Mode_Change(new GameMode_Init(this));
    }

    /// <summary>
    /// 今のプレイヤーのデータ
    /// </summary>
    /// <returns></returns>
    public PlayerSessionData NowPlayer()
    {
        return Session_Data[TurnList[CurrentTurnNum]];
    }

    /// <summary>
    /// カードをドローする。引数は対象と枚数
    /// </summary>
    public void DeckDraw(PlayerSessionData player, int num)
    {
        //複数回ドローも可能
        for (var i = 0; i < num; i++)
        {
            float Max = 0;
            float Current = 0;

            //確率総和
            foreach (var x in cards)
            {
                if (x.ProbabilityNum != null)
                {
                    Max += (float)x.ProbabilityNum;
                }
            }

            var random = UnityEngine.Random.Range(0, Max);

            bool DrawSuccess = false;
            foreach (var x in cards)
            {
                if (x.ProbabilityNum != null)
                {
                    Current += (float)x.ProbabilityNum;

                    if (random < Current)
                    {
                        player.HandCards.Add(x);
                        DrawSuccess = true;
                        break;
                    }
                }
            }
            //乱数が総和以上の時末尾要素に
            if (DrawSuccess == false)
            {
                player.HandCards.Add(cards[cards.Count - 1]);
            }
        }
        //Draw画面に移行する。
    }

    /// <summary>
    /// GameManagerからGameSessionManagerにデータを代入させていく。
    /// </summary>
    public void OnLoadSessionData()
    {
        //プレイヤー名とプレイヤーIDの代入
        foreach (var i in Session_Data)
        {
            i.Value.PlayerId = GameManager.Variable_Data[i.Key].Id;
            i.Value.PlayerName = GameManager.Variable_Data[i.Key].PlayerName;
        }
    }

    /// <summary>
    /// 点数表示
    /// </summary>
    public void OnPointDsp()
    {
        pointNumText.text = "";

        int num = 1;

        //プレイヤー名とプレイヤーIDの代入
        foreach (var i in Session_Data)
        {
            Debug.Log(i.Value.PlayerPoint);
            pointNumText.text += "player" + num + ":" + i.Value.PlayerPoint + "\n";
            num++;
        }
    }

    /// <summary>
    /// 順番シャッフル
    /// </summary>
    public List<int> Shuffle(List<int> array)
    {
        for (var i = array.Count - 1; i > 0; --i)
        {
            // 0以上i以下のランダムな整数を取得
            // Random.Rangeの最大値は第２引数未満なので、+1することに注意
            var j = UnityEngine.Random.Range(0, i + 1);

            // i番目とj番目の要素を交換する
            var tmp = array[i];
            array[i] = array[j];
            array[j] = tmp;
        }

        setPlayerUINum.SetUI(array);

        return array;
    }

    /// <summary>
    /// ゲーム終了時
    /// </summary>
    private void OnDestroy()
    {
        Debug.Log("破壊");
        foreach (var t in Session_Data)
        {
            t.Value.Dispose();
        }
    }

    private void Update() => sceneContext._currentgameMode?.Update();

    private void FixedUpdate() => sceneContext._currentgameMode?.FixUpdate();
}
