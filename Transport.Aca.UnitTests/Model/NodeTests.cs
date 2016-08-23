using System;
using System.Linq;
using NUnit.Framework;

namespace Transport.Aca.UnitTests.Model
{
    [TestFixture]
    public class NodeTests
    {
        [Test]
        public void AddEdge_WithOriginInOtherNode_ThrowsException()
        {
            // Arrange
            var node1 = new Aca.Model.Node();
            var edge = new Aca.Model.Edge
            {
                Origin = new Aca.Model.Node(),
                Destination = new Aca.Model.Node()
            };

            // Act
            var ex = Assert.Catch<ArgumentException>(() => node1.AddEdge(edge));

            // Assert
            StringAssert.Contains("не начинается в текущем узле", ex.Message);
        }

        [Test]
        public void AddEdge_AlreadyAdded_ThrowsException()
        {
            // Arrange
            var node1 = new Aca.Model.Node();
            var node2 = new Aca.Model.Node();
            
            var edge1 = new Aca.Model.Edge
            {
                Origin = node1,
                Destination = node2
            };

            node1.AddEdge(edge1);

            var edge2 = new Aca.Model.Edge
            {
                Origin = node1,
                Destination = node2
            };

            // Act
            var ex = Assert.Catch<ArgumentException>(() => node1.AddEdge(edge2));

            //Assert
            StringAssert.Contains("уже добавлено", ex.Message);
        }

        [Test]
        public void AddEdge_AddValidEdge_ContainsInAdjacentNodes()
        {
            // Arrange
            var node1 = new Aca.Model.Node();
            var node2 = new Aca.Model.Node();
            
            var edge1 = new Aca.Model.Edge
            {
                Origin = node1,
                Destination = node2
            };

            // Act
            node1.AddEdge(edge1);

            // Assert
            Assert.True(node1.AdjacentNodes.Contains(node2));
        }

        [Test]
        public void AddEdge_AddValidEdge_IncAdjacentNodesCountBy1()
        {
            // Arrange
            var node1 = new Aca.Model.Node();
            var node2 = new Aca.Model.Node();

            var edge1 = new Aca.Model.Edge
            {
                Origin = node1,
                Destination = node2
            };

            var count = node1.AdjacentNodes.Count;

            // Act
            node1.AddEdge(edge1);

            // Assert
            Assert.True(node1.AdjacentNodes.Count == count + 1);
        }

        [Test]
        public void AddEdge_AddValidEdge_IncCostsCountBy1()
        {
            // Arrange
            var node1 = new Aca.Model.Node();
            var node2 = new Aca.Model.Node();

            var edge1 = new Aca.Model.Edge
            {
                Origin = node1,
                Destination = node2
            };

            var count = node1.Costs.Count;

            // Act
            node1.AddEdge(edge1);

            // Assert
            Assert.True(node1.Costs.Count == count + 1);
        }

        [Test]
        public void AddNode_AlreadyAdded_ThrowsException()
        {
            // Arrange
            var node1 = new Aca.Model.Node();
            var node2 = new Aca.Model.Node();
            const double cost = 1.0;
            node1.AddAdjacentNode(node2, cost);

            // Act
            var ex = Assert.Catch<ArgumentException>(() => node1.AddAdjacentNode(node2, cost));

            // Assert
            StringAssert.Contains("уже добавлено", ex.Message);
        }

        [Test]
        public void AddNode_AddValidNode_ContainsInAdjacentNode()
        {
            // Arrange
            var node1 = new Aca.Model.Node();
            var node2 = new Aca.Model.Node();
            const double cost = 1.0;

            // Act
            node1.AddAdjacentNode(node2, cost);

            // Assert
            Assert.True(node1.AdjacentNodes.Contains(node2));
        }
    }
}
