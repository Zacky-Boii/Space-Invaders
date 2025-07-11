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
        private static int enemyCount = 0;
        public int enemyNumber;

        private string enemy1Path = "C:\\School\\Projects\\Space Invaders\\Images\\enemy1.png";

        Canvas maincanvas;
        public Image enemy;

        private int enemyX;
        private int enemyY;
        private int row = 0;

        public Enemy1(Canvas canvas)
        {
            enemyNumber = enemyCount;
            enemyCount++;

            maincanvas = canvas;
            Initialise();
        }

        public void Redraw()
        {
            if(enemyX + (int)(enemy.Width * 2.4)>maincanvas.Width)
            {
                row++;
                enemyX = -(int)(enemy.Width * 1.2);
            }

            enemyX = (int)(enemyX + (enemy.Width * 1.2) % maincanvas.Width);
            enemyY = 570 - (int)(enemy.Height * 1.2) * row;

            Canvas.SetBottom(enemy, enemyY);
            Canvas.SetLeft(enemy, enemyX);
        }

        public void Initialise()
        {
            enemy = new Image();
            enemy.Source = new BitmapImage(new Uri(enemy1Path));
            enemy.Height = 30;
            enemy.Width = 50;

            int enemiesPerRow = (int)Math.Ceiling(maincanvas.Width/(enemy.Width*1.2));
            row = enemyNumber / enemiesPerRow;

            enemyX = (int)(enemyNumber * (enemy.Width * 1.2)) % (int)maincanvas.Width;
            enemyY = 570 - (int)(enemy.Height * 1.2) * row;

            maincanvas.Children.Add(enemy);

            Canvas.SetBottom(enemy, enemyY);
            Canvas.SetLeft(enemy, enemyX);
        }

        public bool TouchingLaser(int laserX, int laserY)
        {
            if ((enemyX - 2) <= laserX && laserX <= enemyX + enemy.Width && laserY >= (enemyY - enemy.Height / 2))
            {
                return true;
            }
            return false;
        }
    }
}