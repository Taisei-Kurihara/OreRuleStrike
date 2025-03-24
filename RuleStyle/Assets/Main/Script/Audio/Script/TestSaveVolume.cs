using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSaveVolume : MonoBehaviour
{
    [SerializeField]
    AudioVolume volume;
    private void Awake()
    {
        AudioManager audioManager = AudioManager.Instance();
        //audioManager.SaveVolume(volume);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
