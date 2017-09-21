using UnityEngine;

using System;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour 
{
    [SerializeField]private List<Audio> audioClips;
    public List<Audio> AudioClips{get{return audioClips;} set{audioClips = value;}}
    public event Action<Audio> PlayAudio;

    private void Awake()
    {
        for (var i = 0; i < audioClips.Count; i++)
        {
            GameObject go = new GameObject ("Audio: " + audioClips[i].Name.ToString());
            go.transform.SetParent (this.transform);
            audioClips [i].setSource (go.AddComponent<AudioSource>());
        }
    }

    public void playSound(int id)
    {
        audioClips[id].play ();
        if(PlayAudio != null)
            PlayAudio(audioClips[id]);
    }

    public void stopSound(int id){audioClips[id].stop ();}
    public bool isPlaying(int id){return audioClips[id].Source.isPlaying;}

    public int audioToID(string name)
    {
        for (var i = 0; i < audioClips.Count; i++)
        {
            if (audioClips [i].Name == name) return i;
        }
        return -1;
    }
}