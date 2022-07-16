using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class Audio : SingletonBehavior
{
    [SerializeField] private float dicePitchVariation = 0.2f;

    private enum AudioChannel { MUSIC, UI, DICE, SFX }; // Corresponds to audio group of the same name in the mixer
    private string[] channelNames;
    private Queue<AudioClip> musicQueue = new Queue<AudioClip>();

    private AudioMixer mixer;
    private AudioSource[] channels;
    private Dictionary<string, AudioClip>[] samples; // File naming scheme: channel/sample.mp3

    protected override void Awake()
    {
        base.Awake();

        mixer = Resources.Load<AudioMixer>("Audio/Master");
        channelNames = Enum.GetNames(typeof(AudioChannel));
        channels = new AudioSource[channelNames.Length];
        samples = new Dictionary<string, AudioClip>[channels.Length];

        // Search audio assets for channel directories and load samples
        List<DirectoryInfo> audioAssetDirs = new List<DirectoryInfo>(new DirectoryInfo("Assets/Resources/Audio").GetDirectories());
        for (int channel = 0; channel < channels.Length; channel++)
        {
            DirectoryInfo matchingDir = audioAssetDirs.Find(dir => dir.Name.ToUpper().Equals(channelNames[channel]));
            channels[channel] = gameObject.AddComponent<AudioSource>();
            if (matchingDir != null)
            {
                channelNames[channel] = matchingDir.Name;
            }
            channels[channel].outputAudioMixerGroup = mixer.FindMatchingGroups(channelNames[channel])[0];

            AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/" + channelNames[channel]);
            foreach (AudioClip clip in clips)
            {
                Debug.Log(clip.name);
                samples[channel].Add(clip.name, clip);
            }
        }
    }

    private void Start()
    {
        StartCoroutine(MusicLoop());
        StartCoroutine(TestDice());
        QueueMusic("sonatina_letsadventure_7MemoryBox");
        QueueMusic("sonatina_letsadventure_9MechanicalDancer");
    }


    public void SetRepeat(bool isEnabled)
    {
        AudioSource channel = channels[(int)AudioChannel.MUSIC];
        channel.loop = isEnabled;
    }

    public void QueueMusic(string track)
    {
        if (!samples[(int)AudioChannel.MUSIC].ContainsKey(track))
        {
            Debug.LogError($"Track \"{track}\" does not exist!");
            return;
        }

        musicQueue.Enqueue(samples[(int)AudioChannel.MUSIC][track]);
    }

    public void PlayMusic(string track)
    {
        if (!samples[(int)AudioChannel.MUSIC].ContainsKey(track))
        {
            Debug.LogError($"Track \"{track}\" does not exist!");
            return;
        }
        AudioSource channel = channels[(int)AudioChannel.MUSIC];

        channel.Stop();
        channel.clip = samples[(int)AudioChannel.MUSIC][track];
        channel.Play();
    }

    private IEnumerator MusicLoop()
    {
        AudioSource channel = channels[(int)AudioChannel.MUSIC];

        if (!channel.isPlaying && musicQueue.Count > 0)
        {
            channel.clip = musicQueue.Dequeue();
            channel.Play();
        }

        yield return new WaitForSeconds(channel.clip.length - channel.time);
    }

    private IEnumerator TestDice()
    {
        RollDice(UnityEngine.Random.Range(0.0f, samples[(int)AudioChannel.DICE]["blonkus"].length + 1.0f));
        yield return new WaitForSeconds(4.0f);
    }

    public void RollDice()
    {
        RollDice(0.0f);
    }

    public void RollDice(float minDuration)
    {
        AudioSource channel = channels[(int)AudioChannel.DICE];
        AudioClip track = samples[(int)AudioChannel.DICE]["blonkus"]; // TODO Bad, assumes blonkus exists
        float duration = track.length;

        foreach (AudioClip clip in samples[(int)AudioChannel.DICE].Values){
            if (duration > clip.length && clip.length > minDuration)
            {
                track = clip;
                duration = track.length;
            }
        }

        channel.pitch = UnityEngine.Random.Range(-dicePitchVariation, dicePitchVariation);
        channel.PlayOneShot(track);
    }
}

/*
    [SerializeField] float variety, ambientPassiveMaxWait, ambientPassiveMinWait;

    public void PlaySoundOnce(Channel chan, AudioClip smpl, float vol = 1f, float pitch = 1f, bool rand = false)
    {
        int chin = (int)chan;
        channels[chin].volume = vol;
        channels[chin].pitch = pitch + (rand ? Random.Range(-variety, variety) : 0f);
        channels[chin].PlayOneShot(smpl);
    }

    public void PlaySound(Channel chan, AudioClip smpl, float vol = 1f, float pitch = 1f, bool rand = false)
    {
        StartCoroutine(PlaySoundThrough(chan, smpl, vol, pitch, rand));
    }

    IEnumerator PlaySoundThrough(Channel chan, AudioClip smpl, float vol, float pitch, bool rand)
    {
        int chin = (int)chan;
        channels[chin].volume = vol;
        channels[chin].pitch = pitch + (rand ? Random.Range(-variety, variety) : 0f);
        channels[chin].clip = smpl;
        channels[chin].Play();
        yield return new WaitWhile(() => channels[chin].isPlaying);
    }

    public void StartSound(Channel chan, AudioClip smpl, float vol = 1f, float pitch = 1f)
    {
        int chin = (int)chan;
        ToggleLoop(chan);
        channels[chin].volume = vol;
        channels[chin].pitch = pitch;
        channels[chin].clip = smpl;
        channels[chin].Play();

    }

    public void ToggleLoop(Channel chan)
    {
        int chin = (int)chan;
        channels[chin].loop = !channels[chin].loop;
    }

    public void StopSound(Channel chan)
    {
        channels[(int)chan].Stop();
        ToggleLoop(chan);

    }

    public bool IsPlaying(Channel chan)
    {
        return channels[(int)chan].isPlaying;
    }

    public AudioClip GetSample(string name)
    {
        foreach (AudioClip smpl in samples)
        {
            if (name.Equals(smpl.name))
                return smpl;
        }
        return null;
    }

    public AudioSource GetChannel(Channel chan)
    {
        return channels[(int)chan];
    }

    public AudioClip GetRandomSample(string sub)
    {
        AudioClip[] possible = new AudioClip[samples.Length];
        int i = 0;
        foreach (AudioClip smpl in samples)
        {
            if (smpl.name.Contains(sub))
                possible.SetValue(smpl, i++);
        }
        if (i == 0)
            return null;
        return possible[Random.Range(0, i)];
    }

    IEnumerator PlayAmbient()
    {
        int chin = (int)Channel.ambientPassive;
        AudioSource chan = channels[chin];
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(ambientPassiveMaxWait, ambientPassiveMinWait));
            chan.clip = GetRandomSample(channelNames[chin]);
            chan.Play();
            yield return new WaitWhile(() => chan.isPlaying);
        }
    }
*/