using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uassociit
{

    //public class Program
    //{
    //    public static void Main(string[] args)
    //    {
    //        int[] permutations;

    //        var number = 1234;
    //        permutations = number.GetPermutations();

    //        var numbers = new[] { 123, 543, 873, 343, 321, 723, 573, 863, 341, 213 };
    //        permutations = numbers.FindPermutations();
    //    }
    //}
    //https://social.msdn.microsoft.com/Forums/en-US/0bde0773-df32-4cb6-809d-2d3406993ea1/get-permutations-from-array?forum=csharplanguage

    internal static class PermutationExtensions
    {
        public static int[] GetPermutations(this int self)
        {
            var digits = self.ToString().Select(digit => digit.ToString());
            int count = digits.Count();
            var permutations = digits;
            while (0 < --count)
            {
                permutations = permutations.Join(digits, permutation => true, digit => true, (permutation, digit) => permutation.Contains(digit) ? null : string.Concat(permutation, digit)).Where(permutation => permutation != null);
            }
            return permutations.Select(permutation => Convert.ToInt32(permutation)).OrderBy(permutation => permutation).ToArray();
        }

        public static int[] FindPermutations(this int[] self)
        {
            if (self == null)
            {
                return null;
            }

            return self.SelectMany(number =>
            {
                var permutations = self.Intersect(number.GetPermutations());
                if (permutations.Count() > 1)
                {
                    return permutations;
                }
                return Enumerable.Empty<int>();
            }).Distinct().ToArray();
        }
    }





}
