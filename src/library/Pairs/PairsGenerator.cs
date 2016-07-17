using FixturesGenerator.Base;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FixturesGenerator.Pairs
{
    public class PairsGenerator<T> : AbstractFixturesGenerator<T>
    {
        private PairsGeneratorSettings _settings = new PairsGeneratorSettings();

        public PairsGenerator() : base()
        {
        }

        public PairsGenerator(ILogger logger) : base(logger)
        {
        }

        public void Configure(PairsGeneratorSettings settings) 
        {
            _settings = settings;
        }

        protected override IEnumerable<Pair<T>> GetPairs(IEnumerable<T> elements)
        {
            List<T> list = elements.ToList();
            for (int i = 0; i < list.Count - 1; i++)
            {
                for(int j = i + 1; j < list.Count; j++)
                {
                    yield return new Pair<T>(list[i], list[j]);
                }
            }
        }

        protected override int GetSetsCount(IEnumerable<T> elements) => 
            elements.Count() - 1;

        protected override bool FixtureItemIsValid(Pair<T> item, IIFixturesGeneratorValidationContext<T> context) =>
            !context.CurrentSet.Any(x => x.IsRelatedTo(item));

        public override FixtureSets<T> GetFixtures(IEnumerable<T> elements, CancellationToken cancellationToken)
        {
            var sourceElements = elements;
            if (_settings.ShuffleItems) sourceElements = Shuffle(sourceElements.ToList());

            var fixturesList = base.GetFixtures(sourceElements, cancellationToken);

            // check options to perform anything else
            if (_settings.IncludeInvertedPairs)
            {
                _logger.LogDebug("Adding inverted pairs");
                foreach (var invertedSet in fixturesList.GetInvertedSets().ToList())
                {
                    fixturesList.Add(invertedSet);
                }

                _logger.LogDebug($"Resulting {fixturesList.Count} sets:");
                foreach (var fixtureSet in fixturesList)
                {
                    _logger.LogDebug(string.Join(", ", fixtureSet));
                }
            }

            return fixturesList;
        }
    }
}
