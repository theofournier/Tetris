using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisPlayer
{
    public class Point : Brique
    {
        public Point()
        {
            this.nom = "Point";

            this.forme = new int[this.dimension, this.dimension];
            for (int i = 0; i < this.dimension; i++)
            {
                for (int j = 0; j < this.dimension; j++)
                    this.forme[i, j] = 0;

            }
            this.forme[0, 0] = 2;
        }

    }
}
