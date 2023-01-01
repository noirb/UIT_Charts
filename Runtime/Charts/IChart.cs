using System.Collections.Generic;
using UnityEngine;

namespace NB.Charts
{
    public enum Palettes
    {
        C_Pastels,
        C_Solarized,
        C_Tableau10,
        S_Blue2Yellow,
        D_Blue2Red,
        D_Teal2Pink,
        Random
    }

    public enum GridDirection
    {
        Horizontal,
        Vertical
    }

    public interface IChart
    {
        void SetData(IList<Vector2> data, string series = "");
        void SetVisibility(string series, bool visible);
        void ToggleVisibility(string series);
        void RemoveDataSeries(string series);

        void SetColor(Color color, string series = "");

        void AddGridLine(GridDirection direction, float pos);
        void ClearGridLines();
        void SetDataRangeX(float lower, float upper);
        void SetDataRangeY(float lower, float upper);

        string AxisLabelBottom { get; set; }
        string AxisLabelLeft { get; set; }
        string AxisLabelRight { get; set; }
        string AxisLabelTop { get; set; }
        Palettes ColorPalette { get; set; }
        Color MajorGridColor { get; set; }
        float MajorGridLineWidth { get; set; }
        bool ShowMajorGrid { get; set; }
        bool ShowMajorGridLabelsHorizontal { get; set; }
        bool ShowMajorGridLabelsVertical { get; set; }
        bool ShowLegend { get; set; }
    }
}
