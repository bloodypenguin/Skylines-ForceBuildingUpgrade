using ColossalFramework.UI;
using UnityEngine;

namespace ForceLevelUp
{
    public static class UIUtil
    {
        private const int ButtonSize = 32;
        private static readonly Vector3 LevelUpButtonOffset = new Vector3(-114.0f, 6.0f, 0.0f);
        private static readonly Vector3 SetOnFireButtonOffset = new Vector3(-150.0f, 6.0f, 0.0f);

        public static UIButton CreateLevelUpButton(UIComponent parentComponent, MouseEventHandler handler)
        {
            return CreateButton("ForceLevelUpButton", "Force Level Up", "IconUpArrow", LevelUpButtonOffset,
                parentComponent, handler);
        }

        public static UIButton CreateSetOnFireButton(UIComponent parentComponent, MouseEventHandler handler)
        {
            return CreateButton("SetOnFireButton", "Set On Fire", "InfoIconFireSafety", SetOnFireButtonOffset,
                parentComponent, handler);
        }

        public static UIButton CreateButton(string buttonName, string tooltip, string fgSpite, Vector3 offset,
            UIComponent parentComponent, MouseEventHandler handler)
        {

            var button = UIView.GetAView().AddUIComponent(typeof(UIButton)) as UIButton;
            button.canFocus = false;
            button.name = buttonName;
            button.width = ButtonSize;
            button.height = ButtonSize;
            button.scaleFactor = 1.0f;
            button.tooltip = tooltip;

            button.pressedBgSprite = "OptionBasePressed";
            button.normalBgSprite = "OptionBase";
            button.hoveredBgSprite = "OptionBaseHovered";
            button.disabledBgSprite = "OptionBaseDisabled";
            button.focusedBgSprite = "OptionBaseFocused";

            button.normalFgSprite = fgSpite;
            button.hoveredFgSprite = fgSpite + "Hovered";
            button.focusedFgSprite = fgSpite + "Focused";
            button.disabledFgSprite = fgSpite + "Disabled";
            button.pressedFgSprite = fgSpite + "Pressed";


            button.textColor = new Color32(255, 255, 255, 255);
            button.disabledTextColor = new Color32(7, 7, 7, 255);
            button.hoveredTextColor = new Color32(255, 255, 255, 255);
            button.focusedTextColor = new Color32(255, 255, 255, 255);
            button.pressedTextColor = new Color32(30, 30, 44, 255);
            button.eventClick += handler;
            button.AlignTo(parentComponent, UIAlignAnchor.TopRight);
            button.relativePosition += offset;
            return button;
        } 
    }
}