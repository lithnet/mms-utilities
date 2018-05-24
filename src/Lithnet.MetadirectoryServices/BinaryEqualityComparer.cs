using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.MetadirectoryServices
{
    public class BinaryEqualityComparer : IEqualityComparer<byte[]>
    {
        private static BinaryEqualityComparer defaultComparer;

        public static BinaryEqualityComparer Default
        {
            get
            {
                if (defaultComparer == null)
                {
                    defaultComparer = new BinaryEqualityComparer();
                }

                return defaultComparer;
            }
        }

        public bool Equals(byte[] x, byte[] y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.SequenceEqual(y);
        }

        public int GetHashCode(byte[] obj)
        {
            unchecked
            {
                int hash = 0;

                foreach (byte item in obj)
                {
                    hash = (31 * hash) + item.GetHashCode();
                }

                return hash;
            }
        }
    }
}
