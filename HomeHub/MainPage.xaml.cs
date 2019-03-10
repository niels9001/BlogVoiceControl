using HomeHub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input.Preview;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace HomeHub
{
    public sealed partial class MainPage : Page
    {
        ObservableCollection<Person> PersonsList { get; set; }
        ObservableCollection<Room> RoomsList { get; set; }

        private CoreDispatcher dispatcher;
        private SpeechRecognizer speechRecognizer;

        public MainPage()
        {
            this.InitializeComponent();

            RoomsList = new ObservableCollection<Room>();
            RoomsList.Add(new Room() { Name = "Bathroom", Image = "ms-appx:///Assets/Images/Bathroom.jpg" });
            RoomsList.Add(new Room() { Name = "Entrance", Image = "ms-appx:///Assets/Images/FrontDoor.jpg" });
            RoomsList.Add(new Room() { Name = "Living room", Image = "ms-appx:///Assets/Images/LivingRoom.jpg" });
            RoomsList.Add(new Room() { Name = "Terrace", Image = "ms-appx:///Assets/Images/Terrace.jpg" });
            RoomsList.Add(new Room() { Name = "Garden", Image = "ms-appx:///Assets/Images/Garden.jpg" });

            PersonsList = new ObservableCollection<Person>();
            PersonsList.Add(new Person() { Name = "Dianna", Image = "ms-appx:///Assets/Images/Dianna.jpg" });
            PersonsList.Add(new Person() { Name = "Loki", Image = "ms-appx:///Assets/Images/Loki.jpeg" });
            PersonsList.Add(new Person() { Name = "Ana", Image = "ms-appx:///Assets/Images/Ana.jpg" });
           
        }
        private void RoomsView_ItemClick(object sender, ItemClickEventArgs e)
        {
            NavigateToRoom((Room)e.ClickedItem);
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            StartSpeechRecognition();
        }

        private async void StartSpeechRecognition()
        {

            this.dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            this.speechRecognizer = new SpeechRecognizer();
            SpeechRecognitionCompilationResult result = await this.speechRecognizer.CompileConstraintsAsync();
            this.speechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;
            this.speechRecognizer.HypothesisGenerated += SpeechRecognizer_HypothesisGenerated;
            this.speechRecognizer.ContinuousRecognitionSession.Completed += ContinuousRecognitionSession_Completed;
            //Starting recognizer


            await speechRecognizer.ContinuousRecognitionSession.StartAsync();
        }


        private async void ContinuousRecognitionSession_Completed(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
        {
            await speechRecognizer.ContinuousRecognitionSession.StartAsync();
        }

        private async void SpeechRecognizer_HypothesisGenerated(SpeechRecognizer sender, SpeechRecognitionHypothesisGeneratedEventArgs args)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SpeechRecognizedText.Text = args.Hypothesis.Text;

            });
        }

        private async void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                string Result = args.Result.Text.ToLower();
                SpeechRecognizedText.Text = args.Result.Text;

                foreach (Room R in RoomsList)
                {
                    if (Result.Contains(R.Name.ToLower()))
                    {
                        NavigateToRoom(R);
                    }
                }

                if (Result == "select")
                {
                    GetFocusedItem();
                }
            });
        }

        private async void NavigateToRoom(Room SelectedRoom)
        {
            this.speechRecognizer.ContinuousRecognitionSession.Completed -= ContinuousRecognitionSession_Completed;
            this.speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
            await this.speechRecognizer.ContinuousRecognitionSession.CancelAsync();

            this.speechRecognizer.Dispose();
            Frame.Navigate(typeof(DetailsView), SelectedRoom);
        }


        private void GetFocusedItem()
        {
            object x = FocusManager.GetFocusedElement();
            if (x.GetType() == typeof(GridViewItem))
            {
                var item = (GridViewItem)x;
                NavigateToRoom((Room)item.Content);
            }
        }
    }
}