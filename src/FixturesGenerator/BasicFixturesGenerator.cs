using System;
using System.Threading;

namespace FixturesGenerator.Generators
{
    public class BasicFixturesGenerator : IFixturesGenerator
    {
        public const byte MaxNumElements = 32;

        protected int NumElements { get; private set; }
        protected int RowCount { get; private set; }
        protected int RowCountHalf { get; private set; }
        protected int LastRowIndex { get; private set; }
        protected int ColCount { get; private set; }
        protected int LastColIndex { get; private set; }
        
        protected readonly CancellationToken m_CancellationToken;

        public BasicFixturesGenerator() : this(default(CancellationToken))
        {
        }

        public BasicFixturesGenerator(CancellationToken cancellationToken)
        {
            if (cancellationToken == null)
            {
                m_CancellationToken = CancellationToken.None;
            }
            else
            {
                m_CancellationToken = cancellationToken;
            }
        }

        /// <summary>Generate fixtures for a given number of elements.</summary>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="numElements"/> is any of the following:
        /// <list type="bullet">
        /// <item><description>less than 2</description></item>
        /// <item><description>greater than <see cref="MaxNumElements"/></description></item>
        /// <item><description>an odd number</description></item>
        /// </list>
        /// </exception>
        public int[,,] GenerateFixtures(byte numElements)
        {
            if (numElements < 2) throw new ArgumentException($"{nameof(numElements)} must be >= 2");
            if (numElements > MaxNumElements) throw new ArgumentException($"{nameof(numElements)} must be <= {MaxNumElements}");
            if ((numElements & 1) != 0) throw new ArgumentException($"{nameof(numElements)} must be a number divisible by 2");

            // Initialise variables
            NumElements = numElements;
            RowCountHalf = numElements - 1;
            RowCount = RowCountHalf << 1;
            ColCount = numElements >> 1;
            LastRowIndex = RowCount - 1;
            LastColIndex = ColCount - 1;

            int[,,] fixtures = new int[RowCount, ColCount, 2];
            int[] coordinates = new int[2]; // Accessed [Y, X]

            var options = new BidimensionalSet[RowCount, ColCount];
            options[0, 0] = new BidimensionalSet(
                GenerateElementsCombinations(numElements, RowCountHalf, ColCount));

            // [Bactracking Algorithm] Push first node
            options[0, 0].LowerBound = 0;
            fixtures[0, 0, 0] = options[0, 0].Array[0, 0];
            fixtures[0, 0, 1] = options[0, 0].Array[0, 1];

            MoveCoordinatesForward(ref coordinates, LastColIndex);
            GenerateOptionsAt(ref options, coordinates);

            // [Bactracking Algorithm] While stack is not empty
            while (!m_CancellationToken.IsCancellationRequested && 
                coordinates[0] >= 0 && coordinates[1] >= 0)
            {
                var currentPositionOptions = options[coordinates[0], coordinates[1]];

                // [Bactracking Algorithm] If the node at the top of the stack is leaf
                if (currentPositionOptions.UpperBound == -1)
                {
                    // [Bactracking Algorithm] If is a goal node
                    if (coordinates[0] == LastRowIndex && 
                        coordinates[1] == LastColIndex &&
                        currentPositionOptions.LowerBound > -1)
                    {
                        return fixtures;
                    }
                    else
                    {
                        // [Bactracking Algorithm] Pop node off the stack
                        MoveCoordinatesBack(ref coordinates, LastColIndex);
                    }
                }
                else
                {
                    // [Bactracking Algorithm] If the node at the top of the stack has untried children
                    if (currentPositionOptions.LowerBound < currentPositionOptions.UpperBound)
                    {
                        // [Bactracking Algorithm] Push the next untried child onto the stack
                        UseNextOption(ref options, ref fixtures, coordinates);

                        // Prepare next iteration
                        MoveCoordinatesForward(ref coordinates, LastColIndex);
                        if (coordinates[0] < RowCount)
                        {
                            GenerateOptionsAt(ref options, coordinates);
                        }
                        else
                        {
                            return fixtures;
                        }
                    }
                    else
                    {
                        // [Bactracking Algorithm] Pop node off the stack
                        MoveCoordinatesBack(ref coordinates, LastColIndex);
                    }
                }
            }

            return null;
        }

        /// <summary>Decrease coordinates by X then by Y.</summary>
        protected static void MoveCoordinatesBack(ref int[] coordinates, int maxValidX)
        {
            if (coordinates[1] == 0) // if X=0
            {
                coordinates[0]--; // Y--
                coordinates[1] = maxValidX; // X=maxValidX
            }
            else
            {
                coordinates[1]--; // X--
            }
        }

