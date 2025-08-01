using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NekoDeskTop.UI
{

    public partial class Music : Window
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            int extendedStyle = GetWindowLong(helper.Handle, GWL_EXSTYLE);
            SetWindowLong(helper.Handle, GWL_EXSTYLE,
                extendedStyle |
                WS_EX_NOACTIVATE |
                WS_EX_TRANSPARENT);
        }

        public int SampleCount { get; set; } = 1024;
        private int _barCount = 128;
        private double _barThickness = 1;
        private double _barCornerRadius = 0;

        private WasapiLoopbackCapture _capture;
        private float[] _audioBuffer;
        private List<Rectangle> _spectrumBars;
        private MMDeviceEnumerator _deviceEnumerator;
        private List<MMDevice> _audioDevices;

        public int BarCount
        {
            get => _barCount;
            set
            {
                _barCount = value;
                InitializeVisualization();
            }
        }

        public double BarThickness
        {
            get => _barThickness;
            set
            {
                _barThickness = value;
                UpdateBarsAppearance();
            }
        }

        public double BarCornerRadius
        {
            get => _barCornerRadius;
            set
            {
                _barCornerRadius = value;
                UpdateBarsAppearance();
            }
        }

        public Music()
        {
            InitializeComponent();

            this.Topmost = true;
            this.Focusable = false;
            this.ShowActivated = false;
            this.IsHitTestVisible = false;
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.Background = Brushes.Transparent;
            this.SetValue(Window.IsEnabledProperty, false);

            _deviceEnumerator = new MMDeviceEnumerator();
            RefreshAudioDevices();
            InitializeVisualization();
            StartCapture();
        }


        public void RefreshAudioDevices()
        {
            _audioDevices = _deviceEnumerator
                .EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
                .ToList();
        }
        public IEnumerable<string> GetAudioDeviceNames()
        {
            return _audioDevices.Select(d => d.FriendlyName);
        }

        public void SelectAudioDevice(int deviceIndex)
        {
            if (deviceIndex >= 0 && deviceIndex < _audioDevices.Count)
            {
                StopCapture();
                StartCapture(_audioDevices[deviceIndex]);
            }
        }

        private void InitializeVisualization()
        {
            spectrumCanvas.Children.Clear();
            _spectrumBars = new List<Rectangle>();

            for (int i = 0; i < BarCount; i++)
            {
                var bar = new Rectangle
                {
                    Fill = new LinearGradientBrush(Colors.Blue, Colors.Cyan, 90),
                    Width = BarThickness,
                    RadiusX = BarCornerRadius,
                    RadiusY = BarCornerRadius
                };
                _spectrumBars.Add(bar);
                spectrumCanvas.Children.Add(bar);
            }
        }

        private void UpdateBarsAppearance()
        {
            if (_spectrumBars == null) return;

            foreach (var bar in _spectrumBars)
            {
                bar.Width = BarThickness;
                bar.RadiusX = BarCornerRadius;
                bar.RadiusY = BarCornerRadius;
            }
        }

        private void StartCapture(MMDevice device = null)
        {
            try
            {
                var targetDevice = device ?? _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

                _capture = new WasapiLoopbackCapture(targetDevice);
                _capture.DataAvailable += Capture_DataAvailable;
                _capture.WaveFormat = new WaveFormat(44100, 16, 2);
                _audioBuffer = new float[SampleCount];
                _capture.StartRecording();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: Unable to start audio capture: {ex.Message}");
                Close();
            }
        }

        private void StopCapture()
        {
            _capture?.StopRecording();
            _capture?.Dispose();
            _capture = null;
        }

        private void Capture_DataAvailable(object sender, WaveInEventArgs e)
        {
            int bytesPerSample = _capture.WaveFormat.BitsPerSample / 8;
            int samplesRecorded = e.BytesRecorded / bytesPerSample;

            for (int i = 0; i < samplesRecorded && i < _audioBuffer.Length; i++)
            {
                _audioBuffer[i] = BitConverter.ToInt16(e.Buffer, i * bytesPerSample) / 32768f * 2.5f;
            }

            Dispatcher.Invoke(UpdateSpectrum);
        }

        private void UpdateSpectrum()
        {
            if (spectrumCanvas.ActualWidth <= 0 || spectrumCanvas.ActualHeight <= 0)
                return;

            double width = spectrumCanvas.ActualWidth;
            double height = spectrumCanvas.ActualHeight / 2;
            double barWidth = Math.Max(1, (width - 20 - (BarCount - 1) * 2) / BarCount);

            for (int i = 0; i < BarCount; i++)
            {
                int start = i * (_audioBuffer.Length / BarCount);
                int end = (i + 1) * (_audioBuffer.Length / BarCount);
                float sum = 0;

                for (int j = start; j < end && j < _audioBuffer.Length; j++)
                {
                    sum += Math.Abs(_audioBuffer[j]);
                }

                float avg = sum / (end - start);
                double barHeight = Math.Max(1, avg * height * 4);

                var bar = _spectrumBars[i];
                bar.Height = barHeight;
                bar.Width = BarThickness;
                Canvas.SetLeft(bar, 10 + i * (barWidth + 2));
                Canvas.SetTop(bar, height - barHeight / 2);

                var gradient = (LinearGradientBrush)bar.Fill;
                gradient.GradientStops[1].Color = Color.FromRgb(
                    (byte)(255 * (1 - avg * 2)),
                    (byte)(255 * avg * 2),
                    255);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            StopCapture();
            _deviceEnumerator?.Dispose();
            base.OnClosed(e);
        }
    }
}