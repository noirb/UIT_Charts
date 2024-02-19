using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NB.Charts
{
    public class Legend : VisualElement
    {
        public static readonly string legendUssClassName = "nb-chart__legend";
        public static readonly string activeUssClassName = "nb-chart__legend-active";

        public static readonly string legendEntriesUssClassName = "nb-chart__legend__entries";
        public static readonly string legendEntriesMinimizedClassName = "nb-chart__legend__entries-minimized";
        public static readonly string legendEntriesMinimizeButtonClassName = "nb-chart__legend__minimize-button";

        public static readonly string legendEntryUssClassName = "nb-chart__legend__entry";
        public static readonly string legendEntryDisabledUssClassName = "nb-chart__legend__entry-disabled";
        public static readonly string legendLabelUssClassName = "nb-chart__legend__label";
        public static readonly string legendColorUssClassName = "nb-chart__legend__color";

        #region Element boilerplate
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
            }
        }
        #endregion

        public Action<string> OnToggleSeries { get; set; }

        VisualElement entryContainer;
        Button minimizeButton;
        public Legend()
        {
            AddToClassList(legendUssClassName);

            entryContainer = new VisualElement();
            entryContainer.AddToClassList(legendEntriesUssClassName);
            Add(entryContainer);

            minimizeButton = new Button();
            minimizeButton.AddToClassList(legendEntriesMinimizeButtonClassName);
            minimizeButton.text = "▲";
            minimizeButton.RegisterCallback<ClickEvent>((evt) => {
                entryContainer.ToggleInClassList(legendEntriesMinimizedClassName);
                if (minimizeButton.text == "▲")
                    minimizeButton.text = "▼";
                else
                    minimizeButton.text = "▲";
                entryContainer.SetEnabled(!entryContainer.enabledSelf);
            });
            Add(minimizeButton);

            RegisterCallback<PointerEnterEvent>((evt) =>
            {
                AddToClassList(activeUssClassName);
            });

            RegisterCallback<PointerLeaveEvent>((evt) =>
            {
                RemoveFromClassList(activeUssClassName);
            });
        }

        Dictionary<string, VisualElement> entries = new Dictionary<string, VisualElement>();
        Dictionary<string, Color> entryColors = new Dictionary<string, Color>();

        public void AddEntry(string name, Color color)
        {
            VisualElement ele = new VisualElement();
            ele.AddToClassList(legendEntryUssClassName);

            VisualElement colorBlock = new VisualElement();
            colorBlock.name = "colorblock";
            colorBlock.AddToClassList(legendColorUssClassName);
            colorBlock.style.backgroundColor = color;
            ele.Add(colorBlock);

            Label label = new Label();
            label.AddToClassList(legendLabelUssClassName);
            label.text = name;
            ele.Add(label);

            ele.RegisterCallback<ClickEvent>((evt) =>
            {
                ToggleEnabled(name);

                OnToggleSeries?.Invoke(name);
            });

            entryContainer.Add(ele);

            entries[name] = ele;
            entryColors[name] = color;
        }

        public void RemoveEntry(string name)
        {
            if (!entries.ContainsKey(name))
                return;

            entries[name].RemoveFromHierarchy();
            entryColors.Remove(name);
            entries.Remove(name);
        }

        public void SetColor(Color color, string name)
        {
            if (!entries.ContainsKey(name))
                return;

            entryColors[name] = color;
            if (!entries[name].ClassListContains(legendEntryDisabledUssClassName))
                entries[name].Q("colorblock").style.backgroundColor = color;
        }

        public void SetEnabled(string name, bool enabled)
        {
            if (enabled)
            {
                entries[name].RemoveFromClassList(legendEntryDisabledUssClassName);
                entries[name].Q("colorblock").style.backgroundColor = entryColors[name];
            }
            else
            {
                entries[name].AddToClassList(legendEntryDisabledUssClassName);
                entries[name].Q("colorblock").style.backgroundColor = Color.gray;
            }
        }

        public void ToggleEnabled(string name)
        {
            Debug.LogFormat("Toggling {0}", name);
            entries[name].ToggleInClassList(legendEntryDisabledUssClassName);
            if (entries[name].ClassListContains(legendEntryDisabledUssClassName))
                entries[name].Q("colorblock").style.backgroundColor = Color.gray;
            else
                entries[name].Q("colorblock").style.backgroundColor = entryColors[name];
        }

        /// <summary>
        /// Collapses legend to its minimized state
        /// </summary>
        public void Collapse()
        {
            entryContainer.AddToClassList(legendEntriesMinimizedClassName);
            minimizeButton.text = "▼";
            entryContainer.SetEnabled(false);
        }

        /// <summary>
        /// Expands legend to its visible state
        /// </summary>
        public void Expand()
        {
            entryContainer.RemoveFromClassList(legendEntriesMinimizedClassName);
            minimizeButton.text = "▲";
            entryContainer.SetEnabled(true);
        }
    }
}
