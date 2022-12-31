using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NB.Charts
{
    public abstract class ChartBase : VisualElement, IChart
    {
        #region USS
        public static readonly string elementUssClassName = "nb-chart";

        public static readonly string axisLabelUssClassName = "nb-chart__axis-label";
        public static readonly string axisLabelBottomUssClassName = "nb-chart__axis-label__bottom";
        public static readonly string axisLabelLeftUssClassName = "nb-chart__axis-label__left";
        public static readonly string axisLabelRightUssClassName = "nb-chart__axis-label__right";
        public static readonly string axisLabelTopUssClassName = "nb-chart__axis-label__top";
        #endregion

        #region Element boilerplate
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_axisLabelTop = new UxmlStringAttributeDescription { name = "axis-label-top" };
            UxmlStringAttributeDescription m_axisLabelBottom = new UxmlStringAttributeDescription { name = "axis-label-bottom" };
            UxmlStringAttributeDescription m_axisLabelLeft = new UxmlStringAttributeDescription { name = "axis-label-left" };
            UxmlStringAttributeDescription m_axisLabelRight = new UxmlStringAttributeDescription { name = "axis-label-right" };

            UxmlEnumAttributeDescription<Palettes> m_colorPalette = new UxmlEnumAttributeDescription<Palettes> { name = "color-palette", defaultValue = Palettes.C_Pastels };

            UxmlBoolAttributeDescription m_showLegend = new UxmlBoolAttributeDescription { name = "show-legend", defaultValue = true };

            UxmlColorAttributeDescription m_majorGridColor = new UxmlColorAttributeDescription { name = "major-grid-color", defaultValue = new Color32(88, 110, 117, 255) };
            UxmlFloatAttributeDescription m_majorGridLineWidth = new UxmlFloatAttributeDescription { name = "major-grid-line-width", defaultValue = 2f };
            UxmlBoolAttributeDescription m_showMajorGrid = new UxmlBoolAttributeDescription { name = "show-major-grid", defaultValue = true };
            UxmlBoolAttributeDescription m_majorGridAutoSpacing = new UxmlBoolAttributeDescription { name = "major-grid-auto-spacing", defaultValue = true };
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var cb = ve as ChartBase;
                cb.AxisLabelTop = m_axisLabelTop.GetValueFromBag(bag, cc);
                cb.AxisLabelBottom = m_axisLabelBottom.GetValueFromBag(bag, cc);
                cb.AxisLabelLeft = m_axisLabelLeft.GetValueFromBag(bag, cc);
                cb.AxisLabelRight = m_axisLabelRight.GetValueFromBag(bag, cc);

                cb.ColorPalette = m_colorPalette.GetValueFromBag(bag, cc);

                cb.ShowLegend = m_showLegend.GetValueFromBag(bag, cc);

                cb.MajorGridColor = m_majorGridColor.GetValueFromBag(bag, cc);
                cb.MajorGridLineWidth = m_majorGridLineWidth.GetValueFromBag(bag, cc);
                cb.ShowMajorGrid = m_showMajorGrid.GetValueFromBag(bag, cc);
                cb.MajorGridAutoSpacing = m_majorGridAutoSpacing.GetValueFromBag(bag, cc);
            }
        }
        #endregion

        protected VisualElement content;
        Legend legend;
        Label axisLabelTop;
        Label axisLabelBottom;
        Label axisLabelLeft;
        Label axisLabelRight;

        public ChartBase()
        {
            AddToClassList(elementUssClassName);

            axisLabelTop = new Label();
            axisLabelTop.AddToClassList(axisLabelUssClassName);
            axisLabelTop.AddToClassList(axisLabelTopUssClassName);
            Add(axisLabelTop);

            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexShrink = 0;
            container.style.flexGrow = 1;
            axisLabelLeft = new Label();
            axisLabelLeft.AddToClassList(axisLabelUssClassName);
            axisLabelLeft.AddToClassList(axisLabelLeftUssClassName);
            container.Add(axisLabelLeft);

            content = new VisualElement();
            content.style.flexGrow = 1;
            content.style.flexShrink = 0;
            container.Add(content);

            axisLabelRight = new Label();
            axisLabelRight.AddToClassList(axisLabelUssClassName);
            axisLabelRight.AddToClassList(axisLabelRightUssClassName);
            container.Add(axisLabelRight);
            Add(container);

            axisLabelBottom = new Label();
            axisLabelBottom.AddToClassList(axisLabelUssClassName);
            axisLabelBottom.AddToClassList(axisLabelBottomUssClassName);
            Add(axisLabelBottom);

            legend = new Legend();
            legend.OnToggleSeries += (name) =>
            {
                ToggleVisibility(name);
                content.MarkDirtyRepaint();
            };
            Add(legend);

            content.generateVisualContent += GenerateVisualContent;
        }

        public string AxisLabelBottom
        {
            get => axisLabelBottom.text;
            set => axisLabelBottom.text = value;
        }

        public string AxisLabelLeft
        {
            get => axisLabelLeft.text;
            set => axisLabelLeft.text = value;
        }

        public string AxisLabelRight
        {
            get => axisLabelRight.text;
            set => axisLabelRight.text = value;
        }

        public string AxisLabelTop
        {
            get => axisLabelTop.text;
            set => axisLabelTop.text = value;
        }

        protected Palettes colorPalette = Palettes.C_Tableau10;
        public Palettes ColorPalette
        {
            get => colorPalette;
            set
            {
                if (colorPalette != value)
                {
                    colorPalette = value;
                    content.MarkDirtyRepaint();
                }
            }
        }

        protected Color majorGridColor = new Color32(88, 110, 117, 255);
        public Color MajorGridColor
        {
            get => majorGridColor;
            set
            {
                if (majorGridColor != value)
                {
                    majorGridColor = value;
                    content.MarkDirtyRepaint();
                }
            }
        }

        protected float majorGridLineWidth = 2;
        public float MajorGridLineWidth
        {
            get => majorGridLineWidth;
            set
            {
                if (majorGridLineWidth != value)
                {
                    majorGridLineWidth = value;
                    content.MarkDirtyRepaint();
                }
            }
        }

        protected bool showMajorGrid = true;
        public bool ShowMajorGrid
        {
            get => showMajorGrid;
            set
            {
                if (showMajorGrid != value)
                {
                    showMajorGrid = value;
                    content.MarkDirtyRepaint();
                }
            }
        }

        protected bool showLegend = true;
        public bool ShowLegend
        {
            get => showLegend;
            set
            {
                showLegend = value;
                legend.style.display = showLegend ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        protected bool majorGridAutoSpacing = true;
        public bool MajorGridAutoSpacing
        {
            get => majorGridAutoSpacing;
            set
            {
                if (majorGridAutoSpacing != value)
                {
                    majorGridAutoSpacing = value;
                    content.MarkDirtyRepaint();
                }
            }
        }

        protected Dictionary<string, IList<Vector2>> series = new Dictionary<string, IList<Vector2>>();
        protected Dictionary<string, bool> seriesVisibility = new Dictionary<string, bool>();
        protected Dictionary<string, Color> seriesColors = new Dictionary<string, Color>();
        protected List<float> horizontalGridMarks = new List<float>();
        protected List<float> verticalGridMarks = new List<float>();
        protected float MapRange(float v, Vector2 fromRange, Vector2 toRange)
        {
            return (v - fromRange.x) * (toRange.y - toRange.x) / (fromRange.y - fromRange.x) + toRange.x;
        }

        public void SetColor(Color color, string series = "")
        {
            seriesColors[series] = color;
            content.MarkDirtyRepaint();
        }

        public void AddGridLine(GridDirection direction, float position)
        {
            switch (direction)
            {
                case GridDirection.Horizontal:
                    horizontalGridMarks.Add(position);
                    break;
                case GridDirection.Vertical:
                    verticalGridMarks.Add(position);
                    break;
                default:
                    throw new System.ArgumentException("Invalid direction specified");
            }

            content.MarkDirtyRepaint();
        }

        public void ClearGridLines()
        {
            horizontalGridMarks.Clear();
            verticalGridMarks.Clear();
            content.MarkDirtyRepaint();
        }

        Vector2 dataRangeX = Vector2.zero;
        public void SetDataRangeX(float lower, float upper)
        {
            dataRangeX = new Vector2(lower, upper);
            content.MarkDirtyRepaint();
        }

        Vector2 dataRangeY = Vector2.zero;
        public void SetDataRangeY(float lower, float upper)
        {
            dataRangeY = new Vector2(lower, upper);
            content.MarkDirtyRepaint();
        }

        public void SetVisibility(string series, bool visible)
        {
            seriesVisibility[series] = visible;
        }

        public void ToggleVisibility(string series)
        {
            seriesVisibility[series] = !seriesVisibility[series];
        }

        public void SetData(IList<Vector2> data, string series = "")
        {
            if (!seriesColors.ContainsKey(series))
            {
                seriesColors[series] = Utils.Colors.FromPalette(colorPalette, seriesColors.Count);
            }

            if (!this.series.ContainsKey(series))
            {
                legend.AddEntry(series, seriesColors[series]);
            }
            this.series[series] = data;
            seriesVisibility[series] = true;

            content.MarkDirtyRepaint();
        }

        public virtual void RemoveDataSeries(string series)
        {
            this.series.Remove(series);
            this.seriesColors.Remove(series);
            this.seriesVisibility.Remove(series);
        }

        void DrawMajorGrid(Painter2D p, MeshGenerationContext mgc, Vector2 dataRangeX, Vector2 dataRangeY, Vector2 eleRangeX, Vector2 eleRangeY)
        {
            p.strokeColor = majorGridColor;
            p.lineWidth = majorGridLineWidth;

            if (majorGridAutoSpacing)
            {
                for (float i = 0; i <= 5; i++)
                {
                    float y_val = Mathf.Lerp(dataRangeY.x, dataRangeY.y, i / 5f);
                    float y_tick = MapRange(y_val, dataRangeY, eleRangeY);
                    p.BeginPath();
                    p.MoveTo(new Vector2(0, y_tick));
                    p.LineTo(new Vector2(content.resolvedStyle.width, y_tick));
                    p.Stroke();
                    mgc.DrawText(y_val.ToString("N3"), new Vector2(8, y_tick - 10), 7, Color.white);

                    float x_val = Mathf.Lerp(dataRangeX.x, dataRangeX.y, i / 5f);
                    float x_tick = MapRange(x_val, dataRangeX, eleRangeX);
                    p.BeginPath();
                    p.MoveTo(new Vector2(x_tick, 0));
                    p.LineTo(new Vector2(x_tick, content.resolvedStyle.height));
                    p.Stroke();
                    mgc.DrawText(x_val.ToString("N3"), new Vector2(x_tick, content.resolvedStyle.height - 10), 7, Color.white);
                }
            }

            for (int i = 0; i < verticalGridMarks.Count; i++)
            {
                float tick = MapRange(verticalGridMarks[i], dataRangeX, eleRangeX);
                p.BeginPath();
                p.MoveTo(new Vector2(tick, 0));
                p.LineTo(new Vector2(tick, content.resolvedStyle.height));
                p.Stroke();
                mgc.DrawText(verticalGridMarks[i].ToString("N3"), new Vector2(tick, content.resolvedStyle.height - 10), 7, Color.white);
            }
            for (int i = 0; i < horizontalGridMarks.Count; i++)
            {
                float tick = MapRange(horizontalGridMarks[i], dataRangeY, eleRangeY);
                p.BeginPath();
                p.MoveTo(new Vector2(0, tick));
                p.LineTo(new Vector2(content.resolvedStyle.width, tick));
                p.Stroke();
                mgc.DrawText(horizontalGridMarks[i].ToString("N3"), new Vector2(8, tick - 10), 7, Color.white);
            }
        }

        private void GenerateVisualContent(MeshGenerationContext mgc)
        {
            if (series.Count < 1)
            {
                return;
            }

            var eleDim = new Vector2(content.resolvedStyle.width - content.resolvedStyle.paddingRight,
                                     content.resolvedStyle.height - content.resolvedStyle.paddingBottom);
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
            Vector2 eleRangeX = new Vector2(content.resolvedStyle.paddingLeft, eleDim.x);
            Vector2 eleRangeY = new Vector2(eleDim.y, content.resolvedStyle.paddingTop); // flipped due to flipped coordinate system in UI

            var p = mgc.painter2D;

            if (showMajorGrid)
            {
                DrawMajorGrid(p, mgc, dataRangeX, dataRangeY, eleRangeX, eleRangeY);
            }

            DrawChart(p, mgc, dataRangeX, dataRangeY, eleRangeX, eleRangeY);
        }

        protected abstract void DrawChart(Painter2D p, MeshGenerationContext mgc, Vector2 dataRangeX, Vector2 dataRangeY, Vector2 eleRangeX, Vector2 eleRangeY);
    }
}
