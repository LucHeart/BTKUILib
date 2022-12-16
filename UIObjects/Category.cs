﻿using System.Collections.Generic;
using ABI_RC.Core.InteractionSystem;
using BTKUILib.UIObjects.Components;
using cohtml;

namespace BTKUILib.UIObjects
{
    /// <summary>
    /// This act as category with header and row within Cohtml
    /// </summary>
    public class Category : QMUIElement
    {
        /// <summary>
        /// Category name, will update on the fly
        /// </summary>
        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                UpdateCategoryName();
            }
        }

        internal List<QMUIElement> CategoryElements = new();

        private string _categoryName;
        private Page _linkedPage;
        private bool _showHeader = false;

        internal Category(string categoryName, Page page, bool showHeader = true)
        {
            _categoryName = categoryName;
            _linkedPage = page;
            _showHeader = showHeader;
            
            ElementID = "btkUI-Row-" + UUID;
        }

        /// <summary>
        /// Creates a simple button
        /// </summary>
        /// <param name="buttonText">Text to be displayed on the button</param>
        /// <param name="buttonIcon">Icon for the button</param>
        /// <param name="buttonTooltip">Tooltip to be displayed when hovering on the button</param>
        /// <returns></returns>
        public Button AddButton(string buttonText, string buttonIcon, string buttonTooltip)
        {
            var button = new Button(buttonText, buttonIcon, buttonTooltip, this);
            CategoryElements.Add(button);
            
            if(UIUtils.IsQMReady())
                button.GenerateCohtml();

            return button;
        }

        /// <summary>
        /// Simple toggle element
        /// </summary>
        /// <param name="toggleText">Text to be displayed on toggle</param>
        /// <param name="toggleTooltip">Tooltip to be displayed when hovering on the toggle</param>
        /// <param name="state">Initial state of the toggle</param>
        /// <returns>Newly created toggle object</returns>
        public ToggleButton AddToggle(string toggleText, string toggleTooltip, bool state)
        {
            var toggle = new ToggleButton(toggleText, toggleTooltip, state, this);
            CategoryElements.Add(toggle);
            
            if(UIUtils.IsQMReady())
                toggle.GenerateCohtml();

            return toggle;
        }

        /// <summary>
        /// Create a new subpage as well as the button required to open it
        /// </summary>
        /// <param name="pageName">Name of the new page, this will appear at the top of the page</param>
        /// <param name="pageIcon">Icon to be used on the button</param>
        /// <param name="pageTooltip">Tooltip to be displayed when hovering on the button</param>
        /// <param name="modName">Mod name, this should be the same as your root page</param>
        /// <returns>Newly created page object</returns>
        public Page AddPage(string pageName, string pageIcon, string pageTooltip, string modName)
        {
            var page = new Page(modName, pageName);
            CategoryElements.Add(page);

            var pageButton = new Button($"Open {pageName}", pageIcon, pageTooltip, this);
            CategoryElements.Add(pageButton);
            pageButton.OnPress += () =>
            {
                page.OpenPage();
            };

            if (UIUtils.IsQMReady())
            {
                page.GenerateCohtml();
                pageButton.GenerateCohtml();
            }

            return page;
        }

        internal override void GenerateCohtml()
        {
            if(!IsGenerated)
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkCreateRow", _linkedPage.ElementID, UUID, _showHeader ? _categoryName : null);
            
            foreach(var element in CategoryElements)
                element.GenerateCohtml();

            IsGenerated = true;
        }
        
        private void UpdateCategoryName()
        {
            if (!BTKUILib.Instance.IsOnMainThread())
            {
                BTKUILib.Instance.MainThreadQueue.Enqueue(UpdateCategoryName);
                return;
            }
            
            if (!UIUtils.IsQMReady()) return;
            
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkUpdateText", $"btkUI-Row-HeaderText-{UUID}", _categoryName);
        }
    }
}