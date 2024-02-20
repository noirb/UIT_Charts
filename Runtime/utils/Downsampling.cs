using System;
using System.Collections.Generic;
using UnityEngine;

namespace NB.Charts.Utils
{
    public static class Downsampling
    {
        /// <summary>
        /// Downsamples a given series of (time, value) values to fit within a specified size.
        /// This works according to the Largest-Triangle-Three-Buckets approach described by Sveinn Steinarsson
        /// in his thesis: https://skemman.is/handle/1946/15343
        /// </summary>
        /// <param name="data">The data series to downsample</param>
        /// <param name="threshold">The maximum number of points to allow in the output.</param>
        /// <param name="result">The downsampled data. The caller should ensure this contains at least threshold elements.</param>
        /// <returns>The number of points written to result (may be less than threshold)</returns>
        public static int LTTB(ref ReadOnlySpan<Vector2> data, uint threshold, ref List<Vector2> result)
        {
            if (threshold <= 0 || threshold >= data.Length)
            {
                for (int i = 0; i < MathF.Min(data.Length, threshold); i++)
                    result[i] = data[i];
                return Mathf.Min(data.Length, (int)threshold);
            }

            // bucket size
            float bucket_size = (float)(data.Length - 2) / (threshold - 2);

            int a = 0;
            Vector2 maxAreaPoint = new Vector2();
            int nextA = a;

            result[0] = data[a]; // always include first point

            for (int i = 0; i < threshold - 2; i++)
            {
                // calc point average for next bucket (containing c)
                float avgX = 0;
                float avgY = 0;
                int avgRangeStart = Mathf.FloorToInt((i + 1) * bucket_size + 1);
                int avgRangeEnd   = Mathf.FloorToInt((i + 2) * bucket_size + 1);
                avgRangeEnd = avgRangeEnd < data.Length ? avgRangeEnd : data.Length;

                int avgRangeLen = avgRangeEnd - avgRangeStart;

                for (; avgRangeStart < avgRangeEnd; avgRangeStart++)
                {
                    avgX += data[avgRangeStart].x;
                    avgY += data[avgRangeStart].y;
                }
                avgX /= avgRangeLen;
                avgY /= avgRangeLen;

                int rangeOffset = Mathf.FloorToInt(i * bucket_size + 1);
                int rangeTo = Mathf.FloorToInt((i + 1) * bucket_size + 1);

                // point a
                float pointAx = data[a].x;
                float pointAy = data[a].y;

                float maxArea = -1;

                for (; rangeOffset < rangeTo; rangeOffset++)
                {
                    float area = Mathf.Abs(
                        (pointAx - avgX) * (data[rangeOffset].y - pointAy) -
                        (pointAx - data[rangeOffset].x) * (avgY - pointAy)
                    ) * 0.5f;

                    if (area > maxArea)
                    {
                        maxArea = area;
                        maxAreaPoint = data[rangeOffset];
                        nextA = rangeOffset; // next 'a' is this 'b'
                    }
                }

                result[i] = maxAreaPoint;
                a = nextA;
            }

            result[(int)(threshold - 2)] = data[data.Length - 1]; // always include last point
            return (int)(threshold-1);
        }
    }
}
