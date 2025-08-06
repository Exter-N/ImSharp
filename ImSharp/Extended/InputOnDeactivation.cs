namespace ImSharp;

public static partial class ImEx
{
    /// <summary> A wrapper class containing input functions that only return on deactivation. </summary>
    public static class InputOnDeactivation
    {
        /// <summary> A text input that only returns true when the item was deactivated this frame and edited, not on every edit. </summary>
        /// <param name="label"> The label as text. If this is a UTF8 string, it HAS to be null-terminated. </param>
        /// <param name="input"> The input string as text. Does not have to be null-terminated. </param>
        /// <param name="newLength"> The new length of the resulting string that is stored within <seealso cref="InputStringHandlerBuffer"/> while this is active up until another text input activates. </param>
        /// <param name="hint"> An optional hint to display in the input box while it is empty. </param>
        /// <param name="flags"> Additional flags controlling the input behavior. </param>
        /// <returns> True when the control has been active, edited and was deactivated this frame. False otherwise. </returns>
        /// <remarks> Changing <paramref name="input"/> across frames while this is active will not have an effect. </remarks>
        public static bool Text(Utf8LabelHandler label, Utf8TextHandler input, out int newLength, Utf8HintHandler hint = default,
            InputTextFlags flags = InputTextFlags.None)
            => Text(ref label, ref input, out newLength, ref hint, flags);

        /// <summary> A text input that only returns true when the item was deactivated this frame and edited, not on every edit. </summary>
        /// <param name="label"> The label as text. If this is a UTF8 string, it HAS to be null-terminated. </param>
        /// <param name="input"> The input string as text. Does not have to be null-terminated. </param>
        /// <param name="output"> When returning true, the resulting string output encoded in UTF8, otherwise undefined.  </param>
        /// <param name="hint"> An optional hint to display in the input box while it is empty. </param>
        /// <param name="flags"> Additional flags controlling the input behavior. </param>
        /// <returns> True when the control has been active, edited and was deactivated this frame. False otherwise. </returns>
        /// <remarks> Changing <paramref name="input"/> across frames while this is active will not have an effect. </remarks>
        public static bool Text(Utf8LabelHandler label, Utf8TextHandler input, out StringU8 output, Utf8HintHandler hint = default,
            InputTextFlags flags = InputTextFlags.None)
        {
            if (Text(ref label, ref input, out var length, ref hint, flags))
            {
                output = length is 0 ? StringU8.Empty : new StringU8(InputStringHandlerBuffer.Span[..length]);
                return true;
            }

            output = StringU8.Empty;
            return false;
        }

        /// <summary> A text input that only returns true when the item was deactivated this frame and edited, not on every edit. </summary>
        /// <param name="label"> The label as text. If this is a UTF8 string, it HAS to be null-terminated. </param>
        /// <param name="input"> The input string as text. Does not have to be null-terminated. </param>
        /// <param name="output"> When returning true, the resulting string output encoded in UTF16, otherwise undefined.  </param>
        /// <param name="hint"> An optional hint to display in the input box while it is empty. </param>
        /// <param name="flags"> Additional flags controlling the input behavior. </param>
        /// <returns> True when the control has been active, edited and was deactivated this frame. False otherwise. </returns>
        /// <remarks> Changing <paramref name="input"/> across frames while this is active will not have an effect. </remarks>
        public static bool Text(Utf8LabelHandler label, Utf8TextHandler input, out string output, Utf8HintHandler hint = default,
            InputTextFlags flags = InputTextFlags.None)
        {
            if (Text(ref label, ref input, out var length, ref hint, flags))
            {
                output = length is 0 ? string.Empty : Encoding.UTF8.GetString(InputStringHandlerBuffer.Span[..length]);
                return true;
            }

            output = string.Empty;
            return false;
        }

