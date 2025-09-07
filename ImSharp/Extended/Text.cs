using ImSharp.Internal;

namespace ImSharp;

public static partial class ImEx
{
    /// <summary> Draw the given text framed as if it were a button but without interactivity. </summary>
    /// <param name="text"> The given text. Does not have to be null-terminated. </param>
    /// <param name="frameColor"> The background color of the frame. If this is <see cref="ColorParameter.Default"/>, <see cref="ImGuiColor.FrameBackground"/> is used. </param>
    /// <param name="textColor"> The color of the text. If this is <see cref="ColorParameter.Default"/>, <see cref="ImGuiColor.Text"/> is used. </param>
    /// <param name="borderColor"> The color of the frame border. If this is <see cref="ColorParameter.Default"/>, <see cref="ImGuiColor.Text"/> is used. </param>
    /// <param name="size"> The size of the frame. If 0, the text size is used. Otherwise, Text is aligned according to ButtonTextAlign and corners are rounded according to style. </param>
    public static void TextFramed(Utf8TextHandler text, Vector2 size = default, ColorParameter frameColor = default,
        ColorParameter textColor = default, ColorParameter borderColor = default)
    {
        var textSize = CalcAndUpdateSize(ref text, ref size);
        var rect     = Im.Cursor.ScreenRectangle(size);
        Im.Render.Frame(rect, frameColor.CheckDefault(ImGuiColor.FrameBackground), Im.Style.FrameRounding,
            borderColor.CheckDefault(ImGuiColor.Border));
        using var color = Im.Color.Push(ImGuiColor.Text, textColor);
        Im.DrawList.Window.TextClipped(rect, ref text, textSize, Im.Style.ButtonTextAlignment);
        Im.Item.SetSize(rect.Size, Im.Style.FramePadding.Y);
        Im.Item.Add(rect, 0, ItemFlags.ReadOnly | ItemFlags.NoNavigation);
    }

    /// <summary> Draw the given text bordered as if it were a button with a border and no background but without interactivity. </summary>
    /// <param name="text"> The given text. Does not have to be null-terminated. </param>
    /// <param name="textColor"> The color of the text. If this is <see cref="ColorParameter.Default"/>, <see cref="ImGuiColor.Text"/> is used. </param>
    /// <param name="borderColor"> The color of the frame border. If this is <see cref="ColorParameter.Default"/>, <see cref="ImGuiColor.Border"/> is used. </param>
    /// <param name="size"> The size of the frame. If 0, the text size is used. Otherwise, Text is aligned according to ButtonTextAlign and corners are rounded according to style. </param>
    public static void TextBordered(Utf8TextHandler text, Vector2 size = default, ColorParameter borderColor = default,
        ColorParameter textColor = default)
    {
        var textSize = CalcAndUpdateSize(ref text, ref size);
        var rect     = Im.Cursor.ScreenRectangle(size);
        Im.Render.FrameBorder(rect, borderColor.CheckDefault(ImGuiColor.Border), Im.Style.FrameRounding);
        using var color = Im.Color.Push(ImGuiColor.Text, textColor);
        Im.DrawList.Window.TextClipped(rect, ref text, textSize, Im.Style.ButtonTextAlignment);
        Im.Item.SetSize(rect.Size, Im.Style.FramePadding.Y);
        Im.Item.Add(rect, 0, ItemFlags.ReadOnly | ItemFlags.NoNavigation);
    }

    /// <summary> Draw text aligned to the frame, i.e. offset by <seealso cref="Im.ImGuiStyle.FramePadding"/>.Y </summary>
    /// <param name="text"> The given text. Does not have to be null-terminated. </param>
    [MethodImpl(ImSharpConfiguration.Inl)]
    public static void TextFrameAligned<T>(ref Utf8StringHandler<T> text) where T : IStringHandlerBuffer
    {
        Im.Cursor.FrameAlign();
        Im.Text(ref text);
    }

    /// <inheritdoc cref="TextFrameAligned{T}"/>
    [MethodImpl(ImSharpConfiguration.Inl)]
    public static void TextFrameAligned(Utf8TextHandler text)
        => TextFrameAligned(ref text);

    /// <summary> Draw text using the monospaced font configured in the <seealso cref="ImSharpContext"/> if it is available  </summary>
    /// <param name="text"> The given text. Does not have to be null-terminated. </param>
    [MethodImpl(ImSharpConfiguration.Inl)]
    public static void MonoText<T>(ref Utf8StringHandler<T> text) where T : IStringHandlerBuffer
    {
        using var mono = Im.Font.PushMono();
        Im.Text(ref text);
    }

    /// <inheritdoc cref="MonoText{T}"/>
    [MethodImpl(ImSharpConfiguration.Inl)]
    public static void MonoText(Utf8TextHandler text)
        => MonoText(ref text);

    /// <summary> Draw text aligned to the right of the current content region. </summary>
    /// <param name="text"> The given text. Does not have to be null-terminated. </param>
    /// <param name="offset"> Optional additional offset from the right of the available region. </param>
    /// <param name="knownWidth"> If the width of the text is already known, you can pass it here. If this is non-positive, the width will be calculated. </param>
    public static void TextRightAligned(Utf8TextHandler text, float offset = 0, float knownWidth = 0)
    {
        var size      = knownWidth <= 0 ? Im.Font.CalculateSize(text, false).X : knownWidth;
        var available = Im.ContentRegion.Available.X;
        Im.Cursor.X = available - size - offset;
        Im.Text(ref text);
    }

