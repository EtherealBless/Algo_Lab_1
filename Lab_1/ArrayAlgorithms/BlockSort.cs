using Google.OrTools.ConstraintSolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.ArrayAlgorithms
{
    internal class BlockSort<T> : IArrayAlgorithm<int> where T : INumber<int>
    {
        public static void Sort(int[] arr, int bucketCount)
        {
            if (arr.Length == 0) return;

            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            foreach (var num in arr)
            {
                if (num < minValue) minValue = (float)num;
                if (num > maxValue) maxValue = (float)num;
            }

            float range = maxValue - minValue;
            if (range == 0) return;

            float[] normalizedArr = new float[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                normalizedArr[i] = ((float)arr[i] - minValue) / range;
            }

            List<float>[] buckets = new List<float>[bucketCount];
            for (int i = 0; i < bucketCount; i++)
            {
                buckets[i] = new List<float>();
            }

            foreach (var value in normalizedArr)
            {
                int bucketIndex = (int)(bucketCount * value);
                if (bucketIndex == bucketCount) bucketIndex--;
                buckets[bucketIndex].Add(value);
            }

            for (int i = 0; i < bucketCount; i++)
            {
                buckets[i].Sort();
            }

            int index = 0;
            for (int i = 0; i < bucketCount; i++)
            {
                foreach (var value in buckets[i])
                {
                    arr[index++] = (int)Math.Ceiling(value * range + minValue);
                }
            }
        }


        public Task Execute(int[] data)
        {
            Sort(data, 10);

            return Task.CompletedTask;
        }
    }
}
