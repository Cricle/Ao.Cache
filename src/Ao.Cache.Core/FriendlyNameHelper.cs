namespace Ao.Cache
{
    public static class FriendlyNameHelper<T>
    {
        public static readonly string FriendlyName = TypeNameHelper.GetFriendlyFullName(typeof(T));
    }

}
