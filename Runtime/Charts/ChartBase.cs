using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NB.Charts
{
    public abstract class ChartBase<T> : VisualElement, IChart<T>
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

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var cb = ve as ChartBase<T>;
                cb.AxisLabelTop = m_axisLabelTop.GetValueFromBag(bag, cc);
                cb.AxisLabelBottom = m_axisLabelBottom.GetValueFromBag(bag, cc);
                cb.AxisLabelLeft = m_axisLabelLeft.GetValueFromBag(bag, cc);
                cb.AxisLabelRight = m_axisLabelRight.GetValueFromBag(bag, cc);

                cb.ColorPalette = m_colorPalette.GetValueFromBag(bag, cc);

                cb.ShowLegend = m_showLegend.GetValueFromBag(bag, cc);
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

                    int i = 0;
                    foreach (var data in series)
                    {
                        SetColor(Utils.Colors.FromPalette(colorPalette, i), data.Key);
                        i++;
                    }
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

        protected Dictionary<string, T> series = new Dictionary<string, T>();
        protected Dictionary<string, bool> seriesVisibility = new Dictionary<string, bool>();
        protected Dictionary<string, Color> seriesColors = new Dictionary<string, Color>();

        protected float MapRange(float v, Vector2 fromRange, Vector2 toRange)
        {
            return (v - fromRange.x) * (toRange.y - toRange.x) / (fromRange.y - fromRange.x) + toRange.x;
        }

        public void SetColor(Color color, string series = "")
        {
            seriesColors[series] = color;
            legend.SetColor(color, series);
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

        public void SetData(T data, string series = "")
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

            if (!seriesVisibility.ContainsKey(series))
                seriesVisibility[series] = true;

            content.MarkDirtyRepaint();
        }

        public virtual void RemoveDataSeries(string series)
        {
            this.series.Remove(series);
            this.seriesColors.Remove(series);
            this.seriesVisibility.Remove(series);
        }

        protected abstract void GenerateVisualContent(MeshGenerationContext mgc);

        protected abstract void DrawChart(Painter2D p, MeshGenerationContext mgc, Vector2 dataRangeX, Vector2 dataRangeY, Vector2 eleRangeX, Vector2 eleRangeY);
    }
}
