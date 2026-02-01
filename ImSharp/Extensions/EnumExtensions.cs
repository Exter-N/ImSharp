namespace ImSharp;

/// <summary> Extensions for enum types. </summary>
public static class EnumExtensions
{
    /// <summary> Check whether at least one of the given flags is set. </summary>
    /// <typeparam name="TEnum"> The type of the enum, which should consist of flags. </typeparam>
    /// <param name="value"> The value to check against the flags. </param>
    /// <param name="flags"> The flags to check for. </param>
    /// <returns> True if at least one flag in <paramref name="flags"/> is set in <paramref name="value"/>. </returns>
    /// <remarks> If <paramref name="flags"/> is 0, this always returns false. </remarks>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static bool CheckAny<TEnum>(this TEnum value, TEnum flags)
        where TEnum : unmanaged, Enum
        => value.And(flags) is not 0;

    /// <summary> Check whether all given flags are set. </summary>
    /// <typeparam name="TEnum"> The type of the enum, which should consist of flags. </typeparam>
    /// <param name="value"> The value to check against the flags. </param>
    /// <param name="flags"> The flags to check for. </param>
    /// <returns> True if each flag in <paramref name="flags"/> is set in <paramref name="value"/>. </returns>
    /// <remarks> If <paramref name="flags"/> is 0, this always returns true. </remarks>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static bool CheckAll<TEnum>(this TEnum value, TEnum flags)
        where TEnum : unmanaged, Enum
        => EqualityComparer<TEnum>.Default.Equals(value.And(flags), flags);

    /// <summary> Check whether none of the given flags are set. </summary>
    /// <typeparam name="TEnum"> The type of the enum, which should consist of flags. </typeparam>
    /// <param name="value"> The value to check against the flags. </param>
    /// <param name="flags"> The flags to check for. </param>
    /// <returns> True if not a single flag in <paramref name="flags"/> is set in <paramref name="value"/>. </returns>
    /// <remarks> If <paramref name="flags"/> is 0, this always returns true. </remarks>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static bool CheckNone<TEnum>(this TEnum value, TEnum flags)
        where TEnum : unmanaged, Enum
        => value.And(flags) is 0;

    /// <summary> Get the aggregate bit-wise OR of all passed values. </summary>
    /// <typeparam name="TEnum"> The type of the enum. </typeparam>
    /// <param name="values"> The separate enum values to OR up. </param>
    /// <returns> The aggregate value. </returns>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static TEnum Or<TEnum>(this IEnumerable<TEnum> values)
        where TEnum : unmanaged, Enum
        => values.Aggregate(default(TEnum), Or);

    /// <summary> Return the bit-wise OR of two generic enum values. </summary>
    /// <typeparam name="TEnum"> The type of the enum. </typeparam>
    /// <param name="lhs"> The left-hand value. </param>
    /// <param name="rhs"> The right-hand value. </param>
    /// <returns> The bit-wise OR of both values. </returns>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static unsafe TEnum Or<TEnum>(this TEnum lhs, TEnum rhs) where TEnum : unmanaged, Enum
        => sizeof(TEnum) switch
        {
            1 => ConvertOr<TEnum, byte>(lhs, rhs),
            2 => ConvertOr<TEnum, ushort>(lhs, rhs),
            4 => ConvertOr<TEnum, uint>(lhs, rhs),
            8 => ConvertOr<TEnum, ulong>(lhs, rhs),
            _ => throw new BitwiseEnumException<TEnum>(),
        };

    /// <summary> Return the bit-wise AND of two generic enum values. </summary>
    /// <typeparam name="TEnum"> The type of the enum. </typeparam>
    /// <param name="lhs"> The left-hand value. </param>
    /// <param name="rhs"> The right-hand value. </param>
    /// <returns> The bit-wise AND of both values. </returns>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static unsafe TEnum And<TEnum>(this TEnum lhs, TEnum rhs) where TEnum : unmanaged, Enum
        => sizeof(TEnum) switch
        {
            1 => ConvertAnd<TEnum, byte>(lhs, rhs),
            2 => ConvertAnd<TEnum, ushort>(lhs, rhs),
            4 => ConvertAnd<TEnum, uint>(lhs, rhs),
            8 => ConvertAnd<TEnum, ulong>(lhs, rhs),
            _ => throw new BitwiseEnumException<TEnum>(),
        };

    /// <summary> Return the bit-wise AND of two generic enum values, with the right-hand value bit-wise inverted. </summary>
    /// <typeparam name="TEnum"> The type of the enum. </typeparam>
    /// <param name="lhs"> The left-hand value. </param>
    /// <param name="rhs"> The right-hand value. </param>
    /// <returns> The bit-wise AND of <paramref cref="lhs"/> and the bit-wise inverse of <paramref cref="rhs"/>. </returns>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static unsafe TEnum AndNot<TEnum>(this TEnum lhs, TEnum rhs) where TEnum : unmanaged, Enum
        => sizeof(TEnum) switch
        {
            1 => ConvertAndNot<TEnum, byte>(lhs, rhs),
            2 => ConvertAndNot<TEnum, ushort>(lhs, rhs),
            4 => ConvertAndNot<TEnum, uint>(lhs, rhs),
            8 => ConvertAndNot<TEnum, ulong>(lhs, rhs),
            _ => throw new BitwiseEnumException<TEnum>(),
        };

