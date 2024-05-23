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
    public class JeuPlayer
    {
        PlateauJeu plateau;
        Joueur joueur;
        RelationServeur relationServeur;

        public JeuPlayer()
        {
        }


        #region Test
        public void Test1()
        {
            this.plateau = new PlateauJeu();
            plateau.NombreLigne = 10;
            plateau.NombreColonne = 5;
            plateau.Speed = 1000;
            Joueur j1 = new Joueur("Aurelie", 1);
            j1.Penalites += 2;
            Joueur j2 = new Joueur("Theo", 2);
            j2.Penalites += 1;

            plateau.LigneFinies = 3;
            plateau.Adversaires.Add(j1);
            plateau.Adversaires.Add(j2);

            plateau.AfficherPlateau();
            Console.ReadKey();



            Brique brique = new Carre();

            plateau.CurrentBrique = brique;
            plateau.Xi = 0;
            plateau.Yi = 2;
            plateau.Xf = 0;
            plateau.Yf = 2;
            plateau.Bouge();
            plateau.AfficherPlateau();
            Console.ReadKey();



            plateau.Xi = 0;
            plateau.Yi = 2;
            plateau.Xf = 8;
            plateau.Yf = 2;
            plateau.Bouge();
            plateau.AfficherPlateau();
            Console.ReadKey();

            plateau.Xi = 8;
            plateau.Yi = 2;
            plateau.Xf = 9;
            plateau.Yf = 2;
            plateau.Bouge();
            plateau.AfficherPlateau();
            Console.ReadKey();

            Brique brique2 = new Point();
            plateau.CurrentBrique = brique2;
            plateau.Xi = 0;
            plateau.Yi = 2;
            plateau.Xf = 0;
            plateau.Yf = 2;
            plateau.Bouge();
            plateau.AfficherPlateau();
            Console.ReadKey();

            plateau.Xi = 6;
            plateau.Yi = 2;
            plateau.Xf = 7;
            plateau.Yf = 2;
            plateau.Bouge();
            plateau.AfficherPlateau();
            Console.ReadKey();

            plateau.Xi = 7;
            plateau.Yi = 2;
            plateau.Xf = 8;
            plateau.Yf = 2;
            plateau.Bouge();
            plateau.AfficherPlateau();
            Console.ReadKey();

        }

        public void Test2()
        {
            Console.WriteLine("Nombre de ligne ?");
            int nombreLigne = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Nombre de colonne ?");
            int nombreColonne = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Speed en milliseconde ?");
            int speed = Convert.ToInt32(Console.ReadLine());


            plateau = new PlateauJeu();
            plateau.NombreLigne = nombreLigne;
            plateau.NombreColonne = nombreColonne;
            plateau.Speed = speed;
            Joueur j1 = new Joueur("Aurelie", 1);
            Joueur j2 = new Joueur("Theo", 2);

            plateau.Adversaires.Add(j1);
            plateau.Adversaires.Add(j2);

            plateau.AfficherPlateau();
            Console.ReadKey();

            while (plateau.Etat)
            {
                plateau.AfficherPlateau();
                plateau.MouvementTouche();
                plateau.AfficherPlateau();
            }

            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("GAME OVER");

        }

        public void Test3()
        {
            Console.WriteLine("Nombre de ligne ?");
            int nombreLigne = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Nombre de colonne ?");
            int nombreColonne = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Speed en milliseconde ?");
            int speed = Convert.ToInt32(Console.ReadLine());

            plateau = new PlateauJeu();
            plateau.NombreLigne = nombreLigne;
            plateau.NombreColonne = nombreColonne;
            plateau.Speed = speed;

            Joueur j1 = new Joueur("Aurelie", 1);
            Joueur j2 = new Joueur("Theo", 2);

            plateau.Adversaires.Add(j1);
            plateau.Adversaires.Add(j2);

            plateau.AfficherPlateau();
            Console.ReadKey();



            //Thread1
            Thread thTouche = new Thread(plateau.MouvementTouche);
            thTouche.Start();
            //Fin thread1

            Thread thChute = new Thread(plateau.Chute);
            thChute.Start();



            while (plateau.Etat)
            {
                plateau.AfficherPlateau();
                Thread.Sleep(200);
            }


            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("GAME OVER");

        }

        public void TestServeur()
        {
            plateau = new PlateauJeu();
            plateau.NombreLigne = 18;
            plateau.NombreColonne = 8;
            plateau.Speed = 1000;
            relationServeur = new RelationServeur(plateau, joueur.Nom, "192.168.1.44", "1500");
            relationServeur.Connexion();
            for (int i = 0; i < relationServeur.MessageRecu.Length; i++)
                Console.Write("{0}  ", relationServeur.MessageRecu[i]);
            Console.WriteLine("\nDemande brique : ");
            relationServeur.DemandeBrique();
            Console.ReadKey();
            Console.WriteLine("\nDemande brique : ");
            relationServeur.DemandeBrique();
        }

        public void Test4()
        {
            Console.Write("Entrez votre nom : ");
            string nom = Console.ReadLine();
            Console.Write("Indiquez l'adresse IP du serveur : ");
            string ip = Console.ReadLine();
            Console.Write("et le port : ");
            string port = Console.ReadLine();

            plateau = new PlateauJeu();

            relationServeur = new RelationServeur(plateau, nom, ip, port);

            relationServeur.Connexion();

            plateau.RelationServeur = relationServeur;

            plateau.NombreLigne = relationServeur.MessageRecu[0];
            plateau.NombreColonne = relationServeur.MessageRecu[1];
            plateau.Speed = relationServeur.MessageRecu[2];

            this.joueur = new Joueur(nom, relationServeur.MessageRecu[3]);

            Thread thAttenteMessage = new Thread(relationServeur.AttenteMessage);
            thAttenteMessage.Start();

            Console.ReadKey();

            relationServeur.EnvoiePenalite(2);
        }

        public void Test5()
        {
            Console.Write("Entrez votre nom : ");
            string nom = Console.ReadLine();
            Console.Write("Indiquez l'adresse IP du serveur : ");
            string ip = Console.ReadLine();
            Console.Write("et le port : ");
            string port = Console.ReadLine();


            plateau = new PlateauJeu();

            relationServeur = new RelationServeur(plateau, nom, ip, port);

            relationServeur.Connexion();

            plateau.RelationServeur = relationServeur;
            plateau.NombreLigne = relationServeur.MessageRecu[0];
            plateau.NombreColonne = relationServeur.MessageRecu[1];
            plateau.Speed = relationServeur.MessageRecu[2];

            plateau.InitialiserPlateau();

            this.joueur = new Joueur(nom, relationServeur.MessageRecu[3]);


            Console.WriteLine("Appuyer sur une touche pour commencez à jouer !");
            Console.ReadKey();
            plateau.DemandeBriqueLocal();
            plateau.AfficherPlateau();

            Thread thAttenteMessage = new Thread(relationServeur.AttenteMessage);
            thAttenteMessage.Start();

            //Thread1
            Thread thTouche = new Thread(plateau.MouvementTouche);
            thTouche.Start();
            //Fin thread1

            Thread thChute = new Thread(plateau.Chute);
            thChute.Start();

            while (plateau.Etat)
            {
                plateau.AfficherPlateau();
                Thread.Sleep(200);
            }


            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("GAME OVER");

        }

        public void Test6()
        {
            Console.Write("Entrez votre nom : ");
            string nom = Console.ReadLine();
            Console.Write("Haut : ");
            ConsoleKey keyH = Console.ReadKey().Key;
            Console.Write("Droite : ");
            ConsoleKey keyD = Console.ReadKey().Key;
            Console.Write("Bas : ");
            ConsoleKey keyB = Console.ReadKey().Key;
            Console.Write("Gauche : ");
            ConsoleKey keyG = Console.ReadKey().Key;
            Console.Write("\nIndiquez l'adresse IP du serveur : ");
            string ip = Console.ReadLine();
            Console.Write("et le port : ");
            string port = Console.ReadLine();

            plateau = new PlateauJeu(keyH, keyD, keyB, keyG);

            relationServeur = new RelationServeur(plateau, nom, ip, port);

            relationServeur.Connexion();
            
            Thread thAttenteMessage = new Thread(relationServeur.AttenteMessage);
            thAttenteMessage.Start();

            plateau.RelationServeur = relationServeur;
            plateau.NombreLigne = relationServeur.MessageRecu[0];
            plateau.NombreColonne = relationServeur.MessageRecu[1];
            plateau.Speed = relationServeur.MessageRecu[2];

            plateau.InitialiserPlateau();

            this.joueur = new Joueur(nom, relationServeur.MessageRecu[3]);


            Console.WriteLine("Appuyer sur une touche pour commencez à jouer !");
            Console.ReadKey();
            plateau.DemandeBriqueLocal();
            plateau.AfficherPlateau();
            
            Thread thTouche = new Thread(plateau.MouvementTouche);
            thTouche.Start();

            Thread thChute = new Thread(plateau.Chute);
            thChute.Start();

            while (plateau.Etat)
            {
                plateau.AfficherPlateau();
                Thread.Sleep(200);
            }


            relationServeur.EnvoieArretJoueur();
            relationServeur.Listener.Shutdown(SocketShutdown.Both);
            relationServeur.Listener.Close();

            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("GAME OVER");
        }
        #endregion



        // Fonction qui fait fonctionner le jeu côté joueur
        public void Programme(string ip, string port, string haut, string droit, string bas, string gauche)
        {
            Console.Write("Entrez votre nom : ");
            string nom = Console.ReadLine();
            ConsoleKey keyH;
            Enum.TryParse<ConsoleKey>(haut.ToString(), out keyH);
            ConsoleKey keyD;
            Enum.TryParse<ConsoleKey>(droit.ToString(), out keyD);
            ConsoleKey keyB;
            Enum.TryParse<ConsoleKey>(bas.ToString(), out keyB);
            ConsoleKey keyG;
            Enum.TryParse<ConsoleKey>(gauche.ToString(), out keyG);

            plateau = new PlateauJeu(keyH, keyD, keyB, keyG);

            relationServeur = new RelationServeur(plateau, nom, ip, port);

            // Connexion avec le serveur
            relationServeur.Connexion();

            //Thread pour la relation serveur
            Thread thAttenteMessage = new Thread(relationServeur.AttenteMessage);
            thAttenteMessage.Start();

            // Initialise les attributs de plateau
            plateau.RelationServeur = relationServeur;
            plateau.NombreLigne = relationServeur.MessageRecu[0];
            plateau.NombreColonne = relationServeur.MessageRecu[1];
            plateau.Speed = relationServeur.MessageRecu[2];

            plateau.InitialiserPlateau();

            this.joueur = new Joueur(nom, relationServeur.MessageRecu[3]);


            Console.WriteLine("Appuyer sur une touche pour commencez à jouer !");
            Console.ReadKey();

            // Commence par demander une brique en local
            plateau.DemandeBriqueLocal();
            plateau.AfficherPlateau();

            //Thread pour la gestion des touches
            Thread thTouche = new Thread(plateau.MouvementTouche);
            thTouche.Start();

            //Thread pour la descente de la brique
            Thread thChute = new Thread(plateau.Chute);
            thChute.Start();

            //Thread principal qui affiche toutes les 0.2 secondes le plateau
            while (plateau.Etat)
            {
                plateau.AfficherPlateau();
                Thread.Sleep(200);
            }

            // Arrêt du joueur
            relationServeur.EnvoieArretJoueur();
            relationServeur.Listener.Shutdown(SocketShutdown.Both);
            relationServeur.Listener.Close();

            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("GAME OVER");
        }
    }
}
