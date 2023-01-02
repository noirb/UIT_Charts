using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NB.Charts
{
    public interface ITimeSeriesChart : IChart<IList<Vector2>>
    {
        void AddGridLine(GridDirection direction, float pos);
        void ClearGridLines();
        void SetDataRangeX(float lower, float upper);
        void SetDataRangeY(float lower, float upper);
        Color MajorGridColor { get; set; }
        float MajorGridLineWidth { get; set; }
        bool ShowMajorGrid { get; set; }
        bool ShowMajorGridLabelsHorizontal { get; set; }
        bool ShowMajorGridLabelsVertical { get; set; }
    }
}
