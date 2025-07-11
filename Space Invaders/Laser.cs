using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Space_Invaders
{
    public class Laser
    {
        private int laserY;
        private int laserX;
        private bool active = true;
        public Image laser;
        private int laserspeed = 5;

        public int LaserY
        {
            get => laserY;
        }

        public int LaserX
        {
            get => laserX;
        }

        public Laser(int Y, int X, Image laserImage)
        {
            laserY = Y;
            laserX = X;
            laser = laserImage;
        }

        public bool IsActive()
        {
            if (laserY > 584)
            {
                active = false;
            }
            if (active)
            {
                Canvas.SetBottom(laser, laserY += laserspeed);
            }
            return active;
        }

    }
}