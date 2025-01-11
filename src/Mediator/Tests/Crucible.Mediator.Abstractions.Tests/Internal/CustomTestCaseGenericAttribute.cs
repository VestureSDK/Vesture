using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class TestCaseGenericNoParamsAttribute : TestCaseAttribute, ITestBuilder
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public TestCaseGenericNoParamsAttribute(params object[] arguments)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
            : base(arguments)
        {
        }

        public Type[] TypeArguments { get; set; }

        IEnumerable<TestMethod> ITestBuilder.BuildFrom(IMethodInfo method, Test? suite)
        {
            if (!method.IsGenericMethodDefinition)
            {
                return base.BuildFrom(method, suite);
            }

            if (TypeArguments == null || TypeArguments.Length != method.GetGenericArguments().Length)
            {
                var parms = new TestCaseParameters { RunState = RunState.NotRunnable };
                parms.Properties.Set(PropertyNames.SkipReason, $"{nameof(TypeArguments)} should have {method.GetGenericArguments().Length} elements");
                return [new NUnitTestCaseBuilder().BuildTestMethod(method, suite, parms)];
            }

            var genMethod = method.MakeGenericMethod(TypeArguments);
            return base.BuildFrom(genMethod, suite);
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class TestCaseGenericNoParamsAttribute<T> : TestCaseGenericNoParamsAttribute
    {
        public TestCaseGenericNoParamsAttribute(params object[] arguments)
            : base(arguments) => TypeArguments = [typeof(T)];
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class TestCaseGenericNoParamsAttribute<T1, T2> : TestCaseGenericNoParamsAttribute
    {
        public TestCaseGenericNoParamsAttribute(params object[] arguments)
            : base(arguments) => TypeArguments = [typeof(T1), typeof(T2)];
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class TestCaseGenericNoParamsAttribute<T1, T2, T3> : TestCaseGenericNoParamsAttribute
    {
        public TestCaseGenericNoParamsAttribute(params object[] arguments)
            : base(arguments) => TypeArguments = [typeof(T1), typeof(T2), typeof(T3)];
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class TestCaseGenericNoParamsAttribute<T1, T2, T3, T4> : TestCaseGenericNoParamsAttribute
    {
        public TestCaseGenericNoParamsAttribute(params object[] arguments) : base(arguments) => TypeArguments = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];
    }
}
