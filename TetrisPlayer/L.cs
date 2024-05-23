using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisPlayer
{
    public class L : Brique
    {
        public L()
        {
            this.nom = "L";

            this.forme = new int[this.dimension, this.dimension];
            for (int i = 0; i < this.dimension; i++)
            {
                for (int j = 0; j < this.dimension; j++)
                {
                    if (j == 0 && i<this.dimension-1)
                        this.forme[i, j] = 4;                    
                    else
                        this.forme[i, j] = 0;
                }
            }
            this.forme[0, 1] = 4;
        }
    }
}
