using System;
using System.Threading.Tasks;

namespace Stannieman.AudioPlayer
{
    public interface IAudioPlayer : IDisposable
    {
        /// <summary>
        /// Indicates that a track finished playing.
        /// </summary>
        event EventHandler<FinishedStoppedPlayingEventArgs> FinishedPlaying;

        /// <summary>
        /// Indicates that a track stopped playing but was not finished.
        /// </summary>
        event EventHandler<FinishedStoppedPlayingEventArgs> StoppedPlaying;

        /// <summary>
        /// Indicates that the position in a track has changed.
        /// </summary>
        event EventHandler<PositionChangedEventArgs> PositionChanged;


        /// <summary>
        /// Sets a file to play and sets the ID of the track for that file name.
        /// </summary>
        /// <param name="fullFileName">Full path of the file to play.</param>
        /// <param name="trackId">ID of the track.</param>
        /// <returns>Task instance.</returns>
        Task SetFileAsync(string fullFileName, string trackId);

        /// <summary>
        /// Starts playback of the set file.
        /// </summary>
        /// <returns>Task instance.</returns>
        Task PlayAsync();

        /// <summary>
        /// Pauses playback of the set file.
        /// </summary>
        /// <returns>Task instance.</returns>
        Task PauseAsync();

        /// <summary>
        /// Stops playback of the set file.
        /// </summary>
        /// <returns>Task instance.</returns>
        Task StopAsync();

        /// <summary>
        /// Returns the current position in the track.
        /// </summary>
        /// <returns>Current position in the track.</returns>
        Task<TrackPosition> GetCurrentTrackPositionAsync();
    }
}