public class SharedVar
{
    public object value;

    public object Get() => value;
    public override string ToString() => value?.ToString() ?? "null";
}
