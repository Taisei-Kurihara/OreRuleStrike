using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

// 音声タイプの列挙型。BGM（背景音楽）やSE（効果音）の種類を指定
public enum AudioType
{
    BGM, SE,        // 背景音楽と効果音
    NextBGM, NextSE  // 次のBGMと次のSE
}

// 音量設定を格納するクラス
[System.Serializable]
public class AudioVolume
{
    [SerializeField]
    public float BGMVolume = 1;  // BGMの音量（デフォルトは最大）
    [SerializeField]
    public float SEVolume = 1;   // SEの音量（デフォルトは最大）
    [SerializeField]
    public bool BGMmute = true;  // BGMのミュート設定（デフォルトはミュート）
    [SerializeField]
    public bool SEmute = true;   // SEのミュート設定（デフォルトはミュート）
}

// 音声管理を行うクラス
public class AudioManager : SingletonMonoBehaviourBase<AudioManager>
{
    // 各AudioSourceを保持する変数
    AudioSource BGMSource;
    AudioSource NextBGMSource;
    AudioSource SESource;
    AudioSource NextSESource;

    // 音量設定を保持する変数
    AudioVolume volumes;

    // ゲーム開始時の初期化処理
    private void Start()
    {
        // 設定をロード
        SettingManager settingManager = SettingManager.Instance();
        settingManager.Load(SettingOriginator.AudioVolume);

        // AudioSourceを追加
        BGMSource = gameObject.AddComponent<AudioSource>();
        SESource = gameObject.AddComponent<AudioSource>();
        NextBGMSource = gameObject.AddComponent<AudioSource>();
        NextSESource = gameObject.AddComponent<AudioSource>();

        // 音量設定を反映
        SetVolume();
    }

    // 音量設定を適用するメソッド
    public void SetVolume() { StartCoroutine(SetVolumes()); }

    // 音量設定を反映させるコルーチン
    IEnumerator SetVolumes()
    {
        // BGMSourceが設定されるのを待機
        yield return new WaitUntil(() => (BGMSource != null) ? true : false);
        BGMSource.volume = (volumes.BGMmute) ? volumes.BGMVolume : 0;
        BGMSource.loop = true;  // BGMはループする設定

        // NextBGMSourceが設定されるのを待機
        yield return new WaitUntil(() => (NextBGMSource != null) ? true : false);
        NextBGMSource.volume = (volumes.BGMmute) ? volumes.BGMVolume : 0;
        NextBGMSource.loop = true;

        // SESourceが設定されるのを待機
        yield return new WaitUntil(() => (SESource != null) ? true : false);
        SESource.volume = (volumes.SEmute) ? volumes.SEVolume : 0;
        SESource.loop = false;  // SEはループしない設定

        // NextSESourceが設定されるのを待機
        yield return new WaitUntil(() => (NextSESource != null) ? true : false);
        NextSESource.volume = (volumes.SEmute) ? volumes.SEVolume : 0;
        NextSESource.loop = false;
    }

    // 音楽をフェードアウトするメソッド
    public void AudioFadeOut(float maxTime) { AudioFade(maxTime, FadeSpecified._1to0); }

    // 音楽をフェードインするメソッド
    public void AudioFadeIn(float maxTime) { AudioFade(maxTime, FadeSpecified._0to1); }

    // 音量フェード処理
    public void AudioFade(float maxTime, FadeSpecified fade)
    {
        Time_TimerManager time_TimerManager = Time_TimerManager.Instance();
        time_TimerManager.Fade(FadeVolumeWait, maxTime, fade); // フェード処理を実行
    }

    // フェードの待機処理
    void FadeVolumeWait(float fade)
    {
        // フェードに応じた音量を設定
        BGMSource.volume = (volumes.BGMmute) ? volumes.BGMVolume * fade : 0;
    }

    // 音量設定を上書きするメソッド
    public void SetLoad(AudioVolume sstVolume) { volumes = sstVolume; SetVolume(); }

    // 現在の音量設定を取得するプロパティ
    public AudioVolume GetVolume { get { return volumes; } }

    // BGMを設定するプロパティ
    public AudioClip SetBGM { set { BGMSource.clip = value; } }

    // 次のBGMを設定するプロパティ
    public AudioClip SetNextBGM { set { NextBGMSource.clip = value; } }

    // BGMを再生するメソッド
    public void PlayBGM()
    {
        BGMSource.loop = true;  // BGMはループする設定
        StartCoroutine(Check(AudioType.BGM));
    }


    // SEを設定するプロパティ
    public AudioClip SetSE { set { SESource.clip = value; } }

    // 次のSEを設定するプロパティ
    public AudioClip SetNextSE { set { NextSESource.clip = value; } }

    // SEを再生するメソッド
    public void PlaySE() { StartCoroutine(Check(AudioType.SE)); }

    // 指定した音声タイプを再生するメソッド
    public void Play(AudioType type) { StartCoroutine(Check(type)); }

    /// <summary>
    /// AudioTypeに応じて音声の再生を処理
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IEnumerator Check(AudioType type)
    {
        yield return 0;

        switch (type)
        {
            case AudioType.BGM:
                if (BGMSource.clip == null)
                {
                    // 最初のBGMをロード
                    LoadFirst("firstBGM", type);
                    yield return new WaitUntil(() => (BGMSource.clip != null) ? true : false);
                }
                BGMSource.Play();  // BGMを再生
                Debug.Log(BGMSource.isPlaying + "/start BGM : " + BGMSource.clip.name);
                break;
            case AudioType.SE:
                if (SESource.clip == null)
                {
                    // 最初のSEをロード
                    LoadFirst("firstSE", type);
                    yield return new WaitUntil(() => (SESource.clip != null) ? true : false);
                }
                SESource.Play();  // SEを再生
                break;
            case AudioType.NextBGM:
                // 次のBGMはまだ処理なし
                break;
            case AudioType.NextSE:
                if (NextSESource.clip == null)
                {
                    // 次のSEをロード
                    LoadFirst("nextSE", type);
                    yield return new WaitUntil(() => (NextSESource.clip != null) ? true : false);
                }
                NextSESource.Play();  // 次のSEを再生
                break;
        }
    }

    /// <summary>
    /// 最初の音声データ（BGM/SE）をロードするメソッド
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    private void LoadFirst(string name, AudioType type)
    {
        // Addressablesを使って非同期でAudioClipをロード
        AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(name);

        // ロード完了後の処理
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                // ロード成功時に対応するAudioClipを設定
                switch (type)
                {
                    case AudioType.BGM:
                        SetBGM = op.Result;
                        break;
                    case AudioType.SE:
                        SetSE = op.Result;
                        break;
                    case AudioType.NextBGM:
                        SetNextBGM = op.Result;
                        break;
                    case AudioType.NextSE:
                        SetNextSE = op.Result;
                        break;
                }
            }
            else
            {
                // ロード失敗時にエラーログを表示
                Debug.LogError("AudioClipのロードに失敗しました");
            }
        };
    }
}