    /// <inheritdoc cref="TextRightAligned(Utf8TextHandler,float,float)"/>
    public static void TextRightAligned<T>(ref Utf8StringHandler<T> text, float offset = 0, float knownWidth = 0) where T : IStringHandlerBuffer
    {
        var size      = knownWidth <= 0 ? Im.Font.CalculateSize(ref text, false).X : knownWidth;
        var available = Im.ContentRegion.Available.X;
        Im.Cursor.X = available - size - offset;
        Im.Text(ref text);
    }

    /// <summary> Draw text of a known width aligned to the right of the current content region. </summary>
    /// <inheritdoc cref="TextRightAligned(Utf8TextHandler,float,float)"/>
    [MethodImpl(ImSharpConfiguration.Inl)]
    public static void TextRightAligned(in SizedString text, float offset = 0)
        => TextRightAligned(text.Text, offset, text.Size.X);

    /// <summary> Draw the given text horizontally centered in the current content region. </summary>
    /// <param name="text"> The given text. Does not have to be null-terminated. </param>
    /// <param name="knownWidth"> If the width of the text is already known, you can pass it here. If this is non-positive, the width will be calculated. </param>
    public static void TextCentered(Utf8TextHandler text, float knownWidth = 0)
    {
        var size      = knownWidth is 0 ? Im.Font.CalculateSize(ref text, false).X : knownWidth;
        var available = Im.ContentRegion.Maximum.X;
        Im.Cursor.X += (available - size) / 2;
        Im.Text(ref text);
    }

    /// <inheritdoc cref="TextCentered(Utf8TextHandler,float)"/>
    public static void TextCentered<T>(ref Utf8StringHandler<T> text, float knownWidth = 0) where T : IStringHandlerBuffer
    {
        var size      = knownWidth is 0 ? Im.Font.CalculateSize(ref text, false).X : knownWidth;
        var available = Im.ContentRegion.Maximum.X;
        Im.Cursor.X += (available - size) / 2;
        Im.Text(ref text);
    }

    /// <summary> Draw text of a known width horizontally centered in the current content region. </summary>
    /// <inheritdoc cref="TextCentered(Utf8TextHandler,float)"/>
    [MethodImpl(ImSharpConfiguration.Inl)]
    public static void TextCentered(in SizedString text)
        => TextCentered(text.Text, text.Size.X);

    /// <summary> Draw the same text multiple times at the cursor position to simulate a shadowed text. </summary>
    /// <param name="text"> The given text. Does not need to be null-terminated. </param>
    /// <param name="foregroundColor"> The center text color. If <see cref="ColorParameter.Default"/>, <see cref="ImGuiColor.Text"/> is used. </param>
    /// <param name="shadowColor"> The shadow color. If <see cref="ColorParameter.Default"/>, <see cref="Rgba32.Black"/> is used. </param>
    /// <param name="shadowWidth"> The width of the shadow in pixels. Should usually be 1. </param>
    public static void TextShadowed(Utf8TextHandler text, ColorParameter foregroundColor, ColorParameter shadowColor, byte shadowWidth = 1)
    {
        var       shadow   = shadowColor.CheckDefault(Rgba32.Black);
        var       position = Im.Cursor.Position;
        using var color    = ImGuiColor.Text.Push(shadow);
        for (var i = -shadowWidth; i <= shadowWidth; i++)
        {
            for (var j = -shadowWidth; j <= shadowWidth; j++)
            {
                if (i is 0 && j is 0)
                    continue;

                Im.Cursor.Position = new Vector2(position.X + i, position.Y + j);
                Im.Text(ref text);
            }
        }

        color.Pop();
        color.Push(ImGuiColor.Text, foregroundColor);
        Im.Cursor.Position = position;
        Im.Text(ref text);
    }

    /// <summary> Draw the same text multiple times at the given position in a draw list to simulate a shadowed text. </summary>
    /// <param name="drawList"> The draw list. </param>
    /// <param name="position"> The position to draw the text at in screen coordinates. </param>
    /// <param name="text"> The text to draw. </param>
    /// <param name="foregroundColor"> The center text color. If <see cref="ColorParameter.Default"/>, <see cref="ImGuiColor.Text"/> is used. </param>
    /// <param name="shadowColor"> The shadow color. If <see cref="ColorParameter.Default"/>, <see cref="Rgba32.Black"/> is used. </param>
    /// <param name="shadowWidth"> The width of the shadow in pixels. Should usually be 1. </param>
    public static void TextShadowed(Im.DrawList drawList, Vector2 position, Utf8TextHandler text,
        ColorParameter foregroundColor, ColorParameter shadowColor, byte shadowWidth = 1)
    {
        var shadow = shadowColor.CheckDefault(Rgba32.Black);
        for (var i = -shadowWidth; i <= shadowWidth; i++)
        {
            for (var j = -shadowWidth; j <= shadowWidth; j++)
            {
                if (i is 0 && j is 0)
                    continue;

                drawList.Text(position, shadow, ref text);
            }
        }

        drawList.Text(position, foregroundColor.CheckDefault(ImGuiColor.Text), ref text);
    }

    /// <summary> A wrapper for colored text. </summary>
    /// <param name="text"> The text. </param>
    /// <param name="color"> The color. </param>
    public readonly ref struct ColorText(ReadOnlySpan<byte> text, ColorParameter color = default)
    {
        public readonly ReadOnlySpan<byte> Text  = text;
        public readonly ColorParameter     Color = color;
    }

    /// <summary> Draw multiple pieces of text in different colors and no additional spacing between them. </summary>
    /// <param name="text"> The text pieces with their associated colors. </param>
    public static void TextMultiColored(params IEnumerable<ColorText> text)
    {
        var       textColor = ImGuiColor.Text.Get();
        using var group     = Im.Group();
        foreach (var colorText in text)
        {
            Im.Text(colorText.Text, colorText.Color.CheckDefault(textColor));
            Im.Line.Same(0, 0);
        }

        Im.Line.New();
    }
}
