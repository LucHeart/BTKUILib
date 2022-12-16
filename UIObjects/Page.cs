﻿using System.Collections.Generic;
using ABI_RC.Core.InteractionSystem;
using BTKUILib.UIObjects.Components;
using cohtml;

namespace BTKUILib.UIObjects
{
    /// <summary>
    /// This object represents a page that exists in Cohtml
    /// </summary>
    public class Page : QMUIElement
    {
        /// <summary>
        /// Get or set the menu title displayed at the very top of the QM, will update on the fly
        /// </summary>
        public string MenuTitle
        {
            get => _menuTitle;
            set
            {
                _menuTitle = value;
                if(!Protected)
                    QuickMenuAPI.UpdateMenuTitle(_menuTitle, _menuSubtitle);
            }
        }

        /// <summary>
        /// Get or set the menu subtitle displayed at the very top of the QM, will update on the fly
        /// </summary>
        public string MenuSubtitle
        {
            get => _menuSubtitle;
            set
            {
                _menuSubtitle = value;
                if(!Protected)
                    QuickMenuAPI.UpdateMenuTitle(_menuTitle, _menuSubtitle);
            }
        }
        
        internal bool RootPage;
        internal string PageName = "MainPage";
        internal readonly string ModName;
        internal List<QMUIElement> PageElements = new();

        private string _menuSubtitle;
        private string _tabIcon;
        private string _menuTitle;

        /// <summary>
        /// Create a new page object, this will automatically be created within Cohtml when it is ready
        /// </summary>
        /// <param name="modName">Name of your mod, you can use this to have multiple mods use the same root tab</param>
        /// <param name="pageName">Name of the page, this isn't visible anywhere</param>
        /// <param name="rootPage">Sets if this page should also generate a tab</param>
        /// <param name="tabIcon">Icon to be displayed on the tab</param>
        public Page(string modName, string pageName, bool rootPage = false, string tabIcon = null)
        {
            if(!rootPage)
                PageName = pageName;
            ModName = modName;
            RootPage = rootPage;
            _tabIcon = tabIcon;
            
            if(rootPage)
                UserInterface.RootPages.Add(this);
            
            if(!rootPage)
                ElementID = $"btkUI-{UIUtils.GetCleanString(modName)}-{UIUtils.GetCleanString(pageName)}";
            else
                ElementID = $"btkUI-{UIUtils.GetCleanString(modName)}-MainPage";
        }

        /// <summary>
        /// Internal use only, maps this page element to an existing element in the menu
        /// </summary>
        /// <param name="elementID">ElementID matching the existing element</param>
        internal Page(string elementID)
        {
            Protected = true;
            UserInterface.RootPages.Add(this);
            ElementID = elementID;
        }

        /// <summary>
        /// Opens this page in Cohtml
        /// </summary>
        public void OpenPage()
        {
            if (!UIUtils.IsQMReady() || !IsGenerated) return;
            
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkPushPage", ElementID);
        }

        /// <summary>
        /// Add a new category (row) to this page
        /// </summary>
        /// <param name="categoryName">Name of the category, displayed at the top</param>
        /// <returns>A newly created category</returns>
        public Category AddCategory(string categoryName)
        {
            var category = new Category(categoryName, this);
            PageElements.Add(category);

            if (UIUtils.IsQMReady()) 
                category.GenerateCohtml();

            return category;
        }

        /// <summary>
        /// Create a slider on the page
        /// </summary>
        /// <param name="sliderName">Name of the slider, displayed above the slider</param>
        /// <param name="sliderTooltip">Tooltip displayed when hovering on the slider</param>
        /// <param name="initialValue">Initial value of the slider</param>
        /// <param name="minValue">Minimum value that the slider can slide to</param>
        /// <param name="maxValue">Maximum value the slider can slide to</param>
        /// <returns></returns>
        public SliderFloat AddSlider(string sliderName, string sliderTooltip, float initialValue, float minValue, float maxValue)
        {
            var slider = new SliderFloat(this, sliderName, sliderTooltip, initialValue, minValue, maxValue);
            PageElements.Add(slider);
            
            if(UIUtils.IsQMReady())
                slider.GenerateCohtml();

            return slider;
        }

        internal override void GenerateCohtml()
        {
            if(!IsGenerated)
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkCreatePage", PageName, ModName, _tabIcon, ElementID, RootPage, UIUtils.GetCleanString(PageName));
            
            IsGenerated = true;
            
            foreach (var category in PageElements)
            {
                category.GenerateCohtml();
            }
        }
    }
}