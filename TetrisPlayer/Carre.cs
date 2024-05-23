using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisPlayer
{
    public class Carre : Brique
    {
        public Carre()
        {
            this.nom = "Carré";

            this.forme = new int[this.dimension, this.dimension];
            for (int i = 0; i < this.dimension; i++)
            {
                for (int j = 0; j < this.dimension; j++)
                {
                    if ((i == 0 || i == 1) && (j == 0 || j == 1))
                        this.forme[i, j] = 1;
                    else
                        this.forme[i, j] = 0;
                }

            }
        }
        
    }
}
