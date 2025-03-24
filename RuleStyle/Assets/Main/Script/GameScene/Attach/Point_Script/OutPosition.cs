using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// èÍäO
/// </summary>
public class OutPosition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<Player_Attach>()._playerData.TurnEnd();
        Destroy(other.gameObject);
    }
}
