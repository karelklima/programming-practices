using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HuffmanskeKapkyOriginal;
using HuffmanCoding;

namespace cisteni_kodu_test
{
    [TestClass]
    public class ReaderUnitTest
    {
        [TestMethod]
        public void CheckOutput()
        {

            string originalOutput;
            string refactoredOutput;

            using (ConsoleRedirector cr = new ConsoleRedirector())
            {
                var vrcholy = Nacitacka.PrectiSoubor("test.txt");
                if ((vrcholy != null) && (vrcholy.Count != 0))
                {
                    var Huffman = new strom(vrcholy);
                    Huffman.VypisStrom();
                    //Console.Write("\n");
                    Huffman.VypisStrom2();
                    Console.Write("\n");
                }
                originalOutput = cr.ToString();
            }

            Console.Clear();

            using (ConsoleRedirector cr = new ConsoleRedirector())
            {
                //var rankedNodes = HuffmanCoding.Reader.ReadFile("test.txt");

                //if (rankedNodes.Count != 0)
                //{
                    //var huffmanTree = new HuffmanCoding.Tree(rankedNodes);
                    var huffmanTree = Tree.FromFile("test.txt");
                    //huffmanTree.PrintTree();
                    //Console.Write("\n");
                    huffmanTree.PrintTree();
                    Console.Write("\n");
                //}
                refactoredOutput = cr.ToString();
            }

            if (originalOutput.Equals(refactoredOutput))
                return; // ok

            throw new Exception();

        }

        private void Assert(bool p)
        {
            if (!p)
                throw new Exception();
        }
    }

    internal class ConsoleRedirector : IDisposable
    {
        private StringWriter _consoleOutput = new StringWriter();
        private TextWriter _originalConsoleOutput;
        public ConsoleRedirector()
        {
            this._originalConsoleOutput = Console.Out;
            Console.SetOut(_consoleOutput);
        }
        public void Dispose()
        {
            Console.SetOut(_originalConsoleOutput);
            Console.Write(this.ToString());
            this._consoleOutput.Dispose();
        }
        public override string ToString()
        {
            return this._consoleOutput.ToString();
        }
    }
}
