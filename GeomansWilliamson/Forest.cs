using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomansWilliamson
{
    public class Forest : Graph
    {
        public Forest ( string name ) : base(name)
        {
        }

        public void AddEdgeToForest ( Edge e )
        {
            var v1 = new Vertex(e.Vertex1.Id, e.Vertex1.Penalty);
            var v2 = new Vertex(e.Vertex2.Id, e.Vertex2.Penalty);

            v1.AddUndirectedNeighbour(v2, e.Weight);

            AddVertexToGraph(v1);
            AddVertexToGraph(v2);
        }

        public Edge RemoveEdgeBetween ( int keyOf_v1, int keyOf_v2 )
        {
            var v1 = Vertices[keyOf_v1];
            var v2 = Vertices[keyOf_v2];

            var e = v1.RemoveDirectedNeighbour(v2);
            v2.RemoveDirectedNeighbour(v1);

            return e;
        }

        public bool HoldsProperty1 ()
        {
            return Vertices.Select(v => v.Value).Where(v => string.IsNullOrEmpty(v.Label)).
                All(v => v.GetNeighbourVertices().Any(vn => vn.IsRoot));
        }

        public bool HoldsProperty2 ()
        {
            foreach ( var v in Vertices.Select(v => v.Value).Where(v => !string.IsNullOrEmpty(v.Label)) )
            {
                var label = v.Label;
                if ( v.GetNeighbourVertices().Any(vn => vn.IsRoot) )
                {
                    #region Option 1

                    var otherLabels = label.Split('-').ToList();

                    if ( !Vertices.Select(_v => _v.Value).
                        Where(_v => otherLabels.Contains(_v.Label)).
                        All(vLabel => vLabel.GetNeighbourVertices().Any(vn => vn.IsRoot)) )
                    {
                        return false;
                    }

                    #endregion

                    #region Option 2

                    //if ( !Vertices.Select(_v => _v.Value).
                    //    Where(_v => _v.Label.Split('-').ToList().Contains(label)).
                    //    All(vLabel => vLabel.GetNeighbourVertices().Any(vn => vn.IsRoot)) )
                    //{
                    //    return false;
                    //}

                    #endregion
                }
            }

            return true;
        }
    }
}
