using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XMAS2019.Domain
{
    public class WishList
    {
        [Obsolete("Serialization Only")]
        public WishList()
        {
        }

        public WishList(List<Toy> toys)
        {
            Toys = toys ?? throw new ArgumentNullException(nameof(toys));
        }

        [DataMember]
        public List<Toy> Toys { get; set; }
    }
}