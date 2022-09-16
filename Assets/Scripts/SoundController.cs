using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour
{
    public static AudioSource source;

    // Start is called before the first frame update
    void Awake()
    {
        source = GameObject.Find("SoundController").GetComponent<AudioSource>();
    }

    public static void PlaySound(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }
}
