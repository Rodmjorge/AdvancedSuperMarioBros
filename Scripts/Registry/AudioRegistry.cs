using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRegistry : MonoBehaviour
{
    internal static Dictionary<string, AudioSetting> audios = new Dictionary<string, AudioSetting>();

    private void Awake()
    {
        Music();
        Sound();
    }


    protected void Music()
    {
        const float defaultMusicVolume = 0.85f;

        RegisterMusic("time_is_running_out", new AudioSetting() {
            audioClip = GetAudioClip("time_is_running_out", true),

            looping = false,
            volume = defaultMusicVolume
        }, false);

        RegisterMusic("overworld_music", new AudioSetting() {
            audioClip = GetAudioClip("overworld_music", true),

            looping = true,
            timeWhereLoopOccurs = 88.615f,
            toWhatSecLoop = 2.6f,
            volume = defaultMusicVolume
        }, true);
    }
    protected void Sound()
    {
        RegisterAudio("small_mario_jump", new AudioSetting() {
            audioClip = GetAudioClip("small_mario_jump"),
            volume = 0.9f
        });

        RegisterAudio("breaking_block", new AudioSetting() {
            audioClip = GetAudioClip("breaking_block")
        });
        RegisterAudio("container_block", new AudioSetting() {
            audioClip = GetAudioClip("container_block")
        });
        RegisterAudio("hit_block", new AudioSetting() {
            audioClip = GetAudioClip("hit_block")
        });

        RegisterAudio("coin", new AudioSetting() {
            audioClip = GetAudioClip("coin")
        });

        RegisterAudio("step_enemy", new AudioSetting() {
            audioClip = GetAudioClip("step_enemy"),
            volume = 0.95f
        });
        RegisterAudio("kick_enemy", new AudioSetting() {
            audioClip = GetAudioClip("kicked"),
            volume = 0.95f
        });
        RegisterAudio("explosion_enemy", new AudioSetting() {
            audioClip = GetAudioClip("weird_explosion"),
            volume = 0.9f
        });

        RegisterAudio("1UP", new AudioSetting() {
            audioClip = GetAudioClip("one_up"),
            volume = 0.9f
        });
        RegisterAudio("powerup", new AudioSetting() {
            audioClip = GetAudioClip("player_got_powerup"),
            volume = 0.9f
        });
        RegisterAudio("player_hit", new AudioSetting() {
            audioClip = GetAudioClip("player_got_hit"),
            volume = 0.9f
        });
    }

    public static void RegisterAudio(string internalName, AudioSetting settings)
    {
        if (!audios.ContainsKey(internalName)) audios.Add(internalName, settings);
    }
    public static void RegisterMusic(string internalName, AudioSetting settings, bool addHurryUpMusic, float hurryDivision = 1.462f, string hurryUpInternalName = null)
    {
        if (!audios.ContainsKey(internalName)) {
            RegisterAudio(internalName, settings);

            if (addHurryUpMusic) {
                string s = (hurryUpInternalName == null) ? "_hurryUp" : hurryUpInternalName;

                AudioSetting hurryUpSettings = new AudioSetting() {
                    audioClip = GetAudioClip(settings.audioClip.name + s, true),

                    mute = settings.mute,
                    playOnAwake = settings.playOnAwake,

                    looping = true,
                    timeWhereLoopOccurs = settings.timeWhereLoopOccurs / hurryDivision,
                    toWhatSecLoop = settings.toWhatSecLoop,

                    priority = settings.priority,
                    volume = settings.volume,
                    pitch = settings.pitch,
                    stereoPan = settings.stereoPan
                };
                audios.Add(internalName + s, hurryUpSettings);
            }
        }
    }

    public static AudioClip GetAudioClip(string s, bool isMusic = false) { return Resources.Load<AudioClip>($"Audios/{ (isMusic ? "Music/" : string.Empty) }{ s }"); }

    public class AudioSetting
    {
        public AudioClip audioClip;

        public bool mute;
        public bool playOnAwake = true;

        public bool looping;
        public float? timeWhereLoopOccurs = null;
        public float? toWhatSecLoop = null;

        public int priority = 128;
        public float volume = 1f;
        public float pitch = 1f;
        public float stereoPan;
    }
}