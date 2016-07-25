using System;
using System.Threading;

namespace FixturesGenerator.Generators
{
    public class LeagueFixturesGenerator : BasicFixturesGenerator
    {
        public LeagueFixturesGenerator() : this(default(CancellationToken))
        {
        }

        public LeagueFixturesGenerator(CancellationToken cancellationToken) : base(cancellationToken)
        {
        }

        protected override void GenerateOptionsAt(ref BidimensionalSet[,] options, int[] coordinates)
        {
            base.GenerateOptionsAt(ref options, coordinates);
            if (!m_CancellationToken.IsCancellationRequested) DiscardInvertedOptionsInSameLeagueHalf(ref options, coordinates);
            if (!m_CancellationToken.IsCancellationRequested && coordinates[0] > 0) SortOptionsAt(ref options, coordinates);
        }

        /// <summary>Discard inverted options used in the same half of the league.</summary>
        /// <remarks>When generating options for a particular round and event, keep inverted events in each half of the league.</remarks>
        protected void DiscardInvertedOptionsInSameLeagueHalf(ref BidimensionalSet[,] options, int[] coordinates)
        {
            int roundIndex = coordinates[0];

            // Calculate half league bounds based on the current round
            int leagueHalfStartRound = 0;
            int leagueHalfEndRound = roundIndex;
            if (roundIndex >= RowCountHalf)
            {
                // select second half bounds
                leagueHalfStartRound = RowCountHalf;
                leagueHalfEndRound = Math.Min(roundIndex, LastRowIndex);
            }

            int[] startCoordinates = new int[2] { leagueHalfStartRound, 0 };
            int[] endCoordinates = new int[2] { leagueHalfEndRound, coordinates[1] };
            MoveCoordinatesBack(ref endCoordinates, LastColIndex);
            if (endCoordinates[0] < startCoordinates[0]) return;

            // Check for each option generated at current coordinates
            var optionsGenerated = options[coordinates[0], coordinates[1]];
            for (int i = 0; i <= optionsGenerated.UpperBound; i++)
            {
                // Build the inverted option
                int[] optionToDiscard = new int[2]
                {
                    optionsGenerated.Array[i, 1],
                    optionsGenerated.Array[i, 0]
                };

                // If the inverted option was already used in the same half
                // of the league then, discard this option.
                if (OptionHasBeenUsed(
                    options, 
                    optionToDiscard, 
                    startCoordinates, 
                    endCoordinates))
                {
                    DiscardOptionAt(ref optionsGenerated, i);
                    options[coordinates[0], coordinates[1]] = optionsGenerated;
                }
            }
        }

        /// <summary>Returns True if the given option has been used between 2 matrix positions.</summary>
        /// <param name="startCoordinates">Inclusive start coordinates.</param>
        /// <param name="endCoordinates">Inclusive end coordinates.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any of the following:
        /// <list type="bullet">
        /// <item><description><paramref name="options"/> is null</description></item>
        /// <item><description><paramref name="optionToFind"/> is null</description></item>
        /// <item><description><paramref name="startCoordinates"/> is null</description></item>
        /// <item><description><paramref name="endCoordinates"/> is null</description></item>
        /// </list>
        /// </exception>
        protected bool OptionHasBeenUsed(
            BidimensionalSet[,] options, 
            int[] optionToFind,
            int[] startCoordinates,
            int[] endCoordinates)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (optionToFind == null) throw new ArgumentNullException(nameof(optionToFind));
            if (startCoordinates == null) throw new ArgumentNullException(nameof(startCoordinates));
            if (endCoordinates == null) throw new ArgumentNullException(nameof(endCoordinates));

            int endCoordinatesLastValidX = endCoordinates[1];
            int[] currentPosition = new int[2]
            {
                startCoordinates[0],
                startCoordinates[1]
            };

            while (currentPosition[0] <= endCoordinates[0])
            {
                var optionsAtCoordinates = options[currentPosition[0], currentPosition[1]];
                if (optionsAtCoordinates.Array[optionsAtCoordinates.LowerBound, 0] == optionToFind[0] && 
                    optionsAtCoordinates.Array[optionsAtCoordinates.LowerBound, 1] == optionToFind[1]) 
                    {
                        return true;
                    }
                
                int lastAllowedX = currentPosition[0] == endCoordinates[0] ? endCoordinatesLastValidX : LastColIndex;
                MoveCoordinatesForward(ref currentPosition, lastAllowedX);
            }

