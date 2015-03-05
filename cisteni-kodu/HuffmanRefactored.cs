using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HuffmanCoding
{
    using TreeRankedNodes = System.Collections.Generic.SortedDictionary<int, List<Node>>;
    class Node : IComparable<Node>
    {
        public Node leftNode;
        public Node rightNode;
        public int rank;
        public byte symbol;

        private int nodeIndex;

        private static int lastNodeIndex;

        public Node(int weight, byte symbol, Node left, Node right)
        {
            this.rank = weight;
            this.symbol = symbol;
            this.leftNode = left;
            this.rightNode = right;
            nodeIndex = lastNodeIndex;
            lastNodeIndex++;
        }

        /// <summary>
        /// Kdyz nema jedineho syna vraci true.
        /// </summary>
        /// <returns></returns>
        public bool IsLeaf()
        {
            return leftNode == null && rightNode == null;
        }

        /// <summary>
        /// Zvetsi vahu vrcholu o zadany int, vraci upraveny vrchol.
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        public Node IncreaseWeight(int rank)
        {
            rank += rank;
            return this;
        }

        /// <summary>
        /// True o sobe vrchol rekne jestli bude v Huffmanskem strome nalevo od druheho vrcholu.
        /// </summary>
        /// <param name="druhy"></param>
        /// <returns></returns>
        public bool IsNodeLeftward(Node otherNode)
        {
            if (otherNode.rank > rank)
                return true;

            if (otherNode.rank < rank)
                return false;

            //otherNode.rank == rank
            if (otherNode.IsLeaf() && !(IsLeaf()))
                return false;

            if (IsLeaf() && !(otherNode.IsLeaf()))
                return true;

            //otherNode.IsLeaf() == IsLeaf()
            bool nodesAreLeafs = IsLeaf () && otherNode.IsLeaf ();

            if (nodesAreLeafs && (symbol < otherNode.symbol))
                return true;

            if (nodesAreLeafs && (symbol > otherNode.symbol))
                return false;

            //ranks are same, nodes aren't leafs (if symbols are same => algorithm/data is wrong)
            if (nodeIndex < otherNode.nodeIndex)
                return true;

            return false;
        }


        #region IComparable Members

        public int CompareTo(Node otherNode)
        {
            if (this.Equals (otherNode))
                return 0;

            if (IsNodeLeftward(otherNode))
                return -1;

            return 1;
        }

        #endregion

        public static int SumWeights(Node firstNode, Node secondNode)
        {
            return firstNode.rank + secondNode.rank;
        }
    }

    class Tree
    {
        private Node root;
        private int treeCount = 0;

        public Tree(TreeRankedNodes rankedNodes)
        {
            Build(rankedNodes);
        }

        private void Build(TreeRankedNodes rankedNodes)
        {
            List<Node> nodes;
            Node temp1;
            Node temp2;
            Node newNode;
            Node oddNode = null;
            int remainingNodes = 0;
            int rank;

            foreach (var rankedNode in rankedNodes)
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
                        temp1 = nodes[i];
                        temp2 = nodes[++i];

                        if (temp1.IsNodeLeftward(temp2))
                            newNode = new Node(temp1.rank + temp2.rank, temp1.symbol, temp1, temp2);
                        else 
                            newNode = new Node(temp1.rank + temp2.rank, temp1.symbol, temp2, temp1);

                        if (rankedNodes.ContainsKey(newNode.rank))
                            rankedNodes[newNode.rank].Add(newNode);
                        else 
                            rankedNodes.Add(newNode.rank, new List<Node>() { newNode });

                        remainingNodes--;
                    }
                    if (nodes.Count % 2 == 1)
                        oddNode = nodes[nodes.Count - 1];
                    else
                        oddNode = null;
                }
                else
                {
                    temp1 = nodes[0];
                    if (oddNode.IsNodeLeftward(temp1))
                        newNode = new Node(oddNode.rank + temp1.rank, oddNode.symbol, oddNode, temp1);
                    else 
                        newNode = new Node(temp1.rank + oddNode.rank, temp1.symbol, temp1, oddNode);

                    if (rankedNodes.ContainsKey(newNode.rank))
                        rankedNodes[newNode.rank].Add(newNode);
                    else 
                        rankedNodes.Add(newNode.rank, new List<Node>() { newNode });

                    remainingNodes--;

                    for (int i = 1; i < nodes.Count - 1; i++)
                    {
                        temp1 = nodes[i];
                        temp2 = nodes[++i];

                        if (temp1.IsNodeLeftward(temp2))
                            newNode = new Node(temp1.rank + temp2.rank, temp1.symbol, temp1, temp2);
                        else 
                            newNode = new Node(temp1.rank + temp2.rank, temp1.symbol, temp2, temp1);

                        if (rankedNodes.ContainsKey(newNode.rank))
                        {
                            rankedNodes[newNode.rank].Add(newNode);
                        }
                        else rankedNodes.Add(newNode.rank, new List<Node>() { newNode });

                        remainingNodes--;
                    }
                    if (nodes.Count % 2 == 0)
                        oddNode = nodes[nodes.Count - 1];
                    else 
                        oddNode = null;
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
            if (node.IsLeaf ()) {
                //if (!Char.IsControl (Convert.ToChar (node.symbol))) //We cannot use it, because it uses UTF-16
                //32 - first printable char
                //126 - last printable char
                if ((node.symbol >= 32) && (node.symbol <= 126))//printable condition
                    Console.Write (" ['{0}':{1}]\n", (char)node.symbol, node.rank);
                else
                    Console.Write (" [{0}:{1}]\n", node.symbol, node.rank);
            } else {
                Console.Write ("{0,4} -+- ", node.rank);
                prefix += "      ";
                PrintTreePrefixed (node.rightNode, prefix + "|  ");
                Console.Write ("{0}|\n", prefix);
                Console.Write ("{0}`- ", prefix);
                PrintTreePrefixed (node.leftNode, prefix + "   ");
            }
        }
    }

    class Reader
    {
        private const int READ_FILE_BUFFER_SIZE = 16384;//16KB
        private const int SYMBOLS_COUNT = 256;//ascii

        //TODO Maybe divide this methods into two? Data reading + frequency calculation AND node merging into result
        public static TreeRankedNodes ReadFile(string fileName)
        {
            using (var sourceFileStream = new FileStream (fileName, FileMode.Open, FileAccess.Read)) {

                //result
                var rankedNodes = new TreeRankedNodes ();

                //read data & calculate ranks
                Node[] nodes = new Node[SYMBOLS_COUNT];
                byte[] buffer = new byte[READ_FILE_BUFFER_SIZE];

                long remainingBytes = sourceFileStream.Length;
                while (remainingBytes > 0) {
                    int readBytes = sourceFileStream.Read (buffer, 0, READ_FILE_BUFFER_SIZE);
                    remainingBytes -= readBytes;

                    for (int i = 0; i < readBytes; i++) {
                        byte symbol = buffer [i];
                        if (nodes [symbol] == null)
                            nodes [symbol] = new Node (1, (byte)symbol, null, null);
                        else
                            nodes [symbol].rank++;
                    }
                }

                //merge nodes
                for (int i = 0; i < nodes.Length; i++) {
                    if (nodes [i] != null) {
                        if (rankedNodes.ContainsKey (nodes [i].rank))
                            rankedNodes [nodes [i].rank].Add (nodes [i]);
                        else
                            rankedNodes.Add (nodes [i].rank, new List<Node> () { nodes [i] });
                    }
                }

                foreach (var rankedNode in rankedNodes) {
                    rankedNode.Value.Sort ();
                }

                return rankedNodes;
            }
        }
    }
}

namespace MFFUK
{
    class Program
    {
        //   static Stopwatch sw = new Stopwatch();

        static void Main(string[] args)
        {
            //     sw.Start();

            if (args.Length != 1)
            {
                Console.Write("Argument Error");
                Environment.Exit(0);
            }
            var rankedNodes = HuffmanCoding.Reader.ReadFile(args[0]);

            if (rankedNodes.Count != 0)
            {
                var huffmanTree = new HuffmanCoding.Tree(rankedNodes);
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