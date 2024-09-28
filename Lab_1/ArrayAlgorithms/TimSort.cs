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
        public const int MIN_MERGE = 32;
        public const int MIN_GALLOP = 7;
        public const int INITIAL_TMP_STORAGE_LENGTH = 256;

        static void InsertionSort(T[] a, int lo, int hi)
        {
            for (int i = lo; i < hi; i++)
            {
                var val = a[i];
                int j = i - 1;
                while (j >= 0 && (a[j] > val))
                {
                    a[j + 1] = a[j];
                    j--;
                }
                a[j + 1] = val;
            }
        }

        public static void BinarySort(T[] a, int lo, int hi, int start)
        {
            if (start == lo) start++;

            for (; start < hi; start++)
            {
                var pivot = a[start];
                int left = lo, right = start;
                while (left < right)
                {
                    int mid = (left + right) >> 1;
                    if (pivot < a[mid]) right = mid;
                    else left = mid + 1;
                }

                int n = start - left;
                if (n == 2)
                {
                    a[left + 2] = a[left + 1];
                    a[left + 1] = a[left];
                }
                else if (n == 1)
                {
                    a[left + 1] = a[left];
                }
                else
                {
                    ArrayCopy(a, left, a, left + 1, n);
                }
                a[left] = pivot;
            }
        }

        static void ArrayCopy(T[] src, int srcPos, T[] des, int desPos, int len)
        {
            Array.Copy(src, srcPos, des, desPos, len);
        }

        public T[] EnsureCapacity(TimSort<T> tim, int minCapacity)
        {
            T[] tmp = tim.tmp;
            if (tmp.Length < minCapacity)
            {
                int newSize = minCapacity;
                newSize |= newSize >> 1;
                newSize |= newSize >> 2;
                newSize |= newSize >> 4;
                newSize |= newSize >> 8;
                newSize |= newSize >> 16;
                newSize++;

                if (newSize < 0)
                {
                    newSize = minCapacity;
                }
                else
                {
                    newSize = Math.Min(newSize, tim.arr.Length >> 1);
                }

                tim.tmp = new T[newSize];
            }

            return tmp;
        }

        public T[] arr;
        public T[] tmp; // Temporary array used for merging
        public int minGallop = MIN_GALLOP; // Minimum gallop value
        public int stackSize = 0; // Number of pending runs on the stack

        public int[] runBase; // Base indices for each run
        public int[] runLen; // Length of each run

        public TimSort()
        {
        }

        public TimSort(T[] arr)
        {
            this.arr = arr;
            int len = arr.Length;

            // Allocating temporary storage for merging
            tmp = new T[len < 2 * INITIAL_TMP_STORAGE_LENGTH ? len >> 1 : INITIAL_TMP_STORAGE_LENGTH];

            // Maximum stack size based on array length
            int stackLen = (len < 120 ? 5 :
                           len < 1542 ? 10 :
                           len < 119151 ? 19 : 40);

            runBase = new int[stackLen];
            runLen = new int[stackLen];
        }

        public static void Sort(T[] a)
        {
            Sort(a, 0, a.Length);
        }

        public static void Sort(T[] a, int lo, int hi)
        {
            if (lo < 0 || lo > hi || hi > a.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            int nRemaining = hi - lo;

            if (nRemaining < 2)
            {
                return; // Already sorted
            }

            // Use insertion sort for small arrays
            if (nRemaining < MIN_MERGE)
            {
                var initRunLen = CountRunAndMakeAscending(a, lo, hi);

                InsertionSort(a, lo, hi);
                //BinarySort(a, lo, hi, lo + initRunLen, comp);
                return;
            }

            TimSort<T> ts = new TimSort<T>(a);
            int minRun = GetMinRun(nRemaining);

            do
            {
                // Identify the next run
                int runLen = CountRunAndMakeAscending(a, lo, hi);

                if (runLen < minRun)
                {
                    int force = nRemaining <= minRun ? nRemaining : minRun;
                    BinarySort(a, lo, lo + force, lo + runLen-1);
                    //InsertionSort(a, lo, lo + force);
                    runLen = force;
                }

                // Push the run onto the pending run stack
                ts.PushRun(lo, runLen);

                ts.MergeCollapse();

                lo += runLen;
                nRemaining -= runLen;
            } while (nRemaining != 0);

            ts.MergeForceCollapse();
        }

        public static int GetMinRun(int n)
        {
            int r = 0; // Becomes 1 if any bits are shifted off
            while (n >= MIN_MERGE)
            {
                r |= (n & 1);
                n >>= 1;
            }
            return n + r;
        }

        public static int CountRunAndMakeAscending(T[] a, int lo, int hi)
        {
            int runHi = lo + 1;

            if (runHi == hi)
            {
                return 1;
            }

            // Find the end of the run
            if (a[runHi++] < a[lo])
            {
                while (runHi < hi && a[runHi] < a[runHi - 1])
                {
                    runHi++;
                }
                ReverseRange(a, lo, runHi);
            }
            else
            {
                while (runHi < hi && a[runHi] >= a[runHi - 1])
                {
                    runHi++;
                }
            }

            return runHi - lo;
        }

        public static void ReverseRange(T[] a, int lo, int hi)
        {
            hi--;
            while (lo < hi)
            {
                T temp = a[lo];
                a[lo++] = a[hi];
                a[hi--] = temp;
            }
        }

        public void PushRun(int runBase, int runLen)
        {
            this.runBase[stackSize] = runBase;
            this.runLen[stackSize] = runLen;
            stackSize++;
        }

        public void MergeCollapse()
        {
            while (stackSize > 1)
            {
                int n = stackSize - 2;

                if (n >= 1 && runLen[n - 1] <= runLen[n] + runLen[n + 1])
                {
                    if (runLen[n - 1] < runLen[n + 1])
                    {
                        n--;
                    }
                    MergeAt(n);
                }
                else if (runLen[n] <= runLen[n + 1])
                {
                    MergeAt(n);
                }
                else
                {
                    break;
                }
            }
        }

        public void MergeForceCollapse()
        {
            while (stackSize > 1)
            {
                int n = stackSize - 2;

                if (n > 0 && runLen[n - 1] < runLen[n + 1])
                {
                    n--;
                }

                MergeAt(n);
            }
        }

        public void MergeAt(int i)
        {
            int base1 = runBase[i];
            int len1 = runLen[i];
            int base2 = runBase[i + 1];
            int len2 = runLen[i + 1];

            runLen[i] = len1 + len2;

            if (i == stackSize - 3)
            {
                runBase[i + 1] = runBase[i + 2];
                runLen[i + 1] = runLen[i + 2];
            }

            stackSize--;

            int k = GallopRight(arr[base2], arr, base1, len1, 0);

            base1 += k;
            len1 -= k;

            if (len1 == 0)
            {
                return;
            }

            len2 = GallopLeft(arr[base1 + len1 - 1], arr, base2, len2, len2 - 1);

            if (len2 == 0)
            {
                return;
            }

            if (len1 <= len2)
            {
                MergeLow(base1, len1, base2, len2);
            }
            else
            {
                MergeHigh(base1, len1, base2, len2);
            }
        }

        public static int GallopLeft(T key, T[] a, int baseIdx, int len, int hint)
        {
            // assert len > 0 && hint >= 0 && hint < len;
            int lastOfs = 0;
            int ofs = 1;
            if (key > a[baseIdx + hint])
            {
                // Gallop right until a[baseIdx + hint + lastOfs] < key <= a[baseIdx + hint + ofs]
                int maxOfs = len - hint;
                while (ofs < maxOfs && key > a[baseIdx + hint + ofs])
                {
                    lastOfs = ofs;
                    ofs = (ofs << 1) + 1;
                    if (ofs <= 0)
                    {
                        ofs = maxOfs; // int overflow
                    }
                }
                if (ofs > maxOfs)
                {
                    ofs = maxOfs; // int overflow
                }

                // Make offsets relative to base
                lastOfs += hint;
                ofs += hint;
            }
            else // key <= a[baseIdx + hint]
            {
                // Gallop left until a[baseIdx + hint - ofs] < key <= a[baseIdx + hint - lastOfs]
                int maxOfs = hint + 1;
                while (ofs < maxOfs && (key <= a[baseIdx + hint - ofs]))
                {
                    lastOfs = ofs;
                    ofs = (ofs << 1) + 1;
                    if (ofs <= 0)
                    {
                        ofs = maxOfs; // int overflow
                    }
                }
                if (ofs > maxOfs)
                {
                    ofs = maxOfs;
                }

                // Make offsets relative to base
                int tmp = lastOfs;
                lastOfs = hint - ofs;
                ofs = hint - tmp;
            }
            // assert -1 <= lastOfs && lastOfs < ofs && ofs <= len;

            lastOfs++;
            while (lastOfs < ofs)
            {
                int m = lastOfs + ((ofs - lastOfs) >> 1);

                if (key > a[baseIdx + m])
                {
                    lastOfs = m + 1; // a[baseIdx + m] < key 
                }
                else
                {
                    ofs = m; // key <= a[baseIdx + m]
                }
            }
            // assert lastOfs == ofs; // so a[baseIdx + ofs - 1] < key <= a[baseIdx + ofs]

            return ofs;
        }


        public static int GallopRight(T key, T[] a, int baseIdx, int len, int hint)
        {
            // assert len > 0 && hint >= 0 && hint < len;
            int lastOfs = 0;
            int ofs = 1;
            if (key < a[baseIdx + hint])
            {
                // Gallop left until a[baseIdx + hint - ofs] < key <= a[baseIdx + hint - lastOfs]
                int maxOfs = hint + 1;
                while (ofs < maxOfs && key < a[baseIdx + hint - ofs])
                {
                    lastOfs = ofs;
                    ofs = (ofs << 1) + 1;
                    if (ofs <= 0)
                    {
                        ofs = maxOfs; // int overflow
                    }
                }
                if (ofs > maxOfs)
                {
                    ofs = maxOfs; // int overflow
                }

                // Make offsets relative to base
                int tmp = lastOfs;
                lastOfs = hint - ofs;
                ofs = hint - tmp;
            }
            else // key >= a[baseIdx + hint]
            {
                // Gallop right until a[baseIdx + hint + lastOfs] < key <= a[baseIdx + hint + ofs]
                int maxOfs = len - hint;
                while (ofs < maxOfs && key >= a[baseIdx + hint + ofs])
                {
                    lastOfs = ofs;
                    ofs = (ofs << 1) + 1;
                    if (ofs <= 0)
                    {
                        ofs = maxOfs; // int overflow
                    }
                }
                if (ofs > maxOfs)
                {
                    ofs = maxOfs; // int overflow
                }

                // Make offsets relative to base
                lastOfs += hint;
                ofs += hint;
            }
            // assert -1 <= lastOfs && lastOfs < ofs && ofs <= len;

            lastOfs++;
            while (lastOfs < ofs)
            {
                int m = lastOfs + ((ofs - lastOfs) >> 1);

                if (key < a[baseIdx + m])
                {
                    ofs = m; // key < a[baseIdx + m]
                }
                else
                {
                    lastOfs = m + 1; // a[baseIdx + m] <= key
                }
            }
            // assert lastOfs == ofs; // so a[baseIdx + ofs - 1] < key <= a[baseIdx + ofs]

            return ofs;
        }

        public void MergeLow(int base1, int len1, int base2, int len2)
        {
            EnsureCapacity(this, len1);
            // Copy first run into the temporary array
            ArrayCopy(arr, base1, tmp, 0, len1);

            int cursor1 = 0; // Indexes into tmp array
            int cursor2 = base2; // Indexes into arr
            int dest = base1; // Indexes into arr

            // Move the first element of the second run into place
            arr[dest++] = arr[cursor2++];
            len2--;

            if (len2 == 0)
            {
                ArrayCopy(tmp, cursor1, arr, dest, len1);
                return;
            }

            if (len1 == 1)
            {
                ArrayCopy(arr, cursor2, arr, dest, len2);
                arr[dest + len2] = tmp[cursor1]; // Last element of the first run
                return;
            }

            int minGallop = this.minGallop;

            while (true)
            {
                int count1 = 0; // Number of consecutive elements from tmp
                int count2 = 0; // Number of consecutive elements from arr

                // Do the merging
                do
                {
                    if (arr[cursor2] < tmp[cursor1])
                    {
                        arr[dest++] = arr[cursor2++];
                        count2++;
                        count1 = 0;
                        len2--;

                        if (len2 == 0)
                        {
                            goto outer1;
                        }
                    }
                    else
                    {
                        arr[dest++] = tmp[cursor1++];
                        count1++;
                        count2 = 0;
                        len1--;

                        if (len1 == 1)
                        {
                            goto outer1;
                        }
                    }
                } while ((count1 | count2) < minGallop);

                // Gallop mode
                do
                {
                    count1 = GallopRight(arr[cursor2], tmp, cursor1, len1, 0);
                    if (count1 != 0)
                    {
                        ArrayCopy(tmp, cursor1, arr, dest, count1);
                        dest += count1;
                        cursor1 += count1;
                        len1 -= count1;

                        if (len1 <= 1)
                        {
                            goto outer1;
                        }
                    }

                    arr[dest++] = arr[cursor2++];
                    len2--;

                    if (len2 == 0)
                    {
                        goto outer1;
                    }

                    count2 = GallopLeft(tmp[cursor1], arr, cursor2, len2, 0);
                    if (count2 != 0)
                    {
                        ArrayCopy(arr, cursor2, arr, dest, count2);
                        dest += count2;
                        cursor2 += count2;
                        len2 -= count2;

                        if (len2 == 0)
                        {
                            goto outer1;
                        }
                    }

                    arr[dest++] = tmp[cursor1++];
                    len1--;

                    if (len1 == 1)
                    {
                        goto outer1;
                    }

                    minGallop--;
                } while (count1 >= MIN_GALLOP | count2 >= MIN_GALLOP);

                if (minGallop < 0)
                {
                    minGallop = 0;
                }

                minGallop += 2; // Penalize for leaving gallop mode
            }
        outer1:

            this.minGallop = minGallop < 1 ? 1 : minGallop;

            if (len1 == 1)
            {
                ArrayCopy(arr, cursor2, arr, dest, len2);
                arr[dest + len2] = tmp[cursor1];
            }
            else if (len1 == 0)
            {
                throw new InvalidOperationException("Comparison method violates its general contract!");
            }
            else
            {
                ArrayCopy(tmp, cursor1, arr, dest, len1);
            }
        }

        public void MergeHigh(int base1, int len1, int base2, int len2)
        {
            EnsureCapacity(this, len2);
            // Copy second run into the temporary array
            ArrayCopy(arr, base2, tmp, 0, len2);

            int cursor1 = base1 + len1 - 1; // Indexes into arr
            int cursor2 = len2 - 1; // Indexes into tmp array
            int dest = base2 + len2 - 1; // Indexes into arr

            // Move the last element of the first run into place
            arr[dest--] = arr[cursor1--];
            len1--;

            if (len1 == 0)
            {
                ArrayCopy(tmp, 0, arr, dest - (len2 - 1), len2);
                return;
            }

            if (len2 == 1)
            {
                dest -= len1;
                cursor1 -= len1;
                ArrayCopy(arr, cursor1 + 1, arr, dest + 1, len1);
                arr[dest] = tmp[cursor2]; // Last element of the second run
                return;
            }

            int minGallop = this.minGallop;

            while (true)
            {
                int count1 = 0; // Number of consecutive elements from arr
                int count2 = 0; // Number of consecutive elements from tmp

                // Do the merging
                do
                {
                    if (tmp[cursor2] < arr[cursor1])
                    {
                        arr[dest--] = arr[cursor1--];
                        count1++;
                        count2 = 0;
                        len1--;

                        if (len1 == 0)
                        {
                            goto outer2;
                        }
                    }
                    else
                    {
                        arr[dest--] = tmp[cursor2--];
                        count2++;
                        count1 = 0;
                        len2--;

                        if (len2 == 1)
                        {
                            goto outer2;
                        }
                    }
                } while ((count1 | count2) < minGallop);

                // Gallop mode
                do
                {
                    count1 = len1 - GallopRight(tmp[cursor2], arr, base1, len1, len1 - 1);
                    if (count1 != 0)
                    {
                        dest -= count1;
                        cursor1 -= count1;
                        len1 -= count1;
                        ArrayCopy(arr, cursor1 + 1, arr, dest + 1, count1);

                        if (len1 == 0)
                        {
                            goto outer2;
                        }
                    }

                    arr[dest--] = tmp[cursor2--];
                    len2--;

                    if (len2 == 1)
                    {
                        goto outer2;
                    }

                    count2 = len2 - GallopLeft(arr[cursor1], tmp, 0, len2, len2 - 1);
                    if (count2 != 0)
                    {
                        dest -= count2;
                        cursor2 -= count2;
                        len2 -= count2;
                        ArrayCopy(tmp, cursor2 + 1, arr, dest + 1, count2);

                        if (len2 <= 1)
                        {
                            goto outer2;
                        }
                    }

                    arr[dest--] = arr[cursor1--];
                    len1--;

                    if (len1 == 0)
                    {
                        goto outer2;
                    }

                    minGallop--;
                } while (count1 >= MIN_GALLOP | count2 >= MIN_GALLOP);

                if (minGallop < 0)
                {
                    minGallop = 0;
                }

                minGallop += 2; // Penalize for leaving gallop mode
            }
        outer2:

            this.minGallop = minGallop < 1 ? 1 : minGallop;

            if (len2 == 1)
            {
                dest -= len1;
                cursor1 -= len1;
                ArrayCopy(arr, cursor1 + 1, arr, dest + 1, len1);
                arr[dest] = tmp[cursor2];
            }
            else if (len2 == 0)
            {
                throw new InvalidOperationException("Comparison method violates its general contract!");
            }
            else
            {
                ArrayCopy(tmp, 0, arr, dest - (len2 - 1), len2);
            }

        }


        public Task Execute(T[] array)
        {
            TimSort<T>.Sort(array);
            return Task.CompletedTask;
        }
    }
}
