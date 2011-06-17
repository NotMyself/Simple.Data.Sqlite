using NUnit.Framework;

namespace Simple.Data.SqliteTests
{
    [TestFixture]
    public class RealityTests
    {
        [Test]
        public void true_should_be_true()
        {
            Assert.That(true,Is.True);
        }
    }
}