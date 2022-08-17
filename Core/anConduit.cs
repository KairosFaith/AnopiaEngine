using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
public static partial class anCore
{
    public static void TransitionToSnapshot(this AudioMixer mixer, string SnapshotName, float TimeToReach)
    {
        AudioMixerSnapshot snapshot = mixer.FindSnapshot(SnapshotName);
        AudioMixerSnapshot[] ss = new AudioMixerSnapshot[] { snapshot };
        float[] w = new float[] { 1 };
        mixer.TransitionToSnapshots(ss, w, TimeToReach);
        //the weight is nonsense, dun mess with it
    }
    public static anSourcerer SetupSource(Vector3 position, anSourcerer prefab, AudioMixerGroup output, AudioClip clip, float volume)
    {
        anSourcerer s = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
        AudioSource a = s.audioSource;
        a.outputAudioMixerGroup = output;
        a.clip = clip;
        a.volume = volume;
        return s;
    }
    public static anSourcerer PlayClipAtPoint(Vector3 position, AudioClip clip, float volume, AudioMixerGroup output, anSourcerer prefab, Action<anSourcerer> setup = null, Action OnDone = null)
    {
        anSourcerer s = SetupSource(position, prefab, output, clip, volume);
        AudioSource a = s.audioSource;
        setup?.Invoke(s);
        a.Play();
        s.DeleteWhenDone(OnDone);
        return s;
    }
    public static anSourcerer PlayClipScheduled(Transform parent, AudioClip clip, float volume, double startTime, AudioMixerGroup output, anSourcerer prefab, Action<anSourcerer> setup = null)
    {
        anSourcerer s = UnityEngine.Object.Instantiate(prefab, parent);
        AudioSource a = s.audioSource;
        a.outputAudioMixerGroup = output;
        a.clip = clip;
        a.volume = volume;
        setup?.Invoke(s);
        a.PlayScheduled(startTime);
        s.DeleteAfterTime(a, startTime + clip.length);
        return s;
    }
    public static IEnumerator FadeValue(float fadeTime, float startingValue, float targetValue, Action<float> ChangeValue, Action ondone = null)
    {
        for (float lerp = 0; lerp < 1; lerp += Time.unscaledDeltaTime / fadeTime)
        {
            float newValue = Mathf.Lerp(startingValue, targetValue, lerp);
            ChangeValue(newValue);
            yield return new WaitForEndOfFrame();
        }
        ondone?.Invoke();
    }
}