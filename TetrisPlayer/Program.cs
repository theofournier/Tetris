using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TetrisPlayer
{
    class Program
    {

        static void ProgrammeTest()
        {
            JeuPlayer jeuPlayer = new JeuPlayer();

            //jeuPlayer.Test1();
            //jeuPlayer.Test2();
            //jeuPlayer.Test3();
            //jeuPlayer.TestServeur();
            //jeuPlayer.Test4();
            //jeuPlayer.Test5();
            jeuPlayer.Test6();
        }

        static void Main(string[] args)
        {
            //ProgrammeTest();
            JeuPlayer jeuPlayer = new JeuPlayer();
            jeuPlayer.Programme(args[0],args[1],args[2],args[3],args[4],args[5]);
            Console.ReadKey();
        }
    }
}
