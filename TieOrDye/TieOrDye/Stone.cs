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
        public Stone(Texture2D t2, int x, int y, int radius) : base(t2, x, y)
        {

        }
    }
}
