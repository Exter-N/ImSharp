namespace ImSharp;

/// <summary> A text string encoded both in UTF16 and UTF8 that also keeps track of its render size. </summary>
public record SizedStringPair : SizedString
{
    /// <summary> The UTF16-encoded string. </summary>
    public string Utf16
        => field ??= Utf8.ToString();

    /// <inheritdoc cref="SizedString.Empty"/>
    public new static readonly SizedStringPair Empty = new();

    /// <summary> The sized UTF8-encoded string. </summary>
    public SizedString Utf8
        => this;

    /// <inheritdoc cref="StringPair(StringU8)"/>
    [OverloadResolutionPriority(20)]
    public SizedStringPair(StringU8 text)
        : base(text)
    { }

    /// <summary> Create a pair from an already sized UTF8-encoded string. </summary>
    /// <param name="text"> The sized UTF8-encoded string. </param>
    [OverloadResolutionPriority(30)]
    public SizedStringPair(SizedString text)
        : base(text.Text, text.Size)
    { }

    /// <inheritdoc cref="StringPair(DefaultInterpolatedStringHandler)"/>
    [OverloadResolutionPriority(100)]
    public SizedStringPair(DefaultInterpolatedStringHandler handler)
        : this(ToStringAndClear(ref handler, out var utf8), utf8)
    { }

    /// <inheritdoc cref="StringPair(DefaultInterpolatedStringHandler)"/>
    [OverloadResolutionPriority(20)]
    public SizedStringPair(string text)
        : this(text, new StringU8(text))
    { }

    /// <inheritdoc cref="StringPair(Utf8InterpolatedStringHandler)"/>
    [OverloadResolutionPriority(50)]
    public SizedStringPair(Utf8InterpolatedStringHandler handler)
        : base(new SizedString(handler))
    { }

    /// <summary> Get whether the string is empty. </summary>
    public bool IsEmpty
        => Text.IsEmpty;

    public static implicit operator string(SizedStringPair p)
        => p.Utf16;

    public static implicit operator ReadOnlySpan<char>(SizedStringPair p)
        => p.Utf16;

    public static implicit operator StringU8(SizedStringPair p)
        => p.Utf8.Text;

    public static implicit operator ReadOnlySpan<byte>(SizedStringPair p)
        => p.Utf8.Text;

    public static implicit operator StringPair(SizedStringPair p)
        => new(p.Utf16, p.Utf8.Text);

    private static string ToStringAndClear(ref DefaultInterpolatedStringHandler handler, out StringU8 utf16)
    {
        var ret = handler.ToStringAndClear();
        utf16 = new StringU8(ret);
        return ret;
    }

    /// <summary> Create the empty sized string. </summary>
    private SizedStringPair()
        => Utf16 = string.Empty;

    private SizedStringPair(string? utf16, StringU8 utf8)
        : base(utf8)
        => Utf16 = utf16;
}
