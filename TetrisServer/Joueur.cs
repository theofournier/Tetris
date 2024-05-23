using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisServer
{
    public class Joueur
    {
        int id;
        string nom;
        int penalites;
        bool etat;

        public Joueur(string nom,int id)
        {
            this.nom = nom;
            this.id = id;
            this.penalites = 0;
            this.etat = true;
        }

        public string Nom { get => nom; set => nom = value; }
        public int Id { get => id; set => id = value; }
        public int Penalites { get => penalites; set => penalites = value; }
        public bool Etat { get => etat; set => etat = value; }
    }
}
