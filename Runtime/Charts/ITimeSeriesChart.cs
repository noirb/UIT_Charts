using System.Collections.Generic;
using UnityEngine;

namespace NB.Charts
{
    /// <summary>
    /// Base interface for time series charts, e.g. line and scatter plots.
    /// </summary>
    public interface ITimeSeriesChart : IChart<IList<Vector2>>
    {
        /// <summary>
        /// Adds a major gridline to the chart. This allows you to specify your own gridlines based on
        /// your data. Gridlines added in this way are displayed independently of the major grid.
        /// </summary>
        /// <param name="direction">Whether the new gridline should be horizontal or vertical</param>
        /// <param name="pos">The data value (on the axis corresponding to direction) to render the gridline at</param>
        void AddGridLine(GridDirection direction, float pos);

        /// <summary>
        /// Clears all previously-added gridlines.
        /// </summary>
        void ClearGridLines();

        /// <summary>
        /// Sets the visible data range on the X-axis, effectively "zooming" on a specific region.
        /// If set to 0,0, will auto-scale to fit data.
        /// </summary>
        void SetDataRangeX(float lower, float upper);

        /// <summary>
        /// Sets the visible data range on the Y-axis, effectively "zooming" on a specific region.
        /// If set to 0,0, will auto-scale to fit data.
        /// </summary>
        void SetDataRangeY(float lower, float upper);

        /// <summary>
        /// If non-zero, will extend the visible x range on both sides by this amount.
        /// </summary>
        float DataBufferX { get; set; }

        /// <summary>
        /// If non-zero, will extend the visible y-range on both sides by this amount.
        /// </summary>
        float DataBufferY { get; set; }

        /// <summary>
        /// Color to use for major gridlines.
        /// </summary>
        Color MajorGridColor { get; set; }

        /// <summary>
        /// Width, in pixels, of major gridlines.
        /// </summary>
        float MajorGridLineWidth { get; set; }

        /// <summary>
        /// Whether the major gridlines should be displayed.
        /// </summary>
        bool ShowMajorGrid { get; set; }

        /// <summary>
        /// If ShowMajorGrid is true, this controls whether the horizontal gridlines should be shown.
        /// </summary>
        bool ShowMajorGridLabelsHorizontal { get; set; }

        /// <summary>
        /// If ShowMajorGrid is true, this controls whether the vertical gridlines should be shown.
        /// </summary>
        bool ShowMajorGridLabelsVertical { get; set; }
    }
}
