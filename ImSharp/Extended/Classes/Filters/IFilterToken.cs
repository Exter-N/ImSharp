namespace ImSharp;

/// <summary> A single token for a tokenized filter. </summary>
/// <typeparam name="TTokenType"> The enumeration type that defines the different token types. </typeparam>
/// <typeparam name="TSelf"> The own type. </typeparam>
public interface IFilterToken<TTokenType, in TSelf>
{
    /// <summary> The textual part of the token. </summary>
    public string Needle { get; init; }

    /// <summary> The token type. </summary>
    public TTokenType Type { get; init; }

    /// <summary> Whether this token fully contains the search space of another token to reduce redundancy. </summary>
    /// <param name="other"> The other token to compare search spaces with. </param>
    /// <returns> True if this token fully encompasses all search results the other token would produce. </returns>
    /// <remarks>
    ///   For example, if the search space is all strings containing a given text,
    ///   then this token should contain any token of the same type where <see cref="Needle">this.Needle</see> contains <see cref="Needle">other.Needle</see>.
    /// </remarks>
    public bool Contains(TSelf other);

    /// <summary> Try to convert a single character into a token type. </summary>
    /// <param name="tokenCharacter"> The character to convert. </param>
    /// <param name="type"> The returned token type, if any. </param>
    /// <returns> True if the character corresponded to a token type, false otherwise. </returns>
    public abstract static bool ConvertToken(char tokenCharacter, out TTokenType type);

    /// <summary> Get whether a specific token type supports a 'None' keyword to check for the absence of a certain attribute. </summary>
    /// <param name="type"> The token type. </param>
    /// <returns> True if 'None' is supported for this type. </returns>
    public abstract static bool AllowsNone(TTokenType type);
}
