public class SharedVar
{
    public object value;

    public object Get() => value;
    public void Set(object val) => value = val;

    public SharedVar Clone() => new SharedVar { value = this.value };

    public override string ToString() => value?.ToString() ?? "null";
}
