namespace Ao.Cache
{
    public class OptionalDataFinder<TIdentity, TEntity>
    {
        public OptionalDataFinder()
        {
            Options = DefaultDataFinderOptions<TIdentity, TEntity>.Default;
        }

        private IDataFinderOptions<TIdentity, TEntity> options;

        public IDataFinderOptions<TIdentity, TEntity> Options
        {
            get => options;
            set => options = value ?? DefaultDataFinderOptions<TIdentity, TEntity>.Default;
        }
    }

}
