using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// MainŽž‚ÌUI
/// </summary>
[Serializable]
public class Main_UI_Component
{
    public GameObject MainUI;

    public Image CurrentPlayerImage;
    public TMPro.TextMeshProUGUI CurrentPlayerRule_Text;

    public Image OnePlayerImage;
    public TMPro.TextMeshProUGUI OnePlayerRule_Text;

    public Image TwoPlayerImage;
    public TMPro.TextMeshProUGUI TwoPlayerRule_Text;

    public Image ThreePlayerImage;
    public TMPro.TextMeshProUGUI ThreePlayerRule_Text;

    public Button Option;
}