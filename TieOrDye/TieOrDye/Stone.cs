using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*
TieOrDye Stone Class
*/

namespace TieOrDye
{
    class Stone : GameObject
    {
        //attributes
        Texture2D stoneTex;
        int xPos;
        int yPos;
        int stoneWidth; //Diameter of stone

        public Stone(Texture2D t2, int x, int y, int sw) : base(t2, x, y)
        {
            stoneTex = t2; //Should always create stones with gray texture by default
            xPos = x;
            yPos = y;
            stoneWidth = sw; 
        }

        public int XPos
        {
            get { return xPos; }
            set { xPos = value; }
        }

        public int YPos
        {
            get { return yPos; }
            set { yPos = value; }
        }
    }
}
