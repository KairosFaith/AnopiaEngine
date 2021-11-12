using System;
using UnityEngine;
using UnityEngine.Audio;
public class anLinearSong : IanSong
{
    anSourcerer CurrentMainSource;
    AudioMixerGroup Output;
    anLinearMusicMag Mag;
    public void Setup(IanMusicMag mag, AudioMixerGroup output)
    {
        Output = output;
        Mag = (anLinearMusicMag)mag;
    }
    void NewCurrentSource()
    {
        CurrentMainSource = Instantiate(Mag.LoopPrefab, transform).GetComponent<anSourcerer>();
        CurrentMainSource.audioSource.outputAudioMixerGroup = Output;
    }
    void StopCurrentSource(double stopTime)
    {
        AudioSource toDestroy = CurrentMainSource.audioSource;
        toDestroy.SetScheduledEndTime(stopTime);
        CurrentMainSource.StartCoroutine(anCore.DeleteWhenDone(toDestroy, stopTime));
    }
    void PlayScheduleMain(double timecode,AudioClip clip)
    {
        AudioSource a = CurrentMainSource.audioSource;
        a.clip = clip;
        a.PlayScheduled(timecode);
    }
    public override void Play(double startTime)
    {
        AudioClip introClip = Mag.Intro;
        double introLength = 0;
        if(introClip != null)
        {
            introLength = introClip.length;
            anCore.PlayClipAtSchedule(transform, introClip, 1, startTime, Output, Mag.OneShotPrefab);
        }
        NewCurrentSource();
        PlayScheduleMain(startTime + introLength, Mag.MainSection);
    }
    public void CueSection(int key)
    {
        SongSection toPlay = Mag.Sections[key];
        CueSection(toPlay);
    }
    void CueSection(SongSection toPlay)
    {
        void NextSong(double timeCode)
        {
            AudioSource toDestroy = CurrentMainSource.audioSource;
            toDestroy.SetScheduledEndTime(timeCode);
            CurrentMainSource.StartCoroutine(anCore.DeleteWhenDone(toDestroy, timeCode));
            NewCurrentSource();
            AudioSource a = CurrentMainSource.audioSource;
            a.clip = toPlay.Loop;
            a.PlayScheduled(timeCode);
        };
        if (toPlay.Stinger != null)
        {
            void PlayStinger(int beatcount, double timeCode)
            {
                if (beatcount == toPlay.BeatTostart)
                {
                    anCore.PlayClipAtSchedule(transform, toPlay.Stinger, 1, timeCode, Output, Mag.OneShotPrefab);
                    NextSong(anSynchro.NextBar);
                    anSynchro.PlayOnBeat -= PlayStinger;
                }
            };
            anSynchro.PlayOnBeat += PlayStinger;
        }
        else
            NextSong(anSynchro.NextBar);
    }
    public void CueFinal()
    {
        SongSection toPlay = Mag.Final;
        void NextSong(double timeCode)
        {
            StopCurrentSource(timeCode);
            NewCurrentSource();
            PlayScheduleMain(timeCode,toPlay.Loop); 
            AudioSource a = CurrentMainSource.audioSource;
            transform.DetachChildren();
            Destroy(gameObject);
            void onDone()
            {
                anSynchro.StopSynchro();
            };
            CurrentMainSource.StartCoroutine(anCore.DeleteWhenDone(a, onDone));
        };
        if (toPlay.Stinger != null)
        {
            void PlayStinger(int beatcount, double timeCode)
            {
                if (beatcount == toPlay.BeatTostart)
                {
                    anCore.PlayClipAtSchedule(transform, toPlay.Stinger,1, timeCode, Output,Mag.OneShotPrefab);
                    NextSong(anSynchro.NextBar);
                    anSynchro.PlayOnBeat -= PlayStinger;
                }
            };
            anSynchro.PlayOnBeat += PlayStinger;
        }
        else
            NextSong(anSynchro.NextBar);
    }
    public override void StopCue(double stopTime)
    {
        StopCurrentSource(stopTime);
        transform.DetachChildren();
        Destroy(gameObject);
        anSynchro.StopSynchro();
    }
    public override void StopImmediate()
    {
        CurrentMainSource.audioSource.Stop();
        Destroy(gameObject); 
        anSynchro.StopSynchro();
    }
    public override void FadeOut(float t, Action ondone = null)
    {
        AudioSource s = CurrentMainSource.audioSource;
        void Fade(float v)
        {
            s.volume = v;
        };
        ondone += () => 
        { 
            Destroy(gameObject);
            anSynchro.StopSynchro();
        };
        StartCoroutine(anCore.FadeValue(t, 1, 0, Fade, ondone));
    }
    public override void Mute(bool toMute)
    {
        CurrentMainSource.audioSource.mute = toMute;
    }
    public override void FadeIn(float t)
    {
        throw new NotImplementedException("Fade in not available for Linear Intro Song");
    }
}