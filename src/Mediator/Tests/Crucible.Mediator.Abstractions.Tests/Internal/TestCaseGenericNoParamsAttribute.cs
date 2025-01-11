using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestCaseSourceGenericAttribute : TestCaseSourceAttribute, ITestBuilder
    {
        public TestCaseSourceGenericAttribute(string sourceName) : base(sourceName)
        {
        }

        public TestCaseSourceGenericAttribute(Type sourceType) : base(sourceType)
        {
        }

        public TestCaseSourceGenericAttribute(Type sourceType, string sourceName) : base(sourceType, sourceName)
        {
        }

        public TestCaseSourceGenericAttribute(string sourceName, object?[]? methodParams) : base(sourceName, methodParams)
        {
        }

        public TestCaseSourceGenericAttribute(Type sourceType, string sourceName, object?[]? methodParams) : base(sourceType, sourceName, methodParams)
        {
        }

        IEnumerable<TestMethod> ITestBuilder.BuildFrom(IMethodInfo method, Test suite)
        {
            //Method converts every test case source parameter into generic type.

            if (!method.IsGenericMethodDefinition)
                return BuildFrom(method, suite);

            var testCaseSourceTestMethods = BuildFrom(method, suite).ToArray();
            var genericTestMethods = new List<TestMethod>();
            foreach (var testMethod in testCaseSourceTestMethods)
            {
                var listOfTypes = new List<Type>();
                foreach (var argument in testMethod.Arguments)
                {
                    if (argument is Type typeArgument)
                        listOfTypes.Add(typeArgument);
                    else if (argument != null)
                        listOfTypes.Add(argument.GetType());
                }

                var genericMethod = testMethod.Method.MakeGenericMethod(listOfTypes.ToArray());

                var genericTestMethod = new NUnitTestCaseBuilder().BuildTestMethod(genericMethod, suite, new TestCaseParameters(testMethod.Arguments));
                genericTestMethods.Add(genericTestMethod);
            }

            return genericTestMethods;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestCaseGenericNoParamsAttribute : TestCaseAttribute, ITestBuilder
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
    public class TestCaseGenericNoParamsAttribute<T> : TestCaseGenericNoParamsAttribute
    {
        public TestCaseGenericNoParamsAttribute(params object[] arguments)
            : base(arguments) => TypeArguments = [typeof(T)];
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestCaseGenericNoParamsAttribute<T1, T2> : TestCaseGenericNoParamsAttribute
    {
        public TestCaseGenericNoParamsAttribute(params object[] arguments)
            : base(arguments) => TypeArguments = [typeof(T1), typeof(T2)];
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestCaseGenericNoParamsAttribute<T1, T2, T3> : TestCaseGenericNoParamsAttribute
    {
        public TestCaseGenericNoParamsAttribute(params object[] arguments)
            : base(arguments) => TypeArguments = [typeof(T1), typeof(T2), typeof(T3)];
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestCaseGenericNoParamsAttribute<T1, T2, T3, T4> : TestCaseGenericNoParamsAttribute
    {
        public TestCaseGenericNoParamsAttribute(params object[] arguments) : base(arguments) => TypeArguments = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];
    }
}
