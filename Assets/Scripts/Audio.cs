using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Audio : SingletonBehavior<Audio>
{
    [SerializeField] private float dicePitchVariation = 0.2f;

    private Dictionary<string, AudioChannel> channels = new Dictionary<string, AudioChannel>();
    private Dictionary<string, AudioSource> globalSources = new Dictionary<string, AudioSource>();

    public AudioMixer Mixer
    {
        get;
        private set;
    }

    protected override void Awake()
    {
        base.Awake();

        Mixer = Resources.Load<AudioMixer>("Audio/Master");
        if (Mixer == null)
        {
            Debug.LogError("Master mixer does not exist!");
            return;
        }

        foreach (AudioMixerGroup group in Mixer.FindMatchingGroups("Master/"))
        {
            AudioChannel channel = new AudioChannel(group.name);
            if (!channel.Initialize())
            {
                continue;
            }
            channels.Add(channel.Id, channel);
            globalSources.Add(channel.Id, gameObject.AddComponent<AudioSource>());
            channel.Register(globalSources[channel.Id]);
        }
        if (channels.Count < 1)
        {
            Debug.LogError("No channels could be initialized!");
            return;
        }

        globalSources["Music"].loop = true;
    }

    public void Volume(float vol)
    {
        Mixer.SetFloat("masterVolume", vol);
    }

    public void Register(AudioSource source, string channel)
    {
        channels[channel].Register(source);
    }

    public void PlaySound(string channel, string track)
    {
        globalSources[channel].PlayOneShot(channels[channel].GetSample(track));
    }

    public void StopMusic()
    {
        globalSources["Music"].Stop();
    }

    public void PlayMusic(string track)
    {
        AudioSource music = globalSources["Music"];

        if (music.isPlaying)
        {
            music.Stop();
        }
        music.clip = channels["Music"].GetSample(track);
        music.Play();
    }

    public float RollDice(AudioSource source)
    {
        return RollDice(UnityEngine.Random.Range(0.0f, channels["Dice"].LongestClipDuration + 5.0f), source);
    }

    public float RollDice(float minDuration, AudioSource source)
    {
        float duration = channels["Dice"].LongestClipDuration + float.Epsilon;
        AudioClip clip = channels["Dice"].GetSample("roll");

        foreach (AudioClip track in channels["Dice"].GetAllSamples())
        {
            if (duration > track.length && track.length > minDuration)
            {
                clip = track;
                duration = track.length;
            }
        }

        source.pitch = UnityEngine.Random.Range(1.0f - dicePitchVariation, 1.0f + dicePitchVariation);
        source.PlayOneShot(clip);
        return duration;
    }
}
