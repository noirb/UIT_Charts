using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NB.Charts
{
    /// <summary>
    /// A basic pie chart element.
    /// </summary>
    public class PieChart : ChartBase<(string, float)>
    {
        /// <summary>
        /// USS class automatically applied to all elements of this type
        /// </summary>
        public static new readonly string elementUssClassName = "nb-chart-piechart";

        #region Element Boilerplate
        public new class UxmlFactory : UxmlFactory<PieChart, UxmlTraits> { }

        public new class UxmlTraits : ChartBase<(string,float)>.UxmlTraits
        {
            UxmlColorAttributeDescription m_borderColor = new UxmlColorAttributeDescription { name = "border-color", defaultValue = Color.gray };
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var pie = ve as PieChart;
                pie.BorderColor = m_borderColor.GetValueFromBag(bag, cc);
            }
        }
        #endregion


        Color borderColor = Color.gray;
        /// <summary>
        /// Color of the border between pie slices
        /// </summary>
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                if (borderColor != value)
                {
                    borderColor = value;
                    MarkDirty();
                }
            }
        }

        Vector2 pointerPos = new Vector2(-100, -100);
        public PieChart()
        {
            AddToClassList(elementUssClassName);

            content.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            content.RegisterCallback<PointerLeaveEvent>((evt) => HideTooltip());

            if (!Application.isPlaying)
            {
                SetData(("A", 10), "A");
                SetData(("A", 30), "B");
                SetData(("A", 40), "C");
                SetData(("A", 20), "D");
                SetData(("A", 10), "E");
            }
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            pointerPos = evt.localPosition;
            MarkDirty(); // it might not be necessary to do this EVERY time...
        }

        protected override void DrawChart(Painter2D p, MeshGenerationContext mgc, Vector2 dataRangeX, Vector2 dataRangeY, Vector2 eleRangeX, Vector2 eleRangeY)
        {
            float total = 0;
            Dictionary<string, float> contributions = new Dictionary<string, float>();
            foreach (var data in series)
            {
                if (seriesVisibility[data.Key] == false)
                    continue;

                total += data.Value.Item2;
                contributions[data.Key] = data.Value.Item2;
            }

            float radius = Mathf.Min(Mathf.Abs((eleRangeX.y - eleRangeX.x) / 2f), Mathf.Abs((eleRangeY.y - eleRangeY.x) / 2f));
            Vector2 center = new Vector2(Mathf.Abs((eleRangeX.y - eleRangeX.x)), Mathf.Abs((eleRangeY.y - eleRangeY.x))) / 2f;
            float start_ang = 0f;
            foreach (var c in contributions.Keys)
            {
                float pct = contributions[c] / total;
                float end_ang = start_ang + 360 * pct;

                // check if last pointer position overlaps this wedge
                var dir = center - pointerPos;
                var pang = Vector2.SignedAngle(-Vector2.right, dir);
                if (pang < 0)
                    pang = 360f + pang;
                bool hovered = dir.magnitude < radius && start_ang <= pang && pang <= end_ang;

                p.strokeColor = borderColor;
                p.fillColor = hovered ? seriesColors[c] * 1.1f : seriesColors[c];
                p.BeginPath();
                if (Mathf.Approximately(pct, 1f)) // handle weird mesh generation cases
                    p.MoveTo(center + new Vector2(radius, 0));
                else
                    p.MoveTo(center);
                p.Arc(center, radius, new Angle(start_ang, AngleUnit.Degree), new Angle(end_ang, AngleUnit.Degree));

                if (!Mathf.Approximately(pct, 1f)) // complete wedge shape unless we're actually drawing a circle
                    p.LineTo(center);

                p.ClosePath();
                p.Fill();
                p.Stroke();

                start_ang = end_ang;

                if (hovered)
                {
                    // Can't mesh with VisualElements from within this fn, so schedule tooltip changes for later.
                    // This introduces a slight delay, but it's probably fine.
                    schedule.Execute(() => ShowTooltip(pointerPos - new Vector2(0, 16), series[c].ToString()));
                }
            }

            // hide tooltip if pointer has left chart
            if ((center - pointerPos).magnitude >= radius)
            {
                schedule.Execute(() => HideTooltip());
            }
        }

        protected override void GenerateVisualContent(MeshGenerationContext mgc)
        {
            if (series.Count < 1)
                return;

            var eleDim = new Vector2(content.resolvedStyle.width - content.resolvedStyle.paddingRight,
                         content.resolvedStyle.height - content.resolvedStyle.paddingBottom);

            Vector2 dataRangeX = Vector2.zero;
            Vector2 dataRangeY = Vector2.zero;
            Vector2 eleRangeX = new Vector2(content.resolvedStyle.paddingLeft, eleDim.x);
            Vector2 eleRangeY = new Vector2(eleDim.y, content.resolvedStyle.paddingTop); // flipped due to flipped coordinate system in UI

            DrawChart(mgc.painter2D, mgc, dataRangeX, dataRangeY, eleRangeX, eleRangeY);
        }
    }
}
