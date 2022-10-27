using UnityEngine;
using UnityEngine.Audio;

namespace GameCore
{
    [CreateAssetMenu(fileName = "SystemConfig", menuName = "GameCore/Config/SystemConfig")]
    public class SystemConfig : BaseConfig
    {
        [Header("Audio Setting")]
        public AudioMixer AudioMixer;
        public string MasterVolumeParameter = "MasterVolume";
        public string BGMVolumeParameter = "BGMVolume";
        public string SFXVolumeParameter = "SFXVolume";
        public string ChatVolumeParameter = "ChatVolume";
    }
}
