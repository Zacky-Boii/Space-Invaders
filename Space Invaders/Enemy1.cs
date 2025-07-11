using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Space_Invaders
{
    public class Enemy1
    {
        private static int shipCount = 0;
        public int shipNumber;

        private string enemy1Path = "C:\\School\\Projects\\Space Invaders\\Images\\enemy1.png";
        
        Canvas maincanvas;
        public Image enemy;

        private int enemyX;
        private int enemyY;
        private int row;

        public Enemy1(Canvas canvas)
        {
            shipNumber = shipCount;
            shipCount++;

            enemyX = (shipNumber * 60) % 600;
            enemyY = 570 - 35 * (shipCount / 11);
            row = shipCount / 11;

            maincanvas = canvas;
            Initialise();
        }

        public void Redraw()
        {
            enemyX = enemyX + 60 % 600;
            enemyY = 570 - 35 * (shipCount / 11);
        }

        public void Initialise()
        {
            enemy = new Image();
            enemy.Source = new BitmapImage(new Uri(enemy1Path));
            enemy.Height = 30;
            enemy.Width = 50;

            maincanvas.Children.Add(enemy);

            Canvas.SetBottom(enemy, enemyY);
            Canvas.SetLeft(enemy, enemyX);
        }

        public bool TouchingLaser(int laserX, int laserY)
        {
            if ( (enemyX-2) <= laserX && laserX<=enemyX+enemy.Width && laserY >= (enemyY -enemy.Height/2))
            {
                return true;
            }
            return false;
        }
    }
}
