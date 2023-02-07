using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Pool;

namespace NB.Charts
{
    public class LineChart : TimeSeriesChartBase
    {
        public static new readonly string elementUssClassName = "nb-chart-lineplot";
        public static readonly string markerUssClassName = "nb-chart-lineplot__marker";

        #region Element Boilerplate
        public new class UxmlFactory : UxmlFactory<LineChart, UxmlTraits> { }

        public new class UxmlTraits : TimeSeriesChartBase.UxmlTraits
        {
            UxmlFloatAttributeDescription m_defaultDataWidth = new UxmlFloatAttributeDescription { name = "default-data-width", defaultValue = 5f };
            UxmlBoolAttributeDescription m_showMarkers = new UxmlBoolAttributeDescription { name = "show-markers", defaultValue = false };
            UxmlStringAttributeDescription m_markerClass = new UxmlStringAttributeDescription { name = "marker-class" };
            UxmlBoolAttributeDescription m_smearMarkerUpdates = new UxmlBoolAttributeDescription { name = "smear-marker-updates", defaultValue = false };
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ve.AddToClassList(elementUssClassName);

                var sp = ve as LineChart;
                sp.DefaultDataWidth = m_defaultDataWidth.GetValueFromBag(bag, cc);
                sp.ShowMarkers = m_showMarkers.GetValueFromBag(bag, cc);
                sp.MarkerClass = m_markerClass.GetValueFromBag(bag, cc);
                sp.SmearMarkerUpdates = m_smearMarkerUpdates.GetValueFromBag(bag, cc);
            }
        }
        #endregion

        public LineChart()
        {
            if (!Application.isPlaying)
            {
                SetData(new List<Vector2> {
                    new Vector2(1, 1),
                    new Vector2(2, 2),
                    new Vector2(3, 1),
                    new Vector2(4, 3),
                    new Vector2(5, 2),
                }, "A");
                SetData(new List<Vector2> {
                    new Vector2(1, 3),
                    new Vector2(2, 1.1f),
                    new Vector2(3, 1.5f),
                    new Vector2(4, 0.5f),
                    new Vector2(5, -0.5f),
                }, "B");
            }
        }

        List<VisualElement> activeMarkers = new List<VisualElement>();
        ObjectPool<VisualElement> markerPool = new ObjectPool<VisualElement>(() =>
            {
                var marker = new VisualElement();

                marker.usageHints = UsageHints.DynamicTransform;
                marker.AddToClassList(markerUssClassName);

                return marker;
            },
            (marker) =>
            {
            },
            (marker) =>
            {
                marker.style.opacity = 0f;
                marker.style.translate = new StyleTranslate(new Translate(new Length(-64, LengthUnit.Pixel), new Length(-64, LengthUnit.Pixel)));
            },
            (marker) =>
            {
                marker.RemoveFromHierarchy();
            }
        );

        float defaultDataWidth = 2;
        /// <summary>
        /// Default width of line traces
        /// </summary>
        public float DefaultDataWidth
        {
            get => defaultDataWidth;
            set
            {
                if (defaultDataWidth != value)
                {
                    defaultDataWidth = value;
                    MarkDirty();
                }
            }
        }

        bool showMarkers = false;
        /// <summary>
        /// If true, will display additional marker glyphs
        /// on each datapoint.
        /// </summary>
        public bool ShowMarkers
        {
            get => showMarkers;
            set
            {
                if (showMarkers != value)
                {
                    showMarkers = value;
                    MarkDirty();
                }
            }
        }

        string markerClass = null;
        /// <summary>
        /// Custom USS class to assign to all marker glyphs.
        /// </summary>
        public string MarkerClass
        {
            get => markerClass;
            set
            {
                if (markerClass != value)
                {
                    markerClass = value;
                    for (int i = 0; i < activeMarkers.Count; i++)
                    {
                        markerPool.Release(activeMarkers[i]);
                    }
                    activeMarkers.Clear();
                    markerPool.Clear();
                    MarkDirty();
                }
            }
        }

        bool smearMarkerUpdates = false;
        /// <summary>
        /// Because updating large sets of marker glyphs can be expensive,
        /// setting this to true will spread glyph updates across multiple
        /// frames. This can yield a smoother experience, but comes with
        /// the cost of some visual artifacts during frequent updates.
        /// </summary>
        public bool SmearMarkerUpdates
        {
            get => smearMarkerUpdates;
            set => smearMarkerUpdates = value;
        }


        Dictionary<string, float> seriesWidths = new Dictionary<string, float>();

        public override void RemoveDataSeries(string series)
        {
            base.RemoveDataSeries(series);

            seriesWidths.Remove(series);
        }

