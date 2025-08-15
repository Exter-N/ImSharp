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
    /// <param name="text"> The given text. Does not have to be null-terminated. </param>
    /// <param name="offset"> Optional additional offset from the right of the available region. </param>
    [MethodImpl(ImSharpConfiguration.Inl)]
    public static void TextRightAligned(SizedString text, float offset = 0)
        => TextRightAligned(text.Text, offset, text.Size.X);
}
