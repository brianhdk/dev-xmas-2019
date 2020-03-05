using System.Collections.Generic;
using System.Linq;

namespace XMAS2019.Domain
{
    public class ToyDistributionSolution
    {
        public Dictionary<string, string> List { get; set; }

        public bool IsValidFor(ToyDistributionProblem problem)
        {
            bool b1 = List.Count == ToyDistributionProblem.ListLength;
            bool b2 = List.Keys.Distinct().Count() == ToyDistributionProblem.ListLength;
            bool b3 = List.Values.Distinct().Count() == ToyDistributionProblem.ListLength;
            bool b4 = !problem.Toys.Except(List.Values.Select(y => new Toy(y))).Any();
            bool b5 = !List.Values.Select(y => new Toy(y)).Except(problem.Toys).Any();
            bool b6 = problem.Children.Aggregate(seed: true,
                (result, child) => result && 
                    List.ContainsKey(child.Name) &&
                    child.WishList.Toys.Contains(new Toy(List[child.Name])));

            return b1 && b2 && b3 && b4 && b5 && b6;
        }
    }
}