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

        private int enemiesPerRow;
        private int rowPos;

        private bool forwards = true;
        private int movements = 0; // how many times have they gone onto a new row
        private int pixelsMoved = 0; // this will allow me to increase speed baseed off of enemies
        private int lastPixelsMoved = 0;
        private int moveBy; // how many pixels to move at once, the bigger it is the harder it is

        public Enemy1(Canvas canvas)
        {
            enemyNumber = enemyCount;
            enemyCount++;

            maincanvas = canvas;
            Initialise();
        }

        public void Redraw(int enemiesLeft, int enemyCount)
        {
            moveBy = enemyCount / enemiesLeft;

            if (forwards) enemyX += moveBy;
            else enemyX -= moveBy;

            enemyY = (int)(maincanvas.Height - enemy.Height) - (int)(enemy.Height * spriteSeperation) * row;

            Canvas.SetBottom(enemy, enemyY);
            Canvas.SetLeft(enemy, enemyX);
        }

        public bool TouchingBorder()
        {
            //right hand border
            if (enemyX + enemy.Width >= maincanvas.Width)
            {
                return true;
            }
            //left hand border
            if (enemyX <= 0)
            {
                return true;
            }
            return false;
        }

        public void AddRow()
        {
            row++;
            movements++;

            if (movements % 2 == 0) forwards = true;
            else forwards = false;
        }

        public void Initialise()
        {
            enemy = new Image();
            enemy.Source = new BitmapImage(new Uri(enemy1Path));
            enemy.Height = 30;
            enemy.Width = 30;

            enemiesPerRow = (int)Math.Floor((maincanvas.Width - borderSeperation * 2) / (enemy.Width * spriteSeperation));
            row = enemyNumber / enemiesPerRow;
            rowPos = enemyNumber % (enemiesPerRow);
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
            if (((enemyX - 2) < laserX && laserX < enemyX + enemy.Width) && (enemyY-14<=laserY && laserY<=(enemyY+enemy.Height-14)))// 14 is laser height ans 2 is laser width midpoint
            {
                Debug.WriteLine(enemyNumber);
                return true;
            }
            return false;
        }
    }
}