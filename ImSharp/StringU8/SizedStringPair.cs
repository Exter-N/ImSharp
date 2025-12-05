namespace ImSharp;

/// <summary> A text string encoded both in UTF16 and UTF8 that also keeps track of its render size. </summary>
/// <param name="utf16"> The UTF16-encoded string. </param>
/// <param name="utf8"> The sized UTF8-encoded string. </param>
public struct SizedStringPair(string? utf16, SizedString? utf8 = null)
{
    private string?      _utf16 = utf16;
    private SizedString? _utf8  = utf8;

    /// <summary> The UTF16-encoded string. </summary>
    public string Utf16
        => _utf16 ??= _utf8!.ToString();

    /// <summary> The sized UTF8-encoded string. </summary>
    public SizedString Utf8
        => _utf8 ??= new SizedString($"{_utf16}");

    /// <inheritdoc cref="StringPair(StringU8)"/>
    [OverloadResolutionPriority(20)]
    public SizedStringPair(StringU8 text)
        : this(text.ToString())
    { }

    /// <summary> Create a pair from an already sized UTF8-encoded string. </summary>
    /// <param name="text"> The sized UTF8-encoded string. </param>
    [OverloadResolutionPriority(30)]
    public SizedStringPair(SizedString text)
        : this(null, text)
    { }

    /// <inheritdoc cref="StringPair(DefaultInterpolatedStringHandler)"/>
    [OverloadResolutionPriority(100)]
    public SizedStringPair(DefaultInterpolatedStringHandler handler)
        : this(handler.ToStringAndClear())
    { }

    /// <inheritdoc cref="StringPair(Utf8InterpolatedStringHandler)"/>
    [OverloadResolutionPriority(50)]
    public SizedStringPair(Utf8InterpolatedStringHandler handler)
        : this(null, new SizedString(handler))
    { }

    /// <summary> Get whether the string is empty. </summary>
    public bool IsEmpty
        => _utf16 is null ? _utf8!.Text.IsEmpty : _utf16.Length is 0;

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
