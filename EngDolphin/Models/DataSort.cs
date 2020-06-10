using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngDolphin.Client.Models
{
    public class DataSort
    {
        static public void Merge(float[] arr, int left, int middle, int right)
        {
            int i, j, k;
            int n1 = middle - left + 1;
            int n2 = right - middle;
            float[] L = new float[n1];
            float[] R = new float[n2];
            for (i = 0; i < n1; i++)
            {
                L[i] = arr[left + i];
            }
            for (j = 0; j < n2; j++)
            {
                R[j] = arr[middle + 1 + j];
            }
            i = 0;
            j = 0;
            k = left;
            while (i < n1 && j < n2)
            {
                if (L[i] <= R[j])
                {
                    arr[k] = L[i];
                    i++;
                }
                else
                {
                    arr[k] = R[j];
                    j++;
                }
                k++;
            }
            while (i < n1)
            {
                arr[k] = L[i];
                i++;
                k++;
            }
            while (j < n2)
            {
                arr[k] = R[j];
                j++;
                k++;
            }
        }
        static public void MergeSort(float[] arr, int left, int right)
        {
            if (left < right)
            {
                int middle = (left + right) / 2;
                MergeSort(arr, left, middle);
                MergeSort(arr, middle + 1, right);
                Merge(arr, left, middle, right);
            }
        }
        static public void Sort(float[] arr)
        {
            MergeSort(arr, 0, arr.Length);
        }

    }
}
