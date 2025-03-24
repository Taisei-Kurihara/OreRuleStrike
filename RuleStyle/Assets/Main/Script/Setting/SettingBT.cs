using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BTType
{
    slider,
    toggle,
    button
}

public enum SettingType
{
    Apply,
    BGMVolume,
    SEVolume,
    BGMmute,
    SEmute,
    PlayerNumPlus,
    PlayerNumMinus,
}

public class SettingBT : MonoBehaviour
{
    [SerializeField]
    private SettingType settingType;

    private BTType settingTypeValue;

    public SettingType GetSettingType { get { return settingType; } }

    [SerializeField]
    TextMeshProUGUI visualPercentText;

    Slider slider;

    Toggle toggle;

    float value = 1;

    public float GetValue {  get { return value; } }

    int myNum = -1;

    float unchangevalue = 0;

    Button button;


    BitArray valuChange = new BitArray(2,false);


    private void Awake()
    {
        SettingManager settingManager = SettingManager.Instance();
        settingManager.Load(settingType);
        switch (settingType)
        {
            case SettingType.BGMVolume:
            case SettingType.SEVolume:
                settingTypeValue = BTType.slider;
                slider = GetComponent<Slider>();
                if (slider == null) { Debug.LogError(SceneManager.GetActiveScene().name + "/" + gameObject.name + "/" + "SettingBT"); }
                myNum = settingManager.SetSettingBTs(GetComponent<SettingBT>());
                break;
            case SettingType.BGMmute:
            case SettingType.SEmute:
                settingTypeValue = BTType.toggle;
                toggle = GetComponent<Toggle>();
                if (toggle == null) { Debug.LogError(SceneManager.GetActiveScene().name + "/" + gameObject.name + "/" + "SettingBT"); }
                myNum = settingManager.SetSettingBTs(GetComponent<SettingBT>());
                break;
            case SettingType.Apply:
            case SettingType.PlayerNumPlus:
            case SettingType.PlayerNumMinus:
                settingTypeValue = BTType.button;
                button = GetComponent<Button>();
                if (button == null) { Debug.LogError(SceneManager.GetActiveScene().name + "/" + gameObject.name + "/" + "SettingBT"); }
                else { button.onClick.AddListener(OnApplyBT); }

                if (settingType == SettingType.Apply) { button.interactable = false; StartCoroutine(ApplySetWait()); }
                GameManager gameManager = GameManager.Instance();
                if (visualPercentText != null) { visualPercentText.text = gameManager.PlayerNum.ToString("F0") + "P"; }
                StartCoroutine(NumChangeBT());

                break;
        }
    }

    void OnApplyBT()
    {
        switch (settingType)
        {

            case SettingType.Apply:
                SettingManager settingManager = SettingManager.Instance();
                settingManager.Save();
                break;
            case SettingType.PlayerNumPlus:
            case SettingType.PlayerNumMinus:
                StartCoroutine(PlayerNumChange((settingType == SettingType.PlayerNumPlus) ? 1:-1));

                break;
        }
    }


    IEnumerator PlayerNumChange(int num)
    {
        yield return 0;
        GameManager gameManager = GameManager.Instance();

        yield return new WaitUntil(() => (gameManager != null) ? true : false);

        gameManager.PlayerNumChange(gameManager.PlayerNum+num);
        if (visualPercentText != null) { visualPercentText.text = gameManager.PlayerNum.ToString("F0") + "P"; }
        StartCoroutine(NumChangeBT());
    }

    IEnumerator NumChangeBT()
    {
        GameManager gameManager = GameManager.Instance();
        int min = 2;
        if (settingType == SettingType.PlayerNumPlus && gameManager.PlayerNum >= 4 || settingType == SettingType.PlayerNumMinus && gameManager.PlayerNum <= min)
        {
            button.interactable = false;
            yield return new WaitForSeconds(1 / 30);
            StartCoroutine(NumChangeBT());
        }
        else
        { button.interactable = true; }
    }

    void Start()
    {
        if(settingTypeValue != BTType.button)
        {
            StartCoroutine(SetWait());
        }
    }

    IEnumerator ApplySetWait()
    {
        yield return new WaitForSeconds(1 / 30);
        SettingManager settingManager = SettingManager.Instance();
        button.interactable = settingManager.GetValueChangedCheck();

        StartCoroutine(ApplySetWait());
    }

    IEnumerator SetWait()
    {
        SettingManager settingManager = SettingManager.Instance();
        yield return new WaitUntil(() => settingManager.Search_LoadCompletionCheck(settingType));

        switch (settingType)
        {
            case SettingType.BGMVolume:
            case SettingType.SEVolume:
                if (slider != null)
                {
                    slider.value = settingManager.GetValue(settingType);
                    value = slider.value;
                    unchangevalue = slider.value;

                    if(visualPercentText != null) { visualPercentText.text = ((value / 1) * 100).ToString("F0") + "%"; }
                }
                break;
            case SettingType.BGMmute:
            case SettingType.SEmute:
                if (toggle != null)
                {
                    toggle.isOn = (settingManager.GetValue(settingType) == 1) ? true : false;
                    value = (toggle.isOn) ? 1 : 0;
                    unchangevalue = (toggle.isOn) ? 1:0;
                }
                break;
        }

        StartCoroutine(ChangeValuLoop());
    }

    IEnumerator ChangeValuLoop()
    {
        yield return new WaitForSeconds(1/30);

        ValuCheck();

        StartCoroutine(ChangeValuLoop());
    }

    public void ValueReset()
    {
        switch (settingType)
        {
            case SettingType.BGMVolume:
            case SettingType.SEVolume:
                if (slider != null)
                {
                    unchangevalue = slider.value;
                }
                break;
            case SettingType.BGMmute:
            case SettingType.SEmute:
                if (toggle != null)
                {
                    unchangevalue = (toggle.isOn) ? 1 : 0;
                }
                break;
        }
    }

    void ValuCheck()
    {
        SettingManager settingManager = SettingManager.Instance();
        switch (settingType)
        {
            case SettingType.BGMVolume:
            case SettingType.SEVolume:
                settingManager.SetChanged(myNum, (unchangevalue != slider.value) ? true : false);
                value = slider.value;
                if (visualPercentText != null) { visualPercentText.text = ((value / 1) * 100).ToString("F0") + "%"; }
                break;
            case SettingType.BGMmute:
            case SettingType.SEmute:
                settingManager.SetChanged(myNum, (unchangevalue != ((toggle.isOn) ? 1 : 0)) ? true : false);
                value = (toggle.isOn) ? 1 : 0;
                break;
        }
    }
}
