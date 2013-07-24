using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyButtonGame
{
    public class PlayerHighScoreItem : IComparable<PlayerHighScoreItem>
    {
        /// <summary>
        /// Name for highscore
        /// </summary>
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// Points for highscore
        /// </summary>
        private int _Score;
        public int Score
        {
            get { return _Score; }
            set { _Score = value; }
        }

        /// <summary>
        /// Constructor, don't need much
        /// </summary>
        public PlayerHighScoreItem()
        {
            //
        }


        /// <summary>
        /// Compares one item to another, returns 1, 0 or -1 accordingly
        /// </summary>
        /// <param name="compared"></param>
        /// <returns></returns>
        public int CompareTo(PlayerHighScoreItem compared)
        {
            if (this.Score > compared.Score) return -1;
            if (this.Score == compared.Score) return 0;
            return 1;
        }
    }
}
