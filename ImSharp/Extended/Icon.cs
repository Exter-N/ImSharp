// ReSharper disable MethodOverloadWithOptionalParameter

namespace ImSharp;

public static partial class ImEx
{
    /// <summary> A wrapper around functions using a specific font for icons. </summary>
    public static class Icon
    {
        /// <inheritdoc cref="Draw{T}(T,Rgba32)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static void Draw<T>(T icon) where T : IIconStandIn
        {
            using var _ = T.Font.Push();
            Im.Text(icon.Span);
        }

        /// <summary> Draw a stand-alone icon as text. </summary>
        /// <typeparam name="T"> The icon type. </typeparam>
        /// <param name="icon"> The icon. </param>
        /// <param name="textColor"> The color of the icon. Uses text color if 0. </param>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static void Draw<T>(T icon, Rgba32 textColor) where T : IIconStandIn
        {
            using var _ = T.Font.Push();
            Im.Text(icon.Span, textColor);
        }

        /// <inheritdoc cref="DrawAligned{T}(T,Rgba32)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static void DrawAligned<T>(T icon) where T : IIconStandIn
        {
            Im.Cursor.FrameAlign();
            using var _ = T.Font.Push();
            Im.Text(icon.Span);
        }

        /// <summary> Draw a stand-alone icon as text aligned to the frame offset. </summary>
        /// <typeparam name="T"> The icon type. </typeparam>
        /// <param name="icon"> The icon. </param>
        /// <param name="textColor"> The color of the icon. Uses text color if 0. </param>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static void DrawAligned<T>(T icon, Rgba32 textColor) where T : IIconStandIn
        {
            Im.Cursor.FrameAlign();
            using var _ = T.Font.Push();
            Im.Text(icon.Span, textColor);
        }

        /// <summary> Calculate the size of a stand-alone icon. </summary>
        /// <typeparam name="T"> The icon type. </typeparam>
        /// <param name="icon"> The icon. </param>
        /// <returns> The size of the icon. </returns>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static Vector2 CalculateSize<T>(T icon) where T : IIconStandIn
        {
            using var _ = T.Font.Push();
            return Im.Font.CalculateSize(icon.Span, false);
        }

        /// <summary> Draw a button with the given icon as label. </summary>
        /// <typeparam name="T"> The icon type. </typeparam>
        /// <param name="icon"> The icon. </param>
        /// <param name="tooltip"> A tooltip shown when hovering the button regardless of whether it is disabled or not as text. Does not have to be null-terminated. </param>
        /// <param name="config"> Additional parameters to configure the design and behavior of the button. </param>
        /// <returns> True if the button has been clicked in this frame. </returns>
        /// <remarks> The tooltip is always evaluated. If this is expensive, prefer leaving it empty and using <seealso cref="Im.Tooltip.OnHover(HoveredFlags,ref HoverUtf8StringHandler)"/> manually. </remarks>
        [OverloadResolutionPriority(20)]
        public static bool Button<T>(T icon, Utf8TextHandler tooltip = default, in ButtonConfiguration config = default) where T : IIconStandIn
        {
            var size = new Vector2(config.Size.X is 0 ? Im.Style.FrameHeight : config.Size.X,
                config.Size.Y is 0 ? Im.Style.FrameHeight : config.Size.Y);

            using var color = Im.Color.Push(ImGuiColor.Button, config.ButtonColor)
                .Push(ImGuiColor.ButtonHovered, config.HoveredColor)
                .Push(ImGuiColor.ButtonActive,  config.ActiveColor)
                .Push(ImGuiColor.Text,          config.TextColor)
                .Push(ImGuiColor.Border,        config.BorderColor);
            using var style = Im.Style.Push(ImStyleSingle.FrameBorderThickness, Im.Style.GlobalScale, config.BorderColor.IsVisible);
            using var _     = Im.Disabled(config.Disabled);
            bool      ret;
            using (T.Font.Push())
            {
                ret = Im.Button(icon.Span, size, config.Flags);
            }

            if (tooltip.GetSpan(out var span))
                Im.Tooltip.OnHover(HoveredFlags.AllowWhenDisabled, span);
            return ret;
        }


