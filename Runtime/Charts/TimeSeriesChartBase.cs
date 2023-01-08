using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NB.Charts
{
    public abstract class TimeSeriesChartBase : ChartBase<IList<Vector2>>, ITimeSeriesChart
    {
        #region Element boilerplate
        public new class UxmlTraits : ChartBase<IList<Vector2>>.UxmlTraits
        {
            UxmlColorAttributeDescription m_majorGridColor = new UxmlColorAttributeDescription { name = "major-grid-color", defaultValue = new Color32(88, 110, 117, 255) };
            UxmlFloatAttributeDescription m_majorGridLineWidth = new UxmlFloatAttributeDescription { name = "major-grid-line-width", defaultValue = 2f };
            UxmlBoolAttributeDescription m_showMajorGrid = new UxmlBoolAttributeDescription { name = "show-major-grid", defaultValue = true };
            UxmlBoolAttributeDescription m_showMajorGridLabelsHorizontal = new UxmlBoolAttributeDescription { name = "show-major-grid-labels-horizontal", defaultValue = true };
            UxmlBoolAttributeDescription m_showMajorGridLabelsVertical = new UxmlBoolAttributeDescription { name = "show-major-grid-labels-vertical", defaultValue = true };
            UxmlFloatAttributeDescription m_majorGridLabelFontSize = new UxmlFloatAttributeDescription { name = "major-grid-label-font-size", defaultValue = 10 };
            UxmlBoolAttributeDescription m_majorGridAutoSpacingHorizontal = new UxmlBoolAttributeDescription { name = "major-grid-auto-spacing-horizontal", defaultValue = true };
            UxmlBoolAttributeDescription m_majorGridAutoSpacingVertical = new UxmlBoolAttributeDescription { name = "major-grid-auto-spacing-vertical", defaultValue = true };
            UxmlStringAttributeDescription m_gridLabelFormatString = new UxmlStringAttributeDescription { name = "grid-label-format-string", defaultValue = "N3" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var ts = ve as TimeSeriesChartBase;

                ts.MajorGridColor = m_majorGridColor.GetValueFromBag(bag, cc);
                ts.MajorGridLineWidth = m_majorGridLineWidth.GetValueFromBag(bag, cc);
                ts.ShowMajorGrid = m_showMajorGrid.GetValueFromBag(bag, cc);
                ts.ShowMajorGridLabelsHorizontal = m_showMajorGridLabelsHorizontal.GetValueFromBag(bag, cc);
                ts.ShowMajorGridLabelsVertical = m_showMajorGridLabelsVertical.GetValueFromBag(bag, cc);
                ts.MajorGridAutoSpacingHorizontal = m_majorGridAutoSpacingHorizontal.GetValueFromBag(bag, cc);
                ts.MajorGridAutoSpacingVertical = m_majorGridAutoSpacingVertical.GetValueFromBag(bag, cc);
                ts.MajorGridLabelFontSize = m_majorGridLabelFontSize.GetValueFromBag(bag, cc);
                ts.GridLabelFormatString = m_gridLabelFormatString.GetValueFromBag(bag, cc);
            }
        }
        #endregion

        protected Color majorGridColor = new Color32(88, 110, 117, 255);
        public Color MajorGridColor
        {
            get => majorGridColor;
            set
            {
                if (majorGridColor != value)
                {
                    majorGridColor = value;
                    MarkDirty();
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
                    MarkDirty();
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
                    MarkDirty();
                }
            }
        }

        protected bool showMajorGridLabelsHorizontal = true;
        public bool ShowMajorGridLabelsHorizontal
        {
            get => showMajorGridLabelsHorizontal;
            set
            {
                if (showMajorGridLabelsHorizontal != value)
                {
                    showMajorGridLabelsHorizontal = value;
                    MarkDirty();
                }
            }
        }

        protected bool showMajorGridLabelsVertical = true;
        public bool ShowMajorGridLabelsVertical
        {
            get => showMajorGridLabelsVertical;
            set
            {
                if (showMajorGridLabelsVertical != value)
                {
                    showMajorGridLabelsVertical = value;
                    MarkDirty();
                }
            }
        }

        protected float majorGridLabelFontSize = 10f;
        public float MajorGridLabelFontSize
        {
            get => majorGridLabelFontSize;
            set
            {
                if (majorGridLabelFontSize != value)
                {
                    majorGridLabelFontSize = value;
                    MarkDirty();
                }
            }
        }

        protected bool majorGridAutoSpacingHorizontal = true;
        public bool MajorGridAutoSpacingHorizontal
        {
            get => majorGridAutoSpacingHorizontal;
            set
            {
                if (majorGridAutoSpacingHorizontal != value)
                {
                    majorGridAutoSpacingHorizontal = value;
                    MarkDirty();
                }
            }
        }

        protected bool majorGridAutoSpacingVertical = true;
        public bool MajorGridAutoSpacingVertical
        {
            get => majorGridAutoSpacingVertical;
            set
            {
                if (majorGridAutoSpacingVertical != value)
                {
                    majorGridAutoSpacingVertical = value;
                    MarkDirty();
                }
            }
        }

        protected string gridLabelFormatString = "N3";
        public string GridLabelFormatString
        {
            get => gridLabelFormatString;
            set
            {
                if (gridLabelFormatString != value)
                {
                    gridLabelFormatString = value;
                    MarkDirty();
                }
            }
        }

        protected List<float> horizontalGridMarks = new List<float>();
        protected List<float> verticalGridMarks = new List<float>();

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

            MarkDirty();
        }

        public void ClearGridLines()
        {
            horizontalGridMarks.Clear();
            verticalGridMarks.Clear();
            MarkDirty();
        }

        protected Vector2 dataRangeX = Vector2.zero;
        public void SetDataRangeX(float lower, float upper)
        {
            dataRangeX = new Vector2(lower, upper);
            MarkDirty();
        }

        protected Vector2 dataRangeY = Vector2.zero;
        public void SetDataRangeY(float lower, float upper)
        {
            dataRangeY = new Vector2(lower, upper);
            MarkDirty();
        }

        void DrawMajorGrid(Painter2D p, MeshGenerationContext mgc, Vector2 dataRangeX, Vector2 dataRangeY, Vector2 eleRangeX, Vector2 eleRangeY)
        {
            p.strokeColor = majorGridColor;
            p.lineWidth = majorGridLineWidth;

            if (majorGridAutoSpacingHorizontal || majorGridAutoSpacingVertical)
            {
                for (float i = 0; i <= 5; i++)
                {
                    if (majorGridAutoSpacingVertical)
                    {
                        float y_val = Mathf.Lerp(dataRangeY.x, dataRangeY.y, i / 5f);
                        float y_tick = MapRange(y_val, dataRangeY, eleRangeY);
                        p.BeginPath();
                        p.MoveTo(new Vector2(0, y_tick));
                        p.LineTo(new Vector2(content.resolvedStyle.width, y_tick));
                        p.Stroke();

                        if (showMajorGridLabelsHorizontal)
                            mgc.DrawText(y_val.ToString(gridLabelFormatString), new Vector2(8, y_tick - 10), majorGridLabelFontSize, Color.white);
                    }

                    if (majorGridAutoSpacingHorizontal)
                    {
                        float x_val = Mathf.Lerp(dataRangeX.x, dataRangeX.y, i / 5f);
                        float x_tick = MapRange(x_val, dataRangeX, eleRangeX);
                        p.BeginPath();
                        p.MoveTo(new Vector2(x_tick, 0));
                        p.LineTo(new Vector2(x_tick, content.resolvedStyle.height));
                        p.Stroke();

                        if (showMajorGridLabelsVertical)
                            mgc.DrawText(x_val.ToString(gridLabelFormatString), new Vector2(x_tick, content.resolvedStyle.height + 5), majorGridLabelFontSize, Color.white);
                    }
                }
            }

            for (int i = 0; i < verticalGridMarks.Count; i++)
            {
                float tick = MapRange(verticalGridMarks[i], dataRangeX, eleRangeX);
                p.BeginPath();
                p.MoveTo(new Vector2(tick, 0));
                p.LineTo(new Vector2(tick, content.resolvedStyle.height));
                p.Stroke();

                if (showMajorGridLabelsVertical)
                    mgc.DrawText(verticalGridMarks[i].ToString(gridLabelFormatString), new Vector2(tick - 7, content.resolvedStyle.height + 5), majorGridLabelFontSize, Color.white);
            }
            for (int i = 0; i < horizontalGridMarks.Count; i++)
            {
                float tick = MapRange(horizontalGridMarks[i], dataRangeY, eleRangeY);
                p.BeginPath();
                p.MoveTo(new Vector2(0, tick));
                p.LineTo(new Vector2(content.resolvedStyle.width, tick));
                p.Stroke();

                if (showMajorGridLabelsHorizontal)
                    mgc.DrawText(horizontalGridMarks[i].ToString(gridLabelFormatString), new Vector2(8, tick - 10), majorGridLabelFontSize, Color.white);
            }
        }

        protected override void GenerateVisualContent(MeshGenerationContext mgc)
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
    }
}
