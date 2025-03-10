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
        private TimeSpanToDoubleConverter timespanToDouble = new TimeSpanToDoubleConverter();

        public MainPage()
        {
            InitializeComponent();

            BindingContext = this; // Make sure MainPage is the binding context
            Instance = this;
            AudioSlider.BindingContext = mediaPlayer;
            var timespanToDouble = new TimeSpanToDoubleConverter();

            AudioSlider.SetBinding(Slider.ValueProperty, nameof(MediaElement.Position), BindingMode.TwoWay, timespanToDouble);
            AudioSlider.SetBinding(Slider.MaximumProperty, nameof(MediaElement.Duration), BindingMode.OneWay, timespanToDouble);

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
        public async Task LoadAndStartAudio(string filePath, bool stream = false)
        {


            // Stop and Reset Before Changing Source
            mediaPlayer.Stop();
            mediaPlayer.Source = null;
            await Task.Delay(100); // Allow time for reset (prevents errors)

            // Now Set the New Source and Play
            mediaPlayer.ShouldAutoPlay = true;

            if (stream = true)
            {
                mediaPlayer.Source = MediaSource.FromUri(filePath);
            }
            else
            {

                mediaPlayer.Source = MediaSource.FromFile(filePath);
            }
            AudioSlider.Value = 0;

            IsPlaying = true;




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
            var (totalMin, seconds) = CalculateTotalSecoundsToMinutes(AudioSlider.Value);
            var (totalMinDuration, secondsDuration) = CalculateTotalSecoundsToMinutes(AudioSlider.Maximum);

            await Application.Current.Dispatcher.DispatchAsync(async () => DebugLabelSlider.Text = totalMin + ":" + seconds.ToString("00"));
            await Application.Current.Dispatcher.DispatchAsync(async () => DebugLabel.Text = totalMinDuration + ":" + secondsDuration.ToString("00"));
            //MainThread.BeginInvokeOnMainThread(
            //    async () => DebugLabelSlider.Text = totalMin + ":" + seconds.ToString("00"));
            //MainThread.BeginInvokeOnMainThread(
            //    async () => DebugLabel.Text = totalMinDuration + ":" + secondsDuration.ToString("00"));

            void DebugSliderValues(object sender)
            {
                DebugLabel.Text = $"{(int)mediaPlayer.Position.TotalSeconds} of {(int)mediaPlayer.Duration.TotalSeconds} {sender.GetType()}";
                DebugLabelSlider.Text = "Slider value: " + AudioSlider.Value + " Slider Length:" + AudioSlider.Maximum;
            }

            (int, int) CalculateTotalSecoundsToMinutes(double totalSecounds)
            {
                var totalMin = Math.Floor((totalSecounds / 60));
                var remaingingSec = totalSecounds - totalMin * 60;
                return ((int)totalMin, (int)remaingingSec);
            }
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
                await Application.Current.Dispatcher.DispatchAsync(async () =>
                await mediaPlayer.SeekTo(pos));
                //MainThread.BeginInvokeOnMainThread(async () => await mediaPlayer.SeekTo(pos));


                isDraging = false;
                DragingState.SetIsDraging();

                //mediaPlayer.Play();
                //await MediaPlayer.SeekTo(pos);
            }

        }

        //private async void OnPositionChanged(object sender, MediaPositionChangedEventArgs e)
        //{


        //    if (!isDraging)
        //    {
        //        await Task.Delay(800);

        //        AudioSlider.Value = e.Position.TotalSeconds;

        //    }

        //    if (isDraging) return; // 🔹 Ignore position updates while dragging

        //    AudioSlider.Value = e.Position.TotalSeconds; // Update when not dragging

        //}

        private async void OnAudioSliderDragStarted(object sender, EventArgs e)
        {
            //mediaPlayer.Pause();
            isDraging = true;
            DragingState.SetIsDraging();
            AudioSlider.RemoveBinding(Slider.ValueProperty);

        }

        private async void OnMediaOpened(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                AudioSlider.Maximum = mediaPlayer.Duration.TotalSeconds;
            });

        }

        private async void OnAudioSeekCompleted(object sender, EventArgs e)
        {
            await Task.Delay(200);
            await Application.Current.Dispatcher.DispatchAsync(async () =>
            {

                AudioSlider.SetBinding(Slider.ValueProperty,
                nameof(MediaElement.Position),
                BindingMode.TwoWay, timespanToDouble);
            });

            //MainThread.BeginInvokeOnMainThread(async () =>
            //{

            //    AudioSlider.SetBinding(Slider.ValueProperty,
            //    nameof(MediaElement.Position),
            //    BindingMode.TwoWay, timespanToDouble);
            //});
        }
        public async void ShowAudioElement()
        {
            AudioContainer.IsVisible = true;
        }
        public async void HideAudioElement()
        {
            AudioContainer.IsVisible = false;
        }
    }
}
