using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
public delegate void BeatFunc(int beatCount, double timeCode);
public static class anSynchro//this is your new update engine
{
    public static BeatFunc PlayOnBeat;// to schedule sounds
    public static TempoData Tempo;
    static bool isPlaying;
    static double StartTime;
    public static int CurrentBeatCount => _BeatCount;
    static int _BeatCount = 0;
    public static double NextBeat => _NextBeat;
    public static double NextBar => _NextBar;
    static double _NextBeat,_NextBar;
    static MonoBehaviour SynchroHost;
    public static void StartSynchro(MonoBehaviour host, TempoData tempo, Action<double>OnStart = null)
    {
        isPlaying = true;
        Tempo = tempo;
        StartTime = AudioSettings.dspTime + Time.deltaTime;//start next frame
        host.StartCoroutine(Synchro(StartTime));
        OnStart?.Invoke(StartTime);
        SynchroHost = host;
    }
    public static void StopSynchro()
    {
        SynchroHost.StopAllCoroutines();
    }
    static IEnumerator Synchro(double startTime)
    {
        _BeatCount = 0;
        _NextBeat = _NextBar = startTime;
        void Beat(double beatTimeCode)
        {
            _BeatCount++;
            if (_BeatCount > Tempo.BeatsPerBar)
            {
                _NextBar += Tempo.BarLength;
                _BeatCount = 1;
            }
            _NextBeat += Tempo.BeatLength;
            PlayOnBeat?.Invoke(_BeatCount, beatTimeCode);
            //Core.BroadcastEvent("OnBeat", SynchroHost, _BeatCount,beatTimeCode);
        };
        Beat(startTime);
        while (isPlaying)
        {
            yield return new WaitForEndOfFrame();
            if (AudioSettings.dspTime +Time.deltaTime > _NextBeat)//check 1 frame ahead
                Beat(_NextBeat);
        }
        //Core.BroadcastEvent("OnSynchroStop", SynchroHost);
        isPlaying = false;
        _BeatCount = 0;
    }
}
public enum BarValue
{
    Quarter = 1,
    Eight,
    Sixteen = 4,
}
[Serializable]
public class TempoData
{
    public int CrotchetBPM;
    public int BeatsPerBar;
    public BarValue TimeSignature;
    public int BPM => CrotchetBPM * (int)TimeSignature;
    public float BeatLength => 60 / (float)BPM;
    public float BarLength => BeatLength * BeatsPerBar;
}
public abstract class IanMusicMag : IanAudioMag
{
    public TempoData Tempo;
    public abstract SongForm Structure { get; }
    public override IanEvent LoadMag(MonoBehaviour host, AudioMixerGroup output)
    {
        throw new System.NotImplementedException();
    }
}
public enum SongForm
{
    Linear,
    Stem
}
public abstract class IanSong : MonoBehaviour
{
    public abstract void StopCue(double stopTime);
    public abstract void StopImmediate();
    public abstract void Play(double startTime);
    public abstract void FadeIn(float t);
    public abstract void FadeOut(float t, Action ondone = null);
    public abstract void Mute(bool toMute);
}