using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class ScrollSystem : MonoBehaviour
{
    [SerializeField,Header("使用するスライダーのhandleを設定")]
    RectTransform handle;

    [SerializeField]
    Vector2 handleSize = Vector2.one;

    [SerializeField,Header("使用するスライダーを設定")]
    Scrollbar slider;

    [SerializeField,Header("動かすobjectを設定")]
    RectTransform MovePoint;

    [SerializeField, Header("動く距離を設定")]
    Vector2 MoveDis = new Vector2(0,0);

    Vector2 firstMoveDis = Vector2.zero;


    // Start is called before the first frame update
    void Start()
    {
        // 初期位置を設定
        firstMoveDis = MovePoint.transform.position;

        // handleのサイズを設定
        handle.localScale = handleSize;

        // スライダーが動かされたと時に関数を呼び出す
        slider.onValueChanged.AddListener(SlideValue);
    }

    void SlideValue(float value)
    {
        // 位置を設定する
        MovePoint.transform.position = firstMoveDis + (MoveDis*value);
    }

    void OnDestroy()
    {
        // メモリリーク防止のためにリスナーを削除
        slider.onValueChanged.RemoveListener(SlideValue);
    }
}
