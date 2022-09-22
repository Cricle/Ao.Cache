using System;

namespace Ao.Cache.Proxy
{
    public readonly struct ActualTypeInfos
    {
        public readonly Type ActualType;

        public readonly Type FinderType;

        public readonly bool TypesEquals;

        public ActualTypeInfos(Type actualType, Type finderType, bool typesEquals)
        {
            ActualType = actualType;
            FinderType = finderType;
            TypesEquals = typesEquals;
        }
        public override bool Equals(object obj)
        {
            if (obj is ActualTypeInfos info)
            {
                return info.ActualType == ActualType &&
                    info.FinderType == FinderType &&
                    info.TypesEquals == TypesEquals;
            }
            return false;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var h = 31 * 17 + ActualType.GetHashCode();
                h = 31 * h + FinderType.GetHashCode();
                h = 31 * h + (TypesEquals ? 1 : 0);
                return h;
            }
        }
    }
}
