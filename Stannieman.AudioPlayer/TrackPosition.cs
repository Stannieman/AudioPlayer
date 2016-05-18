using System;

namespace Stannieman.AudioPlayer
{
    /// <summary>
    /// Struct representing a track's duration and a position in the track.
    /// </summary>
    public struct TrackPosition : IEquatable<TrackPosition>
    {
        /// <summary>
        /// Duration of the track.
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Position in the track.
        /// </summary>
        public TimeSpan CurrentTime { get; private set; }

        /// <summary>
        /// Constructor taking time spans that represent the duration of a track and a position in the track.
        /// </summary>
        /// <param name="duration">Duration of a track.</param>
        /// <param name="currentTime">Position in a track.</param>
        public TrackPosition(TimeSpan duration, TimeSpan currentTime)
        {
            Duration = duration;
            CurrentTime = currentTime;
        }

        /// <summary>
        /// Equals implementation to check if a given TrackPosition instance equals this one.
        /// </summary>
        /// <param name="trackPosition">Trackposition to check equality of.</param>
        /// <returns>Whether the given instance is equal to this one.</returns>
        public bool Equals(TrackPosition trackPosition)
        {
            // A track position is considered equal if the Duration and CurrentTime properties are equal.
            return Duration == trackPosition.Duration && CurrentTime == trackPosition.CurrentTime;
        }
    }
}
