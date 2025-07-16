using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Space_Invaders
{
    internal class Enemy2 : Enemies
    {
        private string imagePath = "Images/bearEnemy2.png";
        private int basePoints = 20;
        public string image => imagePath;

        public Enemy2(Canvas maincanvas) : base(maincanvas)
        {

        }

        public int AddPoints(int levelNumber)
        {
            return basePoints * levelNumber;
        }

        public override void AddImageSource()
        {
            enemy.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
        }
    }
}
