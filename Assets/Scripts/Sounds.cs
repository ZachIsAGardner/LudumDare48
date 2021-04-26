using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public List<AudioClip> clips;
    static Sounds instance;
    static AudioSource song;

    void Awake()
    {
        instance = this;
    }

    public static void Play(string name, Vector3? position = null, bool isSong = false, float volume = 1, float pitch = 1)
    {
        if (song != null && song.name == name) return;

        AudioClip clip = instance.clips.FirstOrDefault(x => x.name == name);

        if (clip == null) return;

        var go = new GameObject(name);
        go.transform.position = position ?? Vector3.zero;
        AudioSource source = go.AddComponent<AudioSource>();
        source.volume = volume;
        source.pitch = pitch;
        if (position != null)
        {
            source.spatialBlend = 1;
            source.maxDistance = 30;
        }
        if (isSong)
        {
            if (song != null) 
            {
                song.Stop();
                Destroy(song.gameObject);
            }
            source.clip = clip;
            source.loop = true;
            source.gameObject.AddComponent<DontDestroyOnLoad>();
            source.Play();

            song = source;
        }
        else
        {
            source.PlayOneShot(clip);
            Timer.Create(clip.length + 0.1f, () =>
            {
                Destroy(go);
            });
        }
    }
}
