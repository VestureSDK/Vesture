// Ensures the instances are using ctor/disposable for setup/teardown in order to
// mimic XUnit default behavior and transpose tests to another framework more easily.
[assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]

namespace Crucible.Mediator.Abstractions.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}
