namespace ImSharp;

/// <summary> A text string encoded both in UTF16 and UTF8. </summary>
/// <param name="Utf16"> The UTF16-encoded string. </param>
/// <param name="Utf8"> The UTF8-encoded string. </param>
public readonly record struct StringPair(string Utf16, StringU8 Utf8)
{
    /// <summary> Create a pair from an existing UTF16-encoded string. </summary>
    /// <param name="text"> The UTF16-encoded string. </param>
    [OverloadResolutionPriority(20)]
    public StringPair(string text)
        : this(text, new StringU8(text))
    { }

    /// <summary> Create a pair from an existing UTF8-encoded string. </summary>
    /// <param name="text"> The UTF8-encoded string. </param>
    [OverloadResolutionPriority(20)]
    public StringPair(StringU8 text)
        : this(text.ToString(), text)
    { }

    /// <summary> Create a pair from an interpolated string. </summary>
    /// <param name="handler"> The interpolated string. </param>
    [OverloadResolutionPriority(100)]
    public StringPair(DefaultInterpolatedStringHandler handler)
        : this(Convert(ref handler, out var u8), u8)
    { }

    /// <summary> Create a pair from an interpolated string. </summary>
    /// <param name="handler"> The interpolated string. </param>
    [OverloadResolutionPriority(50)]
    public StringPair(Utf8InterpolatedStringHandler handler)
        : this(Convert(ref handler, out var u8), u8)
    { }

    /// <summary> Create a pair from a UTF8 byte span. </summary>
    /// <param name="text"> The byte span. </param>
    [OverloadResolutionPriority(10)]
    public StringPair(ReadOnlySpan<byte> text)
        : this(new StringU8(text))
    { }

    /// <summary> Get whether the string is empty. </summary>
    public bool IsEmpty
        => Utf8.IsEmpty;

    public static implicit operator string(StringPair p)
        => p.Utf16;

    public static implicit operator ReadOnlySpan<char>(StringPair p)
        => p.Utf16;

    public static implicit operator StringU8(StringPair p)
        => p.Utf8;

    public static implicit operator ReadOnlySpan<byte>(StringPair p)
        => p.Utf8;

    /// <summary> Helper to create both types of strings from a handler. </summary>
    internal static string Convert(ref DefaultInterpolatedStringHandler handler, out StringU8 t)
    {
        var ret = handler.ToStringAndClear();
        t = new StringU8(ret);
        return ret;
    }

    /// <summary> Helper to create both types of strings from a handler. </summary>
    internal static string Convert(ref Utf8InterpolatedStringHandler handler, out StringU8 t)
    {
        t = new StringU8(ref handler);
        return t.ToString();
    }
}
