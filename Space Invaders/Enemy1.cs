using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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

        private double spriteSeperation = 1.2; // multiplier
        private int borderSeperation = 30; // pixels

        public Enemy1(Canvas canvas)
        {
            enemyNumber = enemyCount;
            enemyCount++;

            maincanvas = canvas;
            Initialise();
        }

        public void Redraw()
        {
            // check if the sprite will go past 570 pixels which is the border i want currently
            if (enemyX + (int)(enemy.Width * spriteSeperation * 2) > (maincanvas.Width - borderSeperation))
            {
                row++;
                enemyX = -(int)(enemy.Width * spriteSeperation) + borderSeperation;
            }

            enemyX = (int)(enemyX + (enemy.Width * spriteSeperation) % maincanvas.Width);
            enemyY = 570 - (int)(enemy.Height * spriteSeperation) * row;

            Canvas.SetBottom(enemy, enemyY);
            Canvas.SetLeft(enemy, enemyX);
        }

        public void Initialise()
        {
            enemy = new Image();
            enemy.Source = new BitmapImage(new Uri(enemy1Path));
            enemy.Height = 30;
            enemy.Width = 30;

            int enemiesPerRow = (int)Math.Floor((maincanvas.Width - borderSeperation * 2) / (enemy.Width * spriteSeperation));
            row = enemyNumber / enemiesPerRow;
            int rowPos = enemyNumber % (enemiesPerRow);
            Debug.WriteLine(enemiesPerRow);
            if (rowPos != 0)
            {
                if ((int)(rowPos * (enemy.Width * spriteSeperation) + borderSeperation) > (int)maincanvas.Width - borderSeperation)
                {
                    row++;
                    enemyX = borderSeperation;
                }
                else enemyX = (borderSeperation + (int)(rowPos * (enemy.Width * spriteSeperation) + borderSeperation)) % (int)maincanvas.Width - borderSeperation;
            }
            else enemyX = borderSeperation;
            enemyY = (int)(maincanvas.Height - enemy.Height) - (int)(enemy.Height * spriteSeperation) * row;

            maincanvas.Children.Add(enemy);

            Canvas.SetBottom(enemy, enemyY);
            Canvas.SetLeft(enemy, enemyX);
        }

        public bool TouchingLaser(int laserX, int laserY)
        {
            if ((enemyX - 2) <= laserX && laserX <= enemyX + enemy.Width && laserY >= (enemyY - enemy.Height / 2))
            {
                Debug.WriteLine(enemyNumber);
                return true;
            }
            return false;
        }
    }
}