namespace ImSharp;

/// <summary> A string with its size with the given font, mainly for use in caches. </summary>
/// <param name="Text"> The text. </param>
/// <param name="Size"> The font size of the text in pixels. </param>
public readonly record struct SizedString(StringU8 Text, Vector2 Size)
{
    public unsafe SizedString(StringU8 text, bool hideTextAfterDashes = true, float wrapWidth = 0, Im.Font font = default)
        : this(text, font.CalculateTextSize(text))
    { }

    public SizedString(ReadOnlySpan<char> text, bool hideTextAfterDashes = true, float wrapWidth = 0, Im.Font font = default)
        : this(new StringU8(text))
    { }

    public SizedString(Utf8InterpolatedStringHandler text, bool hideTextAfterDashes = true, float wrapWidth = 0, Im.Font font = default)
        : this(new StringU8(ref text))
    { }

    public unsafe SizedString(byte* text, bool hideTextAfterDashes = true, float wrapWidth = 0, Im.Font font = default)
        : this(new StringU8(text))
    { }

    public SizedString(ReadOnlySpan<byte> text, bool hideTextAfterDashes = true, float wrapWidth = 0, Im.Font font = default)
        : this(new StringU8(text))
    { }

    public SizedString(ReadOnlyMemory<byte> text, bool hideTextAfterDashes = true, float wrapWidth = 0, Im.Font font = default)
        : this(new StringU8(text))
    { }
}
