using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using Stashbox.Infrastructure;

namespace Stashbox.Extensions.Tests
{
    [TestClass]
    public class DecoratorExtensionsTests
    {
        private IStashboxContainer container;

        [TestInitialize]
        public void Init()
        {
            this.container = new StashboxContainer();
            this.container.RegisterExtension(new DecoratorExtension());
        }

        [TestCleanup]
        public void CleanUp()
        {
            this.container.Dispose();
        }

        [TestMethod]
        public void DecoratorExtensionsTests_WithoutExtension()
        {
            using (var cont = new StashboxContainer())
            {
                cont.PrepareType<ITest1, Test1>().DecorateWith<TestDecorator1>().Register();
                cont.RegisterDecorator<ITest1, TestDecorator2>();
                var test = cont.Resolve<ITest1>();

                Assert.IsNotNull(test);
                Assert.IsInstanceOfType(test, typeof(Test1));

                Assert.IsNull(test.Test);
            }
        }

        [TestMethod]
        public void DecoratorExtensionsTests_WithoutDecoratorRegistration()
        {
            this.container.RegisterType<ITest1, Test1>();
            var test = this.container.Resolve<ITest1>();

            Assert.IsNotNull(test);
            Assert.IsInstanceOfType(test, typeof(Test1));

            Assert.IsNull(test.Test);
        }

        [TestMethod]
        public void DecoratorExtensionsTests_ExplicitDecoratorTest()
        {
            this.container.PrepareType<ITest1, Test1>().DecorateWith<TestDecorator1>().Register();
            var test = this.container.Resolve<ITest1>();

            Assert.IsNotNull(test);
            Assert.IsInstanceOfType(test, typeof(TestDecorator1));

            Assert.IsNotNull(test.Test);
            Assert.IsInstanceOfType(test.Test, typeof(Test1));
        }

        [TestMethod]
        public void DecoratorExtensionsTests_ExplicitDecoratorTest_Multiple()
        {
            this.container.PrepareType<ITest1, Test1>().DecorateWith<TestDecorator1>().DecorateWith<TestDecorator2>().Register();
            var test = this.container.Resolve<ITest1>();

            Assert.IsNotNull(test);
            Assert.IsInstanceOfType(test, typeof(TestDecorator2));

            Assert.IsNotNull(test.Test);
            Assert.IsInstanceOfType(test.Test, typeof(TestDecorator1));
        }

        [TestMethod]
        public void DecoratorExtensionsTests_GlobalDecoratorTest()
        {
            this.container.RegisterType<ITest1, Test1>();
            this.container.RegisterDecorator<ITest1, TestDecorator1>();
            var test = this.container.Resolve<ITest1>();

            Assert.IsNotNull(test);
            Assert.IsInstanceOfType(test, typeof(TestDecorator1));

            Assert.IsNotNull(test.Test);
            Assert.IsInstanceOfType(test.Test, typeof(Test1));
        }

        [TestMethod]
        public void DecoratorExtensionsTests_GlobalDecoratorTest_Multiple()
        {
            this.container.RegisterType<ITest1, Test1>();
            this.container.RegisterDecorator<ITest1, TestDecorator1>();
            this.container.RegisterDecorator<ITest1, TestDecorator2>();
            var test = this.container.Resolve<ITest1>();

            Assert.IsNotNull(test);
            Assert.IsInstanceOfType(test, typeof(TestDecorator2));

            Assert.IsNotNull(test.Test);
            Assert.IsInstanceOfType(test.Test, typeof(TestDecorator1));
        }

