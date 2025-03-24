using System;
using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public enum FadeSpecified
{
   notSpecified = 0,
   _0to1 = 1,
   _1to0 = -1
}

public class FrameRate
{
    float frameRate = 1 / 30;
    public float GetFrameRate {  get { return frameRate; } }
}

/// <summary>
/// 時間事繰り返し等処理
/// </summary>
public class Time_TimerManager : SingletonMonoBehaviourBase<Time_TimerManager>
{
    FrameRate frameRate = new FrameRate();

    public float GetframeRate { get { return frameRate.GetFrameRate; } }
    float timer = 0;

    private void Awake()
    {
        StartCoroutine(TimerCount());
    }

    IEnumerator TimerCount()
    {
        yield return new WaitForSeconds(frameRate.GetFrameRate);
        timer += Time.deltaTime;
    }

    #region フェードイン/アウト
    /// <summary>
    /// 指定された関数を用いてフェードイン/フェードアウト処理を行う
    /// </summary>
    /// <param name="call"></param>
    /// <param name="maxTime"></param>
    public void Fade(UnityAction<float> call,float maxTime, FadeSpecified fade = FadeSpecified.notSpecified)
    {
        // フェード方向を設定
        // maxTime　が正数なら 0->1 のフェードイン
        // maxTime　が負数なら 1->0 のフェードアウト
        if (fade != FadeSpecified.notSpecified) { maxTime = Mathf.Clamp01(maxTime) * fade.GetHashCode(); }
        StartCoroutine (FadeWait(call, maxTime , MathF.Abs(maxTime)));
    }


    /// <summary>
    /// フェードイン/アウトの繰り返し/比率計算/関数実行命令をおこなう
    /// </summary>
    /// <param name="call"></param>
    /// <param name="maxTime"></param>
    /// <param name="AbsmaxTime"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator FadeWait(UnityAction<float> call, float maxTime , float AbsmaxTime, float time = 0)
    {
        yield return new WaitForSeconds(frameRate.GetFrameRate);

        // 最大値を超えないようにしつつカウント
        time = (time + Time.deltaTime < AbsmaxTime) ? time + Time.deltaTime : AbsmaxTime;

        // フェードイン/アウトの比率設定 と 引数として設定
        object[] fadePerc = { Mathf.Abs((time / AbsmaxTime) - ((maxTime <= 0) ? 1 : 0)) };

        // 指定された関数を実行する関数
        FadeMethod(call, fadePerc);

        if (time != AbsmaxTime) { StartCoroutine(FadeWait( call , maxTime , AbsmaxTime , time )); }
    }

    /// <summary>
    /// 指定された関数にフェード比率を返還実行する
    /// </summary>
    /// <param name="call"></param>
    /// <param name="fadePerc"></param>
    /// <returns></returns>
    object FadeMethod(UnityAction<float> call, object[] fadePerc)
    {
        try { return call.Method.Invoke(call.Target, fadePerc); }
        catch (Exception ex) { Debug.LogError($"Error: {ex.Message}"); return null; }
    }

    #endregion

}