            return false;
        }

        /// <summary>Sort the options collection at given position, from most preferred to less ones.</summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="coordinates"/> is null.</exception>
        protected virtual void SortOptionsAt(ref BidimensionalSet[,] options, int[] coordinates)
        {
            if (options == null) throw new ArgumentNullException(nameof(coordinates));

            // Get the options at the coordinates
            var optionsAtPosition = options[coordinates[0], coordinates[1]];
            if (optionsAtPosition.UpperBound == 0) return;

            // Get each left element from previous row
            int previousRow = coordinates[0] - 1;
            int[] prevRowLeftElements = new int[ColCount];
            for (int i = 0; i < ColCount; i++)
            {
                var prevRowOptions = options[previousRow, i];
                prevRowLeftElements[i] = prevRowOptions.Array[prevRowOptions.LowerBound, 0];
            }

            // Each option compares its left index and its right index against the previous row
            // in order to find out how good it is or which priority does it need (high, medium or low).

            int optionIndex = 0;
            int optionsAtPositionUpperBound = optionsAtPosition.UpperBound;
            int firstMediumPriorityIndex = -1;
            while (optionIndex <= optionsAtPositionUpperBound)
            {
                // Compare this option's left index against the previous row.
                int leftIndex = optionsAtPosition.Array[optionIndex, 0];
                if (IndexInCollection(leftIndex, prevRowLeftElements, ColCount))
                {
                    // Left index of this option is also a left index in the previous row so, give this option as very low preference.
                    SwapOptions(ref optionsAtPosition, optionIndex, optionsAtPositionUpperBound);
                    optionsAtPositionUpperBound--;
                }
                else
                {
                    // Left index of current option is good, now check the right index to determine how good is this option.
                    int rightIndex = optionsAtPosition.Array[optionIndex, 1];
                    if (!IndexInCollection(rightIndex, prevRowLeftElements, ColCount))
                    {
                        // Left index was good but right index is not then, give this option medium preference.

                        // Set a pointer to where the first medium preference option is.
                        if (firstMediumPriorityIndex == -1)
                        {
                            firstMediumPriorityIndex = optionIndex;
                        }
                    }
                    else
                    {
                        // Left and right indexes in this option are good so, keep this option as high preference and keep moving to check the following.
                        
                        // If we know where the first medium preference option is, swap it with the current high prefrence option.
                        if (firstMediumPriorityIndex != -1)
                        {
                            SwapOptions(ref optionsAtPosition, firstMediumPriorityIndex, optionIndex);
                            firstMediumPriorityIndex++;
                        }
                    }
                    optionIndex++;
                }
            }

            // Set the new options array at the same coordinates
            options[coordinates[0], coordinates[1]] = optionsAtPosition;
        }

        /// <summary>Returns whether the index is in the collection or not.</summary>
        /// <param name="count">The number of indexes to check in the collection.</param>
        /// <remarks>If the count if greater than the length of the array, it only searches inside the array.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is null.</exception>
        protected static bool IndexInCollection(int index, int[] collection, int count)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            int validatedCount = Math.Min(count, collection.Length);

            for (int i = 0; i < validatedCount; i++)
            {
                if (collection[i] == index) return true;
            }
            return false;
        }

        /// <summary>Append a new option and increase upper bound.</summary>
        /// <exception cref="ArgumentNullException">Thrown when the options array is null.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown when the options array is full.</exception>
        protected static void AppendOption(ref BidimensionalSet options, int[] optionToAppend)
        {
            if (options.Array == null) throw new ArgumentException(nameof(options));
            if (options.UpperBound == options.Array.Length) throw new IndexOutOfRangeException();

            options.UpperBound++;
            options.Array[options.UpperBound, 0] = optionToAppend[0];
            options.Array[options.UpperBound, 1] = optionToAppend[1];
        }
    }
}