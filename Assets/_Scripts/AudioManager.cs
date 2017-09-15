using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [SerializeField]private List<Audio> audioClips;

    private void Awake()
    {
        for (var i = 0; i < audioClips.Count; i++)
        {
            GameObject source = new GameObject ("Audio: " + audioClips[i].Name.ToString());
            source.transform.SetParent (this.transform);
            audioClips [i].setSource (source.AddComponent<AudioSource>());
        }
    }

    public void playSound(int id){audioClips[id].play ();}
    public void stopSound(int id){audioClips[id].stop ();}
    public bool isPlaying(int id){return audioClips[id].Source.isPlaying;}

    public int audioToID(string name)
    {
        for (var i = 0; i < audioClips.Count; i++)
        {
            if (audioClips [i].Name == name)
                return i;
        }
        return -1;
    }
}