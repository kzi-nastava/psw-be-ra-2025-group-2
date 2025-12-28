using Explorer.BuildingBlocks.Tests;

namespace Explorer.Payments.Tests;

public class BaseStakeholdersIntegrationTest : BaseWebIntegrationTest<PaymentsTestFactory>
{
    public BaseStakeholdersIntegrationTest(PaymentsTestFactory factory) : base(factory) { }
}