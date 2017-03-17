using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomansWilliamson
{
    public class Vertex
    {
        #region Structural Properties

        public int Id { get; set; }
        public List<Edge> Edges { get; set; }

        #endregion

        #region Geomans Williamson Properties

        public bool IsRoot { get; set; }
        public int Penalty { get; set; }
        public string Label { get; set; }
        public double d { get; set; }

        #endregion

        public Vertex ( int id, int penalty )
        {
            Id = id;
            Penalty = penalty;

            Edges = new List<Edge>();
        }

        public void AddUndirectedNeighbour ( Vertex v, int weight )
        {
            if ( Edges.Any(e => e.Vertex2.Id == v.Id) ) return;

            Edges.Add(new Edge(this, v, weight));
            Edges.Add(new Edge(v, this, weight));

            v.AddUndirectedNeighbour(this, weight);
        }

        public Edge RemoveDirectedNeighbour ( Vertex v )
        {
            var edgeToBeRemoved = Edges.FirstOrDefault(e => e.Vertex2.Id == v.Id);

            if ( edgeToBeRemoved == null ) return null;

            Edges.Remove(edgeToBeRemoved);

            return edgeToBeRemoved;
        }

        public IEnumerable<Edge> GetNeighbourEdges ()
        {
            return Edges.Where(e => e.Vertex1 == this);
        }

        public IEnumerable<Vertex> GetNeighbourVertices()
        {
            return GetNeighbourEdges().Select(e => e.Vertex2);
        }
    }
}
