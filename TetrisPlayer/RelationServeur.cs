using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TetrisPlayer
{
    // Gère les réceptions et envoie de données avec le serveur
    public class RelationServeur
    {
        Socket listener;    // Socket entre le client et le serveur
        PlateauJeu plateau;
        string nom;
        string ipString;
        string port;
        int[] messageRecu = new int[4];     // Contient nombreLigne, nombreColonne, Speed, et id du joueur en cours


        public int[] MessageRecu { get => messageRecu; set => messageRecu = value; }
        public Socket Listener { get => listener; set => listener = value; }

        public RelationServeur(PlateauJeu plateau, string nom, string ipString, string port)
        {
            this.plateau = plateau;
            this.nom = nom;
            this.ipString = ipString;
            this.port = port;
        }

        byte[] bytes = new byte[1024];


        // Le client se connecte au serveur, lui envoie son nom et récupère le tableau d'entier messageRecu
        public void Connexion()
        {
            string stringRecu = "";

            // Connexion  
            try
            {
                // Etablie le endpoint de connexion  
                IPAddress ipAddress = IPAddress.Parse(ipString);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Convert.ToInt32(port));

                // Créer le socket avec le serveur
                listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connexion avec le socket  
                try
                {
                    listener.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}", listener.RemoteEndPoint.ToString());

                    // Encode le message à envoyer : nom du joueur
                    string message = nom;
                    byte[] msg = Encoding.ASCII.GetBytes(message + "<EOF>");

                    // Envoie le tableau de byte  
                    int bytesSent = listener.Send(msg);

                    // Reçoit la réponse du serveur : nombreLigne, nombreColonne, speed, idJoueur
                    // Stocke les réponses dans le tableau d'entier messageRecu
                    int bytesRec = listener.Receive(bytes);
                    stringRecu = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    Console.WriteLine("Echoed test = {0}", stringRecu);
                    if (stringRecu != null)
                    {
                        string[] entier = stringRecu.Split(',');
                        for (int i = 0; i < entier.Length; i++)
                        {
                            messageRecu[i] = Convert.ToInt32(entier[i]);
                        }
                    }

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());

                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // Envoie au serveur le string "Brique" qui notifie au serveur de lui renvoyer une nouvelle brique
        public void DemandeBrique()
        {
            try
            {
                string message = "Brique";
                byte[] msg = Encoding.ASCII.GetBytes(message);

                // Send the data through the socket.  
                int bytesSent = listener.Send(msg);
            }
            catch
            {
                Console.WriteLine("Problème envoie demande briques");
            }
        }

        // Envoie au serveur le string "Penalite" pour lui notifier qu'il faut ajouter une pénalité aux adversaires
        public void EnvoiePenalite(int nombrePenalite)
        {
            try
            {
                string message = "Penalite," + Convert.ToString(nombrePenalite);
                byte[] msg = Encoding.ASCII.GetBytes(message);

                // Send the data through the socket.  
                int bytesSent = listener.Send(msg);
            }
            catch
            {
                Console.WriteLine("Problème envoie pénalités");
            }
        }


        // Si le client reçoit "NouveauJoueur", alors change la liste adversaire par une nouvelle liste
        // Le serveur envoie la liste de tous les clients connectés : nom, id, pénalités
        // Exemple : message = "[[NouveauJoueur][Aurelie|1|2][Theo|2|1][<EOF>]"
        public void AjoutNouveauJoueur(string[] message)
        {
            plateau.Adversaires = new List<Joueur>();
            // Parcours le tableau du premier joueur au dernier joueur
            for (int i = 1; i < message.Length - 1; i++)
            {
                string[] adv = message[i].Split('|');
                Joueur j = new Joueur(adv[0], Convert.ToInt32(adv[1]));
                j.Penalites = Convert.ToInt32(adv[2]);
                plateau.Adversaires.Add(j);
            }
        }


        //Fonction qui notifie au serveur que le joueur a perdu ou s'est deconnecté
        public void EnvoieArretJoueur()
        {
            try
            {
                string message = "Arret";
                byte[] msg = Encoding.ASCII.GetBytes(message);

                // Send the data through the socket.  
                int bytesSent = listener.Send(msg);
            }
            catch
            {
                Console.WriteLine("Problème envoie arret");
            }
        }

        // Fonction qui permet à un client d'attendre un message du serveur
        public void AttenteMessage()
        {
            try
            {
                while (plateau.Etat)
                {
                    byte[] bytes = new Byte[1024];
                    string data = null;

                    //Construction du message reçu
                    while (true)
                    {
                        bytes = new byte[1024];
                        int bytesRec = listener.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                    // Sépare le message reçu pour repérer le type du message et ces arguments séparer par ','
                    // Exemple : "NouveauJoueur,Aurelie|1|2,Theo|2|1,<EOF>"
                    // Exemple : "Penalite,2,1"
                    // Exemple : "Brique,Carre"
                    string[] newData = data.Split(',');

                    switch (newData[0])
                    {
                        case "NouveauJoueur":
                            AjoutNouveauJoueur(newData);
                            break;
                        case "Penalite":
                            plateau.AjoutePenalite(Convert.ToInt32(newData[1]), Convert.ToInt32(newData[2]));
                            break;
                        case "Brique":
                            plateau.RecevoirBrique(newData[1]);
                            break;
                    }
                }
            }
            catch
            {
                plateau.Etat = false;
            }
        }
    }
}
