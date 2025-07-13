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
    internal class Enemy3 : Enemies
    {
        const string imagePath = "C:\\School\\Projects\\Space Invaders\\Images\\zain.png";

        public Enemy3(Canvas maincanvas) : base(maincanvas)
        {

        }

        public int AddPoints()
        {
            return 30;
        }

        public override void AddImageSource()
        {
            enemy.Source = new BitmapImage(new Uri(imagePath));
        }
    }
}
