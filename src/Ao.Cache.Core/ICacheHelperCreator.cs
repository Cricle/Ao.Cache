namespace Ao.Cache
{
    public interface ICacheHelperCreator
    {
        ICacheHelper<TReturn> GetHelper<TReturn>();
    }

}
