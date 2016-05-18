using System;

namespace Stannieman.AudioPlayer
{
    /// <summary>
    /// EventArgs for events to report a track has finished playing.
    /// </summary>
    public class FinishedStoppedPlayingEventArgs : EventArgs
    {
        /// <summary>
        /// ID of the track that finished playing.
        /// </summary>
        public string TrackId { get; private set; }

        /// <summary>
        /// Constructor accepting a track ID of a track that finished playing.
        /// </summary>
        /// <param name="trackId">ID of the track.</param>
        public FinishedStoppedPlayingEventArgs(string trackId)
        {
            TrackId = trackId;
        }
    }

    /// <summary>
    /// EventArgs for events to report the current position in a track.
    /// </summary>
    public class PositionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// ID of the track of which the position changed.
        /// </summary>
        public string TrackId { get; private set; }

        /// <summary>
        /// The position in the track.
        /// </summary>
        public TrackPosition Position { get; private set; }

        /// <summary>
        /// Constructor accepting a track ID and a trackposition struct representing the position in the track.
        /// </summary>
        /// <param name="trackId">ID of the track.</param>
        /// <param name="trackPosition">A TrackPosition instance.</param>
        public PositionChangedEventArgs(string trackId, TrackPosition trackPosition)
        {
            TrackId = trackId;
            Position = trackPosition;
        }
    }
}
