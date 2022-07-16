using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Audio : SingletonBehavior<Audio>
{
    [SerializeField] private float dicePitchVariation = 0.2f;

    private Dictionary<string, AudioChannel> channels = new Dictionary<string, AudioChannel>();
    private LinkedList<AudioClip> queue = new LinkedList<AudioClip>();

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
        }
        if (channels.Count < 1)
        {
            Debug.LogError("No channels could be initialized!");
            return;
        }
    }
    private void Start()
    {
        //StartCoroutine(TestDice(channels["Dice"].AddSource(gameObject.AddComponent<AudioSource>())));
    }

    public void PlayMusic()
    {
        // TODO Play music from last place (or start of queue if nothing in progress)
    }

    public void PlayMusic(string track)
    {
        // TODO Play this track now, stop the other one
    }

    public void QueueMusic(string track)
    {
        QueueMusic(track, false);
    }

    public void QueueMusic(string track, bool now)
    {
        AudioClip clip = channels["Music"].GetSample(track);
        if (clip == null) return;

        if (now)
        {
            queue.AddFirst(clip);
            return;
        }
        queue.AddLast(clip);
    }

    private IEnumerator TestDice(AudioSource source)
    {
        while (true)
        {
            yield return new WaitForSeconds(RollDice(source));
        }
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

    /*
    public void SetRepeat(bool isEnabled)
    {
        AudioSource channel = channels[(int)AudioChannel.MUSIC];
        channel.loop = isEnabled;
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
    }*/
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
*/