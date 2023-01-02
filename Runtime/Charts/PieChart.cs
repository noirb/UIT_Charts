using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NB.Charts
{
    public class PieChart : ChartBase<(string, float)>
    {
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
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                if (borderColor != value)
                {
                    borderColor = value;
                    content.MarkDirtyRepaint();
                }
            }
        }

        public PieChart()
        {
            AddToClassList(elementUssClassName);

            if (!Application.isPlaying)
            {
                SetData(("A", 10), "A");
                SetData(("A", 30), "B");
                SetData(("A", 40), "C");
                SetData(("A", 20), "D");
                SetData(("A", 10), "E");
            }
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
                float angle = 360 * pct;

                p.strokeColor = borderColor;
                p.fillColor = seriesColors[c];
                p.BeginPath();
                p.MoveTo(center);
                p.Arc(center, radius, new Angle(start_ang, AngleUnit.Degree), new Angle(start_ang + angle, AngleUnit.Degree));

                if (!Mathf.Approximately(pct, 1f))
                    p.LineTo(center);

                p.ClosePath();
                p.Fill();
                p.Stroke();

                start_ang = start_ang + angle;
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
