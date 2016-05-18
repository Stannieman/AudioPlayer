using System;

namespace Stannieman.AudioPlayer
{
    /// <summary>
    /// Exception regarding an audio player.
    /// </summary>
    public class AudioPlayerException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AudioPlayerException() : base() { }

        /// <summary>
        /// Constructor taking a message that describes the error.
        /// </summary>
        /// <param name="message">Message.</param>
        public AudioPlayerException(string message) : base(message) { }

        /// <summary>
        /// Constructor taking a message that describes the error and an inner exception.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public AudioPlayerException(string message, Exception innerException) : base(message, innerException) { }
    }
}
