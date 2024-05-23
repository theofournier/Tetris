using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisPlayer
{
    public abstract class Brique
    {
        protected string nom;
        protected int[,] forme;     // Contient les valeurs d'une brique
        protected int dimension = 4;

        public int[,] Forme { get => forme; set => forme = value; }
        public int Dimension { get => dimension; set => dimension = value; }
    }
}
