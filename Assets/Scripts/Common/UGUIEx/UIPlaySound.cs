using UnityEngine;

public enum SoundPlayEnum
{
    OnAwake,
    OnEnable,
    OnClick,
}

public class UIPlaySound : MonoBehaviour
{
    public SoundPlayEnum state = SoundPlayEnum.OnClick;
    public AudioClip clip;
    public string audio_name = "SND";
    public float volume = 1;

    // Use this for initialization
    void Awake()
    {
        UIEventListener.Get(gameObject).onClickToSound += OnClick;
        if(state == SoundPlayEnum.OnAwake)
            Play();
    }
    void OnEnable()
    {
        if(state == SoundPlayEnum.OnEnable)
            Play();
    }
    void OnClick(GameObject go)
    {
        if(state == SoundPlayEnum.OnClick)
            Play();
    }

    void Play()
    {
        if(clip == null && !string.IsNullOrEmpty(audio_name))
        {
            clip = ObjectsManager.LoadObject<AudioClip>(BundleBelong.audio, audio_name);
        }
        if(clip == null)
            return;

        AudioSource source = GetComponent<AudioSource>();
        if(source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            source.priority = 50;
            source.pitch = 1;
        }
        source.PlayOneShot(clip, volume);
    }
} 
