using System.Diagnostics;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using BlazorMaui.Components.Pages;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using BlazorMaui.Helpers;


namespace BlazorMaui
{
    public partial class MainPage : ContentPage
    {
        public static MainPage Instance { get; private set; }
        public MediaElement MediaPlayer => mediaPlayer;
        private bool isDraging = false;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this; // 🔹 Make sure MainPage is the binding context
            Instance = this;
            AudioSlider.BindingContext = mediaPlayer;
            var timespanToDouble = new TimeSpanToDoubleConverter();

            AudioSlider.SetBinding(Slider.ValueProperty, nameof(MediaElement.Position), BindingMode.Default, timespanToDouble);
            AudioSlider.SetBinding(Slider.MaximumProperty, nameof(MediaElement.Duration), BindingMode.Default, timespanToDouble);

            //MediaPlayer.BindingContext = "AudioSlider";
            //MediaPlayer.SetBinding(CommunityToolkit.Maui.Views.MediaElement.PositionProperty,"")
        }

        private bool _isPlaying;

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    OnPropertyChanged(nameof(IsPlaying)); // Notify UI
                }
            }
        }
        public async Task LoadAndStartAudio(string filePath)
        {
            if (File.Exists(filePath))
            {

                // Stop and Reset Before Changing Source
                mediaPlayer.Stop();
                mediaPlayer.Source = null;
                await Task.Delay(100); // Allow time for reset (prevents errors)

                // Now Set the New Source and Play
                mediaPlayer.ShouldAutoPlay = true;
                mediaPlayer.Source = MediaSource.FromFile(filePath);




                AudioSlider.Value = 0;

                IsPlaying = true;
            }
        }


        public void PauseAudio()
        {

            MediaPlayer.Pause();
            IsPlaying = false;
        }

        public void StopAudio()
        {
            MediaPlayer.Stop();
            IsPlaying = false;
        }
        

        public event Action? MediaEnded;

        private void mediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            IsPlaying = false;
            MediaEnded?.Invoke();
        }



        private async void OnAudioSliderValueChanged(object sender, ValueChangedEventArgs e)
        {

            DebugLabel.Text = $"Current: {(int)mediaPlayer.Position.TotalSeconds} of {(int)mediaPlayer.Duration.TotalSeconds} {sender.GetType()}";
            DebugLabelSlider.Text = "Slider value: " + AudioSlider.Value + " Slider Length:" + AudioSlider.Maximum;

        }

        public async void OnPlayPauseClick(object sender, EventArgs e)
        {


            if (MediaPlayer.CurrentState == MediaElementState.Stopped ||
        MediaPlayer.CurrentState == MediaElementState.Paused)
            {
                IsPlaying = true;
                MediaPlayer.Play();

            }
            else if (MediaPlayer.CurrentState == MediaElementState.Playing)
            {
                IsPlaying = false;
                MediaPlayer.Pause();

            }
        }

        private async void OnAudioSliderDragCompleted(object sender, EventArgs e)
        {
            if (isDraging)
            {
                var pos = TimeSpan.FromSeconds(AudioSlider.Value);
                await MediaPlayer.SeekTo(pos);
            }

            DebugLabel.Text = $"DragCompleted - Current: {(int)mediaPlayer.Position.TotalSeconds} of {(int)mediaPlayer.Duration.TotalSeconds} {sender.GetType().Name} ";
            isDraging = false;
        }

        //private async void OnPositionChanged(object sender, MediaPositionChangedEventArgs e)
        //{


        //    if (!isDraging)
        //    {
        //        await Task.Delay(800);

        //        AudioSlider.Value = e.Position.TotalSeconds;

        //    }
        //}

        private void OnAudioSliderDragStarted(object sender, EventArgs e)
        {
            isDraging = true;
        }

        private async void OnMediaOpened(object sender, EventArgs e)
        {
            //// Wait for duration to become available
            //for (int i = 0; i < 10; i++) // Try for ~1 second
            //    {
            //        if (mediaPlayer.Duration.TotalSeconds > 0 )
            //            break;

            //        await Task.Delay(100);
            //    }



            MainThread.BeginInvokeOnMainThread(() =>
            {
                AudioSlider.Maximum = mediaPlayer.Duration.TotalSeconds;
            });


            // Reset Slider 


        }
    }
}
