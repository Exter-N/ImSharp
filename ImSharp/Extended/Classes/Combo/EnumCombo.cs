namespace ImSharp;

/// <summary> A basic combo over all available enumeration values. </summary>
/// <typeparam name="T"></typeparam>
public class EnumCombo<T> : SimpleFilterCombo<T>
    where T : unmanaged, Enum
{
    protected readonly T[] Values;

    /// <summary> Create the default combo using all available values. </summary>
    /// <remarks> Use <see cref="Instance"/> instead. </remarks>
    private EnumCombo()
        : base(SimpleFilterType.Text)
        => Values = Enum.GetValues<T>();

    /// <summary> Create a combo using only the supplied values. </summary>
    /// <param name="values"> All values to list. </param>
    public EnumCombo(params T[] values)
        : base(SimpleFilterType.Text)
        => Values = values;

    /// <summary> Create a combo using all named values of the enumeration type except for the supplied values. </summary>
    /// <param name="exclusions"> The excluded values. </param>
    /// <returns> The combo. </returns>
    public static EnumCombo<T> Excluding(params HashSet<T> exclusions)
        => new(Enum.GetValues<T>().Where(v => !exclusions.Contains(v)).ToArray());

    /// <summary> A combo over all available enumeration values. </summary>
    public static readonly EnumCombo<T> Instance = new();

    /// <inheritdoc/>
    public override StringU8 DisplayString(in T value)
        => new($"{value}");

    /// <inheritdoc/>
    public override string FilterString(in T value)
        => $"{value}";

    /// <inheritdoc/>
    public override IEnumerable<T> GetBaseItems()
        => Values;
}
