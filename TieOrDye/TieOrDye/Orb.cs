﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
TieOrDye Orb Class
*/

namespace TieOrDye
{
    class Orb : Player
    {
        //attributes
        bool color;

        public Orb(bool pl) : base(pl)
        {

            color = pl;
        }
    }
}
