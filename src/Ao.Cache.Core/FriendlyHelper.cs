namespace Ao.Cache
{
    public static class FriendlyHelper<T>
    {
        public static readonly string FriendlyName = TypeNameHelper.GetFriendlyFullName(typeof(T));
    }

}
