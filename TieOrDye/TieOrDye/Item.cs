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
    // child of GameObject class
    class Item : GameObject
    {
        // attributes
        Texture2D itemTex;

        Circle itemCirc;
        Circle orbC;
        Circle stoneC;

        int counter = 0;
        int type;

        Random gen = new Random();

        // parameterized constructor
        public Item(Texture2D tex, int x, int y, int rad, int t) : base(tex, x, y)
        {
            itemTex = tex;
            itemCirc = new Circle(x, y, rad);
            type = t;
        }

        // property for type
        public int Type
        {
            get { return type; }
            //set { type = value; }
        }

        // proprty for the item circle
        public Circle ItemCirc
        {
            get { return itemCirc; }
            set { itemCirc = value; }
        }

        // takes circles sfrom orb and stone to be used to determine item drawing
        public void ItemCheckInfo(Circle orb, Circle stone)
        {
            orbC = orb;
            stoneC = stone;
        }

        // properties for orb circle and stone circle
        public Circle OrbC { get { return orbC; } set { orbC = value; } }
        public Circle StoneC { get { return stoneC; } set { stoneC = value; } }

        // class specific draw method
        public void DrawItem(SpriteBatch sb)
        {
            if (orbC.Intersects(stoneC))
            {
                sb.Draw(itemTex, new Rectangle(itemCirc.X, itemCirc.Y, itemCirc.Radius, itemCirc.Radius), Color.White);
                
            }
        }

        // activates ability for certain amount of time, then reverts it back to normal
        public void ItemGet(Player player, int type, List<Orb> orbList, Animation anim, Texture2D orbTex)
        {
            switch (type)
            {
                case 1:  // for speed shot - increases orb speed
                    for (int x = 0; x < orbList.Count; x++)
                    {
                        orbList[x].OrbSpeed += 6;
                    }
                    break;
                case 2:  // changes abilities back to normal
                    counter = 0;
                    for (int x = 0; x < orbList.Count; x++)
                    {
                        if (orbList[x].OrbSpeed > 3)
                            orbList[x].OrbSpeed -= 6;
                        else
                            orbList[x].OrbSpeed -= orbList[x].OrbSpeed * 6;
                    }
                    break;
                case 3:  // for inverter - auto shoots orbs until time runs out, then changes orb direction and speed
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

        // randomly changes which stone item is in when called
        public void changeItemLoc(List<Stone> stonesList, int count)
        {
            int num = gen.Next(0, stonesList.Count);
            if (count == 1)
            {
                if (stonesList[num].Inverter == true)
                    changeItemLoc(stonesList, count);
                else
                    stonesList[num].RapidFire = true;
            }
            if (count == 2)
            {
                if (stonesList[num].RapidFire == true)
                    changeItemLoc(stonesList, count);
                else
                    stonesList[num].Inverter = true;
            }
           
        }
    }
}