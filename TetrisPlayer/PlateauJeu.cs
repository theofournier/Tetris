using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace TetrisPlayer
{
    // Contient la matrice et les méthodes de jeu
    public class PlateauJeu
    {
        int nombreLigne;
        int nombreColonne;
        int speed;
        int nombreForme = 5;  // correspond aux nombres de formes possible + 1 en comptant la case vide
        int[,] plateau;
        int penalites;
        int ligneFinies;
        bool etat;
        List<Joueur> adversaires;
        int xi;     //Coordonnée initiale de la cellule [0,0] de la brique en cours dans le plateau
        int yi;
        int xf;     //Coordonnée finale de la cellule [0,0] de la brique en cours dans le plateau
        int yf;
        Brique currentBrique;   // Forme de la brique en cours
        RelationServeur relationServeur;
        ConsoleKey keyH;
        ConsoleKey keyD;
        ConsoleKey keyB;
        ConsoleKey keyG;

        public List<Joueur> Adversaires { get => adversaires; set => adversaires = value; }
        public int LigneFinies { get => ligneFinies; set => ligneFinies = value; }
        public Brique CurrentBrique { get => currentBrique; set => currentBrique = value; }
        public int Xi { get => xi; set => xi = value; }
        public int Yi { get => yi; set => yi = value; }
        public int Xf { get => xf; set => xf = value; }
        public int Yf { get => yf; set => yf = value; }
        public int[,] Plateau { get => plateau; set => plateau = value; }
        public bool Etat { get => etat; set => etat = value; }
        public int Speed { get => speed; set => speed = value; }
        public RelationServeur RelationServeur { get => relationServeur; set => relationServeur = value; }
        public int NombreLigne { get => nombreLigne; set => nombreLigne = value; }
        public int NombreColonne { get => nombreColonne; set => nombreColonne = value; }
        public int Speed1 { get => speed; set => speed = value; }

        public PlateauJeu()
        {
            this.nombreLigne = 0;
            this.nombreColonne = 0;
            this.speed = 0;
            this.penalites = 0;
            this.adversaires = new List<Joueur>();
            this.etat = true;
            this.xi = 0;
            this.xf = 0;
            this.yi = this.nombreColonne / 2;
            this.yf = this.nombreColonne / 2;
            this.keyH = ConsoleKey.UpArrow;
            this.keyD = ConsoleKey.RightArrow;
            this.keyB = ConsoleKey.DownArrow;
            this.keyG = ConsoleKey.LeftArrow;
        }

        public PlateauJeu(ConsoleKey keyH,ConsoleKey keyD,ConsoleKey keyB,ConsoleKey keyG)
        {
            this.nombreLigne = 0;
            this.nombreColonne = 0;
            this.speed = 0;
            this.penalites = 0;
            this.adversaires = new List<Joueur>();
            this.etat = true;
            this.xi = 0;
            this.xf = 0;
            this.yi = this.nombreColonne / 2;
            this.yf = this.nombreColonne / 2;
            this.keyH = keyH;
            this.keyD = keyD;
            this.keyB = keyB;
            this.keyG = keyG;
        }


        // Instancie le plateau et initialise ces valeurs à 0
        public void InitialiserPlateau()
        {
            this.plateau = new int[nombreLigne, nombreColonne];
            for (int i = 0; i < nombreLigne; i++)
            {
                for (int j = 0; j < nombreColonne; j++)
                {
                    this.plateau[i, j] = 0;
                }
            }
        }

        // Met à 0 toutes les valeurs du tableau qui ne sont pas fixes
        // Permet de nettoyer le tableau avant de faire bouger d'un cran la brique en cours
        public void NettoyerTableau()
        {
            for (int i = 0; i < nombreLigne; i++)
            {
                for (int j = 0; j < nombreColonne; j++)
                {
                    if (this.plateau[i, j] <= this.nombreForme)
                        this.plateau[i, j] = 0;
                }
            }
        }

        // Insère dans le plateau les valeurs du tableau (différentes de 0) de la brique en cours 
        // Cette fonction est appelé après vérification si la brique peut entrer dans le plateau
        public void Insertion()
        {
            for (int i = 0; i < this.currentBrique.Dimension; i++)
            {
                for (int j = 0; j < this.currentBrique.Dimension; j++)
                {
                    if (this.currentBrique.Forme[i, j] != 0)
                    {
                        if (xf + i < this.nombreLigne && yf + j < this.nombreColonne) // Vérifie que les coordonnées sont dans le tableau
                        {
                            this.plateau[xf + i, yf + j] = this.currentBrique.Forme[i, j];
                        }
                    }
                }
            }
            xi = xf;    // Change les coordonnées intiales de la brique
            yi = yf;
        }


        // Fixe toutes cases du plateau qui n'est pas fixé
        // Cad incrémente les cases qui sont < nombreForme && != 0 de nombreForme, qui notifie qu'une case est fixé
        // Toutes cases >= nombreForme est une case fixé
        // Fonction appelé lorsque la brique en cours ne peut plus bouger
        public void Fixe()
        {
            for (int i = 0; i < this.nombreLigne; i++)
            {
                for (int j = 0; j < this.nombreColonne; j++)
                {
                    if (this.plateau[i, j] < this.nombreForme && this.plateau[i, j] != 0)
                        this.plateau[i, j] += this.nombreForme;
                }
            }

        }

        static Random rand = new Random();

        // Demande une brique au hasard à l'ordinateur local, permet de jouer en local et de demander la première brique
        public void DemandeBriqueLocal()
        {
            int b = rand.Next(0, 4);

            if (b == 0)
            {
                this.currentBrique = new Point();
            }
            else if (b == 1)
            {
                this.currentBrique = new Carre();
            }
            else if (b == 2)
                this.currentBrique = new TraitVertical();
            else if (b == 3)
                this.currentBrique = new L();
            this.xi = 0;
            this.xf = 0;
            this.yi = this.nombreColonne / 2;
            this.yf = this.nombreColonne / 2;
            Bouge();
        }

        // Prend en paramètre un string correspondant à une forme
        // Change la brique en cours par un l'objet correspondant et réinitialise ces coordonnées
        public void RecevoirBrique(string newBrique)
        {
            switch (newBrique)
            {
                case "Carre":
                    this.currentBrique = new Carre();
                    break;
                case "Point":
                    this.currentBrique = new Point();
                    break;
                case "TraitVertical":
                    this.currentBrique = new TraitVertical();
                    break;
                case "L":
                    this.currentBrique = new L();
                    break;
            }
            this.xi = 0;
            this.xf = 0;
            this.yi = this.nombreColonne / 2;
            this.yf = this.nombreColonne / 2;
            Bouge();
        }


        // Change les valeurs des cases d'une ligne pour notifier que celle-ci est finie
        // Fonction appelé après vérification qu'une ligne est finie et indice de celle-ci donné en paramètre
        public void LigneFinie(int ligne)
        {
            for (int j = 0; j < nombreColonne; j++)
            {
                this.plateau[ligne, j] = nombreForme * 2;
            }
        }

        // Prend en paramètre le nombres de pénalités reçu par un adversaire ainsi que son id
        public void AjoutePenalite(int nombrePenalite, int idEnvoyeur)
        {
            // Ajoute une pénalité au joueur et à chaque adversaire sauf à l'envoyeur
            this.penalites += nombrePenalite;
            for (int i = 0; i < adversaires.Count; i++)
            {
                if (adversaires[i].Id != idEnvoyeur)
                    adversaires[i].Penalites += nombrePenalite;
            }

            // Création d'un nouveau plateau
            int[,] newPlateau = new int[nombreLigne, nombreColonne];

            // Copie de toutes les cases fixes du plateau en cours vers le nouveau plateau
            // En décalant les lignes vers le haut du nombre de pénalité reçu
            for (int i = 0; i < nombreLigne; i++)
            {
                for (int j = 0; j < nombreColonne; j++)
                {
                    if (i < nombreLigne - nombrePenalite)
                    {
                        if (i < nombrePenalite && this.plateau[i, j] >= nombreForme)
                        {
                            this.etat = false;
                        }
                        else if (this.plateau[i + nombrePenalite, j] >= nombreForme)
                        {
                            newPlateau[i, j] = this.plateau[i + nombrePenalite, j];
                        }
                        else if (this.plateau[i, j] < nombreForme)
                            newPlateau[i, j] = 0;
                    }
                    else // si i >= nombreLigne - nombrePenalite alors ce sont les lignes de pénalités
                    {
                        newPlateau[i, j] = nombreForme * 2 + 1;
                    }
                }
            }
            this.plateau = newPlateau; // Le plateau en cours devient le nouveau plateau
            // Vérifie que l'on peut faire remonter la brique en cours
            if (xf < nombrePenalite)
                xf = 0;
            else
                xf -= nombrePenalite;
        }


        // Fonction qui supprime toutes les lignes qui ont été marqués comme finies et retourne le nombre de ligne finies
        public int SupprimeLigne()
        {
            int nombreLigneSupprimee = 0;

            //Repère le nombre de ligne supprimé et marque celles à supprimer
            int nombreCelluleFixe = 0;
            for (int i = 0; i < this.nombreLigne; i++)
            {
                for (int j = 0; j < this.nombreColonne; j++)
                {
                    if (this.plateau[i, j] != 0 && this.plateau[i, j] != nombreForme * 2 + 1)
                    {
                        nombreCelluleFixe++;
                    }
                }
                if (nombreCelluleFixe == this.nombreColonne)
                {
                    nombreLigneSupprimee++;
                    LigneFinie(i);      // Marquage ligne à supprimer
                    this.ligneFinies++;     // Augmente le score
                }
                nombreCelluleFixe = 0;
            }

            // Si on a supprimé des lignes alors on affiche rapidement celle marqué, et on supprime
            if (nombreLigneSupprimee != 0)
            {
                AfficherPlateau();
                Thread.Sleep(300);  //Temps de pause pour afficher lignes complètes
                // Parcours le tableau depuis la fin et remonte et s'arrête quand i est égale à l'indice de la ligne finie la plus en bas
                int i = this.nombreLigne - 1;
                while (i >= 0 && this.plateau[i, 0] != this.nombreForme * 2)
                {
                    i--;
                }
                int premiereLigneFinie = i; // Stocke la valeur de i
                // Continue à parcourir le tableau en remontant jusqu'à i = ligne au dessus de la dernière ligne finie, la plus haute
                while (i >= 0 && this.plateau[i, 0] == this.nombreForme * 2)
                {
                    i--;
                }
                // Maintenant i = ligne au dessus de la dernière ligne finie
                // On fait descendre toutes les cases qui sont au dessus de la dernière ligne finie au niveau de la première ligne finie
                while (i >= 0)
                {
                    for (int j = 0; j < this.nombreColonne; j++)
                    {
                        this.plateau[premiereLigneFinie, j] = this.plateau[i, j];
                    }
                    premiereLigneFinie--;
                    i--;
                }

            }
            return nombreLigneSupprimee;
        }


        // Vérifie si le mouvement de la pièce est possible et appelle les fonctions en conséquence
        public void Bouge()
        {
            bool mouvement = true;
            int i = 0;
            // Parcours la forme de la pièce, si ce n'est pas une case vide alors vérifie si la place est libre dans le plateau en partant des coordonées finale
            while (i < this.currentBrique.Dimension && mouvement)
            {
                int j = 0;
                while (j < this.currentBrique.Dimension && mouvement)
                {
                    if (this.currentBrique.Forme[i, j] != 0)
                    {
                        if (xf + i < this.nombreLigne && yf + j < this.nombreColonne && xf + i >= 0 && yf + j >= 0)
                        {
                            if (this.plateau[xf + i, yf + j] >= this.nombreForme)
                                mouvement = false;  // Mouvement impossible si rencontre une case fixe
                        }
                        else
                            mouvement = false;  // Mouvement impossible si en dehors du tableau
                    }
                    j++;
                }
                i++;
            }
            if (mouvement)  // Si mouvement possible
            {
                NettoyerTableau();   // Met à 0 toutes les valeurs du tableau qui ne sont pas fixes
                Insertion();    // Insère dans le plateau les valeurs du tableau (différentes de 0) de la brique en cours au coordonnées finale
            }
            else if (xf == xi + 1)      // Si mouvement impossible et que c'était un mouvement de descente
            {
                Fixe();     // Fixe la brique en cours
                int nombreLigneSupprime = SupprimeLigne();  // Supprime les lignes finies
                if (nombreLigneSupprime > 0) // si il y en a à supprimé 
                {
                    relationServeur.EnvoiePenalite(nombreLigneSupprime);    // Envoie les pénalités aux adversaires
                    for (int k = 0; k < adversaires.Count; k++)     // Incrémente le nombre de pénalité de chaque adversaires
                    {
                        adversaires[k].Penalites += nombreLigneSupprime;
                    }
                }
                relationServeur.DemandeBrique();        // Demande une nouvelle brique au serveur
            }
            else if (!mouvement && xi == 0 && yf == yi)     // si mouvement impossible et que la brique est à la ligne 0 et que la brique n'a pas bougé de colonne
                                                            // Alors insertion impossible de la nouvelle brique donc Game Over
            {
                this.etat = false;
            }
            else
            {
                xf = xi;    // Si mouvement impossible mais pas fixe alors la brique de bouge pas
                yf = yi;
            }
        }

        // Fait descendre la brique en cours toutes les millisecondes définie par speed
        public void Chute()
        {
            while (this.etat)
            {
                Thread.Sleep(speed);
                this.xf++;
                Bouge();
            }

        }

        // Contrôle l'appui sur les touches 
        public void MouvementTouche()
        {
            while (this.etat)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == keyG)
                {
                    this.yf -= 1;
                }
                else if (key.Key == keyD)
                {
                    this.yf += 1;
                }
                else if (key.Key == keyB)
                {
                    this.xf += 1;
                }
                Bouge();
            }
        }

        // Fonction qui efface la console, et affiche le plateau de jeu, le score du joueur, les noms et les pénalités des adversaires
        // Nous jouons avec le background pour afficher des blocs en couleurs en fonction de la valeur de la case dans le plateau
        public void AfficherPlateau()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            for (int i = 0; i < this.nombreLigne; i++)
            {
                for (int j = 0; j < this.nombreColonne; j++)
                {
                    switch (this.plateau[i, j])
                    {
                        case 0:
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write("|__");
                            break;
                        case 1:
                        case 6:
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write("|");
                            Console.BackgroundColor = ConsoleColor.Blue;
                            Console.Write("  ");
                            break;
                        case 2:
                        case 7:
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write("|");
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.Write("  ");
                            break;
                        case 3:
                        case 8:
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write("|");
                            Console.BackgroundColor = ConsoleColor.Green;
                            Console.Write("  ");
                            break;
                        case 4:
                        case 9:
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write("|");
                            Console.BackgroundColor = ConsoleColor.Magenta;
                            Console.Write("  ");
                            break;
                        case 10:
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write("|");
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.Write("  ");
                            break;
                        case 11:
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write("|");
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            Console.Write("  ");
                            break;
                    }
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("|");
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(this.nombreColonne * 4, 0);
            Console.Write("Score : {0}", this.ligneFinies);

            for (int i = 0; i < this.adversaires.Count; i++)
            {
                Console.SetCursorPosition(this.nombreColonne * 4, i + 5);
                Console.Write("{0} : {1}", this.adversaires[i].Nom, this.adversaires[i].Penalites);
            }
        }
    }
}
