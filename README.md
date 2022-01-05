# Readme under construction XD
# Setting up
Anopia Engine uses scriptable objects (called Mags) to hold reference data for your audio clips.
Each type of sound effect or music implementation has a corresponding scriptable object.
## Sourcerer
Any Audio Source Game Object that is instantiated by the system will use the `anSourcerer` MonoBehaviour Class.
Some scriptable objects will require you to set up prefabs that hold the AudioSource Component. Settings will need to be adjusted on the prefab component as well. (see ClipObjectMag/ADSRMag/LayerMag below)
## Driver
### In Editor
Manually Add `anDriver` as a component to your game object.
If you are using a `OneShotEvent`(see below), you will need to manually create and assign an audio source too. This Audio Source will be used to play all `OneShotEvent`s assigned to the driver.
### In Runtime
In your gameplay script, call the `SetDriver(MonoBehaviour host, AudioMixerGroup output, params string[] IDs)` Function to assign the driver host, mixer output, and Load audio events using the names of the corresponding scriptable objects.
### Play and Stop Sounds
To start and stop events using the driver, simply activate `anDriver.Play(string SoundID, params object[] args)` and `anDriver.Stop(string SoundID)`. The SoundID will be the name of the scriptable object used to Load the event.
# Events
`IanEvent`s are non MonoBehaviour classes used with the `anDriver` for your convenience, but they can be used on their own as well.
### Transient Sound Events
#### ClipMag/OneShotEvent
This Event randomly selects an audio clip, randomises the volume and plays it using the attached AudioSource. See [AudioSource.PlayOneShot](https://docs.unity3d.com/ScriptReference/AudioSource.PlayOneShot.html)
Each `ClipData` set has an audioClip reference with its own gain adjustment. `VolumeFluctuation` controls the range of randomisation that occurs with the volume.
#### ClipObjectMag/ClipObjectEvent
This Event instantiates a new object every time you play a sound, this allows you to play sounds at any position and at any timecode.
You also have pitch, HighPass and Distortion randomisation in addition to clip and volume randomisation. If you are using HighPass and Distortion randomisation, you MUST add the [AudioHighPassFilter](https://docs.unity3d.com/ScriptReference/AudioHighPassFilter.html) and [AudioDistortionFilter](https://docs.unity3d.com/ScriptReference/AudioDistortionFilter.html) components to the prefab.
Inherits from `anClipMag`/`anClipObjectEvent`.
## Sound Effects
### ADSR
### Transient Sounds
#### ClipObjectMag
## Dynamic Music System
### Stem Music Transitions
### Linear Music Transitions
### Synchro Events
