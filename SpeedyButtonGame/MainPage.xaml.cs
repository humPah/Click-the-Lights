using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using System.Threading;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace SpeedyButtonGame
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : SpeedyButtonGame.Common.LayoutAwarePage
    {
        /// <summary>
        /// Random number that's generated from 0 to 3, so one light gets picked
        /// </summary>
        int randomNr = 0;
        /// <summary>
        /// Randomizer to use in a few spots
        /// </summary>
        Random rand = new Random();
        /// <summary>
        /// Timer for the game
        /// </summary>
        DispatcherTimer timerLights;

        /// <summary>
        /// DependencyProperty for points the user generates
        /// </summary>
        public int GamePoints
        {
            get { return (int)GetValue(GamePointsProperty); }
            set { SetValue(GamePointsProperty, value); }
        }
        public static readonly DependencyProperty GamePointsProperty = DependencyProperty.Register("GamePoints", typeof(int), typeof(MainPage), new PropertyMetadata(0, new PropertyChangedCallback(OnValueChanged)));
        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) { }//nothing should be needed }

        /// <summary>
        /// List of lights that there are in the screen
        /// </summary>
        private List<Ellipse> listLights = new List<Ellipse>();
        /// <summary>
        /// List of lights the user needs to press(have toggled on)
        /// </summary>
        private List<int> listLightstoPress = new List<int>();
        
        

        /// <summary>
        /// Constructors, adds all the lights to the list
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            listLights.Add(GreenLightCircle);
            listLights.Add(GoldLightCircle);
            listLights.Add(RedLightCircle);
            listLights.Add(YellowLightCircle);
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }
        

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }


        /// <summary>
        /// If the timer is on(i.e. the game is already on) does nothing. otherwise begins the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNewGame_Click_1(object sender, RoutedEventArgs e)
        {
            if (timerLights != null) if (timerLights.IsEnabled == true) return;
            BeginGame();            
        }


        /// <summary>
        /// Resets the game if it's on, sets all ellipses opacity to 0.3, points to 0, nullifies timerlight
        /// </summary>
        /// <param name="sender">sender of event</param>
        /// <param name="e">arguments for event</param>
        private void BtnReset_Click_1(object sender, RoutedEventArgs e)
        {
            TextBlockNotification.Text = "";
            if(timerLights != null) timerLights.Stop();
            timerLights = null;
            foreach (Ellipse light in listLights)
            {
                light.Opacity = 0.3;
            }
            GamePoints = 0;
        }


        /// <summary>
        /// Begins the game, sets all ellipses opacity to 0.3, starts the timer for lights and starts animation rotating starting text as well
        /// </summary>
        private void BeginGame()
        {
            foreach (Ellipse light in listLights)
            {
                light.Opacity = 0.3;
            }
            GamePoints = 0;
            TextBlockNotification.Text = "Starting...";
            timerLights = new DispatcherTimer();
            timerLights.Interval = new TimeSpan(0, 0, 0, 0, 2200);
            timerLights.Start();
            timerLights.Tick += timerLights_Tick_1;
            textAnimationStart.Begin();
        }


        /// <summary>
        /// Light is toggled on(opacity to full) and added to the list for user to press.
        /// After that generates random interval for next light to toggle, which is higher more points the user has
        /// </summary>
        /// <param name="sender">sender of event</param>
        /// <param name="e">arguments for event</param>
        private void timerLights_Tick_1(object sender, object e)
        {
            randomNr = rand.Next(0, 4);
            listLights[randomNr].Opacity = 1.0;
            listLightstoPress.Add(randomNr);
            int randomInterval = 0;
            if (GamePoints < 10) randomInterval = rand.Next(650, 850);
            else if (GamePoints < 20) randomInterval = rand.Next(570, 710);
            else if (GamePoints < 40) randomInterval = rand.Next(520, 770);
            else if (GamePoints < 60) randomInterval = rand.Next(350, 620);
            else if (GamePoints < 80) randomInterval = rand.Next(210, 550);
            else if (GamePoints < 100) randomInterval = rand.Next(190, 525);
            else if (GamePoints < 120) randomInterval = rand.Next(180, 480);
            else if (GamePoints < 140) randomInterval = rand.Next(150, 450);
            else randomInterval = rand.Next(130, 380);
            timerLights.Interval = new TimeSpan(0, 0, 0, 0, randomInterval);
            timerLights.Tick -= timerLights_Tick_1;
            timerLights.Tick += timerLights_Tick_2;
        }


        /// <summary>
        /// Short 220ms interval between lights toggling 
        /// </summary>
        /// <param name="sender">sender of event</param>
        /// <param name="e">arguments for event</param>
        private void timerLights_Tick_2(object sender, object e)
        {
            listLights[randomNr].Opacity = 0.3;
            timerLights.Interval = new TimeSpan(0, 0, 0, 0, 220);
            timerLights.Tick -= timerLights_Tick_2;
            timerLights.Tick += timerLights_Tick_1;
        }


        /// <summary>
        /// One of the circles has been pressed, sees if the timer is on, if not, returns
        /// Otherwise if there's no lights to press, puts gameover(user was too anxious and pressed before lights toggled on)
        /// If there is lights to press, checks if the user pressed the right light. If he did, remove that light from the queue and add one point
        /// If user pressed the wrong light, again puts gameover
        /// </summary>
        /// <param name="sender">sender of event</param>
        /// <param name="e">arguments for event</param>
        private void Circle_PointerPressed_1(object sender, PointerRoutedEventArgs e)
        {
            if (timerLights == null || timerLights.IsEnabled == false) return;

            if (listLightstoPress.Count == 0)
            {
                GameOver();
                return;
            }

            Ellipse ellipse = (Ellipse)sender;
            if(ellipse.Name.Equals("GreenLightCircle") == true)
            {
                if (listLightstoPress[0] == 0)
                {
                    GamePoints++;
                    listLightstoPress.RemoveAt(0);
                    return;
                }
                else GameOver(); return;
            }

            if (ellipse.Name.Equals("GoldLightCircle") == true)
            {
                if (listLightstoPress[0] == 1)
                {
                    GamePoints++;
                    listLightstoPress.RemoveAt(0);
                    return;
                }
                else GameOver(); return;
            }

            if (ellipse.Name.Equals("RedLightCircle") == true)
            {
                if (listLightstoPress[0] == 2)
                {
                    GamePoints++;
                    listLightstoPress.RemoveAt(0);
                    return;
                }
                else GameOver(); return;
            }

            if (ellipse.Name.Equals("YellowLightCircle") == true)
            {
                if (listLightstoPress[0] == 3)
                {
                    GamePoints++;
                    listLightstoPress.RemoveAt(0);
                    return;
                }
                else GameOver(); return;
            }
        }


        /// <summary>
        /// Game over, so stops the timer, clear lights to press and begins an animation rotating a Game over!-text
        /// </summary>
        private void GameOver()
        {
            timerLights.Stop();
            listLightstoPress.Clear();
            TextBlockNotification.Text = "Game over!";
            textAnimationEnd.Begin();
        }


        /// <summary>
        /// Event rotating the text finishes, so sets the text to empty
        /// </summary>
        /// <param name="sender">sender of event</param>
        /// <param name="e">arguments for event</param>
        private void DoubleAnimationRotation_Completed_1(object sender, object e)
        {
            TextBlockNotification.Text = "";
        }


        /// <summary>
        /// If the game is on, doesn't do anything. Otherwise opens Help.xaml
        /// </summary>
        /// <param name="sender">sender of event</param>
        /// <param name="e">arguments for event</param>
        private void BtnHelp_Click_1(object sender, RoutedEventArgs e)
        {
            if (timerLights != null) if (timerLights.IsEnabled == true) return;
            if (this.Frame != null)
            {
                this.Frame.Navigate(typeof(Help));
            }
        }


        /// <summary>
        /// If the game is on, doesn't do anything. Otherwise opens HighScores.xaml
        /// </summary>
        /// <param name="sender">sender of event</param>
        /// <param name="e">arguments for event</param>
        private void BtnHighScores_Click_1(object sender, RoutedEventArgs e)
        {
            if (timerLights != null) if (timerLights.IsEnabled == true) return;
            if (this.Frame != null)
            {
                this.Frame.Navigate(typeof(HighScores));
            }
        }


        /// <summary>
        /// GameOver-animation ends, so goes to HighScores.xaml with a parameter of how many points the user got this round
        /// </summary>
        /// <param name="sender">sender of event</param>
        /// <param name="e">arguments for event</param>
        private void DoubleAnimationRotation2_Completed_1(object sender, object e)
        {
            if (this.Frame != null)
            {
                this.Frame.Navigate(typeof(HighScores), GamePoints);
            }
            
        }

        
    }
}
