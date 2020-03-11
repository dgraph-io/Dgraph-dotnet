using System;
using System.Collections.Generic;
using System.Diagnostics;
using DgraphDotNet.Graph;
using DgraphDotNet.Transactions;

// For unit testing.  Allows to make mocks of the internal interfaces and factories
// so can test in isolation from a Dgraph instance.
//
// When I put this in an AssemblyInfo.cs it wouldn't compile any more.
[assembly : System.Runtime.CompilerServices.InternalsVisibleTo("Dgraph-dotnet.tests")]
[assembly : System.Runtime.CompilerServices.InternalsVisibleTo("DynamicProxyGenAssembly2")] // for NSubstitute

namespace DgraphDotNet {
    public class Clients {

        #region ClientBuilders

        /// <summary>
        /// Create a client that can do query and JSON mutations.
        /// </summary>
        public static IDgraphClient NewDgraphClient() {
            return new DgraphClient(new GRPCConnectionFactory(), new TransactionFactory());
        }

        #endregion

        // 
        // ------------------------------------------------------
        //                      Edges 
        // ------------------------------------------------------
        //

        #region Edges

        /// <summary>
        /// Build an Edge --- the edge is not yet added to any request.
        /// </summary>
        /// <remarks>Any null or "" facets are ignored.</remarks>
        /// <remarks>Fails if <paramref name="source"/> or <paramref
        /// name="target"/> is null or <paramref name="edgeName"/> is null or ""
        /// </remarks>
        public static FluentResults.Result<Edge> BuildEdge(INode source, string edgeName, INode target, IDictionary<string, string> facets = null) {

            if (source == null || target == null || string.IsNullOrEmpty(edgeName)) {
                return FluentResults.Results.Fail<Edge>(new FluentResults.ExceptionalError(new ArgumentNullException()));
            }

            Edge result = new Edge(source, edgeName, target);

            AddFacetsToLink(result, facets);

            return FluentResults.Results.Ok<Edge>(result);
        }

        /// <summary>
        /// Build a Property --- the property is not yet added to a request.
        /// </summary>
        /// <remarks>Any null or "" facets are ignored.</remarks>
        /// <remarks>Fails if <paramref name="source"/> or <paramref
        /// name="value"/> is null or <paramref name="predicateName"/> is null
        /// or ""
        /// </remarks>
        public static FluentResults.Result<Property> BuildProperty(INode source, string predicateName, GraphValue value, IDictionary<string, string> facets = null) {

            if (source == null || value == null || string.IsNullOrEmpty(predicateName)) {
                return FluentResults.Results.Fail<Property>(new FluentResults.ExceptionalError(new ArgumentNullException()));
            }

            Property result = new Property(source, predicateName, value);

            AddFacetsToLink(result, facets);

            return FluentResults.Results.Ok<Property>(result);
        }

        // Add the facets ignoring any null or "".
        // 
        // Pre : edge != null
        private static void AddFacetsToLink<TargetType>(GraphLink<TargetType> edge, IDictionary<string, string> facets) where TargetType : IEdgeTarget {
            Debug.Assert(edge != null);

            if (facets != null) {
                foreach (var kv in facets) {
                    if (!string.IsNullOrEmpty(kv.Key) && !string.IsNullOrEmpty(kv.Value)) {
                        edge.Facets.Add(kv.Key, kv.Value);
                    }
                }
            }
        }

        #endregion

    }
}