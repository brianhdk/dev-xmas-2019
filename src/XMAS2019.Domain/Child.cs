using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace XMAS2019.Domain
{
    public class Child : IEquatable<Child>
    {
        [Obsolete("Serialization Only")]
        public Child()
        {
        }

        public Child(string name, WishList wishList)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            Name = name;
            WishList = wishList ?? throw new ArgumentNullException(nameof(wishList));
        }

        [XmlAttribute]
        public string Name { get; set; }

        [DataMember]
        public WishList WishList { get; set; }

        public bool Equals(Child other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Child) obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();;
        }
    }
}