        /// <summary>Increase coordinates by X then by Y.</summary>
        protected static void MoveCoordinatesForward(ref int[] coordinates, int maxValidX)
        {
            if (coordinates[1] == maxValidX) // if X=maxValidX
            {
                coordinates[0]++;   // Y++
                coordinates[1] = 0; // x=0
            }
            else
            {
                coordinates[1]++; // X++
            }
        }

        /// <summary>Set the next untried option.</summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="coordinates"/> is null.</exception>
        protected static void UseNextOption(ref BidimensionalSet[,] options, ref int[,,] fixtures, int[] coordinates)
        {
            if (coordinates == null) throw new ArgumentNullException(nameof(coordinates));

            int y = coordinates[0];
            int x = coordinates[1];

            options[y, x].LowerBound++;

            var optionToUse = options[y, x];
            fixtures[y, x, 0] = optionToUse.Array[optionToUse.LowerBound, 0];
            fixtures[y, x, 1] = optionToUse.Array[optionToUse.LowerBound, 1];
        }

        /// <summary>Generate and set options at the given position.</summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="coordinates"/> is null.</exception>
        protected virtual void GenerateOptionsAt(ref BidimensionalSet[,] options, int[] coordinates)
        {
            if (m_CancellationToken.IsCancellationRequested) return;
            if (coordinates == null) throw new ArgumentNullException(nameof(coordinates));

            // Generate options for Y>0, X=0
            if (coordinates[1] == 0) 
            {
                // Generate new options based on all available options at Y=0, X=0
                int baseOptionsRowIndex = coordinates[0] == RowCountHalf ? 0 : coordinates[0] - 1;
                var rowOptions = options[baseOptionsRowIndex, 0];
                int newOptionsSetLength = rowOptions.UpperBound - rowOptions.LowerBound;
                int[,] newOptionsItems = new int[newOptionsSetLength, 2];
                Array.Copy(
                    rowOptions.Array, 
                    (rowOptions.LowerBound + 1) << 1, 
                    newOptionsItems, 
                    0, 
                    newOptionsItems.Length);
                
                var newOptionsSet = new BidimensionalSet(newOptionsItems);

                // Find and discard all used options
                int[] optionsCoordinates = new int[2];
                // Skip option at 0,0 as we know it won't be in the options array
                MoveCoordinatesForward(ref optionsCoordinates, LastColIndex);
                while (optionsCoordinates[0] < coordinates[0])
                {
                    var optionsAtPosition = options[optionsCoordinates[0], optionsCoordinates[1]];
                    int[] optionToDiscard = new int[2]
                    {
                        optionsAtPosition.Array[optionsAtPosition.LowerBound, 0],
                        optionsAtPosition.Array[optionsAtPosition.LowerBound, 1]
                    };

                    FindAndDiscardOption(ref newOptionsSet, optionToDiscard);
                    MoveCoordinatesForward(ref optionsCoordinates, LastColIndex);
                }

                options[coordinates[0], 0] = newOptionsSet;
            }
            // Generate options for Y>0, X>0
            else
            {
                // Get previous options struct
                int[] previousCoordinates = new int[2]
                {
                    coordinates[0],
                    coordinates[1]
                };
                MoveCoordinatesBack(ref previousCoordinates, LastColIndex);

                var previousPosition = options[previousCoordinates[0], previousCoordinates[1]];

                // Based on the previously used option, generate the new ones
                int newOptionsSetLength = previousPosition.UpperBound - previousPosition.LowerBound;
                if (newOptionsSetLength > 0)
                {
                    int[,] newOptionsItems = new int[newOptionsSetLength, 2];
                    Array.Copy(
                        previousPosition.Array, 
                        (previousPosition.LowerBound + 1) << 1, 
                        newOptionsItems, 
                        0, 
                        newOptionsItems.Length);

                    // New options containing only the untried children of the previous option
                    var newOptionsSet = new BidimensionalSet(newOptionsItems);
                    
                    // Now we need to remove those options which are any combination of the previous selected option.
                    int selectedIndex1 = previousPosition.Array[previousPosition.LowerBound, 0];
                    int selectedIndex2 = previousPosition.Array[previousPosition.LowerBound, 1];
                    
                    int optionIndexToCheck = 0;
                    do
                    {
                        // If an option is found as any combination of the previous selected option then, discard it.
                        if (IsCombinationOfElements(
                                newOptionsItems[optionIndexToCheck, 0], 
                                newOptionsItems[optionIndexToCheck, 1], 
                                selectedIndex1, 
                                selectedIndex2))
                        {
                            DiscardOptionAt(ref newOptionsSet, optionIndexToCheck);
                            // Now at position optionIndexToCheck there's the element that 
                            // was at position UpperBound, which needs to be checked too.
                        }
                        else
                        {
                            optionIndexToCheck++;
                        }
                    } while (optionIndexToCheck <= newOptionsSet.UpperBound);

                    options[coordinates[0], coordinates[1]] = newOptionsSet;
                }
                else
                {
                    options[coordinates[0], coordinates[1]] = new BidimensionalSet(new int[0, 0]);
                }
            }
        }

