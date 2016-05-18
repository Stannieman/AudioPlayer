# Stannieman.AudioPlayer #
A basic audio player for .NET.
It's a simple wrapper for NAudio that allows you to set an audio file together with an ID string.
This file can then be played, paused, stopped.
TrackFinished and TrackStopped events will be fired when the respective events happen and the ID string will be passed in the EventArgs.
A PositionChanged event will be fired every second when the file is playing, the EventArgs contain a TrackPosition instance that has info about the current position in the track.

Wave, Vorbis, MP3 and AAC audio formats are supported, but this may be extended when I need other ones or there is a demand for this.

Currently it's not supported to seek through the playing track, this feature will be implemented in the future.
In the future it will be possible to set the interval at which the PositionChanged event is fired.

This project is available as a NuGet package here: https://www.nuget.org/packages/Stannieman.AudioPlayer/

# Documentation #

# T:Stannieman.AudioPlayer.AudioFormats
Enum representing various audio types.  
  
---
##### E:Stannieman.AudioPlayer.AudioPlayer.FinishedPlaying
Indicates that a track finished playing.  
  
---
##### E:Stannieman.AudioPlayer.AudioPlayer.StoppedPlaying
Indicates that a track stopped playing but was not finished.  
  
---
##### E:Stannieman.AudioPlayer.AudioPlayer.PositionChanged
Indicates that the position in a track has changed.  
  
---
##### M:Stannieman.AudioPlayer.AudioPlayer.#ctor
Default constructor.  
  
---
##### M:Stannieman.AudioPlayer.AudioPlayer.SetFileAsync(System.String,System.String)
Sets a file to play and sets the ID of the track for that file name.  

|Name | Description |
|-----|------|
|fullFileName: |Full path of the file to play.|
|trackId: |ID of the track.|
Returns: Task instance.  
[[T:Stannieman.AudioPlayer.AudioPlayerException|T:Stannieman.AudioPlayer.AudioPlayerException]]: If something goes wrong while setting the file.
  
---
##### M:Stannieman.AudioPlayer.AudioPlayer.PlayAsync
Starts playback of the set file.  
Returns: Task instance.  
[[T:Stannieman.AudioPlayer.AudioPlayerException|T:Stannieman.AudioPlayer.AudioPlayerException]]: If something goes wrong while starting the playback of the set file.
  
---
##### M:Stannieman.AudioPlayer.AudioPlayer.PauseAsync
Pauses playback of the set file.  
Returns: Task instance.  
[[T:Stannieman.AudioPlayer.AudioPlayerException|T:Stannieman.AudioPlayer.AudioPlayerException]]: If something goes wrong while pausing the playback of the set file.
  
---
##### M:Stannieman.AudioPlayer.AudioPlayer.StopAsync
Stops playback of the set file.  
Returns: Task instance.  
[[T:Stannieman.AudioPlayer.AudioPlayerException|T:Stannieman.AudioPlayer.AudioPlayerException]]: If something goes wrong while stopping the playback of the set file.
  
---
##### M:Stannieman.AudioPlayer.AudioPlayer.GetCurrentTrackPositionAsync
Returns the current position in the track.  
Returns: Current position in the track.  
  
---
##### M:Stannieman.AudioPlayer.AudioPlayer.ReportPosition(System.Object)
Callback for the timer to report the position in the track.  

|Name | Description |
|-----|------|
|state: |State.|
  
---
##### M:Stannieman.AudioPlayer.AudioPlayer.OnPlaybackStoppedAsync(System.Object,NAudio.Wave.StoppedEventArgs)
Handler for if playback of a file stopped.  

|Name | Description |
|-----|------|
|sender: |Sender.|
|e: |StoppedEventArgs.|
Returns: Task instance.  
  
---
##### M:Stannieman.AudioPlayer.AudioPlayer.GetAudioFormatForFileName(System.String)
Returns the audio format for a given file name. The returned format is based on the file's extension.  

|Name | Description |
|-----|------|
|fileName: |File name to get audio format for.|
Returns: The audio format of the file.  
  
---
##### M:Stannieman.AudioPlayer.AudioPlayer.Dispose
Disposes the player.  
  
---
# T:Stannieman.AudioPlayer.FinishedStoppedPlayingEventArgs
EventArgs for events to report a track has finished playing.  
  
