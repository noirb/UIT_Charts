using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace NB.Charts
{
    public class LineChart : ChartBase
    {
        public static new readonly string elementUssClassName = "nb-chart-lineplot";

        #region Element Boilerplate
        public new class UxmlFactory : UxmlFactory<LineChart, UxmlTraits> { }

        public new class UxmlTraits : ChartBase.UxmlTraits
        {
            UxmlFloatAttributeDescription m_defaultDataWidth = new UxmlFloatAttributeDescription { name = "default-data-width", defaultValue = 5f };
            UxmlBoolAttributeDescription m_showMarkers = new UxmlBoolAttributeDescription { name = "show-markers", defaultValue = false };
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ve.AddToClassList(elementUssClassName);

                var sp = ve as LineChart;
                sp.DefaultDataWidth = m_defaultDataWidth.GetValueFromBag(bag, cc);
                sp.ShowMarkers = m_showMarkers.GetValueFromBag(bag, cc);
            }
        }
        #endregion

        public LineChart()
        {
        }

        float defaultDataWidth = 2;
        public float DefaultDataWidth
        {
            get => defaultDataWidth;
            set
            {
                if (defaultDataWidth != value)
                {
                    defaultDataWidth = value;
                    content.MarkDirtyRepaint();
                }
            }
        }

        bool showMarkers = false;
        public bool ShowMarkers
        {
            get => showMarkers;
            set
            {
                if (showMarkers != value)
                {
                    showMarkers = value;
                    content.MarkDirtyRepaint();
                }
            }
        }

        Dictionary<string, float> seriesWidths = new Dictionary<string, float>();

        public override void RemoveDataSeries(string series)
        {
            base.RemoveDataSeries(series);

            seriesWidths.Remove(series);
        }

        protected override void DrawChart(Painter2D p, MeshGenerationContext mgc, Vector2 dataRangeX, Vector2 dataRangeY, Vector2 eleRangeX, Vector2 eleRangeY)
        {
            foreach (var data in series)
            {
                if (seriesVisibility[data.Key] != true)
                    continue;

                if (seriesColors.ContainsKey(data.Key))
                    p.strokeColor = seriesColors[data.Key];

                if (seriesWidths.ContainsKey(data.Key))
                    p.lineWidth = seriesWidths[data.Key];
                else
                    p.lineWidth = defaultDataWidth;

                p.lineJoin = LineJoin.Miter;
                p.lineCap = LineCap.Round;
                p.BeginPath();
                p.MoveTo(new Vector2(MapRange(data.Value[0].x, dataRangeX, eleRangeX), MapRange(data.Value[0].y, dataRangeY, eleRangeY)));

                for (int i = 0; i < data.Value.Count; i++)
                {
                    p.LineTo(new Vector2(MapRange(data.Value[i].x, dataRangeX, eleRangeX), MapRange(data.Value[i].y, dataRangeY, eleRangeY)));
                }
                p.Stroke();

                if (showMarkers)
                {
                    p.strokeColor = p.strokeColor * 0.8f;
                    for (int i = 0; i < data.Value.Count; i++)
                    {
                        if (data.Value[i].x < dataRangeX.x || data.Value[i].x > dataRangeX.y)
                            continue;

                        var pt = new Vector2(MapRange(data.Value[i].x, dataRangeX, eleRangeX), MapRange(data.Value[i].y, dataRangeY, eleRangeY));
                        p.BeginPath();
                        p.MoveTo(pt + new Vector2(2 * p.lineWidth, 2 * p.lineWidth));
                        p.LineTo(pt + new Vector2(-2 * p.lineWidth, 2 * p.lineWidth));
                        p.LineTo(pt + new Vector2(-2 * p.lineWidth, -2 * p.lineWidth));
                        p.LineTo(pt + new Vector2(2 * p.lineWidth, -2 * p.lineWidth));
                        p.ClosePath();
                        p.Stroke();
                    }
                }
            }
        }

        public void SetLineWidth(float width, string series = "")
        {
            seriesWidths[series] = width;
            content.MarkDirtyRepaint();
        }
    }
}
