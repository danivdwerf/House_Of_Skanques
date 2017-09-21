using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour 
{
    private AudioManager manager;
    private int _karmaPolice;

    private void Start()
    {
        this.manager = this.GetComponent<AudioManager>();
        this._karmaPolice = manager.audioToID("KarmaPolice");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            manager.playSound(_karmaPolice);
    }
}
