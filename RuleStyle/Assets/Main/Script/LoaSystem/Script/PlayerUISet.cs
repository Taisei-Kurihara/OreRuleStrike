using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerUISet : SingletonMonoBehaviourBase<PlayerUISet>
{


    Sprite[] sprite = new Sprite[4] { null,null,null,null };

    string[] names = { "Genbu", "Seiryuu","Suzaku","Byakko" };
    
    bool spOK = false;

    public bool check { get { return spOK; } }

    private void Awake()
    {
        // 4つのスプライトをロード
        for (int i = 0; i < sprite.Length; i++)
        {
            int index = i; // クロージャ問題を回避するためのローカル変数
            Addressables.LoadAssetAsync<Sprite>(names[i]).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    sprite[index] = handle.Result; // 配列に保存
                }
                else
                {
                    Debug.LogError($"Failed to load sprite: {sprite[index]}");
                }
            };
        }

        StartCoroutine(checkWait());
    }

    IEnumerator checkWait()
    {
        yield return new WaitUntil(() => (sprite[0] != null && sprite[1] != null && sprite[2] != null && sprite[3] != null));
        spOK = true;
    }

    public Sprite GetPlayerUI(int num)
    {
        Debug.Log(num);
        if(num >= 0 && num < 4) 
        {
            return sprite[num];
        }
        return null;
    }



}
