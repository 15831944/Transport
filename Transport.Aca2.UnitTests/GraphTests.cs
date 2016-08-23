using NUnit.Framework;

namespace Transport.Aca2.UnitTests
{
    [TestFixture(typeof(double))]
    public class GraphTests<T> where T : new()
    {
        [Test]
        public void AddNode_AddingNode_IncGraphNodesCountBy1()
        {
            // arrange
            var graph = new Graph<T>();
            var oldCount = graph.NodesCount;

            // act
            graph.AddNode();

            // assert
            Assert.True(graph.NodesCount == oldCount + 1);
        }

        [Test]
        public void RemoveNode_RemovingNode_DecGraphNodesCountBy1()
        {
            // arrange
            var graph = new Graph<T>();
            graph.AddNode();
            var oldCount = graph.NodesCount;

            // act
            graph.RemoveNode(0);

            // assert
            Assert.True(graph.NodesCount == oldCount - 1);
        }
    }
}
