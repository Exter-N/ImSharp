namespace ImSharp;

public static partial class Im
{
    /// <summary> A wrapper around combined color and style pushing. </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class ColorStyleDisposable : IDisposable
    {
        /// <inheritdoc cref="ColorDisposable.Count"/>
        public int ColorCount { get; private set; }

        /// <inheritdoc cref="StyleDisposable.Count"/>
        public int StyleCount { get; private set; }

        /// <summary> Push a border color while also pushing the border thickness for the chosen type to be <see cref="ImGuiStyle.GlobalScale"/> if the color is not transparent and 0 otherwise. </summary>
        /// <param name="borderType"> The type of widget for which the border thickness should be pushed. </param>
        /// <param name="color"> The color to push. </param>
        /// <returns> A disposable object that can be used to push further colors and styles and pops those colors after leaving scope. Use with using. </returns>
        [OverloadResolutionPriority(50), MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable PushBorder(ImStyleBorder borderType, Rgba32 color)
        {
            Native.Methods.Stacks.PushStyleColor(ImGuiColor.Border, color);
            Native.Methods.Stacks.PushStyleVar((ImStyle)borderType, color.IsTransparent ? 0 : Style.GlobalScale);
            ++ColorCount;
            ++StyleCount;
            return this;
        }

        /// <summary> Push a border color while also pushing the border thickness for the chosen type to be <see cref="ImGuiStyle.GlobalScale"/> if the color is not transparent and 0 otherwise. </summary>
        /// <param name="borderType"> The type of widget for which the border thickness should be pushed. </param>
        /// <param name="color"> The color to push. If this is <see cref="ColorParameter.Default"/>, nothing is done. </param>
        /// <returns> A disposable object that can be used to push further colors and styles and pops those colors after leaving scope. Use with using. </returns>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable PushBorder(ImStyleBorder borderType, ColorParameter color)
        {
            if (color.IsDefault)
                return this;

            return PushBorder(borderType, color.Color!.Value);
        }

        /// <inheritdoc cref="PushBorder(ImStyleBorder,Rgba32)"/>
        [OverloadResolutionPriority(100), MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable PushBorder(ImStyleBorder borderType, Vector4 color)
        {
            Native.Methods.Stacks.PushStyleColor(ImGuiColor.Border, color);
            Native.Methods.Stacks.PushStyleVar((ImStyle)borderType, color.W is 0 ? 0 : Style.GlobalScale);
            ++ColorCount;
            ++StyleCount;
            return this;
        }

        /// <inheritdoc cref="PushBorder(ImStyleBorder,Rgba32,float,bool)"/>
        [OverloadResolutionPriority(50), MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable PushBorder(ImStyleBorder borderType, Rgba32 color, float thickness)
        {
            Native.Methods.Stacks.PushStyleColor(ImGuiColor.Border, color);
            Native.Methods.Stacks.PushStyleVar((ImStyle)borderType, thickness);
            ++ColorCount;
            ++StyleCount;
            return this;
        }

        /// <inheritdoc cref="PushBorder(ImStyleBorder,ColorParameter,float,bool)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable PushBorder(ImStyleBorder borderType, ColorParameter color, float thickness)
        {
            if (color.IsDefault)
            {
                Native.Methods.Stacks.PushStyleVar((ImStyle)borderType, thickness);
                ++StyleCount;
                return this;
            }

            return PushBorder(borderType, color.Color!.Value, thickness);
        }

        /// <inheritdoc cref="PushBorder(ImStyleBorder,Vector4,float,bool)"/>
        [OverloadResolutionPriority(100), MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable PushBorder(ImStyleBorder borderType, Vector4 color, float thickness)
        {
            Native.Methods.Stacks.PushStyleColor(ImGuiColor.Border, color);
            Native.Methods.Stacks.PushStyleVar((ImStyle)borderType, thickness);
            ++ColorCount;
            ++StyleCount;
            return this;
        }

        /// <summary> Push a border color while also pushing the border thickness for the chosen type. </summary>
        /// <param name="borderType"> The type of widget for which the border thickness should be pushed. </param>
        /// <param name="color"> The color to push. If this is <see cref="ColorParameter.Default"/>, nothing is done. </param>
        /// <param name="thickness"> The thickness to push. This is pushed even if <paramref name="color"/> is transparent. </param>
        /// <param name="condition"> A condition to push the style at all. If this is false, nothing is done. </param>
        /// <returns> A disposable object that can be used to push further colors and styles and pops those colors after leaving scope. Use with using. </returns>
        [OverloadResolutionPriority(50), MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable PushBorder(ImStyleBorder borderType, Rgba32 color, float thickness, bool condition)
            => condition ? PushBorder(borderType, color, thickness) : this;

        /// <summary> Push a border color while also pushing the border thickness for the chosen type. </summary>
        /// <param name="borderType"> The type of widget for which the border thickness should be pushed. </param>
        /// <param name="color"> The color to push. If this is <see cref="ColorParameter.Default"/>, nothing is done. </param>
        /// <param name="thickness"> The thickness to push. This is pushed even if <paramref name="color"/> is transparent or <see cref="ColorParameter.Default"/>. </param>
        /// <param name="condition"> A condition to push the style at all. If this is false, nothing is done. </param>
        /// <returns> A disposable object that can be used to push further colors and styles and pops those colors after leaving scope. Use with using. </returns>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable PushBorder(ImStyleBorder borderType, ColorParameter color, float thickness, bool condition)
            => condition ? PushBorder(borderType, color, thickness) : this;

        /// <inheritdoc cref="PushBorder(ImStyleBorder,Rgba32,float,bool)"/>
        [OverloadResolutionPriority(100), MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable PushBorder(ImStyleBorder borderType, Vector4 color, float thickness, bool condition)
            => condition ? PushBorder(borderType, color, thickness) : this;


        /// <inheritdoc cref="ColorDisposable.Push(ImGuiColor,Rgba32,bool)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable Push(ImGuiColor type, Rgba32 color, bool condition)
        {
            if (!condition)
                return this;

            Native.Methods.Stacks.PushStyleColor(type, color);
            ++ColorCount;
            return this;
        }

        /// <inheritdoc cref="ColorDisposable.Push(ImGuiColor,ColorParameter)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable Push(ImGuiColor type, ColorParameter color)
        {
            if (color.IsDefault)
                return this;

            Native.Methods.Stacks.PushStyleColor(type, color.Color!.Value);
            ++ColorCount;
            return this;
        }

        /// <inheritdoc cref="ColorDisposable.Push(ImGuiColor,Vector4,bool)"/>
        [MethodImpl(ImSharpConfiguration.OptInl), OverloadResolutionPriority(100)]
        public ColorStyleDisposable Push(ImGuiColor type, Vector4 color, bool condition)
        {
            if (!condition)
                return this;


            Native.Methods.Stacks.PushStyleColor(type, color);
            ++ColorCount;
            return this;
        }

        /// <inheritdoc cref="Push(ImGuiColor,Rgba32,bool)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable Push(ImGuiColor type, Rgba32 color)
        {
            Native.Methods.Stacks.PushStyleColor(type, color);
            ++ColorCount;
            return this;
        }

        /// <inheritdoc cref="Push(ImGuiColor,Vector4,bool)"/>
        [MethodImpl(ImSharpConfiguration.OptInl), OverloadResolutionPriority(100)]
        public ColorStyleDisposable Push(ImGuiColor type, Vector4 color)
        {
            Native.Methods.Stacks.PushStyleColor(type, color);
            ++ColorCount;
            return this;
        }

        /// <summary> Pop a number of colors. </summary>
        /// <param name="num"> The number of colors to pop. This is clamped to the number of colors pushed by this object. </param>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable PopColor(int num = 1)
        {
            num = Math.Min(num, ColorCount);
            Native.Methods.Stacks.PopStyleColor(num);
            ColorCount -= num;
            return this;
        }

        /// <inheritdoc cref="StyleDisposable.Push(ImStyleSingle,float,bool)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable Push(ImStyleSingle type, float value, bool condition)
        {
            if (!condition)
                return this;

            Native.Methods.Stacks.PushStyleVar((ImStyle)type, value);
            ++StyleCount;
            return this;
        }

        /// <inheritdoc cref="StyleDisposable.Push(ImStyleDouble,Vector2,bool)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable Push(ImStyleDouble type, Vector2 value, bool condition)
        {
            if (!condition)
                return this;

            Native.Methods.Stacks.PushStyleVar((ImStyle)type, value);
            ++StyleCount;
            return this;
        }

        /// <inheritdoc cref="StyleDisposable.Push(ImStyleSingle,float)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable Push(ImStyleSingle type, float value)
        {
            Native.Methods.Stacks.PushStyleVar((ImStyle)type, value);
            ++StyleCount;
            return this;
        }

        /// <inheritdoc cref="StyleDisposable.Push(ImStyleDouble,Vector2)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable Push(ImStyleDouble type, Vector2 value)
        {
            Native.Methods.Stacks.PushStyleVar((ImStyle)type, value);
            ++StyleCount;
            return this;
        }

        /// <inheritdoc cref="StyleDisposable.PushX(ImStyleDouble,float,bool)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable PushX(ImStyleDouble type, float value, bool condition)
        {
            if (!condition)
                return this;

            Native.Methods.Stacks.PushStyleVar((ImStyle)type, Style[type] with { X = value });
            ++StyleCount;
            return this;
        }

        /// <inheritdoc cref="StyleDisposable.PushY(ImStyleDouble,float,bool)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable PushY(ImStyleDouble type, float value, bool condition)
        {
            if (!condition)
                return this;

            Native.Methods.Stacks.PushStyleVar((ImStyle)type, Style[type] with { Y = value });
            ++StyleCount;
            return this;
        }

        /// <inheritdoc cref="StyleDisposable.PushX(ImStyleDouble,float)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable PushX(ImStyleDouble type, float value)
        {
            Native.Methods.Stacks.PushStyleVar((ImStyle)type, Style[type] with { X = value });
            ++StyleCount;
            return this;
        }

        /// <inheritdoc cref="StyleDisposable.PushY(ImStyleDouble,float)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable PushY(ImStyleDouble type, float value)
        {
            Native.Methods.Stacks.PushStyleVar((ImStyle)type, Style[type] with { Y = value });
            ++StyleCount;
            return this;
        }

        /// <summary> Pop a number of style variables. </summary>
        /// <param name="num"> The number of style variables to pop. This is clamped to the number of style variables pushed by this object. </param>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public ColorStyleDisposable PopStyle(int num = 1)
        {
            num = Math.Min(num, StyleCount);
            Native.Methods.Stacks.PopStyleVar(num);
            ColorCount -= num;
            return this;
        }

        /// <summary> Pop all pushed colors and styles. </summary>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public void Dispose()
        {
            Native.Methods.Stacks.PopStyleVar(StyleCount);
            Native.Methods.Stacks.PopStyleColor(ColorCount);
            StyleCount = 0;
            ColorCount = 0;
        }
    }
}
