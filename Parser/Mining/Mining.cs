using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database;

namespace Mining
{
    public static class Mining
    {
        public static int MAX_TRANSACTION_COUNT = 10000;
        public static JSONHolder Mine(string address)
        {
            int degressOfSep = 2;
            DBConnect handle = new DBConnect();
            List<JSON> jsonList = transactionList(address, degressOfSep, handle);
            List<Node> nodeList = uniqueNodes(jsonList);
            //trimNodeList(ref nodeList, ref jsonList, degressOfSep);
            List<Link> linkList = makeLinkList(nodeList, jsonList);
            JSONHolder holder = new JSONHolder();
            holder.edges = linkList;
            holder.nodes = nodeList;
            return holder;
        }

        static private List<Link> makeLinkList(List<Node> nodeList, List<JSON> jsonList)
        {
            int counter = 0;
            List<Link> linkList = new List<Link>();
            foreach(JSON item in jsonList)
            {
                Link link = new Link();
                link.source = item.source;
                link.target = item.target;
                link.id = counter.ToString();
                linkList.Add(link);
                counter++;
            }
            return linkList;
        }
        /*
        static private int indexInUniqueList(List<Node> nodeList, string address)
        {
            for(int i = 0; i < nodeList.Count; i++)
            {
                if (nodeList[i].name == address)
                {
                    return i;
                }
            }
            //should not return -1
            return -1;
        }
         
        static private void trimNodeList(ref List<Node> nodeList, ref List<JSON> jsonList, int trimCycles)
        {
            List<Node> removeList = new List<Node>();
            for(int i = 0; i < trimCycles; i++)
            {
                foreach(Node node in nodeList)
                {
                    if(trim(node.id, ref jsonList))
                    {
                        removeList.Add(node);
                    }
                }
                foreach (Node node in removeList)
                {
                    nodeList.Remove(node);
                }
            }
        }
        
        static private bool trim(string address, ref List<JSON> jsonList)
        {
            int index = -1;
            int counter = 0;
            for (int i = 0; i < jsonList.Count; i++)
            {
                if(jsonList[i].target == address || jsonList[i].source == address)
                {
                    index = i;
                    counter++;
                }
            }
            if (counter < 2)
            {
                jsonList.RemoveAt(index);
                return true;
            }
            return false;
        }
        */
        static private List<Node> uniqueNodes(List<JSON> jsonList)
        {
            int counterX = 0;
            int counterY = 0;

            string lastTarget = "";
            string lastSource = "";

            List<Node> uniqueList = new List<Node>();
            foreach (JSON element in jsonList)
            {
                bool foundSource = true;
                bool foundTarget = true;
                foreach(Node uniqueItem in uniqueList)
                {
                    if (element.source == uniqueItem.id)
                    {
                        foundSource = false;
                    }
                    if(element.target == uniqueItem.id)
                    {
                        foundTarget = false;
                    }
                }
                if (foundSource)
                {
                    Node temp = new Node();

                    temp.id = element.source;
                    if (element.target == lastTarget)
                    {
                        counterY++;
                    }
                    else
                    {
                        counterX++;
                    }
                    temp.x = counterX;
                    temp.y = counterY;
                    lastSource = element.source;
                    uniqueList.Add(temp);

                }
                if(foundTarget)
                {
                    Node temp = new Node();

                    temp.id = element.target;
                    if (element.target == lastTarget)
                    {
                        counterY++;
                    }
                    else
                    {
                        counterX++;
                    }
                    lastTarget = element.target;
                    temp.x = counterX;
                    temp.y = counterY;
                    uniqueList.Add(temp);
                }
            }
            return uniqueList;
        }
        
        static private List<JSON> transactionList(string startingAddress, int degreesOfSeparation, DBConnect handle)
        {
            List<JSON> jsonList = new List<JSON>();

            List<JSON> tempRecived1 = handle.getRecivedFrom(startingAddress);
            List<JSON> tempRecived2 = new List<JSON>();
            List<JSON> tempSent1 = handle.getSentTo(startingAddress);
            List<JSON> tempSent2 = new List<JSON>();

            jsonList.AddRange(tempRecived1);
            jsonList.AddRange(tempSent1);

            for (int i = 1; i < degreesOfSeparation; i++)
            {
                foreach (JSON json in tempRecived1)
                {
                    List<JSON> temp = handle.getRecivedFrom(json.source);
                    if (temp.Count < MAX_TRANSACTION_COUNT)
                    {
                        tempRecived2.AddRange(temp);
                    }
                    temp = handle.getSentTo(json.source);
                    if (temp.Count < MAX_TRANSACTION_COUNT)
                    {
                        tempSent2.AddRange(temp);
                    }
                }

                foreach (JSON json in tempSent1)
                {
                    List<JSON> temp = handle.getRecivedFrom(json.target);
                    if (temp.Count < MAX_TRANSACTION_COUNT)
                    {
                        tempRecived2.AddRange(temp);
                    }
                    temp = handle.getSentTo(json.target);
                    if (temp.Count < MAX_TRANSACTION_COUNT)
                    {
                        tempSent2.AddRange(temp);
                    }
                }
                jsonList.AddRange(tempRecived2);
                jsonList.AddRange(tempSent2);
                tempRecived1 = tempRecived2;
                tempSent1 = tempSent2;
            }
            return jsonList;
        }
    }
}
