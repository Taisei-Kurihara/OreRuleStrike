using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum CallType
{
    ExtraOnly = 0,
    addUIMenu = 1,
    closeUIMenu = 2,
    changeUIMenu = 3,
    changescene = 4,
    Quit = 5,
    addUI = 6,
    closeUI = 7,
    changeUI = 8,
    switchingUI = 9,
}
public enum Call
{
    None = 0,
    Now = 1,

    //scene
    Title = 2,
    TitleScene = 3,
    GameScene = 4,
    GameTest = 5,
    Result = 6,

    //UIscene
    Menu = 7,
    VolumeSetting = 8,

    //ロードシーン
    Loadscene = 9,

    GameUIPlayer,
    GameUIchangeCard
}

/// <summary>
/// scene移動/加算以外の効果
/// </summary>
public enum Extra
{
    None,
    NoSound,
    cancelSound,
    OnPose,
    OffPose,
    ChangePose,
}
public class BTFlow : MonoBehaviour
{
    [SerializeField]
    readonly Array enumValues = Enum.GetValues(typeof(Call));

    [SerializeField]
    CallType callType;
    [SerializeField]
    Call call;

    [SerializeField]
    Extra[] extra;

    BitArray extraCompletion = new BitArray(1,false);
    BitArray All = new BitArray(1,true);

    Button thisButton;
    private void Awake()
    {
        thisButton = GetComponent<Button>();
        EventExecution();
    }

    public void EventExecution() { thisButton.onClick.AddListener(Onclick); }
    public void EventUnExecuted() { thisButton.onClick.RemoveListener(Onclick); }


    public void Onclick()
    {
        Debug.Log(this.gameObject.name);
        BitArray SE = new BitArray(2,true);
        for (int i = 0; i < extra.Length; i++)
        {
            switch (extra[i])
            {
                case Extra.OnPose:
                    Time.timeScale = 0;
                    break;
                case Extra.OffPose:
                    Time.timeScale = 1;
                    break;
                case Extra.cancelSound:
                    SE[1] = false;
                    break;
                case Extra.NoSound:
                    SE[0] = false;
                    break;
            }
        }

        if (SE[0])
        {
            StartCoroutine(StartSEWait(SE[1]));
        }

        StartCoroutine(Scene());

        
    }


    IEnumerator Scene()
    {
        yield return new WaitUntil(() => All.Cast<bool>().SequenceEqual(extraCompletion.Cast<bool>()));

        UISceneManager uISceneManager = UISceneManager.Instance();

        switch (callType)
        {
            case CallType.addUIMenu:
                uISceneManager.CallAdvent(call);
                break;
            case CallType.closeUIMenu:
                uISceneManager.CallDelete(call);
                break;
            case CallType.changeUIMenu:
                if (call == uISceneManager.GetLast())
                {

                    if (uISceneManager.Search(call))
                    {
                        Debug.Log("cc");
                        uISceneManager.CallDelete(call);
                        for (int i = 0; i < extra.Length; i++)
                        {
                            if (extra[i] == Extra.ChangePose)
                            {
                                Time.timeScale = 1;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("cca");
                    uISceneManager.CallAdvent(call);
                    for (int i = 0; i < extra.Length; i++)
                    {
                        if (extra[i] == Extra.ChangePose)
                        {
                            Time.timeScale = 0;
                            break;
                        }
                    }
                }

                break;
            case CallType.changescene:
                uISceneManager.ChangeScene(call);
                break;
            case CallType.Quit:
                Application.Quit();
                break;
            case CallType.switchingUI:
                break;
        }
    }


    IEnumerator StartSEWait(bool SE)
    {
        AudioManager audioManager = AudioManager.Instance();

        yield return new WaitUntil(() => (audioManager != null) ? true : false);

        audioManager.Play((SE) ? AudioType.SE : AudioType.NextSE);
        extraCompletion[0] = true;
    }
}
