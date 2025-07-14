using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Space_Invaders
{
    public class Enemies
    {
        public static int enemyCount = 0;
        public int enemyNumber;

        private string errorImage = "C:\\School\\Projects\\Space Invaders\\Images\\sophia.png";

        Canvas maincanvas;
        public Image enemy;

        public double enemyX;
        public double enemyY;
        private int row = 0;

        private double spriteSeperation = 1.2; // multiplier
        private int borderSeperation = 30; // pixels

        private int enemiesPerRow;
        private int rowPos;

        public bool forwards = true;
        private static int movements = 0; // how many times have they gone onto a new row
        public double moveBy; // how many pixels to move at once, the bigger it is the harder it is

        public Enemies(Canvas canvas)
        {
            enemyNumber = enemyCount;
            enemyCount++;

            maincanvas = canvas;
            Initialise();
        }
        DateTime lastUpdate = DateTime.Now;
        private double Speed(int enemiesLeft, int stageNumber)
        {
            var now = DateTime.Now;
            double deltaTime = (now - lastUpdate).TotalSeconds;
            lastUpdate = now;

            if (enemiesLeft <= 0) enemiesLeft = 1;

            double progressRatio = (double)(enemyCount - enemiesLeft) / enemyCount;

            double baseSpeed = 20;
            double maxSpeed = 100; 
            double stageMultiplier = 1 + 0.25 * (stageNumber - 1); // each stage gets 25% faster

            // interpolate between baseSpeed and maxSpeed based on how many enemies are left
            double interpolatedSpeed = baseSpeed + (maxSpeed - baseSpeed) * progressRatio;

            double finalSpeed = interpolatedSpeed * stageMultiplier;

            return finalSpeed * deltaTime; // movement per frame
        }

        public void Redraw(int enemiesLeft, int stageNumber, double deltaTime)
        {
            moveBy = Speed(enemiesLeft, stageNumber);

            if (forwards) enemyX += moveBy;
            else enemyX -= moveBy;

            enemyY = (maincanvas.Height - enemy.Height - 30) - (enemy.Height * spriteSeperation) * row;

            Canvas.SetBottom(enemy, enemyY);
            Canvas.SetLeft(enemy, enemyX);
        }

        public char TouchingBorder()
        {
            //right hand border
            if (enemyX + enemy.Width > maincanvas.Width)
            {
                movements++;
                return 'r';
            }
            //left hand border
            if (enemyX < 0)
            {
                movements++;
                return 'l';
            }
            return ' ';
        }

        public bool TouchingPlayer(double playerX, double playerY, Image player)
        {
            //splitting up into x and y as it is easier for me
            bool withinX = false;
            if ((playerX - enemy.Width <= enemyX) && (enemyX <= playerX + player.Width))
            {
                withinX = true;
            }

            bool withinY = false;
            if ((playerY-enemy.Height <= enemyY) && (enemyY <= playerY+player.Height))
            {
                withinY = true;
            }

            if (withinY && withinX)
            {
                return true;
            }
            return false;
        }

        public void AddRow()
        {
            row++;

            if (movements % 2 == 0) forwards = true;
            else forwards = false;
        }

        public virtual void AddImageSource()
        {
            enemy.Source = new BitmapImage(new Uri(errorImage));
        }

        public void Initialise()
        {
            enemy = new Image();
            AddImageSource();
            enemy.Height = 30;
            enemy.Width = 30;

            enemiesPerRow = (int)Math.Floor((maincanvas.Width - borderSeperation * 2) / (enemy.Width * spriteSeperation));
            row = enemyNumber / enemiesPerRow;
            rowPos = enemyNumber % (enemiesPerRow);
            if (rowPos != 0)
            {
                if ((rowPos * (enemy.Width * spriteSeperation) + borderSeperation) > maincanvas.Width - borderSeperation)
                {
                    row++;
                    enemyX = borderSeperation;
                }
                else enemyX = (borderSeperation + (rowPos * (enemy.Width * spriteSeperation) + borderSeperation)) % maincanvas.Width - borderSeperation;
            }
            else enemyX = borderSeperation;
            enemyY = (maincanvas.Height - enemy.Height - 30) - (enemy.Height * spriteSeperation) * row;

            maincanvas.Children.Add(enemy);

            Canvas.SetBottom(enemy, enemyY);
            Canvas.SetLeft(enemy, enemyX);
        }

        public bool TouchingLaser(double laserX, double laserY)
        {
            if (((enemyX - 2) < laserX && laserX < enemyX + enemy.Width) && (enemyY-14<=laserY && laserY<=(enemyY+enemy.Height-14)))// 14 is laser height ans 2 is laser width midpoint
            {
                return true;
            }
            return false;
        }
    }
}