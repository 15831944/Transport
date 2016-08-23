using NUnit.Framework;

namespace Transport.Aca.UnitTests.Model
{
    [TestFixture]
    public class EdgeTests
    {
        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            // Arrange
            var edge1 = new Aca.Model.Edge();

            // Act
            var result = edge1 == null;

            // Assert
            Assert.False(result);
        }

        [Test]
        public void Equals_WithTheSameOriginAndDest_ReturnsTrue()
        {
            // Arrange
            var node1 = new Aca.Model.Node();
            var node2 = new Aca.Model.Node();

            var edge1 = new Aca.Model.Edge
            {
                Origin = node1,
                Destination = node2
            };

            var edge2 = new Aca.Model.Edge
            {
                Origin = node1,
                Destination = node2
            };

            // Act
            var result = edge1.Equals(edge2);

            // Assert
            Assert.True(result);
        }
    }
}
