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
    internal class Enemy1 : Enemies
    {
        private string imagePath = "Images/Enemy1.png";
        private int basePoints = 10;
        public string image => imagePath;

        public Enemy1(Canvas maincanvas) : base(maincanvas)
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
