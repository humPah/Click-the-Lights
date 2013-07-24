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
using Windows.Storage;
using System.Threading;
using Windows.Storage.Streams;

using System.Threading.Tasks;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace SpeedyButtonGame
{

    

    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class HighScores : SpeedyButtonGame.Common.LayoutAwarePage
    {
        /// <summary>
        /// List of current highscores
        /// </summary>
        private List<PlayerHighScoreItem> listHighScores = new List<PlayerHighScoreItem>();
        /// <summary>
        /// new score, that's compared to old ones
        /// </summary>
        int newScore = int.MinValue;

        /// <summary>
        /// Constructor
        /// </summary>
        public HighScores()
        {
            this.InitializeComponent();            
        }

        /// <summary>
        /// Sees if there is parameters when navigated to, and if there is, takes the parameter as integer and puts to newScore
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                newScore = (int)e.Parameter;
            }
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// Loads highscores waiting the task to finish, then if there is a newscore compares it to old high scores and sets gridaskname visible if it makes the list
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            await LoadFromFile();
            if (newScore >= 0)
            {
                if (listHighScores.Count >= 10)
                {
                    if (newScore > listHighScores[9].Score) GridAskName.Visibility = Visibility.Visible;
                }
                else GridAskName.Visibility = Visibility.Visible;
            }
        }


        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of 
        /// Saves the high scores to the file<see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            SaveToFile();
        }


        /// <summary>
        /// Loads highscores from scores.txt, if it exists
        /// </summary>
        private async Task LoadFromFile()
        {
            String filename = "scores.txt";
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file;
            try
            {
                file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appdata:///local/" + filename));
            }
            catch (FileNotFoundException) { return; }

            string content = await FileIO.ReadTextAsync(file);
            string[] separator = new string[] { "\r\n" };
            string[] scoresArray = content.Split(separator, StringSplitOptions.None);

            for (int i = 0; i < scoresArray.Length; i++)
            {
                char[] separatorChar = new char[] { '|' };
                string[] oneScoreArray = scoresArray[i].Split(separatorChar, StringSplitOptions.None);
                PlayerHighScoreItem item = new PlayerHighScoreItem();
                try
                {
                    item.Name = oneScoreArray[0];
                    item.Score = Convert.ToInt16(oneScoreArray[1]);
                }
                catch (Exception e)
                {
                    if (e is IndexOutOfRangeException || e is FormatException)
                    {
                        break;
                    }
                    else throw;
                }
                if ((item.Name.Equals("") != true) || (item.Score > 0)) listHighScores.Add(item);
            }

            listHighScores.Sort();
            UpdateGridScores();
        }

        /// <summary>
        /// Saves the current highscores to HDD in local folder, deletes existing scores.txt if it exists, and then replaces it with updated scores
        /// </summary>
        private async void SaveToFile()
        {

            String filename = "scores.txt";
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file;
            try
            {
                file = await folder.GetFileAsync(filename);
                await file.DeleteAsync();
            }
            catch (FileNotFoundException) { }
            
            file = await folder.CreateFileAsync(filename);

            string towrite = "";            
            if (listHighScores.Count == 1) towrite = towrite + listHighScores[0].Name + "|" + listHighScores[0].Score + "\r\n";
            else
            {
                for (int i = 0; i < listHighScores.Count - 1; i++)
                {
                    towrite = towrite + listHighScores[i].Name + "|" + listHighScores[i].Score + "\r\n";
                }
            }

            if (listHighScores.Count >= 2)
            {
                towrite = towrite + listHighScores[listHighScores.Count - 1].Name + "|" + listHighScores[listHighScores.Count - 1].Score;
            }
            
            
            await Windows.Storage.FileIO.WriteTextAsync(file, towrite);
        }

        /// <summary>
        /// Clears GridHighscores-grid, and then adds Textblocks for headers(Player & Score) and each score for its own line
        /// </summary>
        private void UpdateGridScores()
        {
            GridHighscores.Children.Clear();
            GridHighscores.Children.Add(TextBlockPlayer);
            GridHighscores.Children.Add(TextBlockScore);
            int row = 1;
            foreach (PlayerHighScoreItem o in listHighScores)
            {
                RowDefinition rd = new RowDefinition();
                rd.Height = GridLength.Auto;
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = GridLength.Auto;
                GridHighscores.RowDefinitions.Add(rd);
                GridHighscores.ColumnDefinitions.Add(cd);

                TextBlock name = new TextBlock();
                TextBlock score = new TextBlock();
                name.Text = o.Name;
                score.Text = "" + o.Score;
                name.FontSize = 18;
                score.FontSize = 18;
                Grid.SetRow(name, row);
                Grid.SetRow(score, row);
                Grid.SetColumn(name, 0);
                Grid.SetColumn(score, 1);
                name.Margin = new Thickness(5, 5, 25, 5);
                score.Margin = new Thickness(25, 5, 5, 5);

                row++;
                GridHighscores.Children.Add(name);
                GridHighscores.Children.Add(score);
            }
        }


        /// <summary>
        /// Saves the highscore, if there's 10 highscores already, removes the last one from the list before saving, and sorts the list
        /// Asks the name user wants to put, removes '|' with ' ' so won't mess up the serialization
        /// Toggles grid for asking name collapsed
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">eventarguments</param>
        private void ButtonHighScore_Click_1(object sender, RoutedEventArgs e)
        {
            PlayerHighScoreItem newPlayerScore = new PlayerHighScoreItem();
            String name = TextBoxHighScore.Text.Replace('|', ' ');
            newPlayerScore.Name = name;
            newPlayerScore.Score = newScore;                        
            if (listHighScores.Count >= 10)
            {
                listHighScores.RemoveAt(9);
            }
            listHighScores.Add(newPlayerScore);
            listHighScores.Sort();
            UpdateGridScores();
            GridAskName.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// Sees if the user presses enter inside TextBoxHighScore and if he does, submits the name and marks the event handled
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">eventarguments</param>
        private void TextBoxHighScore_KeyDown_1(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                EnterHighScore();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Saves the highscore, if there's 10 highscores already, removes the last one from the list before saving, and sorts the list
        /// Asks the name user wants to put, removes '|' with ' ' so won't mess up the serialization
        /// Toggles grid for asking name collapsed
        /// </summary>
        private void EnterHighScore()
        {
            PlayerHighScoreItem newPlayerScore = new PlayerHighScoreItem();
            String name = TextBoxHighScore.Text.Replace('|', ' ');
            newPlayerScore.Name = name;
            newPlayerScore.Score = newScore;
            if (listHighScores.Count >= 10)
            {
                listHighScores.RemoveAt(9);
            }
            listHighScores.Add(newPlayerScore);
            listHighScores.Sort();
            UpdateGridScores();
            GridAskName.Visibility = Visibility.Collapsed;
        }
    }
}

