using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.ArrayAlgorithms
{
    public class TimSort<T> : IArrayAlgorithm<T> where T : IComparisonOperators<T, T, bool>
    {
        class Run(int start, int end, bool isAscending = true)
        {
            public int Start { get; set; } = start;
            public int End { get; set; } = end;
            public bool IsAscending { get; set; } = isAscending;
            public int Size => End - Start;
        }

        class Stack<K> : System.Collections.Generic.Stack<K>
        {
            public K Peek(int index)
            {
                return this.Skip(index).First();
            }
        }

        private static int CalcMinRun(int size)
        {
            int r = 0;
            while (size >= 64)
            {
                r |= size & 1;
                size >>= 1;
            }
            return size + r;
        }

        private static Run FindNextRun(T[] array, int start)
        {
            int i = start;
            int j = start + 1;
            bool isAscending = true;

            while (j < array.Length && array[j] == array[j - 1]) { j++; }

            if (j == array.Length)
            {
                return new(i, j, isAscending);
            }

            if (array[j] < array[j - 1])
            {
                isAscending = false;
                while (j < array.Length && array[j] <= array[j - 1])
                {
                    j++;
                }
            }
            else
            {
                while (j < array.Length && array[j] >= array[j - 1])
                {
                    j++;
                }
            }

            return new(i, j, isAscending);
        }


        private static void InsertionSort(T[] array, Run run)
        {
            for (int i = run.Start + 1; i < run.End; i++)
            {
                int j = i;
                while (j > run.Start && array[j - 1] > array[j])
                {
                    (array[j], array[j - 1]) = (array[j - 1], array[j]);
                    j--;
                }
            }
        }

        private static void ReverseRange(T[] array, int start, int end)
        {
            while (start < end)
            {
                (array[end], array[start]) = (array[start], array[end]);
                start++;
                end--;
            }
        }

        private static List<Run> FirstPass(T[] array, int minRunSize)
        {
            int i = 0;
            var runs = new List<Run>();
            while (i < array.Length)
            {
                var currRun = TimSort<T>.FindNextRun(array, i);
                if (currRun.Size >= minRunSize || currRun.End == array.Length)
                {
                    i += currRun.Size;
                    runs.Add(currRun);
                    if (!currRun.IsAscending)
                    {
                        TimSort<T>.ReverseRange(array, currRun.Start, currRun.End - 1);
                    }
                }
                else
                {
                    if (!currRun.IsAscending)
                    {
                        TimSort<T>.ReverseRange(array, currRun.Start, currRun.End - 1);
                    }

                    int end;
                    if (i + minRunSize - currRun.Size < array.Length)
                    {
                        end = i + minRunSize - currRun.Size;
                    }
                    else
                    {
                        end = array.Length-1;
                    }

                    currRun = new(i, end);
                    TimSort<T>.InsertionSort(array, currRun);
                    runs.Add(currRun);
                    i += currRun.Size;
                }
            }

            return runs;
        }

        private static void Merge(T[] array, Run run1, Run run2)
        {

            var tempArr = new T[run1.Size];

            for (int i = 0; i < tempArr.Length; i++)
            {
                tempArr[i] = array[run1.Start + i];
            }

            var leftArrayIndex = run1.Start;
            var rightArrayIndex = run2.Start;
            var tempArrIndex = 0;

            while (rightArrayIndex < run2.End && tempArrIndex < tempArr.Length)
            {
                if (array[rightArrayIndex] < tempArr[tempArrIndex])
                {
                    array[leftArrayIndex] = array[rightArrayIndex];
                    rightArrayIndex++;
                }
                else
                {
                    array[leftArrayIndex] = tempArr[tempArrIndex];
                    tempArrIndex++;
                }
                leftArrayIndex++;
            }

            if (rightArrayIndex == run2.End)
            {
                while (tempArrIndex < tempArr.Length)
                {
                    array[leftArrayIndex] = tempArr[tempArrIndex];
                    tempArrIndex++;
                    leftArrayIndex++;
                }
            }
            else
            {
                while (rightArrayIndex < run2.End)
                {
                    array[leftArrayIndex] = array[rightArrayIndex];
                    rightArrayIndex++;
                    leftArrayIndex++;
                }
            }
        }

        private static void SecondPass(T[] array, List<Run> runs)
        {
            Stack<Run> stack = new();
            int i = 0;
            while (i < runs.Count)
            {
                stack.Push(runs[i]!);

                if (stack.Count >= 3 && (stack.Peek(2).Size > stack.Peek(1).Size + stack.Peek().Size && stack.Peek(1).Size > stack.Peek().Size))
                {
                    var rightRun = stack.Pop();
                    var leftRun = stack.Pop();
                    TimSort<T>.Merge(array, leftRun, rightRun);
                }
                else while (stack.Count >= 3 && (stack.Peek(2).Size < stack.Peek(1).Size + stack.Peek().Size || stack.Peek(1).Size < stack.Peek().Size))
                    {
                        var firstRun = stack.Pop();
                        var secondRun = stack.Pop();
                        var thirdRun = stack.Pop();
                        if (firstRun.Size < thirdRun.Size)
                        {
                            TimSort<T>.Merge(array, secondRun, firstRun);
                            stack.Push(thirdRun);
                            stack.Push(new(secondRun.Start, firstRun.End));
                        }
                        else
                        {
                            TimSort<T>.Merge(array, thirdRun, secondRun);
                            stack.Push(new(thirdRun.Start, secondRun.End));
                            stack.Push(firstRun);
                        }
                    }
                i++;
            }
            if (stack.Count == 2)
            {
                var firstRun = stack.Pop();
                var secondRun = stack.Pop();
                TimSort<T>.Merge(array, secondRun, firstRun);
            }

        }

        private static void Sort(T[] array)
        {
            var runs = TimSort<T>.FirstPass(array, TimSort<T>.CalcMinRun(array.Length));
            TimSort<T>.SecondPass(array, runs);
        }

        public Task Execute(T[] array)
        {
            TimSort<T>.Sort(array);
            return Task.CompletedTask;
        }
    }
}
