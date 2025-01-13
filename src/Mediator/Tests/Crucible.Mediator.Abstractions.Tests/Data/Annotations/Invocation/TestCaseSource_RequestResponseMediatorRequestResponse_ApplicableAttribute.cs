namespace Crucible.Mediator.Abstractions.Tests.Data.Annotations.Invocation
{
    public class TestCaseSource_RequestResponseMediatorRequestResponse_ApplicableAttribute : TestCaseSourceAttribute
    {
        public TestCaseSource_RequestResponseMediatorRequestResponse_ApplicableAttribute(bool isApplicable, params object[] methodParams)
            : this(isApplicable, typeof(object), methodParams) { }

        public TestCaseSource_RequestResponseMediatorRequestResponse_ApplicableAttribute(bool isApplicable, Type assignableTo, params object[] methodParams)
            : base(typeof(TestCaseSource_RequestResponseMediatorRequestResponse_ApplicableAttribute), isApplicable ? nameof(TestDataApplicable) : nameof(TestDataNoApplicable), methodParams?.Length > 0 ? [assignableTo, methodParams] : [assignableTo, Array.Empty<object>()])
        { }

        public static object[] TestDataApplicable(Type assignableTo, object[] methodParams) => MediatorTestData
            .Get_RequestResponseMediatorRequestResponse_IsApplicable_All(d => IsMatch(d.Request.GetType(), assignableTo))
            .Select(x => new object[] { x.Request, x.Response, x.MediatorRequest, x.MediatorResponse }.AppendExtraParams(methodParams))
            .ToArray();

        public static object[] TestDataNoApplicable(Type assignableTo, object[] methodParams) => MediatorTestData
            .Get_RequestResponseMediatorRequestResponse_IsNotApplicable_All(d => IsMatch(d.Request.GetType(), assignableTo))
            .Select(x => new object[] { x.Request, x.Response, x.MediatorRequest, x.MediatorResponse }.AppendExtraParams(methodParams))
            .ToArray();

        private static bool IsMatch(Type contractType, Type assignableTo)
        {
            if (contractType.IsAssignableTo(assignableTo))
            {
                return true;
            }
            else if (assignableTo.IsInterface && assignableTo.IsGenericTypeDefinition)
            {
                var assignableToName = assignableTo.Name;
#pragma warning disable IL2070 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.
                return contractType.GetInterfaces().Any(t => t.Name == assignableToName);
#pragma warning restore IL2070 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.
            }

            return false;
        }
    }
}
