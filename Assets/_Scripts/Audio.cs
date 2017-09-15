using UnityEngine;

[System.Serializable]
public class Audio
{
    [SerializeField]private string name = "";
    public string Name{get{return name;} set{name = value;}}

    [SerializeField]private AudioClip clip = null;
    public AudioClip Clip{get{return clip;} set{clip = value;}}

    [Range(0, 1)][SerializeField]private float volume = 1;
    public float Volume{get{return volume;} set{volume = value;}}

    [Range(-3, 3)][SerializeField]private float pitch = 1;
    public float Pitch{get{return pitch;} set{pitch = value;}}

    [Range(-1, 1)][SerializeField]private float panning = 0;
    public float Panning{get{return panning;} set{panning = value;}}

    [SerializeField]private bool loop = false;
    public bool Loop{get{return loop;} set{loop = value;}}

    [SerializeField]private bool playFromStart = false;
    public bool PlayFromStart{get{return playFromStart;} set{playFromStart = value;}}

    [Range(0,1)][SerializeField]private float spatialBlend = 0;
    public float SpatialBlend{get{return spatialBlend;} set{spatialBlend = value;}}

    [SerializeField]private float minDistance = 0;
    public float MinDistance{get{return minDistance;} set{minDistance = value;}}

    [SerializeField]private float maxDistance = 10;
    public float MaxDistance{get{return maxDistance;} set{maxDistance = value;}}

    [SerializeField]private Transform soundPosition = null;
    public Transform SoundPosition{get{return soundPosition;} set{soundPosition = value;}}

    private AudioSource source;
    public AudioSource Source{get{return source;}}

    public void setSource(AudioSource source)
    {
        this.source = source;
        this.source.playOnAwake = false;
        this.source.clip = this.clip;
        this.source.volume = this.volume;
        this.source.pitch = this.pitch;
        this.source.panStereo = this.panning;
        this.source.loop = this.loop;
        this.source.spatialBlend = this.spatialBlend;

        if (this.spatialBlend > 0)
        {
            if (soundPosition == null)
                soundPosition = this.source.transform;
            this.source.minDistance = this.minDistance;
            this.source.maxDistance = this.maxDistance;
            this.source.rolloffMode = AudioRolloffMode.Linear;
            this.source.transform.position = soundPosition.position;
        }

        if (this.playFromStart)
            this.source.Play();
    }

    public void play()
    {
        if(!this.source.isPlaying)
            this.source.Play ();
    }

    public void stop(){this.source.Stop ();}
}