        /// <summary>Returns all allowed elements combinations.</summary>
        protected static int[,] GenerateElementsCombinations(int numElements, int rowCountHalf, int colCount)
        {
            int numCombinationsWithoutInversePairs = colCount * rowCountHalf;
            int[,] combinations = new int[numCombinationsWithoutInversePairs << 1, 2];

            int combinationsIndex = 0;
            int combinationsIndexForInversePair = numCombinationsWithoutInversePairs;
            
            // Every element is going to be paired with al the others
            for (int i = 0; i < rowCountHalf; i++)
            {
                for (int j = i + 1; j < numElements; j++)
                {
                    // Pair of elements
                    combinations[combinationsIndex, 0] = i;
                    combinations[combinationsIndex, 1] = j;
                    combinationsIndex++;

                    // Inverted pair of elements
                    combinations[combinationsIndexForInversePair, 0] = j;
                    combinations[combinationsIndexForInversePair, 1] = i;
                    combinationsIndexForInversePair++;
                }
            }
            
            // Return all combinations of elements
            return combinations;
        }

        /// <summary>Swap the element at position with the element at UpperBound and decrease UpperBound.</summary>
        /// <exception cref="IndexOutOfRangeException">Thrown when position is outside the option bounds.</exception>
        protected static void DiscardOptionAt(ref BidimensionalSet options, int position)
        {
            if (position < options.LowerBound || position > options.UpperBound)
            {
                throw new IndexOutOfRangeException(nameof(position)); 
            }

            if (position < options.UpperBound)
            {
                SwapOptions(ref options, position, options.UpperBound);
            }
            options.UpperBound--;
            // Options beyond UpperBound won't be considered.
        }

        /// <summary>Find a given option in the given collection and set it as discarded, if found.</summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionToDiscard"/> is null.</exception>
        protected static void FindAndDiscardOption(ref BidimensionalSet options, int[] optionToDiscard)
        {
            if (optionToDiscard == null) throw new ArgumentNullException(nameof(options));

            int i = 0;
            while (i <= options.UpperBound)
            {
                if (options.Array[i, 0] == optionToDiscard[0] &&
                    options.Array[i, 1] == optionToDiscard[1])
                    {
                        DiscardOptionAt(ref options, i);
                        break;
                    }
                i++;
            }
        }

        /// <summary>Swaps 2 options given their positions.</summary>
        /// <exception cref="ArgumentNullException">Thrown when the options collection is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the indexes indicated are outside the options collection bounds.</exception>
        protected static void SwapOptions(ref BidimensionalSet options, int index1, int index2)
        {
            if (options.Array == null) throw new ArgumentNullException();
            if (index1 < 0 || index1 >= options.Array.Length) throw new ArgumentOutOfRangeException(nameof(index1));
            if (index2 < 0 || index2 >= options.Array.Length) throw new ArgumentOutOfRangeException(nameof(index2));
            if (index1 == index2) return;

            int itemToExchange = options.Array[index1, 0];
            options.Array[index1, 0] = options.Array[index2, 0];
            options.Array[index2, 0] = itemToExchange;

            itemToExchange = options.Array[index1, 1];
            options.Array[index1, 1] = options.Array[index2, 1];
            options.Array[index2, 1] = itemToExchange;
        }

        /// <summary>Returns True when any element is part of both options.</summary>
        protected static bool IsCombinationOfElements(
            int option1Element1, int option1Element2,
            int option2Element1, int option2Element2)
        {
            return option1Element1 == option2Element1 ||
                option1Element1 == option2Element2 ||
                option1Element2 == option2Element1 ||
                option1Element2 == option2Element2;
        }
    }
}