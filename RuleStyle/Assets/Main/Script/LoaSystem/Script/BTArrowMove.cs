using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
class BTVertical
{
    [SerializeField]
    Button[] BThorizontal;
}

public class BTArrowMove : MonoBehaviour
{
    [SerializeField]
    BTVertical[] BT;

    Vector2 num;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