    /// <summary> Return the bit-wise NOT of a generic enum value. </summary>
    /// <typeparam name="TEnum"> The type of the enum. </typeparam>
    /// <param name="lhs"> The value. </param>
    /// <returns> The bit-wise inverse of the value. </returns>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static unsafe TEnum Not<TEnum>(this TEnum lhs) where TEnum : unmanaged, Enum
        => sizeof(TEnum) switch
        {
            1 => ConvertNot<TEnum, byte>(lhs),
            2 => ConvertNot<TEnum, ushort>(lhs),
            4 => ConvertNot<TEnum, uint>(lhs),
            8 => ConvertNot<TEnum, ulong>(lhs),
            _ => throw new BitwiseEnumException<TEnum>(),
        };

    /// <summary> Return the bit-wise XOR of two generic enum values. </summary>
    /// <typeparam name="TEnum"> The type of the enum. </typeparam>
    /// <param name="lhs"> The left-hand value. </param>
    /// <param name="rhs"> The right-hand value. </param>
    /// <returns> The bit-wise XOR of both values. </returns>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static unsafe TEnum Xor<TEnum>(this TEnum lhs, TEnum rhs) where TEnum : unmanaged, Enum
        => sizeof(TEnum) switch
        {
            1 => ConvertXor<TEnum, byte>(lhs, rhs),
            2 => ConvertXor<TEnum, ushort>(lhs, rhs),
            4 => ConvertXor<TEnum, uint>(lhs, rhs),
            8 => ConvertXor<TEnum, ulong>(lhs, rhs),
            _ => throw new BitwiseEnumException<TEnum>(),
        };

    [MethodImpl(ImSharpConfiguration.OptInl)]
    private static unsafe TEnum ConvertOr<TEnum, TInteger>(TEnum lhs, TEnum rhs)
        where TEnum : unmanaged, Enum
        where TInteger : unmanaged, IBitwiseOperators<TInteger, TInteger, TInteger>
    {
        var ret = *(TInteger*)&lhs | *(TInteger*)&rhs;
        return *(TEnum*)&ret;
    }

    [MethodImpl(ImSharpConfiguration.OptInl)]
    private static unsafe TEnum ConvertAnd<TEnum, TInteger>(TEnum lhs, TEnum rhs)
        where TEnum : unmanaged, Enum
        where TInteger : unmanaged, IBitwiseOperators<TInteger, TInteger, TInteger>
    {
        var ret = *(TInteger*)&lhs & *(TInteger*)&rhs;
        return *(TEnum*)&ret;
    }

    [MethodImpl(ImSharpConfiguration.OptInl)]
    private static unsafe TEnum ConvertNot<TEnum, TInteger>(TEnum lhs)
        where TEnum : unmanaged, Enum
        where TInteger : unmanaged, IBitwiseOperators<TInteger, TInteger, TInteger>
    {
        var ret = ~*(TInteger*)&lhs;
        return *(TEnum*)&ret;
    }

    [MethodImpl(ImSharpConfiguration.OptInl)]
    private static unsafe TEnum ConvertAndNot<TEnum, TInteger>(TEnum lhs, TEnum rhs)
        where TEnum : unmanaged, Enum
        where TInteger : unmanaged, IBitwiseOperators<TInteger, TInteger, TInteger>
    {
        var ret = *(TInteger*)&lhs & ~*(TInteger*)&rhs;
        return *(TEnum*)&ret;
    }

    [MethodImpl(ImSharpConfiguration.OptInl)]
    private static unsafe TEnum ConvertXor<TEnum, TInteger>(TEnum lhs, TEnum rhs)
        where TEnum : unmanaged, Enum
        where TInteger : unmanaged, IBitwiseOperators<TInteger, TInteger, TInteger>
    {
        var ret = *(TInteger*)&lhs ^ *(TInteger*)&rhs;
        return *(TEnum*)&ret;
    }

    /// <summary> Exception thrown when a bitwise operator on a generic enum fails. </summary>
    /// <typeparam name="TEnum"> The type of Enum, used for name and size. </typeparam>
    /// <param name="name"> The name of the method, automatically supplied. </param>
    private sealed unsafe class BitwiseEnumException<TEnum>([CallerMemberName] string? name = null)
        : InvalidOperationException(
            $"Unable to use bit-wise enum extension {name} on {typeof(TEnum).Name} since its size is {sizeof(TEnum)}. Must be 1, 2, 4, or 8.")
        where TEnum : unmanaged, Enum;

    extension<T>(T) where T : unmanaged, Enum
    {
        /// <summary> Get all values of an enumeration more efficiently than with <see cref="Enum.GetValues"/>. </summary>
        /// <remarks> If used in a generic context, you can invoke <see cref="ImSharp.EnumExtensions.get_Values{T}"/> instead. </remarks>
        public static IReadOnlyList<T> Values
            => Values<T>.Data;
    }

    /// <summary> The static container for the data is only initialized when used. </summary>
    private static class Values<T> where T : unmanaged, Enum
    {
#pragma warning disable
        public static readonly IReadOnlyList<T> Data = Enum.GetValues<T>();
#pragma warning restore
    }
}
