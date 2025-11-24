namespace ImSharp;

/// <summary> A text string encoded both in UTF16 and UTF8 that also keeps track of its render size. </summary>
/// <param name="Utf16"> The UTF16-encoded string. </param>
/// <param name="Utf8"> The sized UTF8-encoded string. </param>
public readonly record struct SizedStringPair(string Utf16, SizedString Utf8)
{
    /// <inheritdoc cref="StringPair(string)"/>
    [OverloadResolutionPriority(20)]
    public SizedStringPair(string text)
        : this(text, new SizedString(new StringU8(text)))
    { }

    /// <inheritdoc cref="StringPair(StringU8)"/>
    [OverloadResolutionPriority(20)]
    public SizedStringPair(StringU8 text)
        : this(text.ToString(), new SizedString(text))
    { }

    /// <summary> Create a pair from an already sized UTF8-encoded string. </summary>
    /// <param name="text"> The sized UTF8-encoded string. </param>
    [OverloadResolutionPriority(30)]
    public SizedStringPair(SizedString text)
        : this(text.ToString(), text)
    { }

    /// <inheritdoc cref="StringPair(DefaultInterpolatedStringHandler)"/>
    [OverloadResolutionPriority(100)]
    public SizedStringPair(DefaultInterpolatedStringHandler handler)
        : this(StringPair.Convert(ref handler, out var u8), new SizedString(u8))
    { }

    /// <inheritdoc cref="StringPair(Utf8InterpolatedStringHandler)"/>
    [OverloadResolutionPriority(50)]
    public SizedStringPair(Utf8InterpolatedStringHandler handler)
        : this(StringPair.Convert(ref handler, out var u8), new SizedString(u8))
    { }

    /// <summary> Get whether the string is empty. </summary>
    public bool IsEmpty
        => Utf16.Length is 0;

    public static implicit operator string(SizedStringPair p)
        => p.Utf16;

    public static implicit operator ReadOnlySpan<char>(SizedStringPair p)
        => p.Utf16;

    public static implicit operator SizedString(SizedStringPair p)
        => p.Utf8;

    public static implicit operator StringU8(SizedStringPair p)
        => p.Utf8.Text;

    public static implicit operator ReadOnlySpan<byte>(SizedStringPair p)
        => p.Utf8.Text;

    public static implicit operator StringPair(SizedStringPair p)
        => new(p.Utf16, p.Utf8.Text);
}
