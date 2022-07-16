using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class AudioChannel
{
    private static List<DirectoryInfo> sampleDirectories = new List<DirectoryInfo>(new DirectoryInfo("Assets/Resources/Audio").GetDirectories());
    private static int lastIdx = -1;

    private Dictionary<string, AudioClip> samples; // File naming scheme: channel/sample.mp3
    private List<AudioSource> sources = new List<AudioSource>();

    public int Number
    {
        get;
    }

    // Corresponds to audio group of the same name in the mixer and resource directory
    public string Id
    {
        get;
    }

    public AudioChannel(string id)
    {
        this.Id = id;
        Number = ++lastIdx;
    }

    public bool Initialize()
    {
        DirectoryInfo matchingDirectory = sampleDirectories.Find(dir => dir.Name.Equals(Id));
        if (matchingDirectory == null)
        {
            Debug.LogError($"No directory matching {Id}!");
            return false;
        }

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
                //Debug.Log(track.name);
                samples.Add(track.name, track);
            }
        }

        sources[0].outputAudioMixerGroup = matchingGroups[0];
        return true;
    }
}

