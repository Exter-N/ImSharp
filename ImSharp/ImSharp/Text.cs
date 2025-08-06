namespace ImSharp;

public static unsafe partial class Im
{
    /// <summary> Draw text. </summary>
    /// <param name="text"> The given text. Does not have to be null-terminated. </param>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static void Text(Utf8TextHandler text)
        => Native.Methods.Text.TextUnformatted(text.Start(out var end), end);

    /// <summary> Draw text in a given color. </summary>
    /// <param name="text"> The given text as a UTF8 string. Does not have to be null-terminated. </param>
    /// <param name="color"> The desired color as RGBA32. </param>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static void Text(Utf8TextHandler text, Rgba32 color)
    {
        var style = Style.AsWritable();
        var old   = style[ImGuiColor.Text];
        style[ImGuiColor.Text] = color.ToVector();
        Native.Methods.Text.TextUnformatted(text.Start(out var end), end);
        style[ImGuiColor.Text] = old;
    }

    /// <summary> Draw text in a given color. </summary>
    /// <param name="text"> The given text. Does not have to be null-terminated. </param>
    /// <param name="color"> The desired color. </param>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static void Text(Utf8TextHandler text, in Vector4 color)
    {
        var style = Style.AsWritable();
        var old   = style[ImGuiColor.Text];
        style[ImGuiColor.Text] = color;
        Native.Methods.Text.TextUnformatted(text.Start(out var end), end);
        style[ImGuiColor.Text] = old;
    }

    /// <summary> Draw text in the <seealso cref="ImGuiColor.TextDisabled"/> color. </summary>
    /// <param name="text"> The given text. Does not have to be null-terminated. </param>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static void TextDisabled(Utf8TextHandler text)
    {
        var style = Style.AsWritable();
        var old   = style[ImGuiColor.Text];
        style[ImGuiColor.Text] = style[ImGuiColor.TextDisabled];
        Native.Methods.Text.TextUnformatted(text.Start(out var end), end);
        style[ImGuiColor.Text] = old;
    }

    /// <summary> Can be used to avoid copying an already established <seealso cref="Utf8TextHandler"/>. </summary>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    [OverloadResolutionPriority(100)]
    public static void Text<T>(ref Utf8StringHandler<T> text) where T : IStringHandlerBuffer
        => Native.Methods.Text.TextUnformatted(text.Start(out var end), end);
}
