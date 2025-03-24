using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPlayerUINum : MonoBehaviour
{
    [SerializeField]
    Image[] images = new Image[4];

    public void SetUI(List<int> num)
    {
        StartCoroutine(SetWait(num));
    }

    IEnumerator SetWait(List<int> num)
    {
        Debug.Log("UISet");
        PlayerUISet playerUISet = PlayerUISet.Instance();
        yield return new WaitUntil(() => playerUISet.check);
        Debug.Log(num.Count);

        for (int i = 0; i < num.Count; i++)
        {
            images[i].sprite = playerUISet.GetPlayerUI(num[i]-1);
        }
    }

    public void SetUI(int num)
    {
        PlayerUISet playerUISet = PlayerUISet.Instance();
        Debug.Log(num);
        for (int i = 0; i < num; i++)
        {
            images[i].sprite = playerUISet.GetPlayerUI(i);
        }
    }
}
