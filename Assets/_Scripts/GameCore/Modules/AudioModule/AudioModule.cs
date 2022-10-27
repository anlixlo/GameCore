using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace GameCore
{
    public class AudioModule : BaseModule
    {
        public SystemConfig systemConfig;

        public AudioMixer AudioMixer => _audioMixer;
        private AudioMixer _audioMixer;

        public override void Initialize(params object[] param)
        {
            if (systemConfig.AudioMixer == null)
            {
                Debug.LogWarning("[AudioModule:Initialize] Can't find audio mixer in system setting, check system config settings");
                return;
            }

            _audioMixer = systemConfig.AudioMixer;
        }

        public async Task<AudioSource> PlayAudio(string audioAddress)
        {
            if (_audioMixer == null) return null;

            AudioSource audio = new AudioSource();

            //AudioSource audio = await Entry.Instance.GetModule<ResourcesModule>().Spawn<AudioSource>(audioAddress);

            //if (audio.clip == null)
            //{
            //    Debug.LogError($"[AudioModule:PlayAudio] AudioSource setting error check audio source and clip is setted up, Address : {audioAddress}");
            //    Entry.Instance.GetModule<ResourcesModule>().DeSpawn(audio);
            //    return null;
            //}

            //audio.Play();

            return audio;
        }

        public void StopAudio(AudioSource audio)
        {
            audio.Stop();
            //Entry.Instance.GetModule<ResourcesModule>().DeSpawn(audio);
        }

        public void SetMasterVolume(float volume)
        {
            if (_audioMixer == null) return;

            _audioMixer.SetFloat(systemConfig.MasterVolumeParameter, LinearToLogarithmicScale(volume));
        }

        public void SetBGMVolume(float volume)
        {
            if (_audioMixer == null) return;

            _audioMixer.SetFloat(systemConfig.BGMVolumeParameter, LinearToLogarithmicScale(volume));
        }

        public void SetSFXVolume(float volume)
        {
            if (_audioMixer == null) return;

            _audioMixer.SetFloat(systemConfig.SFXVolumeParameter, LinearToLogarithmicScale(volume));
        }

        private float LinearToLogarithmicScale(float value)
        {
            return Mathf.Log(Mathf.Clamp(value, 0.001f, 1)) * 20.0f;
        }
    }
}
