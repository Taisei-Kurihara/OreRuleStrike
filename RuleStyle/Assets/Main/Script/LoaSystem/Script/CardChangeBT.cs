using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 変更を行う対象を指定するclass
/// </summary>
public class CardChangeBT : MonoBehaviour
{
    [SerializeField,Header("変更を行う対象プレイヤー")]
    Button[] buttons = new Button[0];

    private void Awake()
    {
        //play人数を設定する

        GameManager gameManager = GameManager.Instance();
        for (int i = gameManager.PlayerNum; i < 4; i++)
        {
            RectTransform rect = buttons[i].transform.gameObject.GetComponent<RectTransform>();
            rect.position = new Vector2(Screen.width, Screen.height) * 2;
        }

        OnClick(0);
    }

    void AllActiveChange(bool c)
    {
        foreach (var button in buttons) { button.interactable = c; }
    }

    void OnClick(int s)
    {
        Debug.Log(s);

        // 配列の範囲チェック
        if (s < 0 || s >= buttons.Length)
        {
            Debug.LogError("Invalid index: " + s);
            return; // 範囲外なら処理を中止
        }

        AllActiveChange(true);

        buttons[s].onClick.RemoveListener(() => OnClick(s));
        buttons[s].interactable = false;

        // i の現在の値をキャプチャしないように、別の変数を使う
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // 一時的に変数に格納
            if (index != s)
            {
                Debug.Log(index);
                // s 以外のボタンにリスナーを追加
                buttons[index].onClick.AddListener(() => OnClick(index)); // ここで index を使う
            }
        }
    }


}
