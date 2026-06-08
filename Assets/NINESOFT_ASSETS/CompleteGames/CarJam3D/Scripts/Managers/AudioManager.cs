
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class AudioManager : Manager<AudioManager>
    {
        [Space(20)]
        public AudioClip wayFindedSound;
        public AudioClip noWaySound;
        public AudioClip boxSound;
        public AudioClip winSound;
        public AudioClip failSound;
        public AudioClip popSound;
        public AudioClip purchaseSound;

        public void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            AudioSource source = PoolManager.Instance.GetObjectFromPool<AudioSource>("AudioSource", null, null, lifeTime: 2f);
            source.volume = volume;
            source.pitch = pitch;
            source.PlayOneShot(clip);
        }


    }
}