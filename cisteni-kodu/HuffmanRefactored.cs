using System;
using System.Collections.Generic;
using System.IO;

namespace HuffmanskeKapky
{

    class Node : IComparable<Node>
    {
        public Node rightNode;
        public int weight;
        public byte symbol;
        public Node leftNode;

        int index;

        static int lastNodeIndex;

        public Node(int weight, byte symbol, Node left, Node right)
        {
            this.weight = weight;
            this.symbol = symbol;
            this.leftNode = left;
            this.rightNode = right;
            index = lastNodeIndex;
            lastNodeIndex++;
        }

        /// <summary>
        /// Kdyz nema jedineho syna vraci true.
        /// </summary>
        /// <returns></returns>
        public bool IsLeaf()
        {
            if ((leftNode == null) && (rightNode == null))
            {
                return true;
            }
            else return false;
        }

        public static int SumWeights(Node firstNode, Node secondNode)
        {
            return firstNode.weight + secondNode.weight;
        }

        /// <summary>
        /// Zvetsi vahu vrcholu o zadany int, vraci upraveny vrchol.
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        public Node IncreaseWeight(int rank)
        {
            weight += rank;
            return this;
        }

        /// <summary>
        /// True o sobe vrchol rekne jestli bude v Huffmanskem strome nalevo od druheho vrcholu.
        /// </summary>
        /// <param name="druhy"></param>
        /// <returns></returns>
        public bool IsLeftNode(Node otherNode)
        {
            if (otherNode.weight > weight)
            {
                return true;
            }
            else if (otherNode.weight < weight)
            {
                return false;
            }
            else if (otherNode.IsLeaf() && !(IsLeaf()))
            {
                return false;
            }
            else if (IsLeaf() && !(otherNode.IsLeaf()))
            {
                return true;
            }
            else if ((IsLeaf()) && (otherNode.IsLeaf()) && (symbol < otherNode.symbol))
            {
                return true;
            }
            else if ((IsLeaf()) && (otherNode.IsLeaf()) && (symbol > otherNode.symbol))
            {
                return false;
            }
            else if (index < otherNode.index)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #region IComparable Members

        public int CompareTo(Node otherNode)
        {
            if (this == otherNode)
            {
                return 0;
            }
            else if (IsLeftNode(otherNode))
            {
                return -1;
            }
            else
            {
                return 1;
            }

        }

        #endregion
    }

    class Tree
    {
        private Node root;

        public Tree(SortedDictionary<int, List<Node>> rankedNodes)
        {
            Build(rankedNodes);
        }

        int treeCount = 0;

        private void Build(SortedDictionary<int, List<Node>> rankedNodes)
        {
            List<Node> nodes;
            Node pom1;
            Node pom3;
            Node novy;
            Node oddNode = null;
            int remainingNodes = 0;
            int rank;

            foreach (KeyValuePair<int, List<Node>> rankedNode in rankedNodes)
            {
                remainingNodes += rankedNode.Value.Count;
            }

            if (remainingNodes != 1)
            {
                treeCount = treeCount + 1;
            }

            while (remainingNodes != 1)
            {
                nodes = rankedNodes[rankedNodes.Keys.ElementAt(0)];
                rank = rankedNodes.Keys.ElementAt(0);

                if (oddNode == null)
                {
                    for (int i = 0; i < nodes.Count - 1; i++)
                    {
                        pom1 = nodes[i];
                        pom3 = nodes[++i];

                        if (pom1.IsLeftNode(pom3))
                        {
                            novy = new Node(pom1.weight + pom3.weight, pom1.symbol, pom1, pom3);
                        }
                        else novy = new Node(pom1.weight + pom3.weight, pom1.symbol, pom3, pom1);

                        if (rankedNodes.ContainsKey(novy.weight))
                        {
                            rankedNodes[novy.weight].Add(novy);
                        }
                        else rankedNodes.Add(novy.weight, new List<Node>() { novy });


                        remainingNodes--;
                    }
                    if (nodes.Count % 2 == 1)
                    {
                        oddNode = nodes[nodes.Count - 1];

                    }
                    else
                    {
                        oddNode = null;
                    }

                }
                else
                {
                    pom1 = nodes[0];
                    if (oddNode.IsLeftNode(pom1))
                    {
                        novy = new Node(oddNode.weight + pom1.weight, oddNode.symbol, oddNode, pom1);
                    }
                    else novy = new Node(pom1.weight + oddNode.weight, pom1.symbol, pom1, oddNode);

                    if (rankedNodes.ContainsKey(novy.weight))
                    {
                        rankedNodes[novy.weight].Add(novy);
                    }
                    else rankedNodes.Add(novy.weight, new List<Node>() { novy });

                    remainingNodes--;

                    for (int i = 1; i < nodes.Count - 1; i++)
                    {
                        pom1 = nodes[i];
                        pom3 = nodes[++i];

                        if (pom1.IsLeftNode(pom3))
                        {
                            novy = new Node(pom1.weight + pom3.weight, pom1.symbol, pom1, pom3);
                        }
                        else novy = new Node(pom1.weight + pom3.weight, pom1.symbol, pom3, pom1);

                        if (rankedNodes.ContainsKey(novy.weight))
                        {
                            rankedNodes[novy.weight].Add(novy);
                        }
                        else rankedNodes.Add(novy.weight, new List<Node>() { novy });

                        remainingNodes--;
                    }
                    if (nodes.Count % 2 == 0)
                    {
                        oddNode = nodes[nodes.Count - 1];
                    }
                    else oddNode = null;
                }
                rankedNodes.Remove(rank);
            }
            root = rankedNodes[rankedNodes.Keys.ElementAt(0)][0];
        }

