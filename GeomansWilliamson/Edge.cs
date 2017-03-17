using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomansWilliamson
{
    public class Edge
    {
        public Vertex Vertex1 { get; set; }
        public Vertex Vertex2 { get; set; }
        public int Weight { get; set; }
        public Edge ( Vertex n1, Vertex n2, int weight )
        {
            Vertex1 = n1;
            Vertex2 = n2;

            Weight = weight;
        }
    }
}