        /// <summary> Draw a button with the given icon as label. </summary>
        /// <typeparam name="T"> The icon type. </typeparam>
        /// <param name="icon"> The icon. </param>
        /// <param name="config"> Additional parameters to configure the design and behavior of the button. </param>
        /// <returns> True if the button has been clicked in this frame. </returns>
        [OverloadResolutionPriority(25)]
        public static bool Button<T>(T icon, in ButtonConfiguration config = default) where T : IIconStandIn
        {
            var size = new Vector2(config.Size.X is 0 ? Im.Style.FrameHeight : config.Size.X,
                config.Size.Y is 0 ? Im.Style.FrameHeight : config.Size.Y);

            using var color = Im.Color.Push(ImGuiColor.Button, config.ButtonColor)
                .Push(ImGuiColor.ButtonHovered, config.HoveredColor)
                .Push(ImGuiColor.ButtonActive,  config.ActiveColor)
                .Push(ImGuiColor.Text,          config.TextColor)
                .Push(ImGuiColor.Border,        config.BorderColor);
            using var style = Im.Style.Push(ImStyleSingle.FrameBorderThickness, Im.Style.GlobalScale, config.BorderColor.IsVisible);
            using var _     = Im.Disabled(config.Disabled);
            using var font  = T.Font.Push();
            return Im.Button(icon.Span, size, config.Flags);
        }

        /// <summary> Draw a button with the given icon as label. </summary>
        /// <typeparam name="T"> The icon type. </typeparam>
        /// <param name="icon"> The icon. </param>
        /// <param name="size"> The desired size for the button. If (0, 0), it will be frame size. </param>
        /// <param name="tooltip"> A tooltip shown when hovering the button regardless of whether it is disabled or not as text. Does not have to be null-terminated. </param>
        /// <param name="disabled"> Whether the button should be disabled or not. </param>
        /// <param name="buttonColor"> The color of the button's background. </param>
        /// <param name="textColor"> The color of the button's label. </param>
        /// <param name="flags"> Additional flags to control the button's behaviour. </param>
        /// <returns> True if the button has been clicked in this frame. </returns>
        /// <remarks> The tooltip is always evaluated. If this is expensive, prefer leaving it empty and using <seealso cref="Im.Tooltip.OnHover(HoveredFlags,ref HoverUtf8StringHandler)"/> manually. </remarks>
        [OverloadResolutionPriority(50)]
        public static bool Button<T>(T icon, Utf8TextHandler tooltip = default, bool disabled = false,
            Rgba32? buttonColor = null, Rgba32? textColor = null, Vector2 size = default, ButtonFlags flags = ButtonFlags.None)
            where T : IIconStandIn
        {
            if (size.X is 0)
                size.X = Im.Style.FrameHeight;
            if (size.Y is 0)
                size.Y = Im.Style.FrameHeight;
            using var color = Im.Color.Push(ImGuiColor.Button, buttonColor)
                .Push(ImGuiColor.Text, textColor);
            using var _ = Im.Disabled(disabled);
            bool      ret;
            using (T.Font.Push())
            {
                ret = Im.Button(icon.Span, size, flags);
            }

            if (tooltip.GetSpan(out var span))
                Im.Tooltip.OnHover(HoveredFlags.AllowWhenDisabled, span);
            return ret;
        }


        /// <inheritdoc cref="Button{T}(T,Utf8TextHandler,bool,Rgba32?,Rgba32?,Vector2,ButtonFlags)"/>
        [OverloadResolutionPriority(100)]
        public static bool Button<T>(T icon, Utf8TextHandler tooltip = default, bool disabled = false,
            Vector2 size = default, ButtonFlags flags = ButtonFlags.None) where T : IIconStandIn
        {
            if (size.X is 0)
                size.X = Im.Style.FrameHeight;
            if (size.Y is 0)
                size.Y = Im.Style.FrameHeight;
            using var _ = Im.Disabled(disabled);
            bool      ret;
            using (T.Font.Push())
            {
                ret = Im.Button(icon.Span, size, flags);
            }

            if (tooltip.GetSpan(out var span))
                Im.Tooltip.OnHover(HoveredFlags.AllowWhenDisabled, span);
            return ret;
        }

        /// <inheritdoc cref="Button{T}(T,Utf8TextHandler,bool,Rgba32?,Rgba32?,Vector2,ButtonFlags)"/>
        [OverloadResolutionPriority(200)]
        public static bool Button<T>(T icon, Utf8TextHandler tooltip = default,
            Vector2 size = default, ButtonFlags flags = ButtonFlags.None) where T : IIconStandIn
        {
            if (size.X is 0)
                size.X = Im.Style.FrameHeight;
            if (size.Y is 0)
                size.Y = Im.Style.FrameHeight;
            bool ret;
            using (T.Font.Push())
            {
                ret = Im.Button(icon.Span, size, flags);
            }

            if (tooltip.GetSpan(out var span))
                Im.Tooltip.OnHover(HoveredFlags.AllowWhenDisabled, span);
            return ret;
        }

