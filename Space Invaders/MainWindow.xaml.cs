using System;
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
        private bool gameOver = false;
        private Image gameOverScreen = new Image();
        private string gameOverScreenPath = "C:\\School\\Projects\\Space Invaders\\Images\\gameover.png";

        private Button restart = new Button();

        private Button nextLevelButton = new Button();
        private Image nextLevelScreen = new Image();
        private string nextLevelScreenPath = "C:\\School\\Projects\\Space Invaders\\Images\\levelup.png";

        private TextBox scoreDisplay = new TextBox();
        private TextBox levelDisplay = new TextBox();
        private int levelNumber = 1;

        private Canvas maincanvas;
        private Image playerShip;
        private Image laser;

        private string playerShipPath = "C:\\School\\Projects\\Space Invaders\\Images\\Player Ship.png";
        private string laserPath = "C:\\School\\Projects\\Space Invaders\\Images\\laser.png";

        private double playerShipX = 0;
        private double playerShipY = 0;
        private double speed = 300; //speed of game

        private int laserDelay;
        private int laserDelayAmount = 1; // in frames
        private int renderDelay;
        private int renderDelayAmount = 1; // in frames

        HashSet<Key> keysPressed = new HashSet<Key>();

        List<Laser> activeLasers = new List<Laser>();

        List<Enemy1> enemy1s = new List<Enemy1>();
        private int enemy1Count = 30;

        List<Enemy2> enemy2s = new List<Enemy2>();
        private int enemy2Count = 30;

        List<Enemy3> enemy3s = new List<Enemy3>();
        private int enemy3Count = 15;

        private int score;

        public static int TitleHeight() => 30;


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
            levelDisplay.Text = "Level: " + levelNumber;

            //use delta time so it doesnt matter what monitor refresh rate is
            var now = DateTime.Now;
            double deltaTime = (now - lastUpdate).TotalSeconds;
            lastUpdate = now;
            if (laserDelay> 0) laserDelay-= 1;
            if (renderDelay > 0) renderDelay -= 1;

            if ((keysPressed.Contains(Key.W) || keysPressed.Contains(Key.Up)) && playerShipY + (speed * deltaTime) + playerShip.Height < 600)
            {
                Canvas.SetBottom(playerShip, playerShipY += (speed * deltaTime));
            }
            else if ((keysPressed.Contains(Key.S) || keysPressed.Contains(Key.Down)) && playerShipY - (speed * deltaTime) > 0)
            {
                Canvas.SetBottom(playerShip, playerShipY -= (speed * deltaTime));
            }
            if ((keysPressed.Contains(Key.A) || keysPressed.Contains(Key.Left)) && playerShipX - (speed * deltaTime) > 0)
            {
                Canvas.SetLeft(playerShip, playerShipX -= (speed * deltaTime));
            }
            else if ((keysPressed.Contains(Key.D) || keysPressed.Contains(Key.Right)) && playerShipX + (speed * deltaTime) + playerShip.Width < 600)
            {
                Canvas.SetLeft(playerShip, playerShipX += (speed * deltaTime));
            }
            if (laserDelay == 0 && keysPressed.Contains(Key.Space))
            {
                FireLaser();
                laserDelay = laserDelayAmount;
            }

            RemoveLaser(); // remove it when laser goes off screen

            CheckIfAtBottom();

            DetectHit();
            
            if (renderDelay == 0)
            {
                DrawEnemies(deltaTime);
                renderDelay = renderDelayAmount;
            }

            CheckGameState();

        }
        private void CheckIfAtBottom()
        {
            for (int i = enemy1s.Count - 1; i >= 0; i--)
            {
                if (enemy1s[i].Row == enemy1s[i].TotalRows-1 && (enemy1s[i].enemyX + enemy1s[i].enemy.Height < 0 || enemy1s[i].enemyX>maincanvas.Width))
                {
                    score -= enemy1s[i].AddPoints(levelNumber);
                    scoreDisplay.Text = "Score: " + score;
                    maincanvas.Children.Remove(enemy1s[i].enemy);
                    enemy1s.Remove(enemy1s[i]);
                }
            }

            for (int i = enemy2s.Count - 1; i >= 0; i--)
            {
                if (enemy2s[i].Row == enemy2s[i].TotalRows - 1 && (enemy2s[i].enemyX + enemy2s[i].enemy.Height < 0 || enemy2s[i].enemyX > maincanvas.Width))
                {
                    score -= enemy2s[i].AddPoints(levelNumber);
                    scoreDisplay.Text = "Score: " + score;
                    maincanvas.Children.Remove(enemy2s[i].enemy);
                    enemy2s.Remove(enemy2s[i]);
                }
            }

            for (int i = enemy3s.Count - 1; i >= 0; i--)
            {
                if (enemy3s[i].Row == enemy3s[i].TotalRows - 1 && (enemy3s[i].enemyX + enemy3s[i].enemy.Height < 0 || enemy3s[i].enemyX > maincanvas.Width))
                {
                    score -= enemy3s[i].AddPoints(levelNumber);
                    scoreDisplay.Text = "Score: " + score;
                    maincanvas.Children.Remove(enemy3s[i].enemy);
                    enemy3s.Remove(enemy3s[i]);
                }
            }
        }

        private void CheckGameState()
        {
            //check if any ships are touching the player
            for (int i = enemy1s.Count - 1; i >= 0; i--)
            {
                if (enemy1s[i].TouchingPlayer(playerShipX, playerShipY, playerShip))
                {
                    gameOver = true;
                }
            }

            for (int i = enemy2s.Count - 1; i >= 0; i--)
            {
                if (enemy2s[i].TouchingPlayer(playerShipX, playerShipY, playerShip))
                {
                    gameOver = true;
                }
            }

            for (int i = enemy3s.Count - 1; i >= 0; i--)
            {
                if (enemy3s[i].TouchingPlayer(playerShipX, playerShipY, playerShip))
                {
                    gameOver = true;
                }
            }

            //check if all enemies are dead
            if(enemy1s.Count == 0 && enemy2s.Count == 0 && enemy3s.Count == 0)
            {
                NextLevel();
            }

            if(gameOver)
            {
                GameOver();
            }
        }

        private void NextLevel()
        {
            CompositionTarget.Rendering -= GameLoop;

            ClearScreen();

            nextLevelScreen.Source = new BitmapImage(new Uri(nextLevelScreenPath));
            nextLevelScreen.Height = 300;
            nextLevelScreen.Width = 600;
            maincanvas.Children.Add(nextLevelScreen);

            nextLevelButton.Content = "Next Level!";
            nextLevelButton.Height = 50;
            nextLevelButton.Width = 200;
            Canvas.SetLeft(nextLevelButton, (maincanvas.Width / 2) - (nextLevelButton.Width / 2));
            Canvas.SetTop(nextLevelButton, (maincanvas.Height / 2) - (nextLevelButton.Height / 2));

            maincanvas.Children.Add(nextLevelButton);

            nextLevelButton.Click += NextLevelButton_Click;
        }
        private void NextLevelButton_Click(object sender, RoutedEventArgs e)
        {
            InitialiseNextLevel();
        }
        private void InitialiseNextLevel()
        {
            nextLevelButton.Click -= NextLevelButton_Click;

            levelNumber++;

            ClearScreen();

            maincanvas.Children.Remove(nextLevelScreen);
            maincanvas.Children.Remove(nextLevelButton);

            Enemies.enemyCount = 0;
            playerShipX = ((maincanvas.Width / 2) - (playerShip.Width / 2));
            playerShipY = 0;

            InitialiseEnemys();
            CompositionTarget.Rendering += GameLoop;
        }

        private void GameOver()
        {
            CompositionTarget.Rendering -= GameLoop;

            ClearScreen();

            gameOverScreen.Source = new BitmapImage(new Uri(gameOverScreenPath));
            gameOverScreen.Height = 300;
            gameOverScreen.Width = 600;
            maincanvas.Children.Add(gameOverScreen);
            Canvas.SetTop(gameOverScreen, (maincanvas.Height/2) - (gameOverScreen.Height/2));

            restart.Content = "Play Again?";
            restart.Height = 50;
            restart.Width = 200;
            Canvas.SetLeft(restart, (maincanvas.Width/2) - (restart.Width / 2));
            Canvas.SetTop(restart, 450);

            maincanvas.Children.Add(restart);

            restart.Click += RestartButton_Click;
            
        }
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            Restart();
        }
        private void Restart()
        {
            restart.Click -= RestartButton_Click;

            maincanvas.Children.Remove(gameOverScreen);
            maincanvas.Children.Remove(restart);

            ClearScreen();

            score = 0;
            levelNumber = 1;

            Enemies.enemyCount = 0;
            playerShipX = ((maincanvas.Width / 2) - (playerShip.Width / 2));
            playerShipY = 0;

            gameOver = false;

            InitialiseEnemys();
            CompositionTarget.Rendering += GameLoop;
        }

        private void ClearScreen()
        {
            for (int i = enemy1s.Count - 1; i >= 0; i--)
            {
                maincanvas.Children.Remove(enemy1s[i].enemy);
                enemy1s.Remove(enemy1s[i]);
            }

            for (int i = enemy2s.Count - 1; i >= 0; i--)
            {
                maincanvas.Children.Remove(enemy2s[i].enemy);
                enemy2s.Remove(enemy2s[i]);
            }

            for (int i = enemy3s.Count - 1; i >= 0; i--)
            {
                maincanvas.Children.Remove(enemy3s[i].enemy);
                enemy3s.Remove(enemy3s[i]);
            }

            for (int j = activeLasers.Count - 1; j >= 0; j--)
            {
                maincanvas.Children.Remove(activeLasers[j].laser);
                activeLasers.Remove(activeLasers[j]);
            }
                maincanvas.Children.Remove(playerShip);
        }

        private void DrawEnemies(double deltaTime)
        {
            int enemiesRemaining = enemy1s.Count + enemy2s.Count + enemy3s.Count;
            bool addRow = false;

            for (int i = enemy1s.Count - 1; i >= 0; i--)
            {
                enemy1s[i].Redraw(enemiesRemaining, levelNumber, deltaTime);
            }
            for (int i = enemy2s.Count - 1; i >= 0; i--)
            {
                enemy2s[i].Redraw(enemiesRemaining, levelNumber, deltaTime);
            }
            for (int i = enemy3s.Count - 1; i >= 0; i--)
            {
                enemy3s[i].Redraw(enemiesRemaining, levelNumber, deltaTime);
            }

            for (int i = enemy1s.Count - 1; i >= 0; i--)
            {
                char border = enemy1s[i].TouchingBorder();
                if (border != ' ')
                {
                    UndoLastMovement(border);
                    IncrementRow();
                    return;
                }
            }
            for (int i = enemy2s.Count - 1; i >= 0; i--)
            {
                char border = enemy2s[i].TouchingBorder();
                if (border != ' ')
                {
                    UndoLastMovement(border);
                    IncrementRow();
                    return;
                }
            }
            for (int i = enemy3s.Count - 1; i >= 0; i--)
            {
                char border = enemy3s[i].TouchingBorder();
                if (border != ' ')
                {
                    UndoLastMovement(border);
                    IncrementRow();                  
                    return;
                }
            }
        }
        private void UndoLastMovement(char border)
        {
            for (int i = enemy1s.Count - 1; i >= 0; i--)
            {
                if (border == 'l') enemy1s[i].enemyX += enemy1s[i].MoveBy;
                else enemy1s[i].enemyX -= enemy1s[i].MoveBy;
            }
            for (int i = enemy2s.Count - 1; i >= 0; i--)
            {
                if (border == 'l') enemy2s[i].enemyX += enemy2s[i].MoveBy;
                else enemy2s[i].enemyX -= enemy2s[i].MoveBy;
            }
            for (int i = enemy3s.Count - 1; i >= 0; i--)
            {
                if (border == 'l') enemy3s[i].enemyX += enemy3s[i].MoveBy;
                else enemy3s[i].enemyX -= enemy3s[i].MoveBy;
            }
        }

        private void IncrementRow()
        {
            //enemy 1s
            for (int i = enemy1s.Count - 1; i >= 0; i--)
            {
                enemy1s[i].AddRow();
            }

            //enemy 2s
            for (int i = enemy2s.Count - 1; i >= 0; i--)
            {
                enemy2s[i].AddRow();
            }

            //enemy 3s
            for (int i = enemy3s.Count - 1; i >= 0; i--)
            {
                enemy3s[i].AddRow();
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
                    if (enemy1s[i].TouchingLaser(activeLasers[j].LaserX, activeLasers[j].LaserY))
                    {
                        score += enemy1s[i].AddPoints(levelNumber);
                        scoreDisplay.Text = "Score: " + score;
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

            for (int i = enemy2s.Count - 1; i >= 0; i--)
            {
                for (int j = activeLasers.Count - 1; j >= 0; j--)
                {
                    if (enemy2s[i].TouchingLaser(activeLasers[j].LaserX, activeLasers[j].LaserY))
                    {
                        score += enemy2s[i].AddPoints(levelNumber);
                        scoreDisplay.Text = "Score: " + score;
                        //remove enemy once touched by laser
                        maincanvas.Children.Remove(enemy2s[i].enemy);
                        enemy2s.Remove(enemy2s[i]);

                        //remove laser once enemy has been killed so it doesnt kill anyone else
                        maincanvas.Children.Remove(activeLasers[j].laser);
                        activeLasers.Remove(activeLasers[j]);

                        //no need to check for other lasers once this enemy has been removed
                        break;
                    }
                }
            }

            for (int i = enemy3s.Count - 1; i >= 0; i--)
            {
                for (int j = activeLasers.Count - 1; j >= 0; j--)
                {
                    if (enemy3s[i].TouchingLaser(activeLasers[j].LaserX, activeLasers[j].LaserY))
                    {
                        score += enemy3s[i].AddPoints(levelNumber);
                        scoreDisplay.Text = "Score: " + score;
                        //remove enemy once touched by laser
                        maincanvas.Children.Remove(enemy3s[i].enemy);
                        enemy3s.Remove(enemy3s[i]);

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
            Canvas.SetLeft(playerShip, playerShipX);
            Canvas.SetBottom(playerShip, playerShipY);

            maincanvas.Children.Add(playerShip);

            //enemy 3s
            for (int i = 0; i < enemy3Count; i++)
            {
                enemy3s.Add(new Enemy3(maincanvas));
            }

            //enemy 2s
            for (int i = 0; i < enemy2Count; i++)
            {
                enemy2s.Add(new Enemy2(maincanvas));
            }

            //enemy 1s
            for (int i = 0; i < enemy1Count; i++)
            {
                enemy1s.Add(new Enemy1(maincanvas));
            }
        }

        private void FireLaser()
        {
            laser = new Image();
            laser.Source = new BitmapImage(new Uri(laserPath));
            maincanvas.Children.Add(laser);
            double laserY = playerShipY + 55;
            double laserX = playerShipX + 27;
            Canvas.SetBottom(laser, laserY);
            Canvas.SetLeft(laser, laserX);

            activeLasers.Add(new Laser(laserY, laserX, laser, maincanvas));
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
            playerShipX = ((maincanvas.Width / 2) - (playerShip.Width / 2));

            scoreDisplay.Text = "Score: " + score;
            scoreDisplay.Height = 16;
            scoreDisplay.Width = maincanvas.Width;
            scoreDisplay.Background = Brushes.Black;
            scoreDisplay.BorderBrush = Brushes.Black;
            scoreDisplay.Foreground = Brushes.White;
            scoreDisplay.FontWeight = FontWeights.Bold;
            scoreDisplay.IsReadOnly = true;
            scoreDisplay.IsHitTestVisible = false;
            maincanvas.Children.Add(scoreDisplay);

            TextBox Title = new TextBox();
            Title.Text = "SPACE INVADERS";
            Title.Height = TitleHeight();
            Title.Width = 200;
            Title.Background = Brushes.Black;
            Title.BorderBrush = Brushes.Black;
            Title.Foreground = Brushes.White;
            Title.FontSize = 22.5;
            Title.FontWeight = FontWeights.ExtraBold;
            Title.IsReadOnly = true;
            Title.IsHitTestVisible = false;
            Canvas.SetLeft(Title, (maincanvas.Width / 2) - (Title.Width / 2));
            maincanvas.Children.Add(Title);

            levelDisplay.Text = "Level: " + levelNumber;
            levelDisplay.Height = 18;
            levelDisplay.Width = 60;
            levelDisplay.Background = Brushes.Black;
            levelDisplay.BorderBrush = Brushes.Black;
            levelDisplay.Foreground = Brushes.White;
            levelDisplay.FontWeight = FontWeights.Bold;
            levelDisplay.IsReadOnly = true;
            levelDisplay.IsHitTestVisible = false;
            Canvas.SetLeft(levelDisplay, maincanvas.Width-levelDisplay.Width);
            maincanvas.Children.Add(levelDisplay);
        }

    }
}