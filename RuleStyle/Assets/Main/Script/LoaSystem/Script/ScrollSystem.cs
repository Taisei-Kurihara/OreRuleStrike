using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class ScrollSystem : MonoBehaviour
{
    [SerializeField,Header("�g�p����X���C�_�[��handle��ݒ�")]
    RectTransform handle;

    [SerializeField]
    Vector2 handleSize = Vector2.one;

    [SerializeField,Header("�g�p����X���C�_�[��ݒ�")]
    Scrollbar slider;

    [SerializeField,Header("������object��ݒ�")]
    RectTransform MovePoint;

    [SerializeField, Header("����������ݒ�")]
    Vector2 MoveDis = new Vector2(0,0);

    Vector2 firstMoveDis = Vector2.zero;


    // Start is called before the first frame update
    void Start()
    {
        // �����ʒu��ݒ�
        firstMoveDis = MovePoint.transform.position;

        // handle�̃T�C�Y��ݒ�
        handle.localScale = handleSize;

        // �X���C�_�[���������ꂽ�Ǝ��Ɋ֐����Ăяo��
        slider.onValueChanged.AddListener(SlideValue);
    }

    void SlideValue(float value)
    {
        // �ʒu��ݒ肷��
        MovePoint.transform.position = firstMoveDis + (MoveDis*value);
    }

    void OnDestroy()
    {
        // ���������[�N�h�~�̂��߂Ƀ��X�i�[���폜
        slider.onValueChanged.RemoveListener(SlideValue);
    }
}
