using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.ArrayAlgorithms
{
    internal class StrandSort<T> : IArrayAlgorithm<T> where T : IComparisonOperators<T, T, bool>
    {
        static List<T> Merge(List<T> result, List<T> strand)
        {
            List<T> merged = new List<T>();
            int i = 0, j = 0;

            while (i < result.Count && j < strand.Count)
            {
                if (result[i] < strand[j])
                {
                    merged.Add(result[i]);
                    i++;
                }
                else
                {
                    merged.Add(strand[j]);
                    j++;
                }
            }

            while (i < result.Count)
            {
                merged.Add(result[i]);
                i++;
            }

            while (j < strand.Count)
            {
                merged.Add(strand[j]);
                j++;
            }

            return merged;
        }

        static List<T> ExtractStrand(List<T> inputList)
        {
            List<T> strand = new List<T>();
            strand.Add(inputList[0]);
            inputList.RemoveAt(0);

            for (int i = 0; i < inputList.Count;)
            {
                if (inputList[i] >= strand[strand.Count - 1])
                {
                    strand.Add(inputList[i]);
                    inputList.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            return strand;
        }

        static List<T> StrandSortMethod(List<T> inputList)
        {
            List<T> result = new List<T>();

            while (inputList.Count > 0)
            {
                List<T> strand = ExtractStrand(inputList);
                result = Merge(result, strand);
            }

            return result;
        }
        public Task Execute(T[] data)
        {
            StrandSortMethod(data.ToList());
            return Task.CompletedTask;
        }
    }
}
