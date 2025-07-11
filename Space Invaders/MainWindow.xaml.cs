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

        private string playerShipPath = "C:\\School\\Projects\\Space Invaders\\Player Ship.png";
        private string laserPath = "C:\\School\\Projects\\Space Invaders\\laser.png";

        private int playerShipX = 0;
        private int playerShipY = 0;
        private double speed = 300;

        HashSet<Key> keysPressed = new HashSet<Key>();
        DispatcherTimer gameTimer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            SetupCanvas();
            this.KeyDown += Key_Down;
            this.KeyUp += Key_Up;

            CompositionTarget.Rendering += GameLoop;
        }

        DateTime lastUpdate = DateTime.Now;
        private void GameLoop(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            double deltaTime = (now - lastUpdate).TotalSeconds;
            lastUpdate = now;

            if ((keysPressed.Contains(Key.W) || keysPressed.Contains(Key.Up)) && playerShipY + (speed * deltaTime) + playerShip.Height < 600)
            {
                Canvas.SetBottom(playerShip, playerShipY += (int)(speed * deltaTime));
            }
            else if ((keysPressed.Contains(Key.S) || keysPressed.Contains(Key.Down)) && playerShipY - (speed*deltaTime) > 0)
            {
                Canvas.SetBottom(playerShip,  playerShipY -= (int)(speed * deltaTime));
            }
            if ((keysPressed.Contains(Key.A) || keysPressed.Contains(Key.Left)) && playerShipX - (speed * deltaTime) > 0)
            {
                Canvas.SetLeft(playerShip, playerShipX -= (int)(speed * deltaTime));
            }
            else if ((keysPressed.Contains(Key.D) || keysPressed.Contains(Key.Right)) && playerShipX + (speed * deltaTime) + playerShip.Width < 600)
            {
                Canvas.SetLeft(playerShip, playerShipX += (int)(speed * deltaTime));
            }
            if (keysPressed.Contains(Key.Space))
            {
                FireLaser();
            }
        }

        private void FireLaser()
        {

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