        public void PrintTree()
        {
            // VypisStrom(this.koren);
        }

        public void PrintTreePrefixed()
        {
            PrintTreePrefixed(this.root, "");
        }

        public void PrintTreePrefixed(Node node, string prefix)
        {
            bool wasLeftNode = false;

            if (node.IsLeaf())
            {
                if ((node.symbol >= 32) && (node.symbol <= 0x7E))
                {
                    Console.Write(" ['{0}':{1}]\n", (char)node.symbol, node.weight);
                    return;
                }
                else
                {
                    Console.Write(" [{0}:{1}]\n", node.symbol, node.weight);
                }
                return;
            }
            else
            {
                // bylVlevo = true;
            }

            if (!wasLeftNode)
            {
                Console.Write("{0,4} -+- ", node.weight);
                wasLeftNode = true;
            }
            prefix = prefix + "      ";
            if (wasLeftNode)
            {
                PrintTreePrefixed(node.rightNode, prefix + "|  ");
                Console.Write("{0}|\n", prefix);
                Console.Write("{0}`- ", prefix);
                PrintTreePrefixed(node.leftNode, prefix + "   ");
            }
        }
    }

    class Reader
    {
        private static FileStream sourceFileStream;

        public static bool OpenFile(string fileName)
        {
            try
            {
                sourceFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                if (!(sourceFileStream.CanRead))
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Console.Write("File Error");
                Environment.Exit(0);
                //    return false;
            }
            return true;
        }

        public static SortedDictionary<int, List<Node>> ReadFile(string fileName)
        {

            if (!(OpenFile(fileName))) return null;
            else
            {
                SortedDictionary<int, List<Node>> rankedNodes = new SortedDictionary<int, List<Node>>();
                byte a = 0;

                Node[] nodes = new Node[256];
                byte[] buffer = new byte[0x4000];

                for (int i = 0; i < sourceFileStream.Length / 0x4000; i++)
                {
                    sourceFileStream.Read(buffer, 0, 16384);

                    for (int j = 0; j < 16384; j++)
                    {
                        a = buffer[j];
                        if (nodes[a] == null)
                        {
                            nodes[a] = new Node(1, (byte)a, null, null);
                            //   vrcholy.Add(prvky[a]);
                        }
                        else
                        {
                            nodes[a].weight++;
                        }
                    }
                }

                for (int i = 0; i < sourceFileStream.Length % 0x4000; i++)
                {
                    a = (byte)sourceFileStream.ReadByte();
                    if (nodes[a] == null)
                    {
                        nodes[a] = new Node(1, (byte)a, null, null);
                        //   vrcholy.Add(prvky[a]);
                    }
                    else
                    {
                        nodes[a].weight++;
                    }
                }

                for (int i = 0; i < 256; i++)
                {
                    if (nodes[i] != null)
                    {
                        if (rankedNodes.ContainsKey(nodes[i].weight))
                        {
                            rankedNodes[nodes[i].weight].Add(nodes[i]);
                        }
                        else rankedNodes.Add(nodes[i].weight, new List<Node>() { nodes[i] });
                    }
                }
                foreach (KeyValuePair<int, List<Node>> rankedNode in rankedNodes)
                {
                    rankedNode.Value.Sort();
                }
                return rankedNodes;
            }
        }

    }

    class Program
    {
        static SortedDictionary<int, List<Node>> rankedNodes;
        static Tree huffmanTree;
        //   static Stopwatch sw = new Stopwatch();

        static void Main(string[] args)
        {
            //     sw.Start();

            if (args.Length != 1)
            {
                Console.Write("Argument Error");
                Environment.Exit(0);
            }
            rankedNodes = Reader.ReadFile(args[0]);


            if ((rankedNodes != null) && (rankedNodes.Count != 0))
            {
                huffmanTree = new Tree(rankedNodes);
                huffmanTree.PrintTree();
                //Console.Write("\n");
                huffmanTree.PrintTreePrefixed();
                Console.Write("\n");
            }

            /*      sw.Stop();
                  string ExecutionTimeTaken = string.Format("Minutes :{0}\nSeconds :{1}\n Mili seconds :{2}", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.TotalMilliseconds);
                  Console.Write(ExecutionTimeTaken);
                  Console.ReadKey();

                  Console.ReadKey(); */
        }
    }
}