using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 回転方向を定義する列挙型 (Enum to define rotation directions)
public enum RotationDis
{
    right = 1,  // 右回転 (Right rotation)
    left = -1,  // 左回転 (Left rotation)
    none = 0    // 回転なし (No rotation)
}

public class RotationUI : MonoBehaviour
{
    // 回転するUI要素（RectTransform）のリスト (List of RectTransforms for rotating UI elements)
    List<RectTransform> rotationUI = new List<RectTransform>();

    Vector2 dspsize = new Vector2(Screen.width, Screen.height); // 画面サイズ (Screen size)
    Vector2 percent = new Vector2(0.9f, 0.7f); // UIが占める割合（0～1） (The percentage of screen the UI takes)

    [SerializeField, Header("完了までの時間")]
    float timeToComplete = 2; // 回転が完了するまでの時間 (Time to complete the rotation)

    RotationDis rotationDis = RotationDis.right; // 回転方向（右回転がデフォルト） (Default direction is right)

    // 各方向の角度データ（180度、90度、0度、270度） (Angles for each direction)
    float[] DirectionsData = new float[4] { 180, 90, 0, 270 };
    List<float> Directions = new List<float>(); // 実際に使用する方向データのリスト (List for actual directions used)

    int playerNumber = 0; // プレイヤー数 (Number of players)

    // 状態管理用のBitArray (State management using BitArray)
    // 0 = 人数取得完了 / 1 = rotationUI取得完了 / 2 = 回転実行状態管理 (Flags for different stages)
    BitArray bit = new BitArray(4, false);

    private void Awake()
    {
        StartCoroutine(SetDirections()); // 回転に必要な情報を設定 (Set up rotation directions)
    }

    /// <summary>
    /// UIのRectTransformを代入するメソッド (Method to assign RectTransform for UI elements)
    /// </summary>
    /// <param name="inRotationUI">追加するRectTransform (RectTransform to add)</param>
    /// <param name="num">プレイヤー番号（1から始まる）(Player number starting from 1)</param>
    public void InRotationUI(RectTransform inRotationUI, int num)
    {
        StartCoroutine(InRotationUIWait(inRotationUI, num)); // プレイヤーUIの追加を待機 (Wait for player UI to be added)
    }

    /// <summary>
    /// 人数が確定するまで待機するコルーチン (Coroutine to wait until the number of players is confirmed)
    /// </summary>
    /// <param name="inRotationUI">追加するRectTransform (RectTransform to add)</param>
    /// <param name="num">プレイヤー番号 (Player number)</param>
    /// <returns></returns>
    IEnumerator InRotationUIWait(RectTransform inRotationUI, int num)
    {
        // 人数が確定するまで待機 (Wait until the number of players is set)
        yield return new WaitUntil(() => bit[0]);
        if (num <= rotationUI.Count)
        {
            rotationUI[num - 1] = inRotationUI; // UIのRectTransformをリストに追加 (Add RectTransform to list)
            playerNumber++; // プレイヤー数を更新 (Update player count)
        }
    }

    /// <summary>
    /// 必要な情報が揃うまで待機して設定を行うコルーチン (Coroutine to wait for required info and then set up)
    /// </summary>
    /// <returns></returns>
    IEnumerator SetDirections()
    {
        GameManager gameManager = GameManager.Instance(); // ゲームマネージャのインスタンスを取得 (Get GameManager instance)
        yield return new WaitUntil(() => gameManager); // ゲームマネージャが準備できるまで待機 (Wait for GameManager to be ready)

        // プレイヤー数に合わせたUIリストを設定 (Set up UI list based on the number of players)
        rotationUI = new List<RectTransform>(new RectTransform[gameManager.PlayerNum]);
        bit[0] = true; // 人数設定完了 (Number of players set)
        Debug.Log("RotationUI 人数設定完了 /num:" + rotationUI.Count);

        // 回転方向の設定 (Set rotation directions based on number of players)
        switch (rotationUI.Count)
        {
            case 4:
                DirectionsData = ((rotationDis == RotationDis.right) ? new float[4] { 180, 90, 0, 270 } : new float[4] { 180, 270, 0, 90 });
                break;
            case 3:
                DirectionsData = ((rotationDis == RotationDis.right) ? new float[3] { 180, 90, 270 } : new float[3] { 180, 270, 90 });
                break;
            case 2:
                DirectionsData = new float[2] { 180, 0 }; // 2人用 (For 2 players)
                break;
        }

        // UI側のRectTransformを人数分受け取るまで待機 (Wait until RectTransforms for all players are received)
        yield return new WaitUntil(() => (rotationUI.Count == playerNumber));
        Debug.Log("RotationUI RectTransform 受け取り完了");


        RotationSet(0, 0); // 回転設定 (Set up rotation)
        bit[1] = true; // UI設定完了 (UI set)
    }

