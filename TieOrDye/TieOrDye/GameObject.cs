using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*
TieOrDye Game Object Class
*/

namespace TieOrDye
{
    class GameObject
    {
        //Game Object attributes
        Texture2D tex; //Texture
        Vector2 pos; //Position

        //Properties for game object
        public float X
        {
            get { return pos.X; }
            set { pos.X = value; }
        }
        public float Y
        {
            get { return pos.Y; }
            set { pos.Y = value; }
        }
        

       //Constructor for GameObject
        public GameObject(Texture2D t2, int x, int y)
        {
            tex = t2;
            pos.X = x;
            pos.Y = y;
        }

        public void Draw(SpriteBatch sb)
        {
            /*
            sb.Draw(tex, pos, Color.White);
            */
        }
    }
}
