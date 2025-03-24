using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestTimeChecker : MonoBehaviour
{
    Transform my;

    [SerializeField]
    Vector3 max = Vector3.one;

    private void Awake()
    {
        System.Random random = new System.Random();

        // 0à»è„10ñ¢ñûÇÃóêêîÇê∂ê¨
        max.x = random.Next(100,500);
        max.y = random.Next(100,500);
        max.z = random.Next(100,500);
        my = GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        my.Rotate((1 / max.x) / Time.deltaTime, (1 / max.y) / Time.deltaTime, (1 / max.z) / Time.deltaTime);
    }
}
