# Audio Manager System

A centralized audio management system

## Features

**Object pooling** for efficient AudioSource reuse, **event-driven** integration with EventManager, **ScriptableObject** configuration for easy audio setup, and **singleton** pattern for global access.

## Files

- `AudioManager.cs`
- `AudioClipData.cs`
- `Audio.asmdef`
- `Events.cs`

## Setup

### 1. Create Audio Clip Data Assets

1. Right-click then : `Create > Audio > Audio Clip Data`
2. Configure the audio clip with:
   - AudioClip: The actual audio file
   - Volume: Base volume (0-1)
   - Min/Max Pitch: Pitch variation range
   - Loop: Whether the audio should loop

### 2. Add audio to a GameObject

1. Select a GameObject (should be the source of the sound, the player for example)
2. Add the `AudioManager` component
3. In the inspector, add your AudioClipData assets to the `Audio Clip Data List`
4. Set initial SFX and Music volume levels

And you're good to go !

## Usage

Technically, you can make a direct function call to play a sound, but **this is not the recommended way to do it**!  
Instead, you should use the **event-driven** approach. Here are some examples:

### Direct Method Calls (not good, really don't use)

```csharp
// Play a sound effect
AudioManager.Instance.PlaySound("footstep");

// Play background music
AudioManager.Instance.PlayMusic("background_music");

// Stop current music
AudioManager.Instance.StopMusic();

// Adjust volumes
AudioManager.Instance.SetSFXVolume(0.7f);
AudioManager.Instance.SetMusicVolume(0.5f);
```

### Event-Driven Approach

```csharp
// Play a sound effect
EventManager.Broadcast(new PlaySoundEvent("footstep"));

// Play background music
EventManager.Broadcast(new PlayMusicEvent("background_music"));

// Stop current music
EventManager.Broadcast(new StopMusicEvent());

// Adjust volumes
EventManager.Broadcast(new SetSFXVolumeEvent(0.7f));
EventManager.Broadcast(new SetMusicVolumeEvent(0.5f));
```
