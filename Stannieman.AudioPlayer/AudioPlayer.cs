using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using NAudio.Wave;

namespace Stannieman.AudioPlayer
{
    public class AudioPlayer : IAudioPlayer
    {
        #region Instance variables and constants

        // Constants
        private static readonly TimeSpan REPORT_POSITION_INTERVAL = new TimeSpan(0, 0, 1);
        private static readonly TrackPosition DEFAULT_TRACK_POSITION = new TrackPosition();

        private AutoResetEvent _resetEvent = new AutoResetEvent(true);

        // Player variables
        private IWavePlayer _player;
        private WaveStream _waveStream;
        private string _trackId;
        private bool _stoppedExplicitly;

        // Variables for position reporting
        private Timer _positionReportTimer;
        private DateTime _lastReportTime;
        private DateTime _timePaused;
        private bool _paused;
        private bool _doNotReportPosition;

        #endregion

        #region Events

        /// <summary>
        /// Indicates that a track finished playing.
        /// </summary>
        public event EventHandler<FinishedStoppedPlayingEventArgs> FinishedPlaying;

        /// <summary>
        /// Indicates that a track stopped playing but was not finished.
        /// </summary>
        public event EventHandler<FinishedStoppedPlayingEventArgs> StoppedPlaying;

        /// <summary>
        /// Indicates that the position in a track has changed.
        /// </summary>
        public event EventHandler<PositionChangedEventArgs> PositionChanged;

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AudioPlayer()
        {
            _player = new WaveOutEvent();
            _player.PlaybackStopped += OnPlaybackStoppedAsync;
        }

