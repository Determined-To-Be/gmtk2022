using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class AudioChannel
{
    private static List<DirectoryInfo> sampleDirectories = new List<DirectoryInfo>(new DirectoryInfo("Assets/Resources/Audio").GetDirectories());
    private static int lastIdx = -1;

    private Dictionary<string, AudioClip> samples;
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

    public float LongestClipDuration
    {
        get;
        private set;
    } = 0.0f;

    public AudioChannel(string id)
    {
        this.Id = id;
        Number = ++lastIdx;
    }

    public AudioClip GetSample(string sample)
    {
        if (!samples.ContainsKey(sample))
        {
            Debug.LogWarning($"Sample {sample} does not exist in channel {Id}!");
            return null;
        }
        return samples[sample];
    }

    public AudioClip[] GetAllSamples()
    {
        AudioClip[] clips = new AudioClip[samples.Count];
        samples.Values.CopyTo(clips, 0);
        return clips;
    }

    public void Register(AudioSource source)
    {
        source.outputAudioMixerGroup = group;
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
            samples = new Dictionary<string, AudioClip>();
            foreach (AudioClip track in tracks)
            {
                samples.Add(track.name, track);

                if (track.length > LongestClipDuration)
                {
                    LongestClipDuration = track.length;
                }
            }
        }

        return true;
    }
}

