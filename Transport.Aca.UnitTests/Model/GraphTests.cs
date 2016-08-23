using NUnit.Framework;

namespace Transport.Aca.UnitTests.Model
{
    [TestFixture]
    public class GraphTests
    {
        [Test]
        public void AddNode_IncGraphSizeBy1()
        {
            // Arrange
            var graph = new Aca.Model.Graph();
            var count = graph.Size;

            // Act
            graph.AddNode(new Aca.Model.Node());

            // Assert
            Assert.IsTrue(graph.Size == count + 1);
        }

        [Test]
        public void RemoveNode_DecGraphSizeBy1()
        {
            // Arrange
            var graph = new Aca.Model.Graph();
            var node = new Aca.Model.Node();
            graph.AddNode(node);
            var count = graph.Size;

            // Act
            graph.RemoveNode(node);

            // Assert
            Assert.IsTrue(graph.Size == count - 1);
        }

        [Test]
        public void AddEdge_WithTwoNewNodes_IncGraphSizeBy2()
        {
            // Arrange
            var graph = new Aca.Model.Graph();
            var edge = new Aca.Model.Edge
            {
                Origin = new Aca.Model.Node(),
                Destination = new Aca.Model.Node()
            };
            var count = graph.Size;

            // Act
            graph.AddEdge(edge);

            // Assert
            Assert.IsTrue(graph.Size == count + 2);
        }

        [Test]
        public void RemoveEdge_NotDecGraphSize()
        {
            // Arrange
            var graph = new Aca.Model.Graph();
            var edge = new Aca.Model.Edge
            {
                Origin = new Aca.Model.Node(),
                Destination = new Aca.Model.Node()
            };
            graph.AddEdge(edge);
            var count = graph.Size;

            // Act
            graph.RemoveEdge(edge);
            
            // Assert
            Assert.IsTrue(graph.Size == count);
        }
    }
}
