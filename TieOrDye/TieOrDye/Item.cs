using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TieOrDye
{
    class Item : GameObject
    {
        

        Texture2D itemTex;
        int xPos;
        int yPos;
        int type;
        Circle itemCirc;
        Random gen = new Random();
        Circle orbC;
        Circle stoneC;

        public Item(Texture2D tex, int x, int y, int rad, int t):base(tex, x, y)
        {
            itemTex = tex;
            xPos = x;
            yPos = y;
            itemCirc = new Circle(x, y, rad);
            type = t;
        }

        public Circle ItemCirc
        {
            get { return itemCirc; }
        }

        public void ItemCheckInfo(Circle orb, Circle stone)
        {
            orbC = orb;
            stoneC = stone;
        }

        public void DrawItem(SpriteBatch sb)
        {
            if (orbC.Intersects(stoneC))
            {
                sb.Draw(itemTex, new Rectangle(orbC.X, orbC.Y, itemCirc.Radius, itemCirc.Radius), Color.White);
            }
        }
        public void ItemGet(Player player, int type, List<Orb> orbList, Animation anim)
        {
            
            switch (type)
            {
                case 1:
                    for(int x = 0; x < orbList.Count; x++)
                    {
                        orbList[x].OrbSpeed += 10;
                    }
                    break;
                case 2:
                    for (int x = 0; x < orbList.Count; x++)
                    {
                        orbList[x].OrbSpeed -= 10;
                    }
                    break;
                case 3:

                    break;
                default:
                    break;
            }
            
        }
    }
}