        /// <summary>
        /// Sets a file to play and sets the ID of the track for that file name.
        /// </summary>
        /// <param name="fullFileName">Full path of the file to play.</param>
        /// <param name="trackId">ID of the track.</param>
        /// <returns>Task instance.</returns>
        /// <exception cref="AudioPlayerException">If something goes wrong while setting the file.</exception>
        public async Task SetFileAsync(string fullFileName, string trackId)
        {
            await Task.Run(() =>
            {
                // We can only set a new file after the stop procedure completed
                _resetEvent.WaitOne();

                lock (this)
                {
                    try
                    {
                        _trackId = trackId;

                        if (_player.PlaybackState != PlaybackState.Stopped)
                            throw new AudioPlayerException("The player is not stopped. You can only set a new file if the player is stopped.");
                        else
                        {
                            // Get the audio format of the new file
                            AudioFormats audioFormat = GetAudioFormatForFileName(fullFileName);

                            if (audioFormat == AudioFormats.Unknown)
                                throw new AudioPlayerException($"The audio format in the file {fullFileName} is not supported.");
                            else
                            {
                                // Set the correct reader for the audio format
                                if (audioFormat == AudioFormats.Vorbis)
                                    _waveStream = new NAudio.Vorbis.VorbisWaveReader(fullFileName);
                                else
                                    _waveStream = new AudioFileReader(fullFileName);
                            }

                            _player.Init(_waveStream);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new AudioPlayerException($"Cannot set the file {fullFileName} ready for playback.", e);
                    }
                }

                // Signal reset event
                _resetEvent.Set();
            });
        }

        /// <summary>
        /// Starts playback of the set file.
        /// </summary>
        /// <returns>Task instance.</returns>
        /// <exception cref="AudioPlayerException">If something goes wrong while starting the playback of the set file.</exception>
        public async Task PlayAsync()
        {
            await Task.Run(() =>
            {
                // We can only start playback after the stop procedure completed
                _resetEvent.WaitOne();

                lock (this)
                {
                    if (_waveStream != null)
                    {
                        // Reset the indicator to check the playback was stopped explicitly or because the track finished
                        _stoppedExplicitly = false;

                        try
                        {
                            _player.Play();
                            _doNotReportPosition = false;
                        }
                        catch (Exception e)
                        {
                            throw new AudioPlayerException("Cannot start the playback of the set file.", e);
                        }

                        // If we are resuming create a timer that will start after the remaining time from when it was paused,
                        // else create a timer that starts after the interval.
                        if (_paused)
                        {
                            var diff = _timePaused.Subtract(_lastReportTime);
                            if (diff.Ticks > TimeSpan.TicksPerSecond)
                                diff = REPORT_POSITION_INTERVAL;

                            _positionReportTimer = new Timer(ReportPosition, null, REPORT_POSITION_INTERVAL.Subtract(diff), REPORT_POSITION_INTERVAL);
                        }
                        else
                            _positionReportTimer = new Timer(ReportPosition, null, REPORT_POSITION_INTERVAL, REPORT_POSITION_INTERVAL);
                    }
                    else
                        throw new AudioPlayerException("No file is set to play. Call SetFile first.");
                    }

                // Signal reset event
                _resetEvent.Set();
            });
        }

        /// <summary>
        /// Pauses playback of the set file.
        /// </summary>
        /// <returns>Task instance.</returns>
        /// <exception cref="AudioPlayerException">If something goes wrong while pausing the playback of the set file.</exception>
        public async Task PauseAsync()
        {
            await Task.Run(() =>
            {
                // We can only pause after the stop procedure completed
                _resetEvent.WaitOne();

                lock (this)
                {
                    try
                    {
                        _player.Pause();
                    }
                    catch (Exception e)
                    {
                        throw new AudioPlayerException("Cannot pause the playback of the set file.", e);
                    }

                    // Dispose timer so we stop reporting the position
                    if (_positionReportTimer != null)
                    {
                        _positionReportTimer.Dispose();
                        _positionReportTimer = null;
                    }

                    _timePaused = DateTime.Now;

                    // Indicate that when we start playback again this is actually a resume
                    _paused = true;  
                }

                // Signal reset event
                _resetEvent.Set();
            });
        }

        /// <summary>
        /// Stops playback of the set file.
        /// </summary>
        /// <returns>Task instance.</returns>
        /// <exception cref="AudioPlayerException">If something goes wrong while stopping the playback of the set file.</exception>
        public async Task StopAsync()
        {
            await Task.Run(() =>
            {
                lock (this)
                {
                    if (_player.PlaybackState != PlaybackState.Stopped)
                    {
                        // Set wait handle, this is only released when the PlaybackStopped event hanlder runs
                        _resetEvent.WaitOne();

                        _stoppedExplicitly = true;

                        try
                        {
                            _player.Stop();
                        }
                        catch (Exception e)
                        {
                            throw new AudioPlayerException("Cannot stop the playback of the set file.", e);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Returns the current position in the track.
        /// </summary>
        /// <returns>Current position in the track.</returns>
        public async Task<TrackPosition> GetCurrentTrackPositionAsync()
        {
            TrackPosition position = DEFAULT_TRACK_POSITION;
            await Task.Run(() =>
            {
                lock (this)
                {
                    if (_waveStream != null)
                        position = new TrackPosition(_waveStream.TotalTime, _waveStream.CurrentTime);
                    else
                        throw new AudioPlayerException("No file is set to play, so there is no track position. Call SetFile first.");
                }
            });

            return position;
        }

        /// <summary>
        /// Callback for the timer to report the position in the track.
        /// </summary>
        /// <param name="state">State.</param>
        private void ReportPosition(object state)
        {
            string trackId = null;
            TimeSpan totalTime = new TimeSpan();
            TimeSpan currentTime = new TimeSpan();
            bool doNotReportPosition;

            lock (this)
            {
                doNotReportPosition = _doNotReportPosition;
                if (!doNotReportPosition)
                {
                    trackId = _trackId;
                    totalTime = _waveStream.TotalTime;
                    currentTime = _waveStream.CurrentTime;
                    // Set the time when we last reported. This is required to calculate the delay for the timer when we are resuming from a pause.
                    _lastReportTime = DateTime.Now;
                }
            }

            if (!doNotReportPosition)
                PositionChanged?.Invoke(this, new PositionChangedEventArgs(trackId, new TrackPosition(totalTime, currentTime)));
        }

        /// <summary>
        /// Handler for if playback of a file stopped.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">StoppedEventArgs.</param>
        /// <returns>Task instance.</returns>
        private async void OnPlaybackStoppedAsync(object sender, StoppedEventArgs e)
        {
            await Task.Run(() =>
            {
                bool stoppedExplicitly;

                lock (this)
                {
                    // Dispose timer so we stop reporting the position
                    if (_positionReportTimer != null)
                    {
                        _positionReportTimer.Dispose();
                        _positionReportTimer = null;
                        // Also indicate we don't want to report because the event may have already been fired
                        _doNotReportPosition = true;
                    }

                    // Dispose the wavestream
                    _waveStream.Dispose();

                    // Set resuming to false so we know we're not resuming the next time we start
                    _paused = false;

                    stoppedExplicitly = _stoppedExplicitly;

                    // Signal reset event
                    _resetEvent.Set();
                }

                // If playback wasn't stopped explicitly that meanse the track finished on it's own
                if (stoppedExplicitly)
                    StoppedPlaying?.Invoke(this, new FinishedStoppedPlayingEventArgs(_trackId));
                else
                    FinishedPlaying?.Invoke(this, new FinishedStoppedPlayingEventArgs(_trackId));
            });
        }

        /// <summary>
        /// Returns the audio format for a given file name.
        /// The returned format is based on the file's extension.
        /// </summary>
        /// <param name="fileName">File name to get audio format for.</param>
        /// <returns>The audio format of the file.</returns>
        private static AudioFormats GetAudioFormatForFileName(string fileName)
        {
            // Get part after last dot
            string extension = fileName.Split('.').Last().ToLower();

            switch (extension)
            {
                case "wav":
                    return AudioFormats.Wave;
                case "aac":
                    return AudioFormats.Aac;
                case "ogg":
                    return AudioFormats.Vorbis;
                case "mp3":
                    return AudioFormats.Mp3;
                default:
                    return AudioFormats.Unknown;
            }
        }

        /// <summary>
        /// Disposes the player.
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                // Dispose player.
                // The PlaybackStopped event will also fire so we don't have to cleanup the things that are already cleaned up there.
                _player.Dispose();
            }
        }
    }
}
