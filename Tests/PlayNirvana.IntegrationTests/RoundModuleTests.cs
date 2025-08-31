using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PlayNirvana.IntegrationTests.Infrastruture;
using PlayNirvana.RoundModule.Application;
using PlayNirvana.RoundModule.Common.Options;

public class RoundModuleTests : IntegrationTestFixture
{
    public RoundModuleTests(IntegrationTestWepAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public void RoundForProcessCountCache_ShouldHaveEnoughForMaintainMinumumActiveRoundsRequirement()
    {
        this.scopeRunner.Run<RoundsForProcessCache, IOptions<RoundOptions>, IConfiguration>(
            (roundsForProcessCache, options, configuration) =>
            {
                var roundOptions = options.Value;

                var generatedRoundsCount = roundOptions.MinimunActiveRounds + roundOptions.CalculateMinimunActiveRoundsSafetyAddition();

                Assert.True(roundsForProcessCache.GetRoundIdList().Count() == generatedRoundsCount, 
                    "Generated rounds for processing is not sufficient to maintain minimum active rounds requirement");
            });
    }

    [Fact]
    public void RoundGeneration_ShouldGenerateAfterIntervalFinish()
    {
        this.scopeRunner.Run<RoundsForProcessCache, IOptions<RoundOptions>>(
            (roundsForProcessCache, options) =>
            {
                var roundOptions = options.Value;

                var generatedRoundsCount = roundOptions.MinimunActiveRounds + roundOptions.CalculateMinimunActiveRoundsSafetyAddition();

                Assert.True(roundsForProcessCache.GetRoundIdList().Count() == generatedRoundsCount,
                    "Generated rounds for processing is not sufficient to maintain minimum active rounds requirement");
            });
    }
}