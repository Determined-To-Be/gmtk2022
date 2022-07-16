using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class AudioChannel
{
    private static List<DirectoryInfo> sampleDirectories = new List<DirectoryInfo>(new DirectoryInfo("Assets/Resources/Audio").GetDirectories());
    private static int lastIdx = -1;

    private List<AudioSource> sources = new List<AudioSource>();
    private AudioMixerGroup group;

    public int Number
    {
        get;
    }

    // Corresponds to audio group of the same name in the mixer and resource directory
    public string Id
    {
        get;
    }


    // File naming scheme: channel/sample.mp3
    public Dictionary<string, AudioClip> Samples
    {
        get;
        private set;
    }

    public AudioClip LongestClip
    {
        get;
        private set;
    }

    public AudioClip ShortestClip
    {
        get;
        private set;
    }

    public float LongestClipDuration
    {
        get;
        private set;
    } = 0.0f;

    public float ShortestClipDuration
    {
        get;
        private set;
    } = float.MaxValue;

    public AudioChannel(string id)
    {
        this.Id = id;
        Number = ++lastIdx;
    }

    public AudioSource AddSource(AudioSource source)
    {
        source.outputAudioMixerGroup = group;
        sources.Add(source);
        return source;
    }

    public bool Initialize()
    {
        // Get sample directory
        DirectoryInfo matchingDirectory = sampleDirectories.Find(dir => dir.Name.Equals(Id));
        if (matchingDirectory == null)
        {
            Debug.LogError($"No directory matching {Id}!");
            return false;
        }

        // Get matching group
        AudioMixer mixer = Audio.GetInstance().Mixer;
        AudioMixerGroup[] matchingGroups = mixer.FindMatchingGroups(Id);
        if (matchingGroups.Length < 1)
        {
            Debug.LogError($"No {mixer.name} mixer group matching {Id}!");
            return false;
        }
        else if (matchingGroups.Length > 1)
        {
            Debug.LogError($"Multiple {mixer.name} mixer groups matching {Id}!");
            return false;
        }
        group = matchingGroups[0];

        // Load tracks
        AudioClip[] tracks = Resources.LoadAll<AudioClip>("Audio/" + Id);
        if (tracks == null)
        {
            Debug.LogError($"Unable to load samples from {Id}! Initialization failed.");
            return false;
        }
        else if (tracks.Length < 1)
        {
            Debug.LogWarning($"No tracks from {Id}.");
        }
        else
        {
            Samples = new Dictionary<string, AudioClip>();
            foreach (AudioClip track in tracks)
            {
                //Debug.Log(track.name);
                Samples.Add(track.name, track);

                if (track.length > LongestClipDuration)
                {
                    LongestClipDuration = track.length;
                    LongestClip = track;
                }
                if (track.length < ShortestClipDuration)
                {
                    ShortestClipDuration = track.length;
                    ShortestClip = track;
                }
            }
        }

        return true;
    }
}

