using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static TheLargestSubgraphWithoutTrianglesAndQuadrilaterals.Graph;
using TheLargestSubgraphWithoutTrianglesAndQuadrilaterals;

namespace TheLargestSubgraphWithoutTrianglesAndQuadrilaterals
{
    static class Shuffler
    {
        public static void Shuffle<T>(this Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
   
    public class Graph
    {
        public int V, E;

        public Edge[] edgeList;

        public Verticle[] verticleList;

        public Graph(int v, int e)
        {
            V = v;
            E = e;
            verticleList = new Verticle[V];
            edgeList = new Edge[E];
            for (int i = 0; i < e; ++i)
                edgeList[i] = new Edge();
        }

        public class Verticle
        {
            public int x_coord, y_coord;
        }

        public class Edge : IComparable<Edge>
        {
            public int src, dest, weight;

            public int CompareTo(Edge compareEdge)
            {
                return  compareEdge.weight - weight;
            }
        }
        public void ReadFile(string fileName)
        {
            using var reader = new StreamReader(fileName);
            string line;
            reader.ReadLine();
            //V = Convert.ToInt32(Regex.Match(line, @"\d+").Value);
            // E = (V * (V - 1)) / 2;
            int i = 0;
            while ((line = reader.ReadLine()) != null)
            {
                string[] tokens = line.Split();
                verticleList[i] = new Verticle
                {
                    x_coord = int.Parse(tokens[0]),
                    y_coord = int.Parse(tokens[1])
                };
                i++;
            }
        }

        public static int CalcVertDistance(Verticle vert1, Verticle vert2)
        {
            return Math.Abs(vert1.x_coord - vert2.x_coord) + Math.Abs(vert1.y_coord - vert2.y_coord);
        }

        // Главный метод
        public void Solver(string name)
        {
            List<Edge> myList = new List<Edge>();

            //Random rnd = new Random();
            //Edge[] MyRandomArray = edgeList.OrderBy(x => rnd.Next()).ToArray();
            Array.Sort(edgeList);// Сортируем по убыванию ребра 

            int i = 1;
            var k = ConsoleKey.Enter;
            ConsoleKeyInfo cki;
            DateTime begin = DateTime.Now;
            //------------------------Сначала строим дерево у которого инцедентность каждой вершины 2 (кроме листьев)---------------------------------------------------------
            List<int> generalVertex = new List<int>();
            List<int> localVertex = new List<int>();

            myList.Add(edgeList[0]);
            generalVertex.Add(edgeList[0].src);
            generalVertex.Add(edgeList[0].dest);
            localVertex.Add(edgeList[0].src);
            localVertex.Add(edgeList[0].dest);

            while (generalVertex.Count != V)
            {
                List<int> localVertex2 = new List<int>();
                foreach (int vert in localVertex)
                {
                    int count = 0;
                    foreach (Edge edge in edgeList)
                    {
                        if (edge.src == vert && !generalVertex.Contains(edge.dest))
                        {
                            if (!myList.Contains(edge))
                            {
                                myList.Add(edge);
                                generalVertex.Add(edge.dest);
                                localVertex2.Add(edge.dest);
                                if (count++ == 2)
                                    break;
                                continue;
                            }                           
                        }

                        if (edge.dest == vert && !generalVertex.Contains(edge.src))
                        {
                            if (!myList.Contains(edge))
                            {
                                myList.Add(edge);
                                generalVertex.Add(edge.src);
                                localVertex2.Add(edge.src);
                                if (count++ == 2)
                                    break;
                            }
                        }
                    }
                }
                localVertex = new List<int>(localVertex2);
            }

            //-------------------------Обычный проход (но уже по меньшему числу ребер, так как некоторые уже есть в списке)--------------------------------------------------------

            foreach (Edge edge in edgeList)
            {
                if (!myList.Contains(edge))
                {
                    myList.Add(edge);

                    Dictionary<int, List<int>> adj = new Dictionary<int, List<int>>();
                    foreach (Edge e in myList)
                    {
                        if (!adj.ContainsKey(e.src))
                            adj.Add(e.src, new List<int>());
                        if (!adj.ContainsKey(e.dest))
                            adj.Add(e.dest, new List<int>());
                        adj[e.src].Add(e.dest);
                        adj[e.dest].Add(e.src);
                    }

                    if (V3.GraphChecker.HasCycle(adj)) // если нашли цикл на трех или четырех вершинах, то возвращаем true
                    {
                        myList.RemoveAt(myList.Count - 1);
                    }
                    if ((DateTime.Now - begin).TotalSeconds >= 60)
                    {
                        begin = DateTime.Now;
                        Console.WriteLine("file № " + V + "__" + begin.ToLongTimeString() + "__" + Math.Round((double)i / this.edgeList.Length * 100, 0) + "%__" + i + "/" + this.edgeList.Length);
                    }
                    i++;
                    if (Console.KeyAvailable)
                    {
                        cki = Console.ReadKey();
                        if (cki.Modifiers.HasFlag(ConsoleModifiers.Control) && cki.Key == k)
                            break;
                    }
                }
            }

            //---------------------------------------------------Результат---------------------------------------------------
            Dictionary<int, int> verts = new Dictionary<int, int>(); // Считаем число вершин в подграфе
            foreach (Edge edgeTemp in myList)
            {
                if (!verts.ContainsKey(edgeTemp.src))
                    verts.Add(edgeTemp.src, 1);

                if (!verts.ContainsKey(edgeTemp.dest))
                    verts.Add(edgeTemp.dest, 1);
            }

            int treeWeight = 0;
            StreamWriter sw = new StreamWriter(@"data\\" + V + "\\file # " + name + ".txt", true, Encoding.UTF8);
            foreach (Edge edgeTemp in myList)
            {
                treeWeight += edgeTemp.weight;
            }
            sw.WriteLine("c " + "Вес подграфа = " + treeWeight);
            sw.WriteLine("p edge " + verts.Count + " " + myList.Count);

            foreach (Edge edgeTemp in myList)
            {
                int v1 = edgeTemp.src;
                int v2 = edgeTemp.dest;
                sw.WriteLine("e" + " " + ++v1 + " " + ++v2);
                //treeWeight += edgeTemp.weight;
            }

            Console.WriteLine("Done!" + name + "/10000");
            sw.Close();
            }
            
        }
    }

