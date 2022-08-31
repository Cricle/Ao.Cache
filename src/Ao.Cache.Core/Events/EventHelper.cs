namespace Ao.Cache.Events
{
    public static class EventHelper
    {
        public static string PrefxKey { get; set; } = "Channel.";

        public static string GetChannelKey<T>(string defaultKey)
        {
            return defaultKey ?? (PrefxKey + FriendlyNameHelper<T>.FriendlyName);
        }

        public static string GetChannelKey<T>()
        {
            return GetChannelKey<T>(null);
        }
    }
}
