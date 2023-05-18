#if NET6_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif

namespace Ao.Cache
{
    public class IdentityGenerater<TIdentity, TEntity> : IIdentityGenerater<TIdentity>
    {
        internal string head = FriendlyNameHelper<TEntity>.FriendlyName;
        internal string headWithPoint = FriendlyNameHelper<TEntity>.FriendlyName + ".";
        public string Head
        {
            get => head;
            set 
            {
                head = value;
                headWithPoint= value+".";
            }
        }

        public virtual string GetPart(TIdentity identity)
        {
            return identity?.ToString();
        }

        public virtual string GetHead()
        {
            return Head;
        }
        public string GetHeadWithPoint()
        {
            return headWithPoint;
        }

        public virtual string GetEntryKey(TIdentity identity)
        {
            return headWithPoint + GetPart(identity);
        }
    }

}
