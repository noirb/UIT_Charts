using UnityEngine;

namespace NB.Charts
{
    /// <summary>
    /// Available built-int color palettes
    /// </summary>
    public enum Palettes
    {
        /// <summary>
        /// A palette designed for geometric data (e.g. positions, velocities, ..)
        /// based on the Solarized palette. Begins with red, green, and blue hues.
        /// </summary>
        C_Geometry,
        C_Pastels,
        /// <summary>
        /// A palette adapted from the solarized color palette.
        /// </summary>
        C_Solarized,
        C_Tableau10,
        S_Blue2Yellow,
        D_Blue2Red,
        D_Teal2Pink,
        /// <summary>
        /// Selects a random color each time a new value is requested.
        /// </summary>
        Random
    }

    public enum GridDirection
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// Base interface for all chart types
    /// </summary>
    /// <typeparam name="TraceData">The type used for data consumed by a chart</typeparam>
    public interface IChart<TraceData>
    {
        /// <summary>
        /// Set the data to display for a given series. Will create a new series if the given
        /// series label does not yet exist, otherwise will replace the data (if any) for the
        /// specified series.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="series">If specified, labels the series this data belongs to.</param>
        void SetData(TraceData data, string series = "");

        /// <summary>
        /// Sets whether a specific series should be visible or not.
        /// </summary>
        /// <param name="series"></param>
        /// <param name="visible"></param>
        void SetVisibility(string series, bool visible);

        /// <summary>
        /// Toggles the visibility of the given series.
        /// </summary>
        /// <param name="series"></param>
        void ToggleVisibility(string series);

        /// <summary>
        /// Removes and clears all data associated with the given series.
        /// </summary>
        /// <param name="series"></param>
        void RemoveDataSeries(string series);

        /// <summary>
        /// Sets the color used when displaying the given series.
        /// Overrides any palette options.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="series"></param>
        void SetColor(Color color, string series = "");

        /// <summary>
        /// Text to display below the chart.
        /// </summary>
        string AxisLabelBottom { get; set; }

        /// <summary>
        /// Text to display to the left of the chart.
        /// </summary>
        string AxisLabelLeft { get; set; }

        /// <summary>
        /// Text to display to the right of the chart.
        /// </summary>
        string AxisLabelRight { get; set; }

        /// <summary>
        /// Text to display above the chart.
        /// </summary>
        string AxisLabelTop { get; set; }

        /// <summary>
        /// Color palette to use.
        /// By selecting a built-in color palette, you do not need to explicitly set
        /// the color of each series in your chart.
        /// </summary>
        Palettes ColorPalette { get; set; }

        /// <summary>
        /// Whether or not to display a legend inside the chart.
        /// </summary>
        bool ShowLegend { get; set; }
    }
}
