using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Space_Invaders
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Canvas maincanvas;
        private Image playerShip;
        private Image laser;

        private string playerShipPath = "C:\\School\\Projects\\Space Invaders\\Images\\Player Ship.png";
        private string laserPath = "C:\\School\\Projects\\Space Invaders\\Images\\laser.png";

        private int playerShipX = 0;
        private int playerShipY = 0;
        private double speed = 300;

        private int laserDelay;
        private int laserDelayAmount = 15;
        private int renderDelay;
        private int renderDelayAmount = 30;

        HashSet<Key> keysPressed = new HashSet<Key>();

        List<Laser> activeLasers = new List<Laser>();
        List<Enemy1> enemy1s = new List<Enemy1>();


        public MainWindow()
        {
            InitializeComponent();
            SetupCanvas();
            InitialiseEnemys();
            this.KeyDown += Key_Down;
            this.KeyUp += Key_Up;

            //similar to vsync so it calls every time monitor refreshes
            CompositionTarget.Rendering += GameLoop;
        }

        DateTime lastUpdate = DateTime.Now;
        private void GameLoop(object sender, EventArgs e)
        {
            //use delta time so it doesnt matter what monitor refresh rate is
            var now = DateTime.Now;
            double deltaTime = (now - lastUpdate).TotalSeconds;
            lastUpdate = now;
            if (laserDelay> 0) laserDelay-= 1;
            if (renderDelay > 0) renderDelay -= 1;

            if ((keysPressed.Contains(Key.W) || keysPressed.Contains(Key.Up)) && playerShipY + (speed * deltaTime) + playerShip.Height < 600)
            {
                Canvas.SetBottom(playerShip, playerShipY += (int)(speed * deltaTime));
            }
            else if ((keysPressed.Contains(Key.S) || keysPressed.Contains(Key.Down)) && playerShipY - (speed * deltaTime) > 0)
            {
                Canvas.SetBottom(playerShip, playerShipY -= (int)(speed * deltaTime));
            }
            if ((keysPressed.Contains(Key.A) || keysPressed.Contains(Key.Left)) && playerShipX - (speed * deltaTime) > 0)
            {
                Canvas.SetLeft(playerShip, playerShipX -= (int)(speed * deltaTime));
            }
            else if ((keysPressed.Contains(Key.D) || keysPressed.Contains(Key.Right)) && playerShipX + (speed * deltaTime) + playerShip.Width < 600)
            {
                Canvas.SetLeft(playerShip, playerShipX += (int)(speed * deltaTime));
            }
            if (laserDelay == 0 && keysPressed.Contains(Key.Space))
            {
                FireLaser();
                laserDelay = laserDelayAmount;
            }

            RemoveLaser(); // remove it when laser goes off screen

            DetectHit();
            /*
            if (renderDelay == 0)
            {
                DrawEnemies();
                renderDelay = renderDelayAmount;
            }
            */
        }

        private void DrawEnemies()
        {
            for (int i = 0; i < enemy1s.Count; i++)
            {
                enemy1s[i].Redraw();
            }
        }

        private void RemoveLaser()
        {
            for (int i = 0; i < activeLasers.Count; i++)
            {
                if (!activeLasers[i].IsActive())
                {
                    //if it goes off the screen remove it to avoid clutter
                    maincanvas.Children.Remove(activeLasers[i].laser);
                    activeLasers.Remove(activeLasers[i]);
                }
            }
        }

        private void DetectHit()
        {
            for (int i = enemy1s.Count - 1; i >= 0; i--)
            {
                for (int j = activeLasers.Count - 1; j >= 0; j--)
                {
                    if (activeLasers[j] != null && enemy1s[i] != null && enemy1s[i].TouchingLaser(activeLasers[j].LaserX, activeLasers[j].LaserY))
                    {
                        //remove enemy once touched by laser
                        maincanvas.Children.Remove(enemy1s[i].enemy);
                        enemy1s.Remove(enemy1s[i]);
                        //remove laser once enemy has been killed so it doesnt kill anyone else
                        maincanvas.Children.Remove(activeLasers[j].laser);
                        activeLasers.Remove(activeLasers[j]);
                        //no need to check for other lasers once this enemy has been removed
                        break;
                    }
                }

            }


        }

        private void InitialiseEnemys()
        {
            for (int i = 0; i < 20; i++)
            {
                enemy1s.Add(new Enemy1(maincanvas));
            }
        }

        private void FireLaser()
        {
            laser = new Image();
            laser.Source = new BitmapImage(new Uri(laserPath));
            maincanvas.Children.Add(laser);
            int laserY = playerShipY + 55;
            int laserX = playerShipX + 27;
            Canvas.SetBottom(laser, laserY);
            Canvas.SetLeft(laser, laserX);

            activeLasers.Add(new Laser(laserY, laserX, laser));
        }

        private void Key_Down(object sender, KeyEventArgs e)
        {
            keysPressed.Add(e.Key);
        }

        private void Key_Up(object sender, KeyEventArgs e)
        {
            keysPressed.Remove(e.Key);
        }

        private void SetupCanvas()
        {
            this.SizeToContent = SizeToContent.WidthAndHeight;

            maincanvas = new Canvas();
            this.Content = maincanvas;
            this.Height = 600;
            this.Width = 600;

            maincanvas.Height = 600;
            maincanvas.Width = 600;
            maincanvas.Background = Brushes.Black;

            playerShip = new Image();
            playerShip.Source = new BitmapImage(new Uri(playerShipPath));
            playerShip.Height = 57;
            playerShip.Width = 57;

            Canvas.SetLeft(playerShip, 0);
            Canvas.SetBottom(playerShip, 0);

            maincanvas.Children.Add(playerShip);
        }

    }
}