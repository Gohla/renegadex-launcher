public sealed class Ref<T>
{
    private T _value;

    public T Value { get { return _value; } set { _value = value; } }

    public Ref()
    {

    }

    public Ref(T value)
    {
        _value = value;
    }

    public static explicit operator T(Ref<T> @ref)
    {
        return @ref._value;
    }

    public static implicit operator Ref<T>(T value)
    {
        return new Ref<T>(value);
    }
}

public interface NullableRef
{
    bool HasValue { get; }
    object ValueObject { get; set; }
}

public sealed class NullableRef<T> : NullableRef
    where T : struct
{
    private Ref<T> _value;
    private bool _hasValue;

    public bool HasValue { get { return _hasValue; } set { _hasValue = value; if(!value) _value = null; } }
    public T Value { get { return _value.Value; } set { _value.Value = value; _hasValue = true; } }
    public object ValueObject { get { return Value; } set { Value = (T)value; } }

    public NullableRef()
    {
        _value = new Ref<T>();
        _hasValue = false;
    }

    public NullableRef(T value)
    {
        _value = new Ref<T>(value);
        _hasValue = true;
    }

    public static explicit operator T(NullableRef<T> @ref)
    {
        if(!@ref._hasValue)
            return default(T);
        return @ref.Value;
    }

    public static explicit operator Ref<T>(NullableRef<T> @ref)
    {
        return @ref._value;
    }

    public static implicit operator NullableRef<T>(T value)
    {
        return new NullableRef<T>(value);
    }
}