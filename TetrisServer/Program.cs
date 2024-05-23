using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TetrisServer
{
    class Program
    {
        #region Test
        static void Test1()
        {
            Jeu jeu = new Jeu(15, 8, 1000, "1500");
            jeu.Connexion();
        }

        static void Test2()
        {
            Jeu jeu = new Jeu(15, 8, 1000, "1500");
            jeu.Connexion();

            while (true)
            {
                jeu.ListeClient[0].AttenteMessage();
            }
        }

        static void Test3()
        {
            Jeu jeu = new Jeu(15, 8, 1000, "1500");
            jeu.LancementServeur();
        }

        static void Test4()
        {
            Jeu jeu = new Jeu(15, 8, 1000, "1500");
            jeu.LancementServeur();

            Thread thConnexion = new Thread(jeu.Connexion);
            thConnexion.Start();

            Console.ReadKey();
        }

        static void Test5()
        {
            Jeu jeu = new Jeu(15, 8, 1000, "1500");
            jeu.LancementServeur();

            Thread thConnexion = new Thread(jeu.Connexion);
            thConnexion.Start();
            while (true)
            {
                Console.ReadKey();
                jeu.AfficherListeClient();
            }
        }

        static void Test6()
        {
            Console.WriteLine("Nombre de ligne ?");
            int nombreLigne = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Nombre de colonne ?");
            int nombreColonne = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Speed en milliseconde ?");
            int speed = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Port à utiliser");
            string port = Console.ReadLine();

            Console.WriteLine("\nAppuyer sur une touche pour lancer le serveur");
            Console.ReadKey();
            Console.Clear();

            Jeu jeu = new Jeu(nombreLigne, nombreColonne, speed, port);
            jeu.LancementServeur();

            Thread thConnexion = new Thread(jeu.Connexion);
            thConnexion.Start();

            while (true)
            {
                Console.ReadKey();
                jeu.AfficherListeClient();
            }
        }
        #endregion



        // Programme principal
        static void Programme(string port, string nombreColonne, string nombreLigne, string speed)
        {

            Console.WriteLine("\nAppuyer sur une touche pour lancer le serveur");
            Console.ReadKey();
            Console.Clear();

            Jeu jeu = new Jeu(Convert.ToInt32(nombreLigne), Convert.ToInt32(nombreColonne), Convert.ToInt32(speed), port);
            jeu.LancementServeur();

            Thread thConnexion = new Thread(jeu.Connexion);
            thConnexion.Start();

            while (true)
            {
                Console.ReadKey();
                jeu.AfficherListeClient();      // Permet d'afficher la liste des clients lors de l'appui sur une touche
            }
        }


        static void Main(string[] args)
        {
            //Test1();
            //Test2();
            ///Test3();
            //Test4();
            //Test5();
            //Test6();


            Programme(args[0], args[1], args[2], args[3]);

            Console.ReadKey();
        }
    }
}
