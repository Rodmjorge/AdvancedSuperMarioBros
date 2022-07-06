using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManager;
    public static GameObject audioManagerGO { get { return audioManager.gameObject; } }


    public static AudioSource musicCurrentlyPlaying = null;

    public static AudioManager CreateAudioManager()
    {
        GameObject GO = new GameObject() { name = "AudioManager" };
        AudioManager audioManager0 = GO.AddComponent<AudioManager>();

        audioManager = audioManager0;
        return audioManager0;
    }

    public static void PlayMusic(string internalName, float startTime = 0f, bool destroyMusic = true)
    {
        if (musicCurrentlyPlaying != null && destroyMusic) 
            Destroy(musicCurrentlyPlaying);

        PlayAudio(internalName, startTime, true);
    }

    public static void ResumeMusic(float startTime = 0f)
    {
        if (musicCurrentlyPlaying != null) {
            if (!musicCurrentlyPlaying.isPlaying) {
                musicCurrentlyPlaying.time = startTime;
                musicCurrentlyPlaying.Play();
            }
        }
    }
    public static void StopMusic()
    {
        if (musicCurrentlyPlaying != null) {
            if (musicCurrentlyPlaying.isPlaying) musicCurrentlyPlaying.Stop();
        }
    }

    public static IEnumerator RunWhenMusicStops(IEnumerator i, string internalName, float startTime = 0f)
    {
        PlayMusic(internalName, startTime);

        while (true) {
            if (!musicCurrentlyPlaying.isPlaying) {
                audioManager.StartCoroutine(i);
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public static void PlayAudio(string internalName, float startTime = 0f, bool isMusic = false)
    {
        if (AudioRegistry.audios.TryGetValue(internalName, out AudioRegistry.AudioSetting settings)) {
            AudioSource source = audioManagerGO.AddComponent<AudioSource>();

            source.clip = settings.audioClip;
            source.mute = settings.mute;
            source.playOnAwake = settings.playOnAwake;
            source.volume = settings.volume;

            if (settings.looping) {
                source.loop = settings.looping;
                audioManager.StartCoroutine(Loop(source, startTime, settings.timeWhereLoopOccurs, settings.toWhatSecLoop));
            }

            source.time = startTime;
            source.Play();

            if (isMusic) musicCurrentlyPlaying = source;
            else audioManager.StartCoroutine(DestroyWhenDone(source));
        }
    }

    private static IEnumerator Loop(AudioSource source, float startTime, float? from, float? to)
    {
        Actor.TimerClass timerT = new Actor.TimerClass(1);
        timerT.SetTimer(startTime);

        while (true) {

            if (source != null) {
                if (from == null ? timerT.UntilTime(source.clip.length) : timerT.UntilTime(from.Value)) {
                    source.Stop();

                    float f = (to == null) ? 0f : to.Value;
                    source.time = f;
                    source.Play();

                    timerT.ResetTimer(1, f);
                }
            }
            else
                yield break;

            yield return new WaitForFixedUpdate();
        }
    }

    private static IEnumerator DestroyWhenDone(AudioSource source)
    {
        while (true) {
            if (!source.isPlaying) {
                Destroy(source);
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}