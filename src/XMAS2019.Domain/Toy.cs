using System;
using System.Xml.Serialization;

namespace XMAS2019.Domain
{
    public class Toy : IEquatable<Toy>
    {
        [Obsolete("Serialization Only")]
        public Toy()
        {
        }

        public Toy(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            Name = name;
        }

        [XmlAttribute]
        public string Name { get; set; }

        public bool Equals(Toy other)
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
            return Equals((Toy) obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}