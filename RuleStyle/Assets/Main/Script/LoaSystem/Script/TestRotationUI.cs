using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestRotationUI : MonoBehaviour
{
    Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(TestRotation);
    }


    void TestRotation()
    {
        Transform parentTransform = transform.parent;
        RotationUI rotationUI = parentTransform.GetComponent<RotationUI>();
        rotationUI.StartRotation();
    }

}
