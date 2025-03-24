using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum SettingOriginator
{
    none = -1,
    AudioVolume = 0
}

public class SettingManager : SingletonMonoBehaviourBase<SettingManager>
{
    /// <summary>
    /// 0 = 設定画面が開いている / 
    /// </summary>
    BitArray settingstatus = new BitArray(2,false);

    List<SettingBT> settingBTs = new List<SettingBT>();

    List<SettingType> valueChanged = new List<SettingType>();

    // 値が変更された対象
    BitArray valueChangedCheck = new BitArray(32, false);
    BitArray All = new BitArray(32, false);


    // 現在変更可能性が存在する物
    List<object> loadSettled = new List<object>(new object[32]);

    // 保存対象
    BitArray saveTarget = new BitArray(32, false);

    // loadSettledに重複していれないようにする
    BitArray SObit = new BitArray(32,false);
    // ロードが完了しているかどうか
    BitArray SOLoadSettledbit = new BitArray(32,false);

    int num = -1;
    #region SetGetReset
    public int SetSettingBTs(SettingBT bt) {  settingBTs.Add(bt); settingstatus[0] = true; num++; return num; }

    public void SetChanged(int num,bool check) { valueChangedCheck[num] = check; }

    public void SetChangedValue(SettingType ST) { valueChanged.Add(ST); }
    public void SetLoadSettled(SettingOriginator SO)
    {
        if (!SObit[(int)SettingOriginator.AudioVolume])
        {
            Type type = Type.GetType(SO.ToString());
            loadSettled[(int)SettingOriginator.AudioVolume] = Activator.CreateInstance(type);
        }
        else
        {
            SObit[(int)SettingOriginator.AudioVolume] = true;
        }
    }

    public bool GetValueChangedCheck() { return !(valueChangedCheck.Cast<bool>().SequenceEqual(All.Cast<bool>())); }

    public void ReseAllt()
    {
        settingstatus[0] = false;
        ResetSettingBTs();
        ResetChangedValue();
        ResetLoadSettled();
    }

    public void ResetSettingBTs() { settingBTs = new List<SettingBT>(); num = -1; valueChangedCheck = ResetSObitAll(); }
    public void ResetChangedValue() { valueChanged = new List<SettingType>(); }
    public void ResetLoadSettled()
    {
        loadSettled = new List<object>(new object[32]);
        SObit = ResetSObitAll();
        SOLoadSettledbit = ResetSObitAll();
        ResetLoadTarget();
    }

    public void ResetLoadTarget() { saveTarget = ResetSObitAll(); }

    private BitArray ResetSObitAll() { return new BitArray(SOLoadSettledbit.Count, false); }

    #endregion

    /// <summary>
    /// 所属検索
    /// </summary>
    public SettingOriginator SearchSaveSlot(SettingType ST)
    {
        switch (ST)
        {
            case SettingType.BGMVolume:
            case SettingType.SEVolume:
            case SettingType.BGMmute:
            case SettingType.SEmute:
                return SettingOriginator.AudioVolume;
        }
        return SettingOriginator.none;
    }

    /// <summary>
    /// 値を取得
    /// </summary>
    public float GetValue(SettingType ST)
    {
        switch (SearchSaveSlot(ST))
        {
            case SettingOriginator.AudioVolume:
                if (!SOLoadSettledbit[(int)SettingOriginator.AudioVolume])
                {
                    Load(SettingOriginator.AudioVolume);
                }

                AudioManager audioManager = AudioManager.Instance();

                switch (ST)
                {
                    case SettingType.BGMVolume:
                        return audioManager.GetVolume.BGMVolume;
                    case SettingType.SEVolume:
                        return audioManager.GetVolume.SEVolume;
                    case SettingType.BGMmute:
                        return (audioManager.GetVolume.BGMmute) ? 1 : 0;
                    case SettingType.SEmute:
                        return (audioManager.GetVolume.SEmute) ? 1 : 0;
                }
                break;
        }

        return 0;
    }


    /// <summary>
    /// ロード完了確認
    /// </summary>
    public bool Search_LoadCompletionCheck(SettingType ST)
    {
        switch (SearchSaveSlot(ST))
        {
            case SettingOriginator.AudioVolume:
                return SOLoadSettledbit[(int)SettingOriginator.AudioVolume];
        }
        return false;
    }

    public void Load(SettingType ST) { Load(SearchSaveSlot(ST)); }
    public void Load(SettingOriginator SO)
    {
        if (!SOLoadSettledbit[(int)SettingOriginator.AudioVolume])
        {
            string filePath = Application.persistentDataPath + "/" + SO.ToString() + ".json";  // 保存先パス

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);  // ファイルから読み込む
                switch (SO)
                {
                    case SettingOriginator.AudioVolume:
                        AudioManager audioManager = AudioManager.Instance();
                        audioManager.SetLoad(JsonUtility.FromJson<AudioVolume>(json));
                        
                        SOLoadSettledbit[(int)SettingOriginator.AudioVolume] = true;
                        SetLoadSettled(SO);
                        
                        break;
                }


            }
        }
        else
        {
            Debug.Log("No saved data found.");
        }
    }


    public void Save() { StartCoroutine(SaveWait()); }

    // 変更されたデータをJSONファイルとして保存
    private void Save<T>(T save, SettingOriginator SO) where T : class
    {
        SOLoadSettledbit[(int)SettingOriginator.AudioVolume] = false;

        // JSONにシリアライズ
        string json = JsonUtility.ToJson(save, true);

        // Addressablesの保存場所のパス（必要なディレクトリを作成）
        string filePath = Application.persistentDataPath + "/" + SO.ToString() + ".json";

        // ディレクトリが存在しない場合は作成する
        string directoryPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // JSONデータをファイルに書き込む
        File.WriteAllText(filePath, json, Encoding.UTF8);
        Debug.Log("JSON saved to: " + filePath);

        Load(SO);
    }

    IEnumerator SaveWait()
    {
        settingstatus[1] = true;
        for (int i = 0; i < settingBTs.Count; i++)
        {
            if (valueChangedCheck[i])
            {
                settingBTs[i].ValueReset();
                SOLoadSettledbit[(int)SearchSaveSlot(settingBTs[i].GetSettingType)] = false;
                saveTarget[(int)SearchSaveSlot(settingBTs[i].GetSettingType)] = true;
                // 型情報を取得
                Type type = loadSettled[(int)SearchSaveSlot(settingBTs[i].GetSettingType)].GetType();

                // フィールドを取得
                FieldInfo fieldInfo = type.GetField(settingBTs[i].GetSettingType.ToString(), BindingFlags.Public | BindingFlags.Instance);

                if (fieldInfo != null)
                {
                    switch (settingBTs[i].GetSettingType)
                    {
                        case SettingType.BGMVolume:
                        case SettingType.SEVolume:
                            // フィールドに新しい値を代入
                            fieldInfo.SetValue(loadSettled[(int)SearchSaveSlot(settingBTs[i].GetSettingType)], settingBTs[i].GetValue);
                            break;
                        case SettingType.BGMmute:
                        case SettingType.SEmute:
                            // フィールドに新しい値を代入
                            fieldInfo.SetValue(loadSettled[(int)SearchSaveSlot(settingBTs[i].GetSettingType)], (settingBTs[i].GetValue == 1) ? true : false);
                            break;
                    }
                }
            }
        }

        for (int i = 0; i < loadSettled.Count; i++)
        {
            if (saveTarget[i])
            {
                Save(loadSettled[i], (SettingOriginator)i);
            }
        }

        settingstatus[1] = false;
        yield return 0;

        ResetLoadTarget();
    }


}
