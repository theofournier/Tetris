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
    // Contient les paramètres de connexion et de nouveau joueur
    public class Jeu
    {
        List<RelationClient> listeClient;
        int nombreLigne;
        int nombreColonne;
        int speed;
        string port;
        IPAddress ipAddress;
        IPEndPoint localEndPoint;
        Socket nouveauJoueur;

        public List<RelationClient> ListeClient { get => listeClient; set => listeClient = value; }

        public Jeu(int nombreLigne, int nombreColonne, int speed, string port)
        {
            this.listeClient = new List<RelationClient>();
            this.nombreLigne = nombreLigne;
            this.nombreColonne = nombreColonne;
            this.speed = speed;
            this.port = port;
        }

        // Permet de lancer le serveur avec le choix des IP
        public void LancementServeur()
        {
            // Affiche les différentes IP
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            for (int i = 0; i < ipHostInfo.AddressList.Length; i++)
            {
                Console.WriteLine("- " + i + " : " + ipHostInfo.AddressList[i]);
            }
            Console.Write("\nChoississez l'indice de l'adresse sur laquelle vous connectez : ");
            int indiceAdress = -1;
            while (indiceAdress < 0 || indiceAdress >= ipHostInfo.AddressList.Length)
            {
                try
                {
                    indiceAdress = Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Tapez un entier");
                }
            }

            ipAddress = ipHostInfo.AddressList[indiceAdress];
            Console.WriteLine("Vous avez choisi : " + ipAddress);
            localEndPoint = new IPEndPoint(ipAddress, Convert.ToInt32(port));
        }

        // Lorsqu'un nouveau joueur s'est connecté, le serveur reçoit son nom, lui renvoie les paramètres de jeu
        public void NouveauJoueur()
        {
            byte[] bytes = new Byte[1024];
            try
            {
                string data = null;
  
                while (true)
                {
                    bytes = new byte[1024];
                    int bytesRec = nouveauJoueur.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        break;
                    }
                }
 
                int n = data.Length;
                string newData = "";
                for (int i = 0; i < n - 5; i++)
                    newData += data[i];
                Console.WriteLine("Texte reçu : {0}", newData);     // Nom du nouveau joueur sans <EOF>
                int id = listeClient.Count + 1;
                // Création d'un relationClient pour nouveau joueur
                RelationClient client = new RelationClient(new Joueur(newData, id), nouveauJoueur, this.listeClient); 
                this.listeClient.Add(client);   // Ajoute le nouveau joueur à la liste de client
                
                // Envoie au client les paramètres de jeu et son id
                byte[] msg = Encoding.ASCII.GetBytes(Convert.ToString(nombreLigne) + "," + Convert.ToString(nombreColonne) + "," + Convert.ToString(speed) + "," + Convert.ToString(id));
                nouveauJoueur.Send(msg);

                // Si il n'est pas tout seul, envoie d'une notification à tous les clients d'un nouveau joueur
                if(listeClient.Count > 1)
                    NotificationNouveauJoueur();

                // Place la relation client dans un état d'attente de message
                client.AttenteMessage();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        // Envoie à tous les clients la liste des autres clients, leur id, et leur pénalité
        // Permet de notifier un nouveau joueur
        public void NotificationNouveauJoueur()
        {
            for (int i = 0; i < listeClient.Count; i++)
            {
                string message = "NouveauJoueur";
                for (int j = 0; j < listeClient.Count; j++)
                {
                    if (listeClient[j].Joueur.Id != listeClient[i].Joueur.Id)
                    {
                        message += "," + listeClient[j].Joueur.Nom + "|" + listeClient[j].Joueur.Id + "|" + listeClient[j].Joueur.Penalites;
                    }
                }
                byte[] msg = Encoding.ASCII.GetBytes(message + ",<EOF>");
                listeClient[i].Listener.Send(msg);
            }
        }

        // Attente de nouvelle connexion en continue
        public void Connexion()
        { 
            byte[] bytes = new Byte[1024];
  
            // Créer un Socket pour le serveur 
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Démarre l'attente de connexion   
                while (true)
                {
                    Console.WriteLine("Nouvelle connexion..."); 
                    nouveauJoueur = listener.Accept();

                    Thread thNouveauJoueur = new Thread(NouveauJoueur);     // Démarre un thread par joueur connecté
                    thNouveauJoueur.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void AfficherListeClient()
        {
            for (int i = 0; i < listeClient.Count; i++)
            {
                Console.WriteLine(listeClient[i].Joueur.Nom + ", " + listeClient[i].Joueur.Id);
            }
        }
    }
}

