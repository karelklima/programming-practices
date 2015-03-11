using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HuffmanCoding
{
    using RankedNodesDictionary = SortedDictionary<int, List<Node>>;

    /// <summary>
    /// A node in huffman tree
    /// </summary>
    class Node : IComparable<Node>
    {
        public byte Character { get; private set; }
        public int Rank { get; private set; }
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
            if (rank < 1)
                throw new ArgumentException("Rank must be greater than one", "rank");
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
        /// True if the node does not have any children.
        /// </summary>
        public bool IsLeaf
        {
            get { return LeftChildNode == null && RightChildNode == null; }
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
     
        /// <summary>
        /// Increases node's rank by the given number
        /// </summary>
        /// <param name="rankIncrease"></param>
        /// <returns></returns>
        public void IncreaseRank(int rankIncrease)
        {
            if (rankIncrease < 1)
                throw new ArgumentException("Rank increase must not be negative", "rankIncrease");
            
            Rank += rankIncrease;
        }

        /// <summary>
        /// Sums ranks of two nodes.
        /// </summary>
        /// <param name="firstNode"></param>
        /// <param name="secondNode"></param>
        /// <returns></returns>
        public static int SumRanks(Node firstNode, Node secondNode)
        {
            return firstNode.Rank + secondNode.Rank;
        }
    }

    /// <summary>
    /// Huffman tree
    /// </summary>
    class Tree
    {
        private const int MIN_PRINT_CHAR = 32;
        private const int MAX_PRINT_CHAR = 126;

        private Node _rootNode;
        private int _treeCount;

        /// <summary>
        /// Constructs Huffman tree from given file and returns it.
        /// Throws exceptions associated with IO operations! See Reader.ReadFile() for details.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Huffman tree or null if the file is empty</returns>
        public static Tree FromFile(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            var charCounts = Reader.GetCharCountsFromFile(fileName);

            var rankedNodes = BuildRankedNodes(charCounts);

            foreach (var rankedNode in rankedNodes)
            {
                rankedNode.Value.Sort();
            }

            return rankedNodes.Count > 0 ? new Tree(rankedNodes) : null;
        }

        private Tree(RankedNodesDictionary rankedNodes)
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
                _treeCount++;

            Node oddNodePlaceholder = null;

            while (remainingNodesCount > 1)
            {
                var rank = rankedNodes.Keys.ElementAt(0);
                var nodes = rankedNodes[rank];
                
                if (oddNodePlaceholder != null)
                    nodes.Insert(0, oddNodePlaceholder);

                oddNodePlaceholder = InsertNodesToRankedNodes(rankedNodes, nodes, oddNodePlaceholder != null);
                remainingNodesCount -= nodes.Count / 2;

                rankedNodes.Remove(rank);
            }

            _rootNode = rankedNodes[rankedNodes.Keys.ElementAt(0)][0];
        }

        /// <summary>
        /// Takes pairs of nodes from input list, creates their parent node and inserts it
        /// to the Huffman tree. Returns last node if number of nodes in list is odd.
        /// </summary>
        /// <param name="rankedNodes"></param>
        /// <param name="nodes"></param>
        /// <param name="previousOddNodeIndicator"></param>
        /// <returns>Last odd node or null</returns>
        private static Node InsertNodesToRankedNodes(RankedNodesDictionary rankedNodes, IList<Node> nodes, bool previousOddNodeIndicator)
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

        private static RankedNodesDictionary BuildRankedNodes(IEnumerable<int> charCounts)
        {
            var rankedNodes = new RankedNodesDictionary();
            foreach (var it in charCounts
                .Select((rank, character) => new { Rank = rank, Character = character})
                .Where(it => it.Rank > 0))
            {
                var newNode = new Node((byte) it.Character, it.Rank, null, null);
                InsertNodeToRankedNodes(rankedNodes, newNode);
            }
            return rankedNodes;
        }

        public void PrintTree()
        {
            // VypisStrom(this.koren);
        }

        public void PrintTreePrefixed()
        {
            PrintTreePrefixed(_rootNode, "");
        }

        public void PrintTreePrefixed(Node node, string prefix)
        {
            if (node.IsLeaf) {
                if ((node.Character >= MIN_PRINT_CHAR) && (node.Character <= MAX_PRINT_CHAR)) //printable condition
                    Console.Write(" ['{0}':{1}]\n", (char)node.Character, node.Rank);
                else
                    Console.Write(" [{0}:{1}]\n", node.Character, node.Rank);
            } else {
                Console.Write("{0,4} -+- ", node.Rank);
                prefix += "      ";
                PrintTreePrefixed(node.RightChildNode, prefix + "|  ");
                Console.Write("{0}|\n{0}`- ", prefix);
                PrintTreePrefixed(node.LeftChildNode, prefix + "   ");
            }
        }

    }

    /// <summary>
    /// Reader for Huffman tree. Reads character occurrences in a file.
    /// </summary>
    class Reader
    {
        private const int READ_FILE_BUFFER_SIZE = 16384; //16KB
        private const int CHARS_COUNT = 256; //ascii

        /// <summary>
        /// Reads given file and returns numbers of character occurrences inside it.
        /// Throws exceptions!
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<int> GetCharCountsFromFile(string fileName)
        {
            using (var sourceFileStream = new FileStream (fileName, FileMode.Open, FileAccess.Read)) {

                //result
                var charCounts = new int[CHARS_COUNT];

                //read data & calculate ranks
                var buffer = new byte[READ_FILE_BUFFER_SIZE];

                var remainingBytes = sourceFileStream.Length;
                while (remainingBytes > 0) {
                    var readBytes = sourceFileStream.Read (buffer, 0, READ_FILE_BUFFER_SIZE);
                    remainingBytes -= readBytes;

                    for (var i = 0; i < readBytes; i++) {
                        charCounts[buffer[i]]++;
                    }
                }

                return new List<int>(charCounts);
            }
        }
    }
}

namespace HuffmanCodingProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("This program expects exactly one argument - an input file name");
            }
            else
            {
                try
                {
                    var tree = HuffmanCoding.Tree.FromFile(args[0]);
                    if (tree != null)
                    {
                        tree.PrintTreePrefixed();
                        Console.Write("\n");
                    }
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("An error occurred, input file was not found!");
                }
                catch (FileLoadException)
                {
                    Console.Write("An error occurred, possibly a problem with input file!");
                }
            }

            Console.ReadKey(true);

        }
    }
}