        void DrawMarkers()
        {
            var dataMin = new Vector2(float.MaxValue, float.MaxValue);
            var dataMax = new Vector2(float.MinValue, float.MinValue);
            foreach (var (name, data) in series)
            {
                if (seriesVisibility[name] != true)
                    continue;

                for (int i = 0; i < data.Count; i++)
                {
                    if (data[i].x < dataMin.x)
                        dataMin = new Vector2(data[i].x, dataMin.y);
                    if (data[i].y < dataMin.y)
                        dataMin = new Vector2(dataMin.x, data[i].y);

                    if (data[i].x > dataMax.x)
                        dataMax = new Vector2(data[i].x, dataMax.y);
                    if (data[i].y > dataMax.y)
                        dataMax = new Vector2(dataMax.x, data[i].y);
                }
            }

            Vector2 dataRangeX = this.dataRangeX == Vector2.zero ? new Vector2(dataMin.x, dataMax.x) : this.dataRangeX;
            Vector2 dataRangeY = this.dataRangeY == Vector2.zero ? new Vector2(dataMin.y, dataMax.y) : this.dataRangeY;

            Vector2 eleRangeX = new Vector2(content.resolvedStyle.paddingLeft, content.resolvedStyle.width);
            Vector2 eleRangeY = new Vector2(content.resolvedStyle.height, content.resolvedStyle.paddingTop); // flipped due to flipped coordinate system in UI

            int seenThisFrame = 0;
            int toDoThisFrame = smearMarkerUpdates ? 64 : int.MaxValue;

            foreach (var data in series)
            {
                Vector2 prev_point = Vector2.zero;
                for (int i = 0; i < data.Value.Count; i++)
                {
                    if (data.Value[i].x < dataRangeX.x || data.Value[i].x > dataRangeX.y)
                        continue;
                    var pt = new Vector2(MapRange(data.Value[i].x, dataRangeX, eleRangeX), MapRange(data.Value[i].y, dataRangeY, eleRangeY));
                    if (i > 0 && Vector2.Distance(pt, prev_point) <= 16)
                        continue;
                    prev_point = pt;
                    seenThisFrame++;
                    if (seenThisFrame < markerPool.CountActive)
                    {
                        continue;
                    }

                    var marker = markerPool.Get();
                    marker.style.translate = new StyleTranslate(new Translate(new Length(pt.x, LengthUnit.Pixel), new Length(pt.y, LengthUnit.Pixel)));
                    marker.style.backgroundColor = seriesColors[data.Key] * 0.8f;
                    marker.userData = data.Value[i];

                    if (showMarkers && seriesVisibility[data.Key])
                        marker.style.opacity = 1f;
                    else
                        marker.style.opacity = 0f;

                    if (marker.parent == null)
                    {
                        if (!string.IsNullOrWhiteSpace(markerClass))
                            marker.AddToClassList(markerClass);

                        content.Add(marker);
                        var mySeries = data.Key;
                        marker.RegisterCallback<PointerEnterEvent>((evt) =>
                        {
                            ShowTooltip(content.WorldToLocal(evt.position) - new Vector2(0, 16), marker.userData.ToString());

                            marker.style.opacity = 1;
                        });
                        marker.RegisterCallback<PointerLeaveEvent>((evt) =>
                        {
                            if (!showMarkers)
                                marker.style.opacity = 0;
                            marker.Clear();
                            HideTooltip();
                        });
                    }
                    marker.pickingMode = seriesVisibility[data.Key] ? PickingMode.Position : PickingMode.Ignore;
                    activeMarkers.Add(marker);

                    toDoThisFrame--;
                    if (toDoThisFrame < 1)
                    {
                        schedule.Execute(() => DrawMarkers());
                        return;
                    }
                }
            }
        }

        protected override void DrawChart(Painter2D p, MeshGenerationContext mgc, Vector2 dataRangeX, Vector2 dataRangeY, Vector2 eleRangeX, Vector2 eleRangeY)
        {
            foreach (var data in series)
            {
                if (seriesVisibility[data.Key] != true)
                    continue;
                if (data.Value.Count < 1)
                    continue;

                if (seriesColors.ContainsKey(data.Key))
                    p.strokeColor = seriesColors[data.Key];

                if (seriesWidths.ContainsKey(data.Key))
                    p.lineWidth = seriesWidths[data.Key];
                else
                    p.lineWidth = defaultDataWidth;

                // start from the first point within the selected dataRange
                int start_idx = 0;
                for (int i = 0; i < data.Value.Count; i++)
                {
                    if (data.Value[i].x < dataRangeX.x)
                        start_idx = i;
                    else
                        break;
                }

                p.lineJoin = LineJoin.Miter;
                p.lineCap = LineCap.Round;
                p.BeginPath();
                p.MoveTo(new Vector2(MapRange(data.Value[start_idx].x, dataRangeX, eleRangeX), MapRange(data.Value[start_idx].y, dataRangeY, eleRangeY)));
                Vector2 prev = new Vector2(MapRange(data.Value[start_idx].x, dataRangeX, eleRangeX), MapRange(data.Value[start_idx].y, dataRangeY, eleRangeY));
                for (int i = start_idx; i < data.Value.Count; i++)
                {
                    var next = new Vector2(MapRange(data.Value[i].x, dataRangeX, eleRangeX), MapRange(data.Value[i].y, dataRangeY, eleRangeY));
                    if (Vector2.Distance(next, prev) > 2) // only draw points which are at least 2 pixels apart from each other (needed to stay under vertex limit for large datasets)
                    {
                        p.LineTo(next);
                        prev = next;
                    }

                    // if we run past the selected data range, bail
                    if (data.Value[i].x > dataRangeX.y)
                        break;
                }
                p.Stroke();
            }
        }

        /// <summary>
        /// Sets the line width of an individual data series without affecting others.
        /// </summary>
        /// <param name="width">Width, in pixels</param>
        /// <param name="series">Name of the series to modify</param>
        public void SetLineWidth(float width, string series = "")
        {
            seriesWidths[series] = width;
            MarkDirty();
        }


        protected override void OnRefreshVisuals()
        {
            for (int i = 0; i < activeMarkers.Count; i++)
            {
                markerPool.Release(activeMarkers[i]);
            }
            activeMarkers.Clear();

            DrawMarkers();
            base.OnRefreshVisuals();
        }
    }
}
