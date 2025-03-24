using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRotationUI : MonoBehaviour
{
    [SerializeField]
    int mynumber = 1;

    private void Awake()
    {
        StartCoroutine(SetDirection());
    }

    IEnumerator SetDirection()
    {

        RectTransform rectTransform = this.GetComponent<RectTransform>();

        GameManager gameManager = GameManager.Instance();

        // ‚±‚±‚Ål”‚ðŽæ“¾‚µ‚Ä‚¨‚­
        yield return new WaitUntil(() => true);
        if (mynumber <= gameManager.PlayerNum)
        {
            Transform parentTransform = transform.parent;
            RotationUI rotationUI = parentTransform.GetComponent<RotationUI>();
            rotationUI.InRotationUI(rectTransform, mynumber);
        }
        else
        {
            rectTransform.position = new Vector2(Screen.width, Screen.height)*2;
        }
    }
}
