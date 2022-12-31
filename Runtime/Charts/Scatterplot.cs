using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NB.Charts
{
    public class Scatterplot : ChartBase
    {
        public static new readonly string elementUssClassName = "nb-chart-scatterplot";

        #region Element Boilerplate
        public new class UxmlFactory : UxmlFactory<Scatterplot, UxmlTraits> { }

        public new class UxmlTraits : ChartBase.UxmlTraits
        {
            UxmlFloatAttributeDescription m_defaultDataWidth = new UxmlFloatAttributeDescription { name = "default-data-width", defaultValue = 5f };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ve.AddToClassList(elementUssClassName);

                var sp = ve as Scatterplot;
                sp.DefaultDataWidth = m_defaultDataWidth.GetValueFromBag(bag, cc);
            }
        }
        #endregion

        float defaultDataWidth;
        public float DefaultDataWidth
        {
            get => defaultDataWidth;
            set
            {
                if (defaultDataWidth != value)
                {
                    defaultDataWidth = value;
                    MarkDirtyRepaint();
                }
            }
        }

        public enum MarkerType
        {
            Square,
            Circle
        }

        void DrawSquare(Painter2D p, Vector2 position, float width)
        {
            p.BeginPath();
            p.MoveTo(position + new Vector2(-width, -width));
            p.LineTo(position + new Vector2(width, -width));
            p.LineTo(position + new Vector2(width, width));
            p.LineTo(position + new Vector2(-width, width));
            p.ClosePath();
            p.Stroke();
        }

        void DrawCircle(Painter2D p, Vector2 position, float radius)
        {
            p.BeginPath();
            p.MoveTo(position + new Vector2(-radius, -radius));
            p.Arc(position, radius, new Angle(0, AngleUnit.Degree), new Angle(360, AngleUnit.Degree));
            p.ClosePath();
            p.Stroke();
        }

        Dictionary<string, float> seriesWidths = new Dictionary<string, float>();
        Dictionary<string, MarkerType> seriesMarkers = new Dictionary<string, MarkerType>();

        public override void RemoveDataSeries(string series)
        {
            base.RemoveDataSeries(series);

            seriesWidths.Remove(series);
            seriesMarkers.Remove(series);
        }

        protected override void DrawChart(Painter2D p, MeshGenerationContext mgc, Vector2 dataRangeX, Vector2 dataRangeY, Vector2 eleRangeX, Vector2 eleRangeY)
        {
            foreach (var data in series)
            {
                if (seriesVisibility[data.Key] != true)
                    continue;

                if (seriesColors.ContainsKey(data.Key))
                    p.fillColor = seriesColors[data.Key];
                p.strokeColor = p.fillColor;

                float width = defaultDataWidth;
                if (seriesWidths.ContainsKey(data.Key))
                    width = seriesWidths[data.Key];

                MarkerType marker = MarkerType.Square;
                if (seriesMarkers.ContainsKey(data.Key))
                    marker = seriesMarkers[data.Key];

                for (int i = 0; i < data.Value.Count; i++)
                {
                    var pt = new Vector2(MapRange(data.Value[i].x, dataRangeX, eleRangeX), MapRange(data.Value[i].y, dataRangeY, eleRangeY));

                    switch (marker)
                    {
                        case MarkerType.Square:
                            DrawSquare(p, pt, width);
                            break;
                        case MarkerType.Circle:
                            DrawCircle(p, pt, width);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void SetMarkerWidth(float width, string series = "")
        {
            seriesWidths[series] = width;
            content.MarkDirtyRepaint();
        }

        public void SetMarkerType(MarkerType type, string series = "")
        {
            seriesMarkers[series] = type;
            content.MarkDirtyRepaint();
        }
    }
}