        /// <summary> Draw a button with the given icon as label. </summary>
        /// <typeparam name="T"> The icon type. </typeparam>
        /// <param name="icon"> The icon. </param>
        /// <param name="size"> The desired size for the button. If (0, 0), it will be frame size. </param>
        /// <param name="disabled"> Whether the button should be disabled or not. </param>
        /// <param name="flags"> Additional flags to control the button's behaviour. </param>
        /// <returns> True if the button has been clicked in this frame. </returns>
        [OverloadResolutionPriority(300)]
        public static bool Button<T>(T icon, bool disabled = false, Vector2 size = default, ButtonFlags flags = ButtonFlags.None)
            where T : IIconStandIn
        {
            if (size.X is 0)
                size.X = Im.Style.FrameHeight;
            if (size.Y is 0)
                size.Y = Im.Style.FrameHeight;
            using var _    = Im.Disabled(disabled);
            using var font = T.Font.Push();
            return Im.Button(icon.Span, size, flags);
        }

        /// <inheritdoc cref="Button{T}(T,bool,Vector2,ButtonFlags)"/>
        [OverloadResolutionPriority(400)]
        public static bool Button<T>(T icon, Vector2 size = default, ButtonFlags flags = ButtonFlags.None) where T : IIconStandIn
        {
            if (size.X is 0)
                size.X = Im.Style.FrameHeight;
            if (size.Y is 0)
                size.Y = Im.Style.FrameHeight;
            using var font = T.Font.Push();
            return Im.Button(icon.Span, size, flags);
        }

        /// <summary> Draw an icon with a label and a tooltip when hovering either of them. </summary>
        /// <typeparam name="T"> The icon type. </typeparam>
        /// <param name="icon"> The icon. </param>
        /// <param name="label"> The label as text. Does not have to be null-terminated. </param>
        /// <param name="tooltip"> The tooltip as text. Does not have to be null-terminated. </param>
        /// <param name="iconColor"> The color for the icon. </param>
        /// <param name="hovered"> Force the tooltip on when this is true. </param>
        public static void Labeled<T>(T icon, Utf8LabelHandler label, Utf8TextHandler tooltip, Rgba32 iconColor, bool hovered = false)
            where T : IIconStandIn
        {
            Draw(icon, iconColor);
            hovered = hovered || Im.Item.Hovered();
            Im.Line.SameInner();
            Im.Text(ref label);
            if (hovered || Im.Item.Hovered())
                Im.Tooltip.Set(tooltip);
        }

        /// <summary> Draw an icon and a label in the same line as the last item and a tooltip when hovering either of them or the last item. </summary>
        /// <typeparam name="T"> The icon type. </typeparam>
        /// <param name="icon"> The icon. </param>
        /// <param name="label"> The label as text. Does not have to be null-terminated. </param>
        /// <param name="tooltip"> The tooltip as text. Does not have to be null-terminated. </param>
        /// <param name="iconColor"> The color for the icon. </param>
        public static void Label<T>(T icon, Utf8LabelHandler label, Utf8TextHandler tooltip, Rgba32 iconColor) where T : IIconStandIn
        {
            Im.Line.SameInner();
            var hovered = Im.Item.Hovered();
            Draw(icon, iconColor);
            hovered = hovered || Im.Item.Hovered();
            Im.Line.SameInner();
            Im.Text(ref label);
            if (hovered || Im.Item.Hovered())
                Im.Tooltip.Set(tooltip);
        }

        /// <inheritdoc cref="Labeled{T}(T,Utf8LabelHandler,Utf8TextHandler,Rgba32,bool)"/>
        public static void Labeled<T>(T icon, Utf8LabelHandler label, Utf8TextHandler tooltip, bool hovered = false) where T : IIconStandIn
        {
            Draw(icon);
            hovered = hovered || Im.Item.Hovered();
            Im.Line.SameInner();
            Im.Text(ref label);
            if (hovered || Im.Item.Hovered())
                Im.Tooltip.Set(tooltip);
        }

        /// <inheritdoc cref="Label{T}(T,Utf8LabelHandler,Utf8TextHandler,Rgba32)"/>
        public static void Label<T>(T icon, Utf8LabelHandler label, Utf8TextHandler tooltip) where T : IIconStandIn
        {
            Im.Line.SameInner();
            var hovered = Im.Item.Hovered();
            Draw(icon);
            hovered = hovered || Im.Item.Hovered();
            Im.Line.SameInner();
            Im.Text(ref label);
            if (hovered || Im.Item.Hovered())
                Im.Tooltip.Set(tooltip);
        }
    }
}
