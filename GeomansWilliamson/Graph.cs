using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomansWilliamson
{
    public class Graph
    {
        #region Structural Properties

        public Dictionary<int, Vertex> Vertices { get; set; }
        public string Name { get; set; }

        #endregion

        public Graph ( string name )
        {
            Vertices = new Dictionary<int, Vertex>();
            Name = name;
        }

        public void AddVertexToGraph ( Vertex v )
        {
            if ( Vertices.ContainsKey(v.Id) ) return;

            Vertices.Add(v.Id, v);
        }

        public void IntroduceYourself ()
        {
            foreach ( var v in Vertices )
            {
                var neighbours = v.Value.GetNeighbourEdges();
                Console.WriteLine("Node {0} introducing, got {1} penalty, {2} neighbours:", v.Value.Id, v.Value.Penalty, neighbours.Count());

                foreach ( var edge in neighbours )
                {
                    Console.WriteLine("\tEdge {0}, Weight: {1}", edge.Vertex2.Id, edge.Weight);
                }
            }
        }
    }

    public enum Lambda
    {
        Inactive = 0,
        Active = 1
    }
}
