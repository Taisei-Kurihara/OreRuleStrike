using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Rule変更画面のコンポーネント
/// </summary>
public class ExChangeComponent : MonoBehaviour
{
    public Image CurrentPlayerImage;

    [Header("プレイヤーのコンポーネント")]
    //Playerの細かい対応UIはRuleComponent内に。
    //UI_Xのようなナンバーは対応するUserがいない場合消失させるものとする。
    #region プレイヤーコンポーネント
    public GameObject UI_One;
    public Rule_UI_RuleComponent Player_One;
    public GameObject UI_Two;
    public Rule_UI_RuleComponent Player_Two;
    public GameObject UI_Three;
    public Rule_UI_RuleComponent Player_Three;
    public GameObject UI_Four;
    public Rule_UI_RuleComponent Player_Four;
    #endregion

    //

    [Header("イベント用変数")]
    public Button ExChangeEndButton;

    public Button ExChange_or_Remove_StartButton;

    #region コスト_UI
    [Header("コスト用変数")]
    public Image Cost_One;
    public Image Cost_Two;
    public Image Cost_Three;
    #endregion


    /// <summary>
    /// プレイヤー番号に紐付けられたUI
    /// </summary>
    public Dictionary<int, Rule_UI_RuleComponent> Player_UI;
    /// <summary>
    /// プレイヤー番号に紐付けられたUIの大枠
    /// </summary>
    public Dictionary<int, GameObject> UI;
    /// <summary>
    /// コストUI
    /// </summary>
    public Dictionary<int, Image> UI_Cost;

    private void Awake()
    {
        Player_UI = new Dictionary<int, Rule_UI_RuleComponent>
        {
            {1, Player_One},
            {2, Player_Two},
            {3, Player_Three},
            {4, Player_Four}
        };

        UI = new Dictionary<int, GameObject>
        {
            { 1, UI_One},
            { 2, UI_Two},
            { 3, UI_Three},
            { 4, UI_Four}
        };

        UI_Cost = new Dictionary<int, Image>
        {
            {1,Cost_One },
            {2,Cost_Two},
            {3,Cost_Three}
        };
    }
}