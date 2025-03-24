using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InTurnFade : MonoBehaviour
{
    [SerializeField]
    List<RectMask2D> rctMasks;

    Vector2 dspsize = new Vector2(Screen.width, Screen.height);
    float maxTime = 1;

    private void Awake()
    {
        //for (int i = 0; i < Images.Length; i++)
        //{
        //    rctTransfomes.Add(Images[i].GetComponent<RectTransform>());
        //    rctMasks.Add(Images[i].GetComponent<RectMask2D>());
        //}
        SetPlayerUINum setPlayerUINum = GetComponent<SetPlayerUINum>();

        GameManager gameManager = GameManager.Instance();

        setPlayerUINum.SetUI(gameManager.PlayerNum);

        NoneMask();
        StartFade();
    }

    IEnumerator FadeInWait(int i = 0)
    {
        yield return new WaitForSeconds(2);
    }

    /// <summary>
    /// mask‚ðoff‚É‚·‚é
    /// </summary>
    public void NoneMask()
    {
        foreach (var masks in rctMasks)
        {
            masks.padding = new Vector4(0, -300, 0, -300);
        }
    }

    public void StartFade()
    {
        Time_TimerManager time_TimerManager = Time_TimerManager.Instance();
        time_TimerManager.Fade(OnFade, maxTime, FadeSpecified._1to0);
    }

    void OnFade(float fadePerc)
    {
        float dis = ((dspsize.y + 300f) * fadePerc) - 300f;
        foreach (var masks in rctMasks)
        {
            masks.padding = new Vector4(0, dis, 0, -300);
        }

    }
}
