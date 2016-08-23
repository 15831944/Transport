using System;
using System.Text;
using System.Threading;

namespace Transport.Common
{
    /// <summary>
    /// An ASCII progress bar
    /// </summary>
    public class ProgressBar : IDisposable, IProgress<int>
    {
        private readonly string _title;
        private int _maxValue;
        private const int BlockCount = 20;
        private readonly TimeSpan _animationInterval = TimeSpan.FromSeconds(1.0 / 8);
        private const string Animation = @"|/-\";

        private Timer _timer;

        private double _currentProgress;
        private int _currentValue;
        private string _currentText = string.Empty;
        private bool _disposed;
        private int _animationIndex;


        public ProgressBar(int maxVal = 0, string title = "")
        {
            _title = title;
            _maxValue = maxVal;
            _timer = new Timer(TimerHandler);

            // A progress bar is only for temporary display in a console window.
            // If the console output is redirected to a file, draw nothing.
            // Otherwise, we'll end up with a lot of garbage in the target file.
            if (!Console.IsOutputRedirected)
            {
                ResetTimer();
            }
        }

        public void Start()
        {
            Console.Clear();
            _currentText = string.Empty;
            _timer.Change(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(-1));
        }

        public void Pause()
        {
            _timer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
        }

        public void Stop()
        {
            _timer.Change(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(-1));
            Pause();
        }

        public void Report(int current)
        {
            var value = (double)current/_maxValue;
            // Make sure value is in [0..1] range
            value = Math.Max(0, Math.Min(1, value));
            Interlocked.Exchange(ref _currentProgress, value);
            Interlocked.Exchange(ref _currentValue, current);
        }

        public void Report(int current, int max)
        {
            if (_maxValue != max)
            {
                Interlocked.Exchange(ref _maxValue, max);
            }

            Report(current);
        }

        private void TimerHandler(object state)
        {
            lock (_timer)
            {
                if (_disposed) return;

                int progressBlockCount = (int)(_currentProgress * BlockCount);
                int percent = (int)(_currentProgress * 100);
                string text = string.Format("{0} [{1}{2}] {3,6}% ({4}/{5}) {6}",
                    _title,
                    new string('#', progressBlockCount), new string('-', BlockCount - progressBlockCount),
                    percent,
                    _currentValue,
                    _maxValue,
                    Animation[_animationIndex++ % Animation.Length]);
                UpdateText(text);

                ResetTimer();
            }
        }

        private void UpdateText(string text)
        {
            // Get length of common portion
            int commonPrefixLength = 0;
            int commonLength = Math.Min(_currentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == _currentText[commonPrefixLength])
            {
                commonPrefixLength++;
            }

            // Backtrack to the first differing character
            StringBuilder outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', _currentText.Length - commonPrefixLength);

            // Output new suffix
            outputBuilder.Append(text.Substring(commonPrefixLength));

            // If the new text is shorter than the old one: delete overlapping characters
            int overlapCount = _currentText.Length - text.Length;
            if (overlapCount > 0)
            {
                outputBuilder.Append(' ', overlapCount);
                outputBuilder.Append('\b', overlapCount);
            }

            Console.Write(outputBuilder);
            _currentText = text;
        }

        private void ResetTimer()
        {
            _timer.Change(_animationInterval, TimeSpan.FromMilliseconds(-1));
        }

        public void Dispose()
        {
            lock (_timer)
            {
                _disposed = true;
                UpdateText(string.Empty);
            }
        }

    }
}