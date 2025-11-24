namespace ImSharp;

/// <summary> Extensions for enum types. </summary>
public static class EnumExtensions
{
    /// <summary> Get the aggregate bit-wise OR of all passed values. </summary>
    /// <typeparam name="TEnum"> The type of the enum. </typeparam>
    /// <param name="values"> The separate enum values to OR up. </param>
    /// <returns> The aggregate value. </returns>
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
}
