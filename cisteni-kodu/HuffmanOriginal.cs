using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace HuffmanskeKapkyOriginal
{

    class vrchol : IComparable<vrchol>
    {
        public vrchol Psyn;
        public int vaha;
        public byte znak;
        public vrchol Lsyn;

        int stari;

        static int cisloVrchola;

        public vrchol(int vaha, byte znak, vrchol Lsyn, vrchol Psyn)
        {
            this.vaha = vaha;
            this.znak = znak;
            this.Lsyn = Lsyn;
            this.Psyn = Psyn;
            stari = cisloVrchola;
            cisloVrchola++;
        }

        /// <summary>
        /// Kdyz nema jedineho syna vraci true.
        /// </summary>
        /// <returns></returns>
        public bool JeList()
        {
            if ((Lsyn == null) && (Psyn == null))
            {
                return true;
            }
            else return false;
        }

        public static int SectiVahy(vrchol prvni, vrchol druhy)
        {
            return prvni.vaha + druhy.vaha;
        }

        /// <summary>
        /// Zvetsi vahu vrcholu o zadany int, vraci upraveny vrchol.
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        public vrchol ZvecVahu(int rank)
        {
            vaha += rank;
            return this;
        }

        /// <summary>
        /// True o sobe vrchol rekne jestli bude v Huffmanskem strome nalevo od druheho vrcholu.
        /// </summary>
        /// <param name="druhy"></param>
        /// <returns></returns>
        public bool BudeVrcholVlevo(vrchol druhy)
        {
            if (druhy.vaha > vaha)
            {
                return true;
            }
            else if (druhy.vaha < vaha)
            {
                return false;
            }
            else if (druhy.JeList() && !(JeList()))
            {
                return false;
            }
            else if (JeList() && !(druhy.JeList()))
            {
                return true;
            }
            else if ((JeList()) && (druhy.JeList()) && (znak < druhy.znak))
            {
                return true;
            }
            else if ((JeList()) && (druhy.JeList()) && (znak > druhy.znak))
            {
                return false;
            }
            else if (stari < druhy.stari)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #region IComparable Members

        public int CompareTo(vrchol obj)
        {
            if (this == obj)
            {
                return 0;
            }
            else if (BudeVrcholVlevo(obj))
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

    class strom
    {
        private vrchol koren;

        public strom(SortedDictionary<int, List<vrchol>> vrcholy)
        {
            postavStrom(vrcholy);
        }

        int pocetStromu = 0;

        private void postavStrom(SortedDictionary<int, List<vrchol>> HuffmanskyLes)
        {
            List<vrchol> seznam;
            vrchol pom1;
            vrchol pom3;
            vrchol novy;
            vrchol lichy = null;
            int ZbyvaZpracovat = 0;
            int rank;

            foreach (KeyValuePair<int, List<vrchol>> item in HuffmanskyLes)
            {
                ZbyvaZpracovat += item.Value.Count;
            }

            if (ZbyvaZpracovat != 1)
            {
                pocetStromu = pocetStromu + 1;
            }

            while (ZbyvaZpracovat != 1)
            {
                seznam = HuffmanskyLes[HuffmanskyLes.Keys.ElementAt(0)];
                rank = HuffmanskyLes.Keys.ElementAt(0);

                if (lichy == null)
                {
                    for (int i = 0; i < seznam.Count - 1; i++)
                    {
                        pom1 = seznam[i];
                        pom3 = seznam[++i];

                        if (pom1.BudeVrcholVlevo(pom3))
                        {
                            novy = new vrchol(pom1.vaha + pom3.vaha, pom1.znak, pom1, pom3);
                        }
                        else novy = new vrchol(pom1.vaha + pom3.vaha, pom1.znak, pom3, pom1);

                        if (HuffmanskyLes.ContainsKey(novy.vaha))
                        {
                            HuffmanskyLes[novy.vaha].Add(novy);
                        }
                        else HuffmanskyLes.Add(novy.vaha, new List<vrchol>() { novy });


                        ZbyvaZpracovat--;
                    }
                    if (seznam.Count % 2 == 1)
                    {
                        lichy = seznam[seznam.Count - 1];

                    }
                    else
                    {
                        lichy = null;
                    }

                }
                else
                {
                    pom1 = seznam[0];
                    if (lichy.BudeVrcholVlevo(pom1))
                    {
                        novy = new vrchol(lichy.vaha + pom1.vaha, lichy.znak, lichy, pom1);
                    }
                    else novy = new vrchol(pom1.vaha + lichy.vaha, pom1.znak, pom1, lichy);

                    if (HuffmanskyLes.ContainsKey(novy.vaha))
                    {
                        HuffmanskyLes[novy.vaha].Add(novy);
                    }
                    else HuffmanskyLes.Add(novy.vaha, new List<vrchol>() { novy });

                    ZbyvaZpracovat--;

                    for (int i = 1; i < seznam.Count - 1; i++)
                    {
                        pom1 = seznam[i];
                        pom3 = seznam[++i];

                        if (pom1.BudeVrcholVlevo(pom3))
                        {
                            novy = new vrchol(pom1.vaha + pom3.vaha, pom1.znak, pom1, pom3);
                        }
                        else novy = new vrchol(pom1.vaha + pom3.vaha, pom1.znak, pom3, pom1);

                        if (HuffmanskyLes.ContainsKey(novy.vaha))
                        {
                            HuffmanskyLes[novy.vaha].Add(novy);
                        }
                        else HuffmanskyLes.Add(novy.vaha, new List<vrchol>() { novy });

                        ZbyvaZpracovat--;
                    }
                    if (seznam.Count % 2 == 0)
                    {
                        lichy = seznam[seznam.Count - 1];
                    }
                    else lichy = null;
                }
                HuffmanskyLes.Remove(rank);
            }
            koren = HuffmanskyLes[HuffmanskyLes.Keys.ElementAt(0)][0];
        }

        public void VypisStrom()
        {
            // VypisStrom(this.koren);
        }

        public void VypisStrom2()
        {
            VypisStrom2(this.koren, "");
        }

        public void VypisStrom2(vrchol vrch, string pre)
        {
            bool bylVlevo = false;

            if (vrch.JeList())
            {
                if ((vrch.znak >= 32) && (vrch.znak <= 0x7E))
                {
                    Console.Write(" ['{0}':{1}]\n", (char)vrch.znak, vrch.vaha);
                    return;
                }
                else
                {
                    Console.Write(" [{0}:{1}]\n", vrch.znak, vrch.vaha);
                }
                return;
            }
            else
            {
                // bylVlevo = true;
            }

            if (!bylVlevo)
            {
                Console.Write("{0,4} -+- ", vrch.vaha);
                bylVlevo = true;
            }
            pre = pre + "      ";
            if (bylVlevo)
            {
                VypisStrom2(vrch.Psyn, pre + "|  ");
                Console.Write("{0}|\n", pre);
                Console.Write("{0}`- ", pre);
                VypisStrom2(vrch.Lsyn, pre + "   ");
            }
        }
    }

    class Nacitacka
    {
        private static FileStream vstup;

        public static bool OtevrSoubor(string nazev)
        {
            try
            {
                vstup = new FileStream(nazev, FileMode.Open, FileAccess.Read);
                if (!(vstup.CanRead))
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

        public static SortedDictionary<int, List<vrchol>> PrectiSoubor(string nazev)
        {

            if (!(OtevrSoubor(nazev))) return null;
            else
            {
                SortedDictionary<int, List<vrchol>> vrcholy = new SortedDictionary<int, List<vrchol>>();
                byte a = 0;

                vrchol[] prvky = new vrchol[256];
                byte[] bafr = new byte[0x4000];

                for (int i = 0; i < vstup.Length / 0x4000; i++)
                {
                    vstup.Read(bafr, 0, 16384);

                    for (int j = 0; j < 16384; j++)
                    {
                        a = bafr[j];
                        if (prvky[a] == null)
                        {
                            prvky[a] = new vrchol(1, (byte)a, null, null);
                            //   vrcholy.Add(prvky[a]);
                        }
                        else
                        {
                            prvky[a].vaha++;
                        }
                    }
                }

                for (int i = 0; i < vstup.Length % 0x4000; i++)
                {
                    a = (byte)vstup.ReadByte();
                    if (prvky[a] == null)
                    {
                        prvky[a] = new vrchol(1, (byte)a, null, null);
                        //   vrcholy.Add(prvky[a]);
                    }
                    else
                    {
                        prvky[a].vaha++;
                    }
                }

                for (int i = 0; i < 256; i++)
                {
                    if (prvky[i] != null)
                    {
                        if (vrcholy.ContainsKey(prvky[i].vaha))
                        {
                            vrcholy[prvky[i].vaha].Add(prvky[i]);
                        }
                        else vrcholy.Add(prvky[i].vaha, new List<vrchol>() { prvky[i] });
                    }
                }
                foreach (KeyValuePair<int, List<vrchol>> item in vrcholy)
                {
                    item.Value.Sort();
                }
                return vrcholy;
            }
        }

    }

    class Program
    {
        static SortedDictionary<int, List<vrchol>> vrcholy;
        static strom Huffman;
        //   static Stopwatch sw = new Stopwatch();

        static void Main(string[] args)
        {
            //     sw.Start();

            if (args.Length != 1)
            {
                Console.Write("Argument Error");
                Environment.Exit(0);
            }
            vrcholy = Nacitacka.PrectiSoubor(args[0]);


            if ((vrcholy != null) && (vrcholy.Count != 0))
            {
                Huffman = new strom(vrcholy);
                Huffman.VypisStrom();
                //Console.Write("\n");
                Huffman.VypisStrom2();
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