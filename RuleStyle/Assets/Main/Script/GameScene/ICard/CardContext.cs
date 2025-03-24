using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カード変更機能
/// </summary>
public class CardContext
{
    CardContext() {
        gameManager = GameManager.Instance();
    }
    private GameManager gameManager=null;
    public void CardChange(ICard card,int i)
    {
        
    }
}
