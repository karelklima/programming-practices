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
        public byte Character { get; private set; }
        public int Rank { get; set; }
        public Node LeftChildNode { get; private set; }
        public Node RightChildNode { get; private set; }
        
        /// <summary>
        /// Holds an order number of an instance beginning with zero
        /// </summary>
        private readonly int _nodeIndex;
        /// <summary>
        /// Holds total number of instances created
        /// </summary>
        private static int _nodeInstancesCount;

        /// <summary>
        /// Default constructor, constructs Huffman tree node
        /// </summary>
        /// <param name="character"></param>
        /// <param name="rank"></param>
        /// <param name="leftChildNode"></param>
        /// <param name="rightChildNode"></param>
        public Node(byte character, int rank, Node leftChildNode, Node rightChildNode)
        {
            Character = character;
            Rank = rank;
            LeftChildNode = leftChildNode;
            RightChildNode = rightChildNode;
            _nodeIndex = _nodeInstancesCount++;
        }

        /// <summary>
        /// Special constructor to construct leaves in Huffman tree
        /// </summary>
        /// <param name="character"></param>
        public Node(byte character) : this(character, 1, null, null)
        {
        }

        /// <summary>
        /// TODO Kdyz nema jedineho syna vraci true.
        /// </summary>
        public bool IsLeaf
        {
            get { return LeftChildNode == null && RightChildNode == null; }
        }

        /// <summary>
        /// TODO proc to vraci this? blby API
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
        /// Calculates the position of current node in Huffman tree relative to a node passed as a parameter.
        /// Returns true if current node is left of the one in the argument.
        /// True o sobe vrchol rekne jestli bude v Huffmanskem strome nalevo od druheho vrcholu.
        /// </summary>
        /// <param name="otherNode"></param>
        /// <returns></returns>
        public bool IsLeftOf(Node otherNode)
        {
            if (otherNode == null)
                throw new ArgumentNullException("otherNode");

            if (Character == otherNode.Character)
            {
                // Characters must differ (if the algorithm is correct)
                throw new ArgumentException("Characters of both Huffman tree nodes must differ", "otherNode");
            }

            if (Rank != otherNode.Rank)
            {
                // Nodes do not have the same Rank
                return Rank < otherNode.Rank;
            }

            if (IsLeaf != otherNode.IsLeaf)
            {
                // One node is a leaf, the other is not
                return IsLeaf;
            }

            if (IsLeaf)
            {
                // Both this and otherNode are leaves
                return Character < otherNode.Character;
            }

            // Ranks are the same and nodes are not leaves
            return _nodeIndex < otherNode._nodeIndex;
        }

        public int CompareTo(Node otherNode)
        {
            if (otherNode == null)
                throw new ArgumentNullException("otherNode");

            if (Equals(otherNode))
                return 0;

            if (IsLeftOf(otherNode))
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
        private int _treeCount;

        public Tree(RankedNodesDictionary rankedNodes)
        {
            BuildTree(rankedNodes);
        }

        /// <summary>
        /// Builds Huffman tree from given ranked nodes lists.
        /// </summary>
        /// <param name="rankedNodes"></param>
        private void BuildTree(RankedNodesDictionary rankedNodes)
        {
            if (rankedNodes == null)
                throw new ArgumentNullException("rankedNodes");

            var remainingNodesCount = rankedNodes.Sum(rankedNodesPair => rankedNodesPair.Value.Count);

            if (remainingNodesCount != 1)
                _treeCount++; // TODO nema to byt nula?

            Node oddNodePlaceholder = null;

            while (remainingNodesCount > 1)
            {
                var rank = rankedNodes.Keys.ElementAt(0);
                var nodes = rankedNodes[rank];
                
                if (oddNodePlaceholder != null)
                    nodes.Insert(0, oddNodePlaceholder);

                oddNodePlaceholder = InsertNodePairsToRankedNodes(rankedNodes, nodes, oddNodePlaceholder != null);
                remainingNodesCount -= nodes.Count / 2;

                rankedNodes.Remove(rank);
            }

            _rootNode = rankedNodes[rankedNodes.Keys.ElementAt(0)][0];
        }

        /// <summary>
        /// TODO prejmenovat na InsertNodesToRankedNodes???
        /// Takes pairs of nodes from input list, creates their parent node and inserts it
        /// to the Huffman tree. Returns last node if number of nodes in list is odd.
        /// </summary>
        /// <param name="rankedNodes"></param>
        /// <param name="nodes"></param>
        /// <param name="previousOddNodeIndicator"></param>
        /// <returns>Last odd node or null</returns>
        private static Node InsertNodePairsToRankedNodes(RankedNodesDictionary rankedNodes, IList<Node> nodes, bool previousOddNodeIndicator)
        {
            if (rankedNodes == null)
                throw new ArgumentNullException("rankedNodes");
            if (nodes == null)
                throw new ArgumentNullException("nodes");

            // Iterates over pairs of nodes
            for (var i = 0; i < nodes.Count - 1; i += 2)
            {
                var newNode = CreateParentNode(nodes[i], nodes[i + 1], !previousOddNodeIndicator);
                InsertNodeToRankedNodes(rankedNodes, newNode);
            }

            // Returns last odd node if the count is not even
            return nodes.Count % 2 == 1 ? nodes.Last() : null;
        }

        /// <summary>
        /// Inserts node to ranked dictionary with its rank as a key.
        /// </summary>
        /// <param name="rankedNodes"></param>
        /// <param name="newNode"></param>
        private static void InsertNodeToRankedNodes(RankedNodesDictionary rankedNodes, Node newNode)
        {
            if (rankedNodes == null)
                throw new ArgumentNullException("rankedNodes");
            if (newNode == null)
                throw new ArgumentNullException("newNode");

            if (rankedNodes.ContainsKey(newNode.Rank))
                rankedNodes[newNode.Rank].Add(newNode);
            else
                rankedNodes.Add(newNode.Rank, new List<Node> { newNode });
        }

        /// <summary>
        /// Creates parent node in Huffman tree with two input nodes as children.
        /// </summary>
        /// <param name="oddNode"></param>
        /// <param name="evenNode"></param>
        /// <param name="forceOddCharacter">Force use of the character of the odd node.</param>
        /// <returns></returns>
        private static Node CreateParentNode(Node oddNode, Node evenNode, bool forceOddCharacter)
        {
            if (oddNode == null)
                throw new ArgumentNullException("oddNode");
            if (evenNode == null)
                throw new ArgumentNullException("evenNode");

            var character = (oddNode.IsLeftOf(evenNode) || forceOddCharacter)  ? oddNode.Character : evenNode.Character;
            var rank = Node.SumRanks(oddNode, evenNode);
            
            return oddNode.IsLeftOf(evenNode)
                ? new Node(character, rank, oddNode, evenNode)
                : new Node(character, rank, evenNode, oddNode);
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
                            nodes [symbol] = new Node ((byte)symbol, 1, null, null);
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