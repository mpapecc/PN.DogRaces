using Microsoft.Extensions.Options;
using PlayNirvana.IntegrationTests.Infrastruture;
using PlayNirvana.RoundModule.Application;
using PlayNirvana.RoundModule.Application.Repositories;
using PlayNirvana.RoundModule.Common.Options;

namespace PlayNirvana.IntegrationTests
{
    public class RoundModuleTests : IntegrationTestFixture
    {
        public RoundModuleTests(IntegrationTestWepAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public void RoundForProcessCountCache_ShouldHaveEnoughForMaintainMinumumActiveRoundsRequirement()
        {
            this.scopeRunner.Run<RoundsForProcessCache, IOptions<RoundOptions>>(
                (roundsForProcessCache, options) =>
                {
                    var roundOptions = options.Value;

                    Assert.True(roundsForProcessCache.GetRoundIdList().Count() >= roundOptions.MinimunActiveRounds,
                        "Generated rounds for processing is not sufficient to maintain minimum active rounds requirement");
                });
        }

        [Fact]
        public void RoundGeneration_ShouldGenerateAfterIntervalFinish()
        {
            this.scopeRunner.Run<IRoundRepository, IOptions<RoundOptions>>(
                (roundRepository, options) =>
                {
                    var roundOptions = options.Value;

                    Thread.Sleep(TimeSpan.FromMinutes(roundOptions.RoundsGeneratorIntervalInMinutes));

                    var roundFroProcessCount = roundRepository.RoundsForProcessQuery().Count();

                    Assert.True(roundFroProcessCount >= roundOptions.MinimunActiveRounds,
                       "There are less rounds then minumum requirement. Check GenerateRounds service");
                });
        }
    }
}
