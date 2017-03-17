using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomansWilliamson
{
    class GeomansWillamsonGraph : Graph
    {
        #region Geomans Williomson Properties

        public double w { get; set; }
        public Lambda L { get; set; }

        public List<GeomansWillamsonGraph> Components { get; set; }
        public Forest F { get; set; }

        #endregion

        public GeomansWillamsonGraph ( string name ) : base(name)
        {
            Components = new List<GeomansWillamsonGraph>();
            F = new Forest("GrownGraph");
        }

        public void Run ()
        {
            Console.WriteLine("Will run");

            InitializeComponents();

            while ( Components.Any(c => c.L == Lambda.Active) )
            {
                Grow();
            }

            Prune();

            Console.WriteLine("Has run");
        }

        /// <summary>
        /// forms a list of components where each component has one item from the graph
        /// </summary>
        void InitializeComponents ()
        {
            // create components from nodes
            foreach ( var v in Vertices.Select(n => n.Value) )
            {
                var c = new GeomansWillamsonGraph("C" + v.Id);
                c.w = 0;

                v.Label = string.Empty;
                v.d = 0;

                c.AddVertexToGraph(v);

                if ( v.IsRoot )
                    c.L = Lambda.Inactive;
                else c.L = Lambda.Active;

                Components.Add(c);
            }
        }

        void Grow ()
        {
            var eps1 = double.MaxValue;
            var eps2 = double.MaxValue;

            // find min edge
            Edge pickedEdge = FindEdgeWithMinE1(ref eps1);

            // find min component
            GeomansWillamsonGraph c_prime = FindComponentWithMinE2(ref eps2);

            var epsilon = Math.Min(eps1, eps2);

            // update weights and penalties
            Update_AllComponents_W(epsilon);
            Update_Component_R_d(pickedEdge, epsilon);

            // the deactivation occurs
            if ( epsilon == eps2 )
            {
                c_prime.L = Lambda.Inactive;
                foreach ( var v in c_prime.Vertices.Where(v => string.IsNullOrEmpty(v.Value.Label)) )
                {
                    v.Value.Label = c_prime.Name;
                }
            }
            // picked edge added to solution - to be pruned maybe
            else
            {
                F.AddEdgeToForest(pickedEdge);
                HandleCompoments(pickedEdge);
            }
        }

        Edge FindEdgeWithMinE1 ( ref double e1 )
        {
            Edge minEdge = null;

            foreach ( var CP in Components )
            {
                foreach ( var CQ in Components )
                {
                    if ( CP.Name == CQ.Name ) continue;

                    foreach ( var v in CP.Vertices )
                    {
                        foreach ( var e in v.Value.Edges )
                        {
                            var epsHere = ( e.Weight - e.Vertex1.d - e.Vertex2.d ) / ( (int)CP.L + (int)CQ.L );

                            if ( epsHere < e1 )
                            {
                                e1 = epsHere;
                                minEdge = e;
                            }
                        }
                    }
                }
            }

            if ( minEdge == null )
            {
                throw new InvalidOperationException("no edge found for E1");
            }

            return minEdge;
        }

        GeomansWillamsonGraph FindComponentWithMinE2 ( ref double e2 )
        {
            GeomansWillamsonGraph c_prime = null;

            foreach ( var c in Components.Where(_c => _c.L == Lambda.Active) )
            {
                var epsHere = c.Vertices.Sum(v => v.Value.Penalty) - c.w;
                if ( epsHere < e2 )
                {
                    e2 = epsHere;
                    c_prime = c;
                }
            }

            if ( c_prime == null )
            {
                throw new InvalidOperationException("no component found for E2");
            }

            return c_prime;
        }

        void Update_AllComponents_W ( double E )
        {
            foreach ( var c in Components )
            {
                c.w = c.w + ( E * (int)c.L );
            }
        }

        void Update_Component_R_d ( Edge pickedEdge, double E )
        {
            // CR is the component where the picked edge does not belong
            foreach ( var CR in Components.Where(_c =>
                !_c.Vertices.ContainsKey(pickedEdge.Vertex1.Id) &&
                !_c.Vertices.ContainsKey(pickedEdge.Vertex2.Id)) )
            {
                foreach ( var v in CR.Vertices.Select(v => v.Value) )
                {
                    v.d = v.d + ( E * (int)CR.L );
                }
            }
        }

        void HandleCompoments ( Edge pickedEdge )
        {
            // component at one side of the edge
            var cI = Components.First(c => c.Vertices.ContainsKey(pickedEdge.Vertex1.Id));
            // component at other side of the edge
            var cJ = Components.First(c => c.Vertices.ContainsKey(pickedEdge.Vertex2.Id));

            // remove them one by one
            Components.Remove(cI);
            Components.Remove(cJ);

            // unite them, form only 1 component out of them
            var unionOf_I_J = new GeomansWillamsonGraph(cI.Name + "-" + cJ.Name);

            foreach ( var v_cI in cI.Vertices )
            {
                if ( !unionOf_I_J.Vertices.ContainsKey(v_cI.Key) )
                    unionOf_I_J.Vertices.Add(v_cI.Key, v_cI.Value);
            }

            foreach ( var v_cJ in cJ.Vertices )
            {
                if ( !unionOf_I_J.Vertices.ContainsKey(v_cJ.Key) )
                    unionOf_I_J.Vertices.Add(v_cJ.Key, v_cJ.Value);
            }

            // set a "w" value to new component
            unionOf_I_J.w = cI.w + cJ.w;
            // set the state of the new component
            unionOf_I_J.L = cI.Vertices.Any(v => v.Value.IsRoot) || cJ.Vertices.Any(v => v.Value.IsRoot) ?
                Lambda.Active :
                Lambda.Inactive;

            // insert the combined component
            Components.Add(unionOf_I_J);
        }

        void Prune ()
        {
            // find how many vertices picked during the growth
            var forestVertexCount = F.Vertices.Count;

            // for each pair of vertex 
            for ( int i = 0; i < forestVertexCount; i++ )
            {
                for ( int j = 0; j < forestVertexCount; j++ )
                {
                    var vi = F.Vertices.ElementAt(i);
                    var vj = F.Vertices.ElementAt(j);

                    if ( vi.Key == vj.Key ) continue;

                    // remove the two edge between two different vertices
                    var edgeTriedToBeRemoved = F.RemoveEdgeBetween(vi.Key, vj.Key);

                    // fails if the two vertices not connected - nothing required to be done
                    if ( edgeTriedToBeRemoved == null ) continue;

                    // if any of the properties does not hold after removal, we add the node back again
                    if ( !F.HoldsProperty1() || !F.HoldsProperty2() )
                    {
                        F.AddEdgeToForest(edgeTriedToBeRemoved);
                    }
                }
            }
        }
    }
}
