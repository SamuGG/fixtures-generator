using FixturesGenerator.Base;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FixturesGenerator.League
{
    public class LeagueFixturesGenerator<T> : AbstractFixturesGenerator<T>
    {
        public LeagueFixturesGenerator() : base()
        {
        }

        public LeagueFixturesGenerator(ILogger logger) : base(logger)
        {
        }

        protected override IEnumerable<Pair<T>> GetPairs(IEnumerable<T> elements)
        {
            foreach (var item1 in elements)
            {
                foreach (var item2 in elements)
                {
                    if (item1.Equals(item2)) continue;
                    yield return new Pair<T>(item1, item2);
                }
            }
        }

        protected override int GetSetsCount(IEnumerable<T> elements) => 
            (elements.Count() - 1) * 2;

        protected override bool FixtureItemIsValid(Pair<T> item, IIFixturesGeneratorValidationContext<T> context)
        {
            bool itemIsRelatedToOthersInSolution = context.CurrentSet.Any(x => x.IsRelatedTo(item));
            bool itemExistsAsInvertedInPreviousSet = context.PartialSolution.Count > 0 ? context.PartialSolution[context.PartialSolution.Count - 1].Exists(x => x.Equals(item.Invert())) : false;

            return !itemIsRelatedToOthersInSolution && !itemExistsAsInvertedInPreviousSet;
        }

        public override FixtureSets<T> GetFixtures(IEnumerable<T> elements, CancellationToken cancellationToken)
        {
            var sourceElements = Shuffle(elements.ToList());
            var fixturesList = base.GetFixtures(sourceElements, cancellationToken);

            for (int i = 1, maxPosition = fixturesList.Count / 2; i < maxPosition; i += 2)
            {
                int j = fixturesList.Count - 1 - i;
                _logger.LogDebug($"Swaping positions {i} and {j}");
                fixturesList.SwapPositions(i, j);
            }

            return fixturesList;
        }
    }
}