        [TestMethod]
        public void DecoratorExtensionsTests_MixedDecoratorTest()
        {
            this.container.PrepareType<ITest1, Test1>().WithName("test1").DecorateWith<TestDecorator1>().DecorateWith<TestDecorator2>().Register();
            this.container.PrepareType<ITest1, Test2>().WithName("test2").DecorateWith<TestDecorator3>().Register();
            this.container.RegisterDecorator<ITest1, TestDecorator4>();
            this.container.RegisterDecorator<ITest1, TestDecorator5>();
            var test1 = this.container.Resolve<ITest1>("test1");
            var test2 = this.container.Resolve<ITest1>("test2");

            Assert.IsNotNull(test1);
            Assert.IsInstanceOfType(test1, typeof(TestDecorator5));

            Assert.IsNotNull(test1.Test);
            Assert.IsInstanceOfType(test1.Test, typeof(TestDecorator4));

            Assert.IsNotNull(test1.Test.Test);
            Assert.IsInstanceOfType(test1.Test.Test, typeof(TestDecorator2));

            Assert.IsNotNull(test1.Test.Test.Test);
            Assert.IsInstanceOfType(test1.Test.Test.Test, typeof(TestDecorator1));


            Assert.IsNotNull(test2);
            Assert.IsInstanceOfType(test2, typeof(TestDecorator5));

            Assert.IsNotNull(test2.Test);
            Assert.IsInstanceOfType(test2.Test, typeof(TestDecorator4));

            Assert.IsNotNull(test2.Test.Test);
            Assert.IsInstanceOfType(test2.Test.Test, typeof(TestDecorator3));
        }

        [TestMethod]
        public void DecoratorExtensionsTests_MixedDecoratorTest_DependencyResolve()
        {
            this.container.RegisterType<ITest2, Test22>();
            this.container.PrepareType<ITest1, Test1>().WithName("test1").DecorateWith<TestDecorator1>().DecorateWith<TestDecorator2>().Register();
            this.container.PrepareType<ITest1, Test2>().WithName("test2").DecorateWith<TestDecorator3>().Register();
            this.container.RegisterDecorator<ITest1, TestDecorator4>();
            this.container.RegisterDecorator<ITest1, TestDecorator5>();
            var test = this.container.Resolve<ITest2>();

            Assert.IsNotNull(test.Test1);
            Assert.IsInstanceOfType(test.Test1, typeof(TestDecorator5));

            Assert.IsNotNull(test.Test1.Test);
            Assert.IsInstanceOfType(test.Test1.Test, typeof(TestDecorator4));

            Assert.IsNotNull(test.Test1.Test.Test);
            Assert.IsInstanceOfType(test.Test1.Test.Test, typeof(TestDecorator2));

            Assert.IsNotNull(test.Test1.Test.Test.Test);
            Assert.IsInstanceOfType(test.Test1.Test.Test.Test, typeof(TestDecorator1));


            Assert.IsNotNull(test.Test2);
            Assert.IsInstanceOfType(test.Test2, typeof(TestDecorator5));

            Assert.IsNotNull(test.Test2.Test);
            Assert.IsInstanceOfType(test.Test2.Test, typeof(TestDecorator4));

            Assert.IsNotNull(test.Test2.Test.Test);
            Assert.IsInstanceOfType(test.Test2.Test.Test, typeof(TestDecorator3));
        }

        [TestMethod]
        public void DecoratorExtensionsTests_CreateCopyTest()
        {
            var decorator = new DecoratorExtension();
            var copy = decorator.CreateCopy();

            Assert.IsNotNull(copy);
            Assert.IsInstanceOfType(copy, typeof(DecoratorExtension));
        }

        public interface ITest1 { ITest1 Test { get; } }

        public interface ITest2
        {
            ITest1 Test1 { get; }
            ITest1 Test2 { get; }
        }

        public class Test1 : ITest1
        {
            public ITest1 Test { get; }
        }

        public class Test2 : ITest1
        {
            public ITest1 Test { get; }
        }

        public class Test22 : ITest2
        {
            public ITest1 Test1 { get; }
            public ITest1 Test2 { get; }

            public Test22([Dependency("test1")]ITest1 test1, [Dependency("test2")]ITest1 test2)
            {
                this.Test1 = test1;
                this.Test2 = test2;
            }
        }

        public class TestDecorator1 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator1(ITest1 test1)
            {
                this.Test = test1;
            }
        }

        public class TestDecorator2 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator2(ITest1 test1)
            {
                this.Test = test1;
            }
        }

        public class TestDecorator3 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator3(ITest1 test1)
            {
                this.Test = test1;
            }
        }

        public class TestDecorator4 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator4(ITest1 test1)
            {
                this.Test = test1;
            }
        }

        public class TestDecorator5 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator5(ITest1 test1)
            {
                this.Test = test1;
            }
        }
    }
}
