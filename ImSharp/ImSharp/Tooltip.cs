namespace ImSharp;

public static partial class Im
{
    public static class Tooltip
    {
        /// <inheritdoc cref="TooltipDisposable(bool)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static TooltipDisposable Begin()
            => new(true);

        /// <summary> Add the given text to the tooltip. </summary>
        /// <param name="text"> The tooltip text as text. Does not have to be null-terminated. </param>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static void Set(Utf8TextHandler text)
        {
            using var tt = Begin();
            Text(text);
        }

        /// <inheritdoc cref="Set(Utf8TextHandler)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static void Set(ref Utf8TextHandler text)
        {
            using var tt = Begin();
            Text(text);
        }

        /// <summary> Add the given text to a tooltip when the prior item is hovered. </summary>
        /// <param name="flags"> The flags to check on hovering. </param>
        /// <param name="text"> The tooltip text as UTF8 string. Does not have to be null-terminated. </param>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static unsafe void OnHover(HoveredFlags flags, ReadOnlySpan<byte> text)
        {
            if (text.Length is 0 || text[0] is 0 || !Native.Methods.Items.IsItemHovered(flags))
                return;

            using var tt = Begin();
            Native.Methods.Text.TextUnformatted(text.Start(out var end), end);
        }

        /// <inheritdoc cref="OnHover(HoveredFlags,ReadOnlySpan{byte})"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        [OverloadResolutionPriority(100)]
        public static void OnHover(ReadOnlySpan<byte> text)
            => OnHover(HoveredFlags.None, text);

        /// <summary> Add the given text to a tooltip when the prior item is hovered. </summary>
        /// <param name="flags"> The flags to check on hovering. </param>
        /// <param name="text"> The tooltip text as UTF16 string. </param>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static unsafe void OnHover(HoveredFlags flags, ReadOnlySpan<char> text)
        {
            if (text.Length is 0 || text[0] is '\0' || !Native.Methods.Items.IsItemHovered(flags))
                return;

            text.CopyInto<TextStringHandlerBuffer>(out var length);
            using var tt = Begin();
            Native.Methods.Text.TextUnformatted(TextStringHandlerBuffer.Buffer, TextStringHandlerBuffer.Buffer + length);
        }

        /// <inheritdoc cref="OnHover(HoveredFlags,ReadOnlySpan{byte})"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static void OnHover(ReadOnlySpan<char> text)
            => OnHover(HoveredFlags.None, text);

        /// <summary> Add the given text to a tooltip when the prior item is hovered. </summary>
        /// <param name="flags"> The flags to check on hovering. </param>
        /// <param name="text"> The tooltip text as interpolated string. This will only get evaluated if the item is hovered. </param>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        [OverloadResolutionPriority(50)]
        // ReSharper disable once EntityNameCapturedOnly.Global
        public static unsafe void OnHover(HoveredFlags flags,
            [InterpolatedStringHandlerArgument(nameof(flags))]
            ref HoverUtf8StringHandler text)
        {
            if (!text.GetEnd(out var end) || *text.Begin is 0)
                return;

            using var tt = Begin();
            Native.Methods.Text.TextUnformatted(text.Begin, end);
        }

        /// <inheritdoc cref="OnHover(HoveredFlags,ref HoverUtf8StringHandler)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static unsafe void OnHover<T>(ref Utf8StringHandler<T> text, HoveredFlags flags = HoveredFlags.None)
            where T : IStringHandlerBuffer
        {
            if (!Native.Methods.Items.IsItemHovered(flags))
                return;

            var start = text.Start(out var end);
            if (*text.Begin is 0 || start == end)
                return;

            using var tt = Begin();
            Native.Methods.Text.TextUnformatted(start, end);
        }
    }
}
