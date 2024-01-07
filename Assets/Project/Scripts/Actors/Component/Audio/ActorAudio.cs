using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class ActorAudio : MonoBehaviour
{
    [SerializeField] public AudioData[] attackSFX;
    [SerializeField] public AudioData[] hitSFX;

    private AudioSource sFXPlayer;
    const float MIN_PITCH = 0.9f;
    const float MAX_PITCH = 1.1f;

    public void OnEnable()
    {
        sFXPlayer = GetComponent<AudioSource>();
    }

    public void PlaySFX(AudioData clip)
    {
        //won't break the clips was played
        sFXPlayer.PlayOneShot(clip.clip, clip.volume);
    }

    /// <summary>
    /// 随机产生声音音调
    /// </summary>
    public void PlayRandomSFX(AudioData clip)
    {
        sFXPlayer.pitch = Random.Range(MIN_PITCH, MAX_PITCH);
        PlaySFX(clip);

        sFXPlayer.pitch = 1;
    }

    /// <summary>
    /// 随机播放一个声音
    /// </summary>
    /// <param name="clips">声音组</param>
    /// <param name="playRandomPitch">是否随机改变音调</param>
    public void PlayRandomSFX(AudioData[] clips, bool playRandomPitch = true)
    {
        if (clips.Length == 0) return;

        if (playRandomPitch)
            PlayRandomSFX(clips[Random.Range(0, clips.Length)]);
        else PlaySFX(clips[Random.Range(0, clips.Length)]);
    }

    /// <summary>
    /// 播放受击音效
    /// </summary>
    /// <param name="playRandomPitch"></param>
    public void PlayHitSFX(bool playRandomPitch = true)
    {
        PlayRandomSFX(hitSFX, playRandomPitch);
    }

    public void PlayAttackSFX(bool playRandomPitch = true)
    {
        PlayRandomSFX(attackSFX, playRandomPitch);
    }
}