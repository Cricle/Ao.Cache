namespace Ao.Cache
{
    public class OptionalDataFinder<TIdentity, TEntity>: DefaultDataFinderOptions<TIdentity, TEntity>, IDataFinderOptions<TIdentity, TEntity>
    {
        public OptionalDataFinder()
        {
            Options = this;
        }

        internal IDataFinderOptions<TIdentity, TEntity> options;

        public IDataFinderOptions<TIdentity, TEntity> Options
        {
            get => options;
            set => options = value ?? this;
        }
    }

}
