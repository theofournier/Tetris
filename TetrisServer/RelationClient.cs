using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TetrisServer
{
    // Chaque client à son RelationClient, traite les communications entre un client et le serveur
    public class RelationClient
    {
        Joueur joueur;
        Socket listener;    // Socket du client
        List<RelationClient> listeClient;   // Liste de tous les clients
        int nbrForme = 4;

        public Joueur Joueur { get => joueur; set => joueur = value; }
        public Socket Listener { get => listener; set => listener = value; }

        public RelationClient(Joueur joueur, Socket listener, List<RelationClient> listeClient)
        {
            this.joueur = joueur;
            this.listener = listener;
            this.listeClient = listeClient;
        }


        // Envoie au client un string contenant "Brique," et le nom de la forme choisit au hasard
        // Fonction appelé si le client demande une nouvelle brique au serveur
        public void EnvoiBrique()
        {
            byte[] bytes = new Byte[1024];
            try
            {
                string strBrique = "Brique,";

                Random rand = new Random();

                int b = rand.Next(0, nbrForme);

                if (b == 0)
                    strBrique += "Point";
                else if (b == 1)
                    strBrique += "Carre";
                else if (b == 2)
                    strBrique += "TraitVertical";
                else if (b == 3)
                    strBrique += "L";


                byte[] msg = Encoding.ASCII.GetBytes(strBrique + ",<EOF>");

                listener.Send(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        // Fonction qui envoie à tous les autres clients un string contenant "Penalite", le nombre de pénalités reçu et l'id du joueur actuel
        // Fonction appelé si le client à envoyer Penalite au serveur
        public void EnvoyerPenaliteListeClient(string nombrePenalite)
        {
            try
            {
                string penalite = "Penalite," + nombrePenalite + "," + joueur.Id;
                byte[] msg = Encoding.ASCII.GetBytes(penalite + ",<EOF>");

                for (int i = 0; i < listeClient.Count; i++)
                {
                    if (listeClient[i].Joueur.Id != joueur.Id)
                    {
                        listeClient[i].Joueur.Penalites += Convert.ToInt32(nombrePenalite);
                        listeClient[i].Listener.Send(msg);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        // Supprime le client de la liste et arrête son etat
        public void SuppressionClient()
        {
            joueur.Etat = false;
            int i = 0;
            while (i < listeClient.Count && listeClient[i].Joueur.Id != joueur.Id)
            {
                i++;
            }
            if (listeClient[i].Joueur.Id == joueur.Id)
            {
                listeClient.RemoveAt(i);
                Console.WriteLine(joueur.Id + " a quitté la partie.");
                listener.Shutdown(SocketShutdown.Both);     // Arrête le socket si le client se déconnecte ou a perdu
                listener.Close();
                NotificationSuppressionJoueur();
            }
        }

        // Envoie à tous les clients la liste des autres clients, leur id, et leur pénalité
        // Permet de notifier un joueur supprimé, réutilisation de la fonction NotificationNouveauJoueur
        public void NotificationSuppressionJoueur()
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

        // Met la relation client en attente d'un message du client
        public void AttenteMessage()
        {
            try
            {
                while (joueur.Etat)
                {
                    byte[] bytes = new Byte[1024];
                    string data = null;

                    int bytesRec = listener.Receive(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    Console.WriteLine("Texte reçu de {0} : {1}", joueur.Id, data);

                    string[] newData = data.Split(',');

                    switch (newData[0])
                    {
                        case "Brique":
                            EnvoiBrique();
                            break;

                        case "Penalite":
                            EnvoyerPenaliteListeClient(newData[1]);
                            break;
                        case "Arret":
                            SuppressionClient();
                            break;
                    }
                }
            }
            catch
            {
                SuppressionClient();    // Arrête le socket si le client se déconnecte ou a perdu
            }
        }
    }
}
