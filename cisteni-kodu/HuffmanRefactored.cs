using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HuffmanCoding
{
    // TODO comment
    using RankedNodesDictionary = System.Collections.Generic.SortedDictionary<int, List<Node>>;

    // TODO comment
    class Node : IComparable<Node>
    {
        public int Rank { get; set; }
        public byte Character { get; private set; }
        public Node LeftChildNode { get; private set; }
        public Node RightChildNode { get; private set; }
        
        private int _nodeIndex;

        private static int _lastNodeIndex;

        public Node(int rank, byte character, Node leftChildNode, Node rightChildNode)
        {
            Rank = rank;
            Character = character;
            LeftChildNode = leftChildNode;
            RightChildNode = rightChildNode;
            _nodeIndex = _lastNodeIndex;
            _lastNodeIndex++;
        }

        /// <summary>
        /// Kdyz nema jedineho syna vraci true.
        /// </summary>
        public bool IsLeaf
        {
            get { return LeftChildNode == null && RightChildNode == null; }
        }

        /// <summary>
        /// Zvetsi vahu vrcholu o zadany int, vraci upraveny vrchol.
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        public Node IncreaseRank(int rank)
        {
            Rank += rank;
            return this;
        }

        /// <summary>
        /// True o sobe vrchol rekne jestli bude v Huffmanskem strome nalevo od druheho vrcholu.
        /// </summary>
        /// <param name="otherNode"></param>
        /// <returns></returns>
        public bool IsNodeLeftward(Node otherNode)
        {
            if (otherNode.Rank > Rank)
                return true;

            if (otherNode.Rank < Rank)
                return false;

            //otherNode.rank == rank
            if (otherNode.IsLeaf && !(IsLeaf))
                return false;

            if (IsLeaf && !(otherNode.IsLeaf))
                return true;

            //otherNode.IsLeaf() == IsLeaf()
            var nodesAreLeafs = IsLeaf && otherNode.IsLeaf;

            if (nodesAreLeafs && (Character < otherNode.Character))
                return true;

            if (nodesAreLeafs && (Character > otherNode.Character))
                return false;

            //ranks are same, nodes aren't leafs (if symbols are same => algorithm/data is wrong)
            if (_nodeIndex < otherNode._nodeIndex)
                return true;

            return false;
        }

        public int CompareTo(Node otherNode)
        {
            if (this.Equals(otherNode))
                return 0;

            if (IsNodeLeftward(otherNode))
                return -1;

            return 1;
        }

        public static int SumRanks(Node firstNode, Node secondNode)
        {
            return firstNode.Rank + secondNode.Rank;
        }
    }

    class Tree
    {
        private Node _rootNode;
        private int _treeCount = 0;

        public Tree(RankedNodesDictionary rankedNodes)
        {
            Build(rankedNodes);
        }

        private void Build(RankedNodesDictionary rankedNodes)
        {
            List<Node> nodes;
            Node temp1;
            Node temp2;
            Node newNode;
            Node oddNode = null;
            var remainingNodes = 0;
            int rank;

            foreach (var rankedNode in rankedNodes)
            {
                remainingNodes += rankedNode.Value.Count;
            }

            if (remainingNodes != 1)
            {
                _treeCount = _treeCount + 1;
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
                            newNode = new Node(temp1.Rank + temp2.Rank, temp1.Character, temp1, temp2);
                        else 
                            newNode = new Node(temp1.Rank + temp2.Rank, temp1.Character, temp2, temp1);

                        if (rankedNodes.ContainsKey(newNode.Rank))
                            rankedNodes[newNode.Rank].Add(newNode);
                        else 
                            rankedNodes.Add(newNode.Rank, new List<Node>() { newNode });

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
                        newNode = new Node(oddNode.Rank + temp1.Rank, oddNode.Character, oddNode, temp1);
                    else 
                        newNode = new Node(temp1.Rank + oddNode.Rank, temp1.Character, temp1, oddNode);

                    if (rankedNodes.ContainsKey(newNode.Rank))
                        rankedNodes[newNode.Rank].Add(newNode);
                    else 
                        rankedNodes.Add(newNode.Rank, new List<Node>() { newNode });

                    remainingNodes--;

                    for (var i = 1; i < nodes.Count - 1; i++)
                    {
                        temp1 = nodes[i];
                        temp2 = nodes[++i];

                        if (temp1.IsNodeLeftward(temp2))
                            newNode = new Node(temp1.Rank + temp2.Rank, temp1.Character, temp1, temp2);
                        else 
                            newNode = new Node(temp1.Rank + temp2.Rank, temp1.Character, temp2, temp1);

                        if (rankedNodes.ContainsKey(newNode.Rank))
                        {
                            rankedNodes[newNode.Rank].Add(newNode);
                        }
                        else rankedNodes.Add(newNode.Rank, new List<Node>() { newNode });

                        remainingNodes--;
                    }
                    if (nodes.Count % 2 == 0)
                        oddNode = nodes[nodes.Count - 1];
                    else 
                        oddNode = null;
                }
                rankedNodes.Remove(rank);
            }
            _rootNode = rankedNodes[rankedNodes.Keys.ElementAt(0)][0];
        }

        public void PrintTree()
        {
            // VypisStrom(this.koren);
        }

        public void PrintTreePrefixed()
        {
            PrintTreePrefixed(this._rootNode, "");
        }

        public void PrintTreePrefixed(Node node, string prefix)
        {
            if (node.IsLeaf) {
                //if (!Char.IsControl (Convert.ToChar (node.Character))) //We cannot use it, because it uses UTF-16
                //32 - first printable char
                //126 - last printable char
                if ((node.Character >= 32) && (node.Character <= 126))//printable condition
                    Console.Write (" ['{0}':{1}]\n", (char)node.Character, node.Rank);
                else
                    Console.Write (" [{0}:{1}]\n", node.Character, node.Rank);
            } else {
                Console.Write ("{0,4} -+- ", node.Rank);
                prefix += "      ";
                PrintTreePrefixed (node.RightChildNode, prefix + "|  ");
                Console.Write ("{0}|\n", prefix);
                Console.Write ("{0}`- ", prefix);
                PrintTreePrefixed (node.LeftChildNode, prefix + "   ");
            }
        }
    }

    class Reader
    {
        private const int READ_FILE_BUFFER_SIZE = 16384;//16KB
        private const int SYMBOLS_COUNT = 256;//ascii

        //TODO Maybe divide this methods into two? Data reading + frequency calculation AND node merging into result
        public static RankedNodesDictionary ReadFile(string fileName)
        {
            using (var sourceFileStream = new FileStream (fileName, FileMode.Open, FileAccess.Read)) {

                //result
                var rankedNodes = new RankedNodesDictionary ();

                //read data & calculate ranks
                var nodes = new Node[SYMBOLS_COUNT];
                var buffer = new byte[READ_FILE_BUFFER_SIZE];

                var remainingBytes = sourceFileStream.Length;
                while (remainingBytes > 0) {
                    var readBytes = sourceFileStream.Read (buffer, 0, READ_FILE_BUFFER_SIZE);
                    remainingBytes -= readBytes;

                    for (var i = 0; i < readBytes; i++) {
                        var symbol = buffer [i];
                        if (nodes [symbol] == null)
                            nodes [symbol] = new Node (1, (byte)symbol, null, null);
                        else
                            nodes [symbol].Rank++;
                    }
                }

                //merge nodes
                for (var i = 0; i < nodes.Length; i++) {
                    if (nodes [i] != null) {
                        if (rankedNodes.ContainsKey (nodes [i].Rank))
                            rankedNodes [nodes [i].Rank].Add (nodes [i]);
                        else
                            rankedNodes.Add (nodes [i].Rank, new List<Node> () { nodes [i] });
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

            Console.ReadKey(true);

            /*      sw.Stop();
                  string ExecutionTimeTaken = string.Format("Minutes :{0}\nSeconds :{1}\n Mili seconds :{2}", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.TotalMilliseconds);
                  Console.Write(ExecutionTimeTaken);
                  Console.ReadKey();

                  Console.ReadKey(); */
        }
    }
}