---
##### P:Stannieman.AudioPlayer.FinishedStoppedPlayingEventArgs.TrackId
ID of the track that finished playing.  
  
---
##### M:Stannieman.AudioPlayer.FinishedStoppedPlayingEventArgs.#ctor(System.String)
Constructor accepting a track ID of a track that finished playing.  

|Name | Description |
|-----|------|
|trackId: |ID of the track.|
  
---
# T:Stannieman.AudioPlayer.PositionChangedEventArgs
EventArgs for events to report the current position in a track.  
  
---
##### P:Stannieman.AudioPlayer.PositionChangedEventArgs.TrackId
ID of the track of which the position changed.  
  
---
##### P:Stannieman.AudioPlayer.PositionChangedEventArgs.Position
The position in the track.  
  
---
##### M:Stannieman.AudioPlayer.PositionChangedEventArgs.#ctor(System.String,Stannieman.AudioPlayer.TrackPosition)
Constructor accepting a track ID and a trackposition struct representing the position in the track.  

|Name | Description |
|-----|------|
|trackId: |ID of the track.|
|trackPosition: |A TrackPosition instance.|
  
---
# T:Stannieman.AudioPlayer.AudioPlayerException
Exception regarding an audio player.  
  
---
##### M:Stannieman.AudioPlayer.AudioPlayerException.#ctor
Default constructor.  
  
---
##### M:Stannieman.AudioPlayer.AudioPlayerException.#ctor(System.String)
Constructor taking a message that describes the error.  

|Name | Description |
|-----|------|
|message: |Message.|
  
---
##### M:Stannieman.AudioPlayer.AudioPlayerException.#ctor(System.String,System.Exception)
Constructor taking a message that describes the error and an inner exception.  

|Name | Description |
|-----|------|
|message: |Message.|
|innerException: |Inner exception.|
  
---
##### E:Stannieman.AudioPlayer.IAudioPlayer.FinishedPlaying
Indicates that a track finished playing.  
  
---
##### E:Stannieman.AudioPlayer.IAudioPlayer.StoppedPlaying
Indicates that a track stopped playing but was not finished.  
  
---
##### E:Stannieman.AudioPlayer.IAudioPlayer.PositionChanged
Indicates that the position in a track has changed.  
  
---
##### M:Stannieman.AudioPlayer.IAudioPlayer.SetFileAsync(System.String,System.String)
Sets a file to play and sets the ID of the track for that file name.  

|Name | Description |
|-----|------|
|fullFileName: |Full path of the file to play.|
|trackId: |ID of the track.|
Returns: Task instance.  
  
---
##### M:Stannieman.AudioPlayer.IAudioPlayer.PlayAsync
Starts playback of the set file.  
Returns: Task instance.  
  
---
##### M:Stannieman.AudioPlayer.IAudioPlayer.PauseAsync
Pauses playback of the set file.  
Returns: Task instance.  
  
---
##### M:Stannieman.AudioPlayer.IAudioPlayer.StopAsync
Stops playback of the set file.  
Returns: Task instance.  
  
---
##### M:Stannieman.AudioPlayer.IAudioPlayer.GetCurrentTrackPositionAsync
Returns the current position in the track.  
Returns: Current position in the track.  
  
---
# T:Stannieman.AudioPlayer.TrackPosition
Struct representing a track's duration and a position in the track.  
  
---
##### P:Stannieman.AudioPlayer.TrackPosition.Duration
Duration of the track.  
  
---
##### P:Stannieman.AudioPlayer.TrackPosition.CurrentTime
Position in the track.  
  
---
##### M:Stannieman.AudioPlayer.TrackPosition.#ctor(System.TimeSpan,System.TimeSpan)
Constructor taking time spans that represent the duration of a track and a position in the track.  

|Name | Description |
|-----|------|
|duration: |Duration of a track.|
|currentTime: |Position in a track.|
  
---
##### M:Stannieman.AudioPlayer.TrackPosition.Equals(Stannieman.AudioPlayer.TrackPosition)
Equals implementation to check if a given TrackPosition instance equals this one.  

|Name | Description |
|-----|------|
|trackPosition: |Trackposition to check equality of.|
Returns: Whether the given instance is equal to this one.  
  
---
