namespace Ao.Cache.Events
{
    public static class EventHelper
    {
        public static string PrefxKey { get; set; } = "Channel";

        public static string JoinString { get; set; } = ".";

        public static string GetChannelKey<T>(string defaultKey, string joinString = null)
        {
            return (defaultKey ?? PrefxKey) + (joinString ?? JoinString) + FriendlyNameHelper<T>.FriendlyName;
        }

        public static string GetChannelKey<T>()
        {
            return GetChannelKey<T>(null);
        }
    }
}
