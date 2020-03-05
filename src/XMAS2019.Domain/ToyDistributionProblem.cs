using System;
using System.Collections.Generic;
using System.Linq;
using XMAS2019.Domain.Infrastructure;

namespace XMAS2019.Domain
{
    public class ToyDistributionProblem
    {
        static readonly string[] Names =
        {
            "Ida", "Emma", "Alma", "Ella", "Sofia", "Freja", "Josefine", "Clara", "Anna", "Karla", "Laura", "Alberte",
            "Olivia",
            "Agnes", "Nora", "Lærke", "Luna", "Isabella", "Frida", "Lily", "Victoria", "Aya", "Ellen", "Ellie", "Maja",
            "Mathilde",
            "Esther", "Mille", "Sofie", "Emily", "Astrid", "Liva", "Marie", "Caroline", "Rosa", "Emilie", "Sara",
            "Saga", "Liv",
            "Andrea", "Alba", "Asta", "Hannah", "Naja", "Vilma", "Johanne", "Lea", "Vigga", "Gry", "Eva", "Willam",
            "Noah", "Oscar",
            "Lucas", "Victor", "Malthe", "Oliver", "Alfred", "Carl", "Valdemar", "Emil", "Elias", "August", "Aksel",
            "Magnus",
            "Frederik", "Arthur", "Felix", "Anton", "Elliot", "Alexander", "Johan", "Theo", "Viggo", "Christian",
            "Hugo", "Adam",
            "Mikkel", "Villads", "Nohr", "Benjamin", "Lauge", "Liam", "Mads", "Theodor", "Otto", "Marius", "Albert",
            "Storm",
            "Louie", "Milas", "Mathias", "Conrad", "Sebastian", "Villum", "Pelle", "Anker", "Laurits", "Philip",
            "Asger", "Morten",
            "Henning", "Brian", "Michael", "Martin", "Marc", "Jesper", "Susanna", "Jette", "Malene", "Mette", "Pia",
            "Kim", "Katrine", "Jeppe", "Boris", "Sommer", "Egon", "Benny", "Keld", "Thea", "Yvonne", "Selma", "Paddy",
            "Bart", "Homer", "Marge", "Lisa", "Maggie", "Knud", "Margrete", "Svend", "Freddy", "Jimmy", "Johnny",
            "Mickey",
            "Tommy", "Willy", "Alice", "Connie", "Helen", "Jane", "Joan", "Arne", "Bo", "Bjarne", "Poul", "Stig",
            "Torsten",
            "Steen",
        };

        static readonly string[] ToyNames =
        {
            "Bobles Krokodille", "Scoot and Ride Highwaykick", "Lego Technic Bugatti Chiron", "Stigegolf",
            "Brio Lære-gå-vogn",
            "Lego Star Wars Millennium Falcon", "Dantoy Scooter", "Krea Sansegynge", "Lego Mindstorms EV3",
            "Træklodser fra BRIO",
            "Bilbaner", "Magformers", "Magicube", "Magic Play Sand", "Lego Duplo togbane", "Lego Friends", "Kuglebane",
            "Nerf gun",
            "Fidgetspinner", "Fodbold", "Håndbold", "Badminton ketcher", "Bordtennisbat", "Jellycat bunny bamse",
            "Gurli Gris LUDO", "Pikachu bamse", "Puslespil", "Dukkehus", "Legekøkken", "Barbie dukke",
            "Barbie bil", "Farveblyanter", "Skoletaske", "Penalhus", "Gurli Gris puslespil", "UNO", "Fortnite skins",
            "Trampolin", "Luvabella newborn dukke", "Lyssværd", "Spiderman kostume", "Gravitrax", "Bumper drone",
            "Playmobil piratskib", "Ternet ninja bamse", "Bordfodbold", "Et stykke kul",
        };

        public const int ListLength = 15;

        public Toy[] Toys { get; set; }
        public Child[] Children { get; set; }

