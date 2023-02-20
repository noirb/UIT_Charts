using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Pool;

namespace NB.Charts
{
    public class Scatterplot : TimeSeriesChartBase
    {
        /// <summary>
        /// USS class automatically applied to charts of this type.
        /// </summary>
        public static new readonly string elementUssClassName = "nb-chart-scatterplot";
        /// <summary>
        /// USS class automatically applied to markers in charts of this type.
        /// </summary>
        public static readonly string markerUssClassName = "nb-chart-scatterplot__marker";

        #region Element Boilerplate
        public new class UxmlFactory : UxmlFactory<Scatterplot, UxmlTraits> { }

        public new class UxmlTraits : TimeSeriesChartBase.UxmlTraits
        {
            UxmlStringAttributeDescription m_markerClass = new UxmlStringAttributeDescription { name = "marker-class" };
            UxmlBoolAttributeDescription m_smearMarkerUpdates = new UxmlBoolAttributeDescription { name = "smear-marker-updates", defaultValue = false };
            UxmlFloatAttributeDescription m_overlapDistance = new UxmlFloatAttributeDescription { name = "overlap-distance", defaultValue = 0.0f };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ve.AddToClassList(elementUssClassName);

                var sp = ve as Scatterplot;
                sp.MarkerClass = m_markerClass.GetValueFromBag(bag, cc);
                sp.SmearMarkerUpdates = m_smearMarkerUpdates.GetValueFromBag(bag, cc);
                sp.OverlapDistance = m_overlapDistance.GetValueFromBag(bag, cc);
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
                    MarkDirty();
                }
            }
        }

        public Scatterplot()
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
        ObjectPool<VisualElement> markerPool = new ObjectPool<VisualElement>(
            () =>
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

        Dictionary<string, string> seriesMarkers = new Dictionary<string, string>();

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

        float overlapDistance = 0;
        /// <summary>
        /// Markers from the same series which are less than OverlapDistance pixels
        /// apart will not be drawn. This prevents stacking of tons and tons of
        /// overlapping markers in cases where the distance between them is very small.
        /// For dense datasets, it may be a good idea to set this to a value close to
        /// the width of your markers.
        /// </summary>
        public float OverlapDistance
        {
            get => overlapDistance;
            set
            {
                overlapDistance = value;
            }
        }
        public override void RemoveDataSeries(string series)
        {
            base.RemoveDataSeries(series);

            seriesMarkers.Remove(series);
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
                    if (i > 0 && Vector2.Distance(pt, prev_point) <= overlapDistance)
                        continue;
                    prev_point = pt;
                    seenThisFrame++;
                    if (seenThisFrame < markerPool.CountActive)
                    {
                        continue;
                    }

                    var marker = markerPool.Get();
                    marker.style.translate = new StyleTranslate(new Translate(new Length(pt.x, LengthUnit.Pixel), new Length(pt.y, LengthUnit.Pixel)));
                    marker.style.backgroundColor = seriesColors[data.Key];
                    marker.userData = data.Value[i];

                    if (seriesVisibility[data.Key])
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
                            marker.Clear();
                            HideTooltip();
                        });
                    }

                    if (seriesMarkers.ContainsKey(data.Key))
                        marker.AddToClassList(seriesMarkers[data.Key]);

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
        }

        protected override void OnRefreshVisuals()
        {
            for (int i = 0; i < activeMarkers.Count; i++)
            {
                foreach (var mm in seriesMarkers)
                {
                    activeMarkers[i].RemoveFromClassList(mm.Value);
                }
                markerPool.Release(activeMarkers[i]);
            }
            activeMarkers.Clear();

            DrawMarkers();
            base.OnRefreshVisuals();
        }

        /// <summary>
        /// Sets a custom USS class for markers in a particular series
        /// </summary>
        /// <param name="className">USS Class to apply</param>
        /// <param name="series">Series to apply the class to</param>
        public void SetMarkerClass(string className, string series = "")
        {
            seriesMarkers[series] = className;
            MarkDirty();
        }
    }
}
