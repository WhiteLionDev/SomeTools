using System.Collections;
using UnityEngine;

public class AudioSourceCrossFade : MonoBehaviour
{
    private AudioSource[] _player;
    private AudioSource[] _soundPlayer;
    private IEnumerator[] fader = new IEnumerator[2];
    private int ActivePlayer = 0;

    private int volumeChangesPerSecond = 15;

    public float fadeDuration = 1.0f;

    private int soundPlayerIndex = 0;

    private static AudioSourceCrossFade _instance;

    /// <summary>
    /// Singleton design pattern
    /// </summary>
    public static AudioSourceCrossFade Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioSourceCrossFade>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(AudioSourceCrossFade).ToString();
                    _instance = obj.AddComponent<AudioSourceCrossFade>();
                }
            }
            return _instance;
        }
    }

    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float _volume = 1.0f;
    public float volume
    {
        get
        {
            return _volume;
        }
        set
        {
            _volume = value;
        }
    }

    /// <summary>
    /// Mutes all AudioSources, but does not stop them!
    /// </summary>
    public bool mute
    {
        set
        {
            foreach (AudioSource s in _player)
            {
                s.mute = value;
            }
        }
        get
        {
            return _player[ActivePlayer].mute;
        }
    }

    /// <summary>
    /// Setup the AudioSources
    /// </summary>
    private void Awake()
    {
        // Generate the two AudioSources
        _player = new AudioSource[2]{
            gameObject.AddComponent<AudioSource>(),
            gameObject.AddComponent<AudioSource>()
        };

        // Set default values
        for (int i = 0; i < _player.Length; i++)
        {
            _player[i].loop = true;
            _player[i].playOnAwake = false;
            _player[i].volume = 0.0f;
        }

        GameObject child = new GameObject("Sound Manager");
        child.transform.parent = gameObject.transform;

        _soundPlayer = new AudioSource[3]
        {
            child.AddComponent<AudioSource>(),
            child.AddComponent<AudioSource>(),
            child.AddComponent<AudioSource>()
        };

        for (int i = 0; i < _soundPlayer.Length; i++)
        {
            _soundPlayer[i].loop = false;
            _soundPlayer[i].playOnAwake = false;
            _soundPlayer[i].volume = _volume;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Starts the fading of the provided AudioClip and the running clip
    /// </summary>
    /// <param name="clip">AudioClip to fade-in</param>
    public void Play(AudioClip clip)
    {
        // Prevent fading the same clip on both players 
        if (clip == _player[ActivePlayer].clip)
        {
            return;
        }

        // Kill all playing
        foreach (IEnumerator i in fader)
        {
            if (i != null)
            {
                StopCoroutine(i);
            }
        }

        // Fade-out the active play, if it is not silent (eg: first start)
        if (_player[ActivePlayer].volume > 0)
        {
            fader[0] = FadeAudioSource(_player[ActivePlayer], fadeDuration, 0.0f, () => { fader[0] = null; });
            StartCoroutine(fader[0]);
        }

        // Fade-in the new clip
        int NextPlayer = (ActivePlayer + 1) % _player.Length;
        _player[NextPlayer].clip = clip;
        _player[NextPlayer].Play();
        fader[1] = FadeAudioSource(_player[NextPlayer], fadeDuration, volume, () => { fader[1] = null; });
        StartCoroutine(fader[1]);

        // Register new active player
        ActivePlayer = NextPlayer;
    }

    /// <summary>
    /// Plays a sound effect
    /// </summary>
    /// <param name="soundClip">AudioClip to play</param>
    public void PlaySound(AudioClip soundClip)
    {
        if (_soundPlayer.Length > soundPlayerIndex)
        {
            _soundPlayer[soundPlayerIndex].clip = soundClip;
            _soundPlayer[soundPlayerIndex].Play();

            soundPlayerIndex++;

            if (soundPlayerIndex >= _soundPlayer.Length)
                soundPlayerIndex = 0;
        }
    }

    /// <summary>
    /// Fades an AudioSource(player) during a given amount of time(duration) to a specific volume(targetVolume)
    /// </summary>
    /// <param name="player">AudioSource to be modified</param>
    /// <param name="duration">Duration of the fading</param>
    /// <param name="targetVolume">Target volume, the player is faded to</param>
    /// <param name="finishedCallback">Called when finshed</param>
    /// <returns></returns>
    IEnumerator FadeAudioSource(AudioSource player, float duration, float targetVolume, System.Action finishedCallback)
    {
        //Calculate the steps
        int Steps = (int)(volumeChangesPerSecond * duration);
        float StepTime = duration / Steps;
        float StepSize = (targetVolume - player.volume) / Steps;

        //Fade now
        for (int i = 1; i < Steps; i++)
        {
            player.volume += StepSize;
            yield return new WaitForSeconds(StepTime);
        }
        //Make sure the targetVolume is set
        player.volume = targetVolume;

        //Callback
        if (finishedCallback != null)
        {
            finishedCallback();
        }
    }
}