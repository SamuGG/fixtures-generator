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
                int k = _rnd.Next(n);  
                n--;
                T item = shuffledList[k];  
                shuffledList[k] = shuffledList[n];  
                shuffledList[n] = item;  
            }  
            return shuffledList;
        }

        protected abstract IEnumerable<Pair<T>> GetPairs(IEnumerable<T> elements);

        protected abstract int GetSetsCount(IEnumerable<T> elements);

        protected abstract bool FixtureItemIsValid(Pair<T> item, IIFixturesGeneratorValidationContext<T> context);

        public virtual FixtureSets<T> GetFixtures(IEnumerable<T> elements, CancellationToken cancellationToken)
        {
            int itemsCount = elements.Count();

            // check parameters
            if (itemsCount % 2 != 0) throw new ArgumentException($"Parameter '{nameof(elements)}' must have an even number of elements.");
            if (itemsCount < 2) return new FixtureSets<T>();

            // build argument
            var args = new GetFixturesArgs<T>();
            args.PairCollection = GetPairs(elements);
            args.PairCount = itemsCount / 2;
            args.SetsCount = GetSetsCount(elements);

            // log start of process
            _logger.LogDebug("Begin of {0}", nameof(GetFixtures));
            _logger.LogDebug("Pairs for [{0}] : [{1}]", string.Join(", ", elements), string.Join(", ", args.PairCollection));
            _logger.LogDebug($"Expecting {args.SetsCount} sets with {args.PairCount} pairs each.");

            // do process
            var fixtures = GetFixtures(args, cancellationToken);

            // log end of process
            _logger.LogDebug("End of {0}", nameof(GetFixtures));
            if (fixtures.Count >= args.SetsCount)
            {
                _logger.LogDebug($"Solution found:");
                foreach (var fixtureSet in fixtures)
                {
                    _logger.LogDebug(string.Join(", ", fixtureSet));
                }
            }
            else
            {
                _logger.LogDebug("Could not find a valid solution.");
            }

            // return result
            return fixtures;
        }

        private FixtureSets<T> GetFixtures(GetFixturesArgs<T> args, CancellationToken cancellationToken)
        {
            var fixturesList = new FixtureSets<T>(args.SetsCount);
            var elements = new List<Pair<T>>(args.PairCollection);
            
            // while solution is not complete, iterate
            while (elements.Count > 0 && fixturesList.Count < args.SetsCount)
            {
                // get the next vaid option
                var fixtureSet = FindValidFixtureSet(
                    elements, 
                    args.PairCount, 
                    fixturesList, 
                    cancellationToken);

                // if a valid option was found, add it to the solution
                if (fixtureSet?.Count > 0)
                {
                    fixturesList.Add(fixtureSet);
                    _logger.LogDebug("Added set [{0}] to solution", string.Join(", ", fixtureSet));
                    foreach (var itemToRemove in fixtureSet) elements.Remove(itemToRemove);
                }
                else
                {
                    // if it reaches a point where can't find a valid option with
                    // the elements remaining, then stop iterating.
                    break;
                }
            }

            // discard incomplete solution
            if (cancellationToken.IsCancellationRequested || fixturesList.Count < args.SetsCount)
            {
                fixturesList.Clear();
            }

            return fixturesList;
        }

        private FixtureSet<T> FindValidFixtureSet(
            IEnumerable<Pair<T>> elements, 
            int numElementsToReturn,
            FixtureSets<T> partialSolution,
            CancellationToken cancellationToken)
        {
            var optionsListStates = new Stack<IEnumerable<Pair<T>>>();
            var validItems = FixtureSets<T>.NewItem(numElementsToReturn);
            var availableItems = new List<Pair<T>>(elements);
            
            var validationContext = new FixturesGeneratorValidationContext<T>(
                partialSolution, 
                validItems);

            // set pointer to the first option
            int index = 0;

            // while the set is not complete, iterate
            while (!cancellationToken.IsCancellationRequested && 
                    validItems.Count < numElementsToReturn)
            {
                // move to the next valid option
                while (!cancellationToken.IsCancellationRequested && 
                        index < availableItems.Count && 
                        !FixtureItemIsValid(availableItems[index], validationContext))
                {
                    index++;
                }
                
                if (index < availableItems.Count)
                {
                    // the element at index is a valid option
                    Pair<T> validItem = availableItems[index];
                    
                    // add valid item and keep the rest of elements (we may need them again if the option needs to be undone)
                    validItems.Add(validItem);
                    optionsListStates.Push(availableItems.Where((x, xindex) => xindex != index));
                    _logger.LogDebug("Sequence: {0}", string.Join(", ", validItems));
                    
                    // make a new list for next iteration
                    availableItems = new List<Pair<T>>(availableItems.UnrelatedItems(validItem));
                }
                else
                {
                    // no valid option found, check there's a previous valid item which can be undone
                    if (!cancellationToken.IsCancellationRequested && validItems.Count > 0)
                    {
                        // restore previous options list
                        availableItems.InsertRange(0, optionsListStates.Pop());
                        
                        // log undo step
                        var previousValidItem = validItems[validItems.Count - 1];
                        _logger.LogDebug("Undo {0} in [{1}]", previousValidItem, string.Join(", ", validItems));
                        
                        // remove previous valid item from partial solution
                        validItems.RemoveAt(validItems.Count - 1);
                    }
                    else
                    {
                        // no valid element found and no steps to undo so, stop iterating
                        break;
                    }
                }
                index = 0;
            }

            // return the complete set or nothing (when incomplete)
            if (validItems.Count >= numElementsToReturn)
            {
                return validItems;
            }
            else
            {
                return null;
            }
        }
    }
}