    class Program
    {
        public static void Main(String[] args)
        {
            //int V = 4; int E = 6;
            //int V = 64; int E = 2016;
            //int V = 128; int E = 8128;
            int V = 512;  int E = 130816;
            //int V = 2048; int E = 2096128;
            //int V = 4096; int E = 8386560;
            Graph graph = new Graph(V, E);
            graph.ReadFile(@"data\\Taxicab_"+V+".txt");
            int v2Index = 0;
            int edgeIndex = 0;
            for (int v1Index = 0; v1Index < V - 1; v1Index++)
            {
                v2Index++;
                Verticle vert1 = new Verticle
                {
                    x_coord = graph.verticleList[v1Index].x_coord,
                    y_coord = graph.verticleList[v1Index].y_coord
                };
                for (int i = v2Index; i < V; i++)
                {
                    Verticle vert2 = new Verticle
                    {
                        x_coord = graph.verticleList[i].x_coord,
                        y_coord = graph.verticleList[i].y_coord
                    };

                    int distance = CalcVertDistance(vert1, vert2);

                    graph.edgeList[edgeIndex] = new Edge
                    {
                        src = v1Index,
                        dest = i,
                        weight = distance
                    };
                    edgeIndex++;
                }
            }

            //for (int i = 0; i< 10000; i++)
            //{
                graph.Solver(Convert.ToString("1"));
            //}
            //int maxGraphValue = 0;
            //string maxGraphValueFileName = "";
            //string[] files = Directory.GetFiles(@"data\\" + V);
            //foreach (string fileName in files)
            //{
            //    using var reader = new StreamReader(fileName);
            //    string line = reader.ReadLine();
            //    line = Regex.Match(line, @"\d+").Value;

            //    int temp = Int32.Parse(line);
            //    if (maxGraphValue < temp)
            //    {
            //        maxGraphValue = temp;
            //        maxGraphValueFileName = fileName;
            //    }

            //}
            //Console.WriteLine(maxGraphValueFileName + "__" + maxGraphValue);
            Console.ReadLine();
        }
    }