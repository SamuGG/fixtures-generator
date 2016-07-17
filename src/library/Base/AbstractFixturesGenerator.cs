using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace FixturesGenerator.Base
{
    public abstract class AbstractFixturesGenerator<T> : IFixturesGenerator<T>
    {
        private static Random _rnd = new Random();

        protected readonly ILogger _logger;

        public AbstractFixturesGenerator()
        {
            // create default logger
            ILoggerFactory loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger<AbstractFixturesGenerator<T>>();
        }

        public AbstractFixturesGenerator(ILogger logger)
        {
            _logger = logger;
        }

        protected static IList<T> Shuffle(IList<T> list)
        {
            List<T> shuffledList = new List<T>(list);
            int n = shuffledList.Count;  
            while (n > 1) {  
                n--;  
                int k = _rnd.Next(n + 1);  
                T value = shuffledList[k];  
                shuffledList[k] = shuffledList[n];  
                shuffledList[n] = value;  
            }  
            return shuffledList;
        }

        protected abstract IEnumerable<Pair<T>> GetPairs(IEnumerable<T> elements);

        protected abstract int GetSetsCount(IEnumerable<T> elements);

        protected abstract bool FixtureItemIsValid(Pair<T> item, IIFixturesGeneratorValidationContext<T> context);

        public virtual FixtureSets<T> GetFixtures(IEnumerable<T> elements, CancellationToken cancellationToken)
        {
            int itemsCount = elements.Count();

            if (itemsCount % 2 != 0) throw new ArgumentException($"Parameter '{nameof(elements)}' must have an even number of elements.");
            if (itemsCount < 2) return new FixtureSets<T>();

            GetFixturesArgs<T> args = new GetFixturesArgs<T>();
            args.PairCollection = GetPairs(elements);
            args.PairCount = itemsCount / 2;
            args.SetsCount = GetSetsCount(elements);

            // log start of process
            _logger.LogDebug("Starting process to find a valid solution");
            _logger.LogDebug("Source elements: {0}", string.Join(", ", elements));
            _logger.LogDebug("Source pairs: {0}", string.Join(", ", args.PairCollection));
            _logger.LogDebug($"Expected sets: {args.SetsCount}");
            _logger.LogDebug($"Expected pairs in each set: {args.PairCount}");

            return GetFixtures(args, cancellationToken);
        }

        private FixtureSets<T> GetFixtures(GetFixturesArgs<T> args, CancellationToken cancellationToken)
        {
            var fixturesList = new FixtureSets<T>();
            List<Pair<T>> elements = new List<Pair<T>>(args.PairCollection);
            
            while (elements.Count > 0 && fixturesList.Count < args.SetsCount)
            {
                var fixtureSet = FindValidFixtureSet(
                    elements, 
                    args.PairCount, 
                    fixturesList, 
                    cancellationToken);

                if (fixtureSet.Count > 0)
                {
                    fixturesList.Add(fixtureSet);
                    _logger.LogDebug("Added set [{0}] to solution", string.Join(", ", fixtureSet));
                    foreach (var itemToRemove in fixtureSet) elements.Remove(itemToRemove);
                }
                else
                {
                    break;
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("The process has been cancelled.");
                fixturesList.Clear();
            }
            else if (fixturesList.Count < args.SetsCount)
            {
                // discard partial solution and return an empty result
                _logger.LogDebug("Could not complete solution with [{0}]", string.Join(", ", elements));
                fixturesList.Clear();
            }
            else
            {
                // log the solution found
                _logger.LogDebug($"Solution found with {fixturesList.Count} sets:");
                foreach (var fixtureSet in fixturesList)
                {
                    _logger.LogDebug(string.Join(", ", fixtureSet));
                }
            }

            return fixturesList;
        }

        private FixtureSet<T> FindValidFixtureSet(
            IEnumerable<Pair<T>> elements, 
            int numElementsToReturn,
            FixtureSets<T> partialSolution,
            CancellationToken cancellationToken)
        {
            Stack<int> stack = new Stack<int>();
            List<Pair<T>> list = new List<Pair<T>>(elements);
            Func<IEnumerable<Pair<T>>> getSelectedElements = () => stack.Select(x => list[x]).Reverse();
            int index = 0;

            while (!cancellationToken.IsCancellationRequested && 
                    stack.Count < numElementsToReturn)
            {
                var validationContext = new FixturesGeneratorValidationContext<T>(
                    partialSolution, 
                    FixtureSets<T>.NewItem(getSelectedElements().ToList()));

                while (!cancellationToken.IsCancellationRequested && 
                        index < list.Count && 
                        !FixtureItemIsValid(list[index], validationContext))
                {
                    index++;
                }
                
                if (index < list.Count)
                {
                    stack.Push(index);
                    Pair<T> selectedItem = list[index];
                    index = 0;
                }
                else
                {
                    if (stack.Count > 0)
                    {
                        _logger.LogDebug("[{0}] was not a valid choice. Discarding {1} and trying next one.", string.Join(", ", getSelectedElements()), list[stack.Peek()]);
                        index = stack.Pop() + 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (stack.Count >= numElementsToReturn)
            {
                return FixtureSets<T>.NewItem(getSelectedElements().ToList());
            }
            else
            {
                // discard partial solution and return an empty result
                return FixtureSets<T>.NewItem();;
            }
        }
    }
}