    /// <summary>
    /// 回転を開始するメソッド (Method to start rotation)
    /// </summary>
    public void StartRotation()
    {
        if (!bit[2]) { StartCoroutine(StartRotationWait()); } // 回転が未実行なら開始 (Start rotation if it hasn't been executed)
    }

    /// <summary>
    /// 回転開始を待機するコルーチン (Coroutine to wait for rotation to start)
    /// </summary>
    /// <returns></returns>
    IEnumerator StartRotationWait()
    {
        bit[2] = true; // 回転実行状態に設定 (Set to rotation in progress)

        yield return new WaitUntil(() => bit[1]); // UI設定完了を待機 (Wait for UI setup to complete)

        Debug.Log("RotationUI 回転開始");

        Time_TimerManager time_TimerManager = Time_TimerManager.Instance();
        time_TimerManager.Fade(OnRotation, timeToComplete); // フェード効果で回転を開始 (Start rotation with fade effect)
    }

    // 回転を行う (Perform the rotation)
    void OnRotation(float perc)
    {
        // 回転角度を計算 (Calculate the rotation angle)
        float dis = ((Mathf.PI * 0.5f) * perc) * ((rotationDis == RotationDis.right) ? 1 : -1);

        RotationSet(dis, perc); // 回転を反映 (Apply the rotation)
        if (perc == 1)
        {
            bit[2] = false; // 回転完了フラグ (Set flag to indicate rotation is complete)
            ResetRotationUI(); // 回転終了後、UIをリセット (Reset the UI after rotation)
        }
    }

    /// <summary>
    /// UIの回転をリセットする (Reset the UI rotation)
    /// </summary>
    public void ResetRotationUI()
    {
        // リスト内のUIを1つずつ左にシフト (Shift UI elements in the list to the left)
        RectTransform zerorect = rotationUI[0];
        for (int i = 0; i < rotationUI.Count - 1; i++)
        {
            rotationUI[i] = rotationUI[i + 1]; // 移動 (Shift the UI elements)
        }

        // 最後に最初のUIを移動 (Move the first UI element to the end)
        rotationUI[rotationUI.Count - 1] = zerorect;
    }

    Vector2 rotation = new Vector2(0.475f, 0.52f);

    /// <summary>
    /// UIの回転を設定する (Set the rotation for the UI elements)
    /// </summary>
    /// <param name="dis">回転量（ラジアン） (Rotation amount in radians)</param>
    void RotationSet(float dis, float perc)
    {
        Debug.Log("RotationSet スタート");
        for (int i = 0; i < rotationUI.Count; i++)
        {
            float xdis = dis;
            switch (rotationUI.Count)
            {
                case 3:
                    xdis *= (i == 2) ? 2 : 1; // 3人のとき、最後のUIの回転量を変更 (For 3 players, adjust the rotation for the last UI element)
                    break;
                case 2:
                    xdis *= 2; // 2人のとき、回転量を変更 (For 2 players, adjust the rotation amount)
                    break;
            }

            // 回転後の位置を計算 (Calculate the new position after rotation)
            Vector2 vec = new Vector2(Mathf.Sin((DirectionsData[i] * Mathf.Deg2Rad) + xdis), Mathf.Cos((DirectionsData[i] * Mathf.Deg2Rad) + xdis));

            float xpos = 0.025f;
            float ypos = 0.575f;

            if (i == 0)
            {
                // 下から移動
                // 距離調整
                vec *= (dspsize / 2) * new Vector2(percent.x, 1 - (perc * Mathf.Abs(percent.y - 1)));
                // 基点調整
                vec += dspsize * new Vector2((Mathf.Abs(perc) * (rotation.x - xpos)) + xpos, (Mathf.Abs(perc) * (rotation.y - ypos)) + ypos);
            }
            else if (i == 1)
            {
                // 下に移動する
                // 距離調整
                vec *= (dspsize / 2) * new Vector2(percent.x, 1 - (Mathf.Abs(perc - 1) * Mathf.Abs(percent.y - 1)));
                // 基点調整
                vec += dspsize * new Vector2((Mathf.Abs(perc - 1) * (rotation.x - xpos)) + xpos, (Mathf.Abs(perc - 1) * (rotation.y - ypos)) + ypos);
            }
            else
            {
                // 距離調整
                vec *= (dspsize / 2) * percent;
                // 基点調整
                vec += dspsize * new Vector2(rotation.x, rotation.y);
            }

            vec += (dspsize / 2) * new Vector2(0.05f, 0);

            rotationUI[i].position = vec; // 計算した位置にUIを移動 (Move UI to the calculated position)
            if (perc == 1) { Debug.Log("perc == 1"); }
        }
    }
}
