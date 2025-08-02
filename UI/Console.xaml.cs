using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace NekoDeskTop.UI
{
    public partial class Console : Window
    {
        private Paragraph _currentInputParagraph;
        private Run _promptRun;
        private Run _inputRun;
        private Process _cmdProcess;
        private StreamWriter _cmdInput;

        public Console()
        {
            InitializeComponent();
            InitUI();
            StartCmdProcess();
        }

        private void InitUI()
        {
            this.Topmost = true;
            this.WindowStyle = WindowStyle.None;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            log.Document.Blocks.Clear();
            var versionPara = new Paragraph();
            versionPara.Inlines.Add(new Run("NekoDeskTopConsole Window [Version:Alpha-1.0.0]")
            { Foreground = Brushes.Gray, FontStyle = FontStyles.Italic });
            log.Document.Blocks.Add(versionPara);
            _promptRun = new Run("Master: ") { Foreground = Brushes.Cyan };
            _inputRun = new Run() { Foreground = Brushes.White };

            _currentInputParagraph = new Paragraph();
            _currentInputParagraph.Inlines.Add(_promptRun);
            _currentInputParagraph.Inlines.Add(_inputRun);

            log.Document.Blocks.Add(_currentInputParagraph);
            log.CaretPosition = _inputRun.ElementEnd;
            log.Focus();
            log.PreviewKeyDown += OnPreviewKeyDown;
        }

        private void StartCmdProcess()
        {
            _cmdProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/k @echo off & prompt $G$ & cls",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                }
            };

            _cmdProcess.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Dispatcher.Invoke(() => AppendOutput(e.Data));
            };

            _cmdProcess.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Dispatcher.Invoke(() => AppendOutput(e.Data, isError: true));
            };

            _cmdProcess.Start();
            _cmdInput = _cmdProcess.StandardInput;

            _cmdProcess.BeginOutputReadLine();
            _cmdProcess.BeginErrorReadLine();
        }

        private void AppendOutput(string text, bool isError = false)
        {
            if (string.IsNullOrEmpty(text)) return;

            Dispatcher.Invoke(() =>
            {
                log.Document.Blocks.Remove(_currentInputParagraph);
                var outputParagraph = new Paragraph(new Run(text))
                {
                    Foreground = isError ? Brushes.Red : Brushes.White,
                    Margin = new Thickness(0)
                };
                log.Document.Blocks.Add(outputParagraph);

                _inputRun.Text = "";
                log.Document.Blocks.Add(_currentInputParagraph);
                log.CaretPosition = _inputRun.ElementEnd;
                log.ScrollToEnd();
            });
        }

        public void AppendString(string msg, SolidColorBrush color)
        {
            if (string.IsNullOrEmpty(msg)) return;

            Dispatcher.Invoke(() =>
            {
                var inputText = _inputRun.Text;
                log.Document.Blocks.Remove(_currentInputParagraph);

                var outputParagraph = new Paragraph(new Run(msg))
                {
                    Foreground = color,
                    Margin = new Thickness(0)
                };
                log.Document.Blocks.Add(outputParagraph);

                _inputRun.Text = inputText;
                log.Document.Blocks.Add(_currentInputParagraph);

                log.ScrollToEnd();
                log.CaretPosition = _inputRun.ElementEnd;
            });
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string command = _inputRun.Text;
                if (!string.IsNullOrWhiteSpace(command))
                {
                    _cmdInput.WriteLine(command);
                    _inputRun.Text = "";
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Back && _inputRun.Text.Length == 0)
            {
                e.Handled = true;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _cmdInput?.Close();
            _cmdProcess?.Close();
            base.OnClosed(e);
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}