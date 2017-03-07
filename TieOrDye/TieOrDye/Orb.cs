using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


/*
TieOrDye Orb Class
*/

namespace TieOrDye
{
    class Orb : GameObject
    {
        //attributes
        bool color;

        public Orb(Texture2D t2, int x, int y, int radius) : base(t2, x, y)
        {

        }
    }
}