        public static unsafe bool Scalar<T>(Utf8LabelHandler label, ref T value, Utf8HintHandler format, T step = default, T stepFast = default,
            InputTextFlags flags = InputTextFlags.None) where T : unmanaged, INumber<T>
        {
            flags &= ~InputTextFlags.EnterReturnsTrue;
            var id = Im.Id.Get(ref label);
            if (id.Active)
            {
                var pointer = (T*)Unsafe.AsPointer(ref Storage<T>.Data);
                Im.Native.Methods.Inputs.InputScalar(label.Start(), DataTypeExtensions.From<T>(), pointer, step.Equals(default) ? null : &step,
                    &stepFast, format.Start(), flags);
            }
            else
            {
                var tmpValue = value;
                if (Im.Native.Methods.Inputs.InputScalar(label.Start(), DataTypeExtensions.From<T>(), &tmpValue,
                        step.Equals(default) ? null : &step,
                        &stepFast, format.Start(), flags)
                 || Im.Item.Activated)
                    Storage<T>.Data = tmpValue;
            }

            return Im.Item.DeactivatedAfterEdit;
        }

        public static unsafe bool Scalar<T>(Utf8LabelHandler label, ref T value, T step = default, T stepFast = default,
            InputTextFlags flags = InputTextFlags.None) where T : unmanaged, INumber<T>
        {
            flags &= ~InputTextFlags.EnterReturnsTrue;
            var id = Im.Id.Get(ref label);
            if (id.Active)
            {
                var pointer = (T*)Unsafe.AsPointer(ref Storage<T>.Data);
                Im.Native.Methods.Inputs.InputScalar(label.Start(), DataTypeExtensions.From<T>(), pointer, step.Equals(default) ? null : &step,
                    &stepFast, Im.Input.DefaultInputFormat<T>().Start(), flags);
            }
            else
            {
                var tmpValue = value;
                if (Im.Native.Methods.Inputs.InputScalar(label.Start(), DataTypeExtensions.From<T>(), &tmpValue,
                        step.Equals(default) ? null : &step,
                        &stepFast, Im.Input.DefaultInputFormat<T>().Start(), flags)
                 || Im.Item.Activated)
                    Storage<T>.Data = tmpValue;
            }

            return Im.Item.DeactivatedAfterEdit;
        }

        public static unsafe bool Drag<T>(Utf8LabelHandler label, ref T value, Utf8HintHandler format) where T : unmanaged, INumber<T>
        //flags &= ~InputTextFlags.EnterReturnsTrue;
        //var id = Im.Id.Get(label);
        //if (Im.Id.Active == id)
        //{
        //    var pointer = (T*)Unsafe.AsPointer(ref Storage<T>.Data);
        //    Native.Methods.DragSliders.DragScalar(label.Start(), DataTypeExtensions.From<T>(), pointer, step.Equals(default) ? null : &step,
        //        &stepFast, format.Start(), flags);
        //}
        //else
        //{
        //    var tmpValue = value;
        //    if (Native.Methods.Inputs.InputScalar(label.Start(), DataTypeExtensions.From<T>(), &tmpValue,
        //            step.Equals(default) ? null : &step,
        //            &stepFast, format.Start(), flags)
        //     || Im.Item.Activated)
        //        Storage<T>.Data = tmpValue;
        //}
        //
            => Im.Item.DeactivatedAfterEdit;

        [MethodImpl(ImSharpConfiguration.Inl)]
        private static unsafe bool Text(ref Utf8LabelHandler label, ref Utf8TextHandler input, out int newLength, ref Utf8HintHandler hint,
            InputTextFlags flags)
        {
            var id     = Im.Id.Get(ref label);
            var buffer = InputStringHandlerBuffer.Buffer;
            var size   = InputStringHandlerBuffer.Size;
            if (!id.Active)
            {
                buffer = TextStringHandlerBuffer.Buffer;
                size   = TextStringHandlerBuffer.Size;
                var begin = input.Start(out var end);
                if (begin != TextStringHandlerBuffer.Buffer)
                    new ReadOnlySpan<byte>(begin, (int)(end - begin)).CopyTo(TextStringHandlerBuffer.Span);
            }

            var length = 0ul;
            flags &= ~InputTextFlags.EnterReturnsTrue;
            if (Im.Input.Text(label.Start(), buffer, (uint)size, hint.Start(), flags, &length) || Im.Item.Activated)
                if (buffer != InputStringHandlerBuffer.Buffer)
                {
                    TextStringHandlerBuffer.Span[..(int)length].CopyTo(InputStringHandlerBuffer.Span);
                    InputStringHandlerBuffer.Buffer[length] = 0;
                }

            newLength = (int)length;
            return Im.Item.DeactivatedAfterEdit;
        }

        private static class Storage<T> where T : unmanaged, INumber<T>
        {
            public static T Data;
        }
    }
}
