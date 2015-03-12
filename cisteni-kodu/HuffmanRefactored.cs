using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HuffmanCoding
{
    using RankedNodesDictionary = SortedDictionary<int, List<Node>>;

    /// <summary>
    /// A node in Huffman tree
    /// </summary>
    class Node : IComparable<Node>
    {
        /// <summary>
        /// Character to be used in parent nodes
        /// </summary>
        public const char PARENT_CHARACTER = '\0';

        /// <summary>
        /// Input character associated with current node or default
        /// parent character if the node is not a leaf
        /// </summary>
        public char Character { get; private set; }

        /// <summary>
        /// Number of occurrences of all characters in leaves under this node
        /// </summary>
        public int Rank { get; private set; }

        /// <summary>
        /// Left child node or empty if current node is a leaf
        /// </summary>
        public Node LeftChildNode { get; private set; }

        /// <summary>
        /// Right child node or empty if current node is a leaf
        /// </summary>
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
        /// Special constructor to construct leaves in Huffman tree
        /// </summary>
        /// <param name="character">Nodes character</param>
        /// <param name="rank">Number of occurrences</param>
        public Node(char character, int rank)
            : this(character, rank, null, null)
        {
            if (rank < 1)
                throw new ArgumentException("Rank must be > 0", "rank");
        }

        /// <summary>
        /// Special constructor to construct parents in Huffman tree. Inserts
        /// default character and calculates the total rank.
        /// </summary>
        /// <param name="leftChildNode">Left child node</param>
        /// <param name="rightChildNode">Right child node</param>
        public Node(Node leftChildNode, Node rightChildNode)
            : this(PARENT_CHARACTER, SumRanks(leftChildNode, rightChildNode),
            leftChildNode, rightChildNode)
        {
            if (leftChildNode == null)
                throw new ArgumentNullException("leftChildNode");
            if (rightChildNode == null)
                throw new ArgumentNullException("rightChildNode");
        }

        /// <summary>
        /// Private default constructor, constructs Huffman tree node
        /// </summary>
        /// <param name="character">Character or default parent character
        /// </param>
        /// <param name="rank">Total number of occurrences of characters in
        /// leaves under this node</param>
        /// <param name="leftChildNode">Left child node if any</param>
        /// <param name="rightChildNode">Right child node if any</param>
        private Node(char character, int rank,
            Node leftChildNode, Node rightChildNode)
        {
            Character = character;
            Rank = rank;
            LeftChildNode = leftChildNode;
            RightChildNode = rightChildNode;
            _nodeIndex = _nodeInstancesCount++;
        }

        /// <summary>
        /// True if the node does not have any children.
        /// </summary>
        public bool IsLeaf
        {
            get { return LeftChildNode == null && RightChildNode == null; }
        }

        /// <summary>
        /// Calculates the position of current node in Huffman tree relative
        /// to a node passed as a parameter.
        /// Returns true if current node is left of the one in the argument.
        /// </summary>
        /// <param name="otherNode">Other node to be compared</param>
        /// <returns>True if current node is left of the other node, false
        /// otherwise</returns>
        public bool IsLeftOf(Node otherNode)
        {
            if (otherNode == null)
                throw new ArgumentNullException("otherNode");

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

        /// <summary>
        /// Compares position of current node in a Huffman tree with another
        /// node.
        /// </summary>
        /// <param name="otherNode">Another node to compare</param>
        /// <returns>0 if nodes are equal, -1 if the current node is left of
        /// the other one, 1 otherwise</returns>
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
        /// <param name="rankIncrease">Positive integer to increase current
        /// node rank</param>
        public void IncreaseRank(int rankIncrease)
        {
            if (rankIncrease < 1)
                throw new ArgumentException("Rank increase < 1", "rankIncrease");
            
            Rank += rankIncrease;
        }

        /// <summary>
        /// Sums ranks of two nodes
        /// </summary>
        /// <param name="firstNode">One node</param>
        /// <param name="secondNode">Other node</param>
        /// <returns>Total rank of both nodes</returns>
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
        /// <summary>
        /// Smallest ASCII character number to be printed as a character
        /// </summary>
        private const int MIN_PRINT_CHAR = 32;

        /// <summary>
        /// Largest ASCII character number to be printed as a character
        /// </summary>
        private const int MAX_PRINT_CHAR = 126;

        /// <summary>
        /// Holds the root node of Huffman tree
        /// </summary>
        private Node _rootNode;

        /// <summary>
        /// Tree counter
        /// </summary>
        private static int _treeCount;

        /// <summary>
        /// Constructs Huffman tree from given file and returns it.
        /// Throws exceptions associated with IO operations!
        /// See Reader.GetCharCountsFromFile() for details.
        /// </summary>
        /// <param name="fileName">Text file to read</param>
        /// <returns>Huffman tree or null if the file is empty</returns>
        public static Tree FromFile(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            var charCounts = FileReader.GetCharacterCounts(fileName);

            var rankedNodes = BuildRankedNodes(charCounts);

            foreach (var rankedNode in rankedNodes)
            {
                rankedNode.Value.Sort();
            }

            return rankedNodes.Count > 0 ? new Tree(rankedNodes) : null;
        }

        /// <summary>
        /// Private constructor. Accepts list of ranked nodes and constructs
        /// the Huffman tree
        /// </summary>
        /// <param name="rankedNodes">Dictionary of ranks and lists of
        /// corresponding nodes</param>
        private Tree(RankedNodesDictionary rankedNodes)
        {
            BuildTree(rankedNodes);
        }

        /// <summary>
        /// Builds Huffman tree from given ranked nodes lists.
        /// </summary>
        /// <param name="rankedNodes">Dictionary of ranks and lists of
        /// corresponding nodes</param>
        private void BuildTree(RankedNodesDictionary rankedNodes)
        {
            if (rankedNodes == null)
                throw new ArgumentNullException("rankedNodes");

            // Total number of nodes in the ranked dictionary
            var remainingNodesCount = rankedNodes
                .Sum(rankedNodesPair => rankedNodesPair.Value.Count);

            if (remainingNodesCount > 0)
                _treeCount++;

            // Placeholder to transport odd nodes to be inserted in next cycle
            Node oddNode = null;

            while (remainingNodesCount > 1)
            {
                var rank = rankedNodes.Keys.ElementAt(0);
                var nodes = rankedNodes[rank];
                
                if (oddNode != null)
                    nodes.Insert(0, oddNode);
                // Insert nodes into the tree and fetch the odd node if any
                oddNode = InsertNodesToRankedNodes(rankedNodes, nodes);
                // For each pair of nodes one parent was inserted
                remainingNodesCount -= nodes.Count / 2;

                rankedNodes.Remove(rank);
            }

            _rootNode = rankedNodes[rankedNodes.Keys.ElementAt(0)][0];
        }

        /// <summary>
        /// Takes pairs of nodes from input list, creates their parent node
        /// and inserts it to the Huffman tree. Returns last node if number
        /// of nodes in list is odd.
        /// </summary>
        /// <param name="rankedNodes">Dictionary of ranks and lists of
        /// corresponding nodes</param>
        /// <param name="nodes">List of nodes to be inserted</param>
        /// <returns>Last odd node or null</returns>
        private static Node InsertNodesToRankedNodes(RankedNodesDictionary
            rankedNodes, IList<Node> nodes)
        {
            if (rankedNodes == null)
                throw new ArgumentNullException("rankedNodes");
            if (nodes == null)
                throw new ArgumentNullException("nodes");

            // Iterates over pairs of nodes
            for (var i = 0; i < nodes.Count - 1; i += 2)
            {
                var newNode = CreateParentNode(nodes[i], nodes[i + 1]);
                InsertNodeToRankedNodes(rankedNodes, newNode);
            }

            // Returns last odd node if the count is not even
            return nodes.Count % 2 == 1 ? nodes.Last() : null;
        }

        /// <summary>
        /// Inserts node to ranked dictionary with its rank as a key.
        /// </summary>
        /// <param name="rankedNodes">Dictionary of ranks and lists of
        /// corresponding nodes</param>
        /// <param name="newNode">Node to be inserted</param>
        private static void InsertNodeToRankedNodes(RankedNodesDictionary
            rankedNodes, Node newNode)
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
        /// Creates parent node in Huffman tree with two input nodes as
        /// children.
        /// </summary>
        /// <param name="oddNode">One child node</param>
        /// <param name="evenNode">Other child node</param>
        /// <returns>New parent node of the nodes provided</returns>
        private static Node CreateParentNode(Node oddNode, Node evenNode)
        {
            if (oddNode == null)
                throw new ArgumentNullException("oddNode");
            if (evenNode == null)
                throw new ArgumentNullException("evenNode");

            return oddNode.IsLeftOf(evenNode)
                ? new Node(oddNode, evenNode)
                : new Node(evenNode, oddNode);
        }

        /// <summary>
        /// Builds ranked nodes dictionary from character frequencies list.
        /// Ranked nodes dictionary is a dictionary of ranks as keys and lists
        /// of corresponding nodes (with this rank) as values.
        /// </summary>
        /// <param name="charCounts">List of number of character occurrences
        /// indexed by character ASCII code</param>
        /// <returns>Dictionary of ranks as keys and lists of corresponding
        /// nodes as values</returns>
        private static RankedNodesDictionary BuildRankedNodes(IEnumerable<int>
            charCounts)
        {
            var rankedNodes = new RankedNodesDictionary();

            // Foreach explanation: 
            // [...,'a','b','c','d',...] - indexes
            // [..., 9,  3,  0,  7,...] - values
            // Convert to struct {Rank = value, Character = index}
            // Filter list by Rank > 0
            foreach (var it in charCounts
                .Select((rank, character) =>
                    new { Rank = rank, Character = character})
                .Where(it => it.Rank > 0))
            {
                var newNode = new Node((char) it.Character, it.Rank);
                InsertNodeToRankedNodes(rankedNodes, newNode);
            }
            return rankedNodes;
        }

        /// <summary>
        /// Prints Huffman tree into the console
        /// </summary>
        public void PrintTree()
        {
            PrintBranchPrefixed(_rootNode, "");
        }

        /// <summary>
        /// Prints a branch of a Huffman tree into the console
        /// </summary>
        /// <param name="node">Root of the branch to be printed</param>
        /// <param name="prefix">Prefix to carry display information from
        /// parent node</param>
        private static void PrintBranchPrefixed(Node node, string prefix)
        {
            if (node.IsLeaf) {
                if ((node.Character >= MIN_PRINT_CHAR)
                    && (node.Character <= MAX_PRINT_CHAR))
                    Console.Write(" ['{0}':{1}]\n", node.Character,
                        node.Rank);
                else
                    Console.Write(" [{0}:{1}]\n", (int)node.Character, node.Rank);
            } else {
                Console.Write("{0,4} -+- ", node.Rank);
                prefix += "      ";
                PrintBranchPrefixed(node.RightChildNode, prefix + "|  ");
                Console.Write("{0}|\n{0}`- ", prefix);
                PrintBranchPrefixed(node.LeftChildNode, prefix + "   ");
            }
        }

    }

    /// <summary>
    /// File reader for Huffman tree. Reads character occurrences in a file.
    /// </summary>
    class FileReader
    {
        /// <summary>
        /// Size of a reading buffer
        /// </summary>
        private const int READ_BUFFER_SIZE = 16384; // 16KB

        /// <summary>
        /// Total number of ASCII characters supported
        /// </summary>
        private const int CHARS_COUNT = 256; // ASCII

        /// <summary>
        /// Reads given file and returns numbers of character occurrences
        /// inside it.
        /// Throws exceptions!
        /// </summary>
        /// <param name="fileName">Name of a text file to read</param>
        /// <returns>List of number of character occurrences
        /// indexed by character ASCII code</returns>
        public static List<int> GetCharacterCounts(string fileName)
        {
            using (var fileStream = new FileStream(fileName,
                FileMode.Open, FileAccess.Read)) {

                //result
                var charCounts = new int[CHARS_COUNT];

                //read data & calculate ranks
                var buffer = new byte[READ_BUFFER_SIZE];

                int readBytes;
                do
                {
                    readBytes = fileStream.Read(buffer, 0,
                        READ_BUFFER_SIZE);

                    for (var i = 0; i < readBytes; i++)
                    {
                        charCounts[buffer[i]]++;
                    }
                } while (readBytes > 0);

                return new List<int>(charCounts);
            }
        }
    }
}

namespace HuffmanCodingProgram
{
    /// <summary>
    /// Huffman coding console application
    /// </summary>
    class Program
    {
        /// <summary>
        /// Application entry point
        /// </summary>
        /// <param name="args">Expects one parameter - a name of a file to
        /// read and construct and print the Huffman tree from</param>
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("This program expects exactly one argument"
                    + " - an input file name");
            }
            else
            {
                try
                {
                    var tree = HuffmanCoding.Tree.FromFile(args[0]);
                    if (tree != null)
                    {
                        tree.PrintTree();
                        Console.Write("\n");
                    }
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("An error occurred, input file was"
                        + " not found!");
                }
                catch (Exception)
                {
                    // Catch all other possible exceptions
                    Console.Write("An error occurred, possibly a problem"
                        + " with input file!");
                }
            }

            Console.ReadKey(true);

        }
    }
}