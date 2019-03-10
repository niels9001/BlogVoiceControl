using HomeHub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input.Preview;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace HomeHub
{
    public sealed partial class DetailsView : Page
    {
        SystemNavigationManager _systemNavigationManager = SystemNavigationManager.GetForCurrentView();
        private CoreDispatcher dispatcher;
        private SpeechRecognizer speechRecognizer;
        ObservableCollection<Room> Devices { get; set; }
        public DetailsView()
        {
            this.InitializeComponent();

            Devices = new ObservableCollection<Room>();
            Devices.Add(new Room() { Name = "Light 1 " });
            Devices.Add(new Room() { Name = "Light 2 " });
            Devices.Add(new Room() { Name = "Light 3 " });
            Devices.Add(new Room() { Name = "Light 4 " });
            Devices.Add(new Room() { Name = "Light 5 " });
            Devices.Add(new Room() { Name = "Light 6 " });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);
            Room SelectedRoom = (Room)e.Parameter;
            RoomImage.Source = new BitmapImage(new Uri(SelectedRoom.Image));
            RoomName.Text = SelectedRoom.Name;
            StartSpeechRecognition();
        }



        private async void StartSpeechRecognition()
        {

            this.dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            this.speechRecognizer = new SpeechRecognizer();
            SpeechRecognitionCompilationResult result = await this.speechRecognizer.CompileConstraintsAsync();
            this.speechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;
            this.speechRecognizer.HypothesisGenerated += SpeechRecognizer_HypothesisGenerated;


            await speechRecognizer.ContinuousRecognitionSession.StartAsync();
        }

        private async void SpeechRecognizer_HypothesisGenerated(SpeechRecognizer sender, SpeechRecognitionHypothesisGeneratedEventArgs args)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                string Result = args.Hypothesis.Text.ToLower();
            });
        }

        private async void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                string Result = args.Result.Text.ToLower();

                SpeechRecognizedText.Text = Result;
                if (Result.Contains("back"))
                {
                    NavigateBack();
                }

            });
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateBack();
        }

        private async void NavigateBack()
        {
            this.speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
            await this.speechRecognizer.ContinuousRecognitionSession.CancelAsync();

            this.speechRecognizer.Dispose();
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}