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

    public interface IChart<TraceData>
    {
        void SetData(TraceData data, string series = "");
        void SetVisibility(string series, bool visible);
        void ToggleVisibility(string series);
        void RemoveDataSeries(string series);
        void SetColor(Color color, string series = "");

        string AxisLabelBottom { get; set; }
        string AxisLabelLeft { get; set; }
        string AxisLabelRight { get; set; }
        string AxisLabelTop { get; set; }
        Palettes ColorPalette { get; set; }
        bool ShowLegend { get; set; }
    }
}
