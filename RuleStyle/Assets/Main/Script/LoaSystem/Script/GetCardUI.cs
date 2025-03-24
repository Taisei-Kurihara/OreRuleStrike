using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GetCardUI : MonoBehaviour
{
    [SerializeField]
    Button cardGetEventEndBT;

    [SerializeField]
    Button startmaskbt;

    [SerializeField]
    RectMask2D mask;

    [SerializeField]
    GameObject cardprefab;
    [SerializeField]
    RectTransform cardSlide;


    [SerializeField]
    Button Left;

    [SerializeField]
    Button Right;

    int Allcardslidenumber = 1;

    int cardMaxnum = 5;

    List<RectTransform> cards = new List<RectTransform>();

    Vector3[] cardPositions = {
        new Vector3(Screen.width * 2, Screen.height * 2,0),
        new Vector3((Screen.width / 3) * 0.5f,Screen.height*0.55f,0),
        new Vector3((Screen.width / 3) * 1.5f, Screen.height*0.55f,0),
        new Vector3((Screen.width / 3) * 2.5f, Screen.height*0.55f,0),
        new Vector3((Screen.width / 3) * 3.5f, Screen.height*0.55f,0),
        new Vector3((Screen.width / 3) * 4.5f, Screen.height*0.55f,0), };

    int[][] cardPosSet = {
        new int[] { 0 },
        new int[] { 2 }, 
        new int[] { 1 , 3 }, 
        new int[] { 1 , 2 , 3 }, 
        new int[] { 1 , 2 , 3 , 4 }, 
        new int[] { 1 , 2 , 3 , 4 , 5 } };

    float right = Screen.width * -1;

    [SerializeField, Header("完了までの時間")]
    float timeToComplete = 2;
    float fadetimer = 0;

    Vector2 dspsize = new Vector2(Screen.width, Screen.height);

    BitArray bit = new BitArray(4,false);
    private void Awake()
    {
        Right.onClick.AddListener(() => CardSlide(RotationDis.right));
        Left.onClick.AddListener(() => CardSlide(RotationDis.left));
        cardGetEventEndBT.transform.gameObject.SetActive(false);
        MaskReset();
        startmaskbt.onClick.AddListener(GetCardEventResetStart);
        StartCoroutine(CardsSet());
        CardSlideBTSet();
    }

    void GetCardEventResetStart()
    {
        cardGetEventEndBT.transform.gameObject.SetActive(false);
        fadetimer = 0;
        MaskReset();
        CardSlideBTSet();
        StartFade();
    }



    IEnumerator CardsSet()
    {
        for (int i = 0; i < cardMaxnum; i++)
        {
            // オブジェクトを生成
            GameObject spawnedObject = Instantiate(cardprefab);

            // 親オブジェクトの子として設定
            spawnedObject.transform.SetParent(cardSlide);

            cards.Add(spawnedObject.GetComponent<RectTransform>());
        }
        yield return new WaitUntil(()=>cards.Count == cardMaxnum);
        CardPositionSet(cardMaxnum);
    }

    void CardPositionSet(int num)
    {
        for (int i = 0;i < cards.Count; i++)
        {
            cards[i].transform.position = cardPositions[0];
        }

        if (num == 0) { return; }
        foreach (int i in cardPosSet[num])
        {
            cards[i-1].position = cardPositions[i];
        }

        Allcardslidenumber = (int)(num/3);
    }

    /// <summary>
    /// maskをresetする
    /// </summary>
    public void MaskReset()
    {
        mask.padding = new Vector4(0, dspsize.y, right, -300);
        cardSlide.position = new Vector3(Screen.width/2, Screen.height/2, 0);
        cardGetEventEndBT.transform.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// maskをoffにする
    /// </summary>
    public void NoneMask()
    {
        cardGetEventEndBT.transform.gameObject.SetActive(true);
        mask.padding = new Vector4(0, -300, right, -300);
    }

    public void StartFade()
    {
        Time_TimerManager time_TimerManager = Time_TimerManager.Instance();
        time_TimerManager.Fade(OnFade, maxTime, FadeSpecified._1to0);
    }

    void OnFade(float fadePerc)
    {
        float dis = ((dspsize.y + 300f) * fadePerc) - 300f;

        mask.padding = new Vector4(0, dis, right, -300);

        bit[2] = (fadePerc == 1) ? true : false;

        if (!bit[2])
        {
            cardGetEventEndBT.transform.gameObject.SetActive(true);
            cardGetEventEndBT.onClick.AddListener(CardChangeSystemAdd);
        }

    }

    /// <summary>
    /// 引いたcardを確認後にcardを改変するsceneを呼び出しする
    /// </summary>
    void CardChangeSystemAdd()
    {
        MaskReset();
        UISceneManager uISceneManager = UISceneManager.Instance();
        uISceneManager.CallAdvent(Call.GameUIchangeCard);

    }


    BitArray LeftRight = new BitArray(4, false);

    float MovePerc = 0;

    float time = 0;

    float maxTime = 1;

    float allmaxTime = 0;



    // カードスライドのボタン設定
    void CardSlideBTSet()
    {
        // 右ボタンのクリックリスナー設定
        if (allmaxTime != Allcardslidenumber) { Right.interactable = true; }
        else { Right.interactable = false; }

        // 左ボタンのクリックリスナー設定
        if (allmaxTime != 0) { Left.interactable = true; }
        else { Left.interactable = false; }
    }

    /// <summary>
    /// カードの一覧を左右に移動させて確認する
    /// </summary>
    void CardSlide(RotationDis dis)
    {
        // 移動方向を設定
        LeftRight[(dis == RotationDis.right) ? 1 : 0] = true;
        LeftRight[(dis != RotationDis.right) ? 1 : 0] = false;

        // 全体の最大スライド時間の更新
        allmaxTime = (Mathf.Abs(allmaxTime + dis.GetHashCode()) <= Allcardslidenumber) ? allmaxTime + dis.GetHashCode() : Allcardslidenumber;
        allmaxTime = (allmaxTime >= 0) ? allmaxTime : 0;


        CardSlideBTSet();

        // フェード処理のために左方向と右方向のフラグを更新
        LeftRight[3] = LeftRight[2];
        StartCoroutine(CardSlidechange(dis));
    }

    // フェード処理後にスライド処理を実行するコルーチン
    IEnumerator CardSlidechange(RotationDis dis)
    {
        // 左右のスライドが終わるまで待機
        yield return new WaitUntil(() => !LeftRight[3]);
        StartCoroutine(CardSlideWait());
        LeftRight[2] = true;
    }

    // フェード処理を実行するコルーチン
    IEnumerator CardSlideWait()
    {
        // タイマーをインスタンス化してフレームレートに基づいて待機
        Time_TimerManager time_TimerManager = Time_TimerManager.Instance();
        yield return new WaitForSeconds(time_TimerManager.GetframeRate);

        Debug.Log(Time.deltaTime);

        // 最大値を超えないようにカウントを進める
        if (LeftRight[1]) { time = (time + Time.deltaTime < allmaxTime) ? time + Time.deltaTime : allmaxTime; }
        if (LeftRight[0]) { time = (time - Time.deltaTime > allmaxTime) ? time - Time.deltaTime : allmaxTime; }

        // フェードイン/アウトの比率設定
        MovePerc = time / maxTime;

        Debug.Log("MovePerc:" + MovePerc);

        // カードスライドの位置を設定
        cardSlide.transform.position = new Vector3((Screen.width / 2) - (Screen.width * MovePerc), Screen.height / 2, 0);

        // 最大時間に達していない場合、再度フェード処理を行う
        if (time != maxTime && !LeftRight[3]) { StartCoroutine(CardSlideWait()); }
        else { LeftRight[2] = false; LeftRight[3] = false; }
    }

}
