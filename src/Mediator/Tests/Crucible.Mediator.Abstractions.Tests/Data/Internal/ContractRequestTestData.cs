namespace Crucible.Mediator.Abstractions.Tests.Data.Internal
{
    public class ContractRequestResponseTestData : ContractRequestTestData
    {
        public required object Response { get; set; }
    }

    public class ContractRequestTestData
    {
        public required object Request { get; set; }
    }
}
