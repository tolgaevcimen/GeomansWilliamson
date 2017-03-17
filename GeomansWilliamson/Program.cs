using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomansWilliamson
{
    class Program
    {
        static void Main ( string[] args )
        {
            var g = new GeomansWillamsonGraph("Graph");
            var components = new List<Graph>();

            var x = new Vertex(1, 1);
            var y = new Vertex(2, 3);
            var z = new Vertex(3, 5);
            var t = new Vertex(4, 2) { IsRoot = true };

            x.AddUndirectedNeighbour(y, 3);
            y.AddUndirectedNeighbour(t, 2);
            z.AddUndirectedNeighbour(x, 5);

            g.AddVertexToGraph(x);
            g.AddVertexToGraph(y);
            g.AddVertexToGraph(z);
            g.AddVertexToGraph(t);

            g.IntroduceYourself();

            g.Run();

            g.IntroduceYourself();
            
            Console.ReadKey();
        }
    }
}
