using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisPlayer
{
    public class Joueur
    {
        int id;
        string nom;
        int penalites;
        bool etat;

        public string Nom { get => nom; set => nom = value; }
        public int Penalites { get => penalites; set => penalites = value; }
        public int Id { get => id; set => id = value; }
        public bool Etat { get => etat; set => etat = value; }

        public Joueur(string nom,int id)
        {
            this.nom = nom;
            this.penalites = 0;
            this.id = id;
            this.etat = true;
        }
    }
}