        public static ToyDistributionProblem Create()
        {
            while (true)
            {
                Toy[] toys = ToyNames.Select(x => new Toy(x)).ToArray();

                Dictionary<string, Toy> toysDictionary = toys.ToDictionary(x => x.Name);

                Child[] children = Names.Shuffle()
                    .Take(ListLength)
                    .Select(x => new Child(x, GetRandomWishList(toysDictionary.Select(t => t.Value).ToArray())))
                    .ToArray();

                bool InternalCreate(out Child[] randomChildren1, out List<Toy> list)
                {
                    randomChildren1 = children;
                    InterestingToy[] byInterest = InterestingToys(randomChildren1);
                    if (byInterest.Take(5).Any(x => x.Counter == 1))
                    {
                        randomChildren1 = null;
                        list = null;
                        return false;
                    }

                    List<Toy> interestingToys = byInterest.SkipWhile(x => x.Counter > 2).Select(x => x.Toy).ToList();

                    list = new List<Toy>(ListLength);
                    foreach (Child c in randomChildren1.OrderBy(x => x.WishList.Toys.Count))
                    {
                        Toy first = c.WishList.Toys.FirstOrDefault(interestingToys.Contains);

                        if (first == null) 
                            return false;

                        interestingToys.Remove(first);
                        
                        list.Add(first);
                    }

                    return true;
                }

                if (InternalCreate(out Child[] randomChildren, out List<Toy> toyPool))
                {
                    var problem =  new ToyDistributionProblem
                    {
                        Children = randomChildren.Shuffle().ToArray(), 
                        Toys = toyPool.Shuffle().ToArray(),
                    };

                    ToyDistributionProblem originalProblem = problem.DeepClone();

                    ToyDistributionSolution solution = problem.CreateSolution();

                    if (solution != null && solution.IsValidFor(originalProblem))
                        return originalProblem;
                }

                // try again :D
            }
        }

        private static InterestingToy[] InterestingToys(Child[] children)
        {
            Dictionary<Toy, int> dictionary = children.SelectMany(x => x.WishList.Toys)
                .Aggregate(new Dictionary<Toy, int>(), (dic, toy) =>
                {
                    if (dic.ContainsKey(toy))
                        dic[toy]++;
                    else
                        dic.Add(toy, 1);

                    return dic;
                });

            return dictionary
                .OrderByDescending(x => x.Value)
                .Select(x => new InterestingToy(x.Key) { Counter = x.Value})
                .ToArray();
        }

        private static WishList GetRandomWishList(Toy[] toys)
        {
            var random = new Random();
            return new WishList(toys.Shuffle().Take(random.Next(2, 4)).ToList());
        }

        private class InterestingToy : IEquatable<InterestingToy>
        {
            public InterestingToy(Toy toy)
            {
                Toy = toy;
            }

            public Toy Toy { get; }
            public int Counter { get; set; }

            public bool Equals(InterestingToy other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(Toy, other.Toy);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((InterestingToy) obj);
            }

            public override int GetHashCode()
            {
                return (Toy != null ? Toy.GetHashCode() : 0);
            }
        }

        public ToyDistributionSolution CreateSolution()
        {
            var dictionary = new Dictionary<string, string>();

            List<Toy> problemToys = Toys.ToList();
            List<Child> problemChildren = Children.ToList();

            foreach (Child c in problemChildren)
            {
                // Remove all toy wishes not in Santas bag
                c.WishList.Toys.RemoveAll(x => !problemToys.Contains(x));
            }

            List<InterestingToy> interestingToys = InterestingToys(problemChildren.ToArray())
                .Where(x => problemToys.Contains(x.Toy))
                .Reverse()
                .ToList();

            while (problemChildren.Any())
            {
                Child[] children = problemChildren.Where(x => x.WishList.Toys.Count == 1).ToArray();

                if (!children.Any())
                    return null;

                foreach (Child child in children)
                {
                    Toy toy = child.WishList.Toys.First();

                    dictionary.Add(child.Name, toy.Name);
                    interestingToys.Remove(interestingToys.Find(x => x.Toy.Equals(toy)));
                    foreach (Child problemChild in problemChildren)
                    {
                        problemChild.WishList.Toys.Remove(toy);
                    }
                }

                foreach (Child child in children)
                {
                    problemChildren.Remove(child);
                }
            }

            return new ToyDistributionSolution {List = dictionary};
        }
    }
}