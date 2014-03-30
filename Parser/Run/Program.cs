using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run
{
    class Program
    {
        static void Main(string[] args)
        {
            var temp = run("1Ka3xYZb6tZQabpkFCnpqods6gC8iNEcZ7");

            temp.removeEndNodes();
            //temp.writeListToFile();
        }

        public static GraphDataStructure.Graph run(string publicAddress)
        {
            int count = 0;
            Queue<string> nextAddresses = new Queue<string>();
            GraphDataStructure.Graph graphList = new GraphDataStructure.Graph();

            graphList.addGraphNode(publicAddress);
            nextAddresses.Enqueue(publicAddress);

            while (count < 2)
            {
                Database.DBConnect getLists = new Database.DBConnect();
                string current = nextAddresses.Dequeue();

                var sendersList = getLists.getSentTo(current);
                var reciverList = getLists.getRecivedFrom(current);

                foreach (var sender in sendersList)
                {
                    nextAddresses.Enqueue(sender.target);
                    graphList.addGraphNode(sender.target);

                    graphList.addDirectedEdge(sender.source, sender.target);
                   // Console.WriteLine(sender.source + "     " + sender.target);
                }

                foreach (var reciver in reciverList)
                {
                    nextAddresses.Enqueue(reciver.source);
                    graphList.addGraphNode(reciver.source);

                    graphList.addDirectedEdge(reciver.source, reciver.target);
                  //  Console.WriteLine(reciver.source + "    " + reciver.target);
                }

                count++;
            }

            return graphList;
        }
    }
}
