namespace JackTheVideoRipper.extensions;

public static class ObjectExtensions
{
    public static T As<T>(this object obj)
    {
        var objType = obj.GetType();

        if (!objType.IsAssignableTo(typeof(T)) && 
            !objType.IsEquivalentTo(typeof(T)) && 
            !objType.IsInstanceOfType(typeof(T)))
        {
            throw new InvalidCastException();
        }

        return (T)obj;
    }
}