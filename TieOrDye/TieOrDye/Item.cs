using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TieOrDye
{
    class Item : GameObject
    {
        Game1 gm1 = new Game1();

        Texture2D itemTex;
        int xPos;
        int yPos;
        int type;
        Circle itemCirc;
        Random gen = new Random();
        Circle orbC;
        Circle stoneC;
        KeyboardState kState;
        KeyboardState prevState;
        int counter = 0;

        public Item(Texture2D tex, int x, int y, int rad, int t) : base(tex, x, y)
        {
            itemTex = tex;
            xPos = x;
            yPos = y;
            itemCirc = new Circle(x, y, rad);
            type = t;
        }

        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        public Circle ItemCirc
        {
            get { return itemCirc; }
            set { itemCirc = value; }
        }

        public void ItemCheckInfo(Circle orb, Circle stone)
        {
            orbC = orb;
            stoneC = stone;
        }

        public Circle OrbC { get { return orbC; } set { orbC = value; } }
        public Circle StoneC { get { return stoneC; } set { stoneC = value; } }

        public void DrawItem(SpriteBatch sb)
        {
            if (orbC.Intersects(stoneC))
            {
                sb.Draw(itemTex, new Rectangle(xPos, yPos, itemCirc.Radius, itemCirc.Radius), Color.Purple);
            }
        }

        public void ItemGet(Player player, int type, List<Orb> orbList, Animation anim, Texture2D orbTex)
        {
            switch (type)
            {
                case 1:
                    for (int x = 0; x < orbList.Count; x++)
                    {
                        orbList[x].OrbSpeed += 6;
                    }
                    break;
                case 2:
                    counter = 0;
                    for (int x = 0; x < orbList.Count; x++)
                    {
                        if (orbList[x].OrbSpeed > 3)
                        {
                            orbList[x].OrbSpeed -= 6;
                        }
                        else
                        {
                            orbList[x].OrbSpeed -= orbList[x].OrbSpeed * 6;
                        }
                    }
                    break;
                case 3:
                    counter++;
                    if(counter % 20 == 0)
                    {
                        Orb oI1 = new Orb(orbTex, 0, 0, player, anim, 10, 1);
                        orbList.Add(oI1);
                        Orb oI2 = new Orb(orbTex, 0, 0, player, anim, 10, 1);
                        orbList.Add(oI2);
                    }
                    

                    break;
                default:
                    break;
            }

        }

        public void changeItemLoc(Stone stone, List<Stone> stonesList)
        {
            int num = gen.Next(25);
            if (stone.ItemSpawn == true)
            {
                stone.ItemSpawn = false;
                stonesList[num].ItemSpawn = true;
            }
            else if (stone.ItemSpawn2 == true)
            {
                stone.ItemSpawn2 = false;
                stonesList[num].ItemSpawn2 = true;
            }
        }
    }
}