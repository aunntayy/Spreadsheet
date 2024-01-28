/// <summary>
/// Author: Joe Zachary
/// Date:   Long Time Ago in a Galaxy Far Far Away
///
/// Updated: H. James de St. Germain 
/// Date:    Spring 2022
/// 
/// Rational Number Mathematics
/// </summary>
namespace RationalNumbers
{
    /// <summary>
    /// <para>
    ///  Provides rational numbers that can be expressed as ratios of integers.
    /// </para>
    /// 
    /// <para>
    ///  Numerator, Denominator (Num / Den)
    /// </para>
    /// 
    /// Representation invariant:
    /// <list type="number">
    ///   <item>
    ///      den > 0
    ///   </item>
    ///   <item>
    ///    gcd(|num|, den) = 1
    ///    <para>in other words, the rational number is always stored simplified</para>
    ///   </item>
    /// </list>
    /// </summary>
    public class Rational
    {
        
     
        /// <summary>
        ///   The Numerator of the fraction stored in reduced form
        /// </summary>
        private int num;

        /// <summary>
        ///   The Denominator of the faction store din reduced form.
        ///   CANNOT be zero.
        /// </summary>
        private int den;

        /// <summary>
        /// Creates rational with default value 0
        /// </summary>
        public Rational()
            : this(0, 1)      // This invokes the 2-argument constructor
        {
        }

        /// <summary>
        ///   Creates a rational number for the given integer, really: N/1
        /// </summary>
        /// <param name="n">value of integer</param>
        public Rational(int n)
            : this(n, 1)      // This invokes the 2-argument constructor
        {
        }

        /// <summary>
        ///   Creates a rational number N / D 
        /// </summary>
        /// <param name="n">numerator</param>
        /// <param name="d">denominator - cannot be zero</param>
        /// <exception cref="ArgumentException"> invalid denominator </exception>
        public Rational(int n, int d)
        {
            if (d == 0)
            {
                throw new ArgumentException("Zero denominator not allowed");
            }

            int g = n.GCD(d);  // Note the use of the extension method Gcd below.  It works because the of the extension class below.

            if (d > 0)
            {
                num = n / g;
                den = d / g;
            }
            else
            {
                num = -n / g;
                den = -d / g;
            }
        }

        /// <summary>
        ///   <para>
        ///     Summation (Addition) of rationals.
        ///   </para>
        ///   <para>
        ///     This method overloads the + operator so that we can write expressions adding two Rationals 
        ///     together as in r1 + r2.
        ///   </para>
        ///   <para>
        ///     Also note the use of the "checked" block. This causes a runtime exception if integer arithmetic
        ///     overflows within that block (rather than just letting the overflow happen).
        ///   </para>
        /// </summary>
        /// <param name="r1"> First rational number</param>
        /// <param name="r2"> Second rational number</param>
        /// <returns>a new rational number that is the sum of r1 and r2</returns>
        public static Rational operator +(Rational r1, Rational r2)
        {
            checked
            {
                return new Rational(r1.num * r2.den + r1.den * r2.num,
                                     r1.den * r2.den);
            }
        }

        /// <summary>
        ///   Returns a standard string representation of a rational number
        ///   <para>
        ///     Note the use of the override keyword, required if you want to override an inherited method.
        ///     In this case, we are overriding Object's ToString.
        ///   </para>
        ///   
        ///   <returns>
        ///     Returns a standard string representation of a rational number
        ///   </returns>
        /// </summary
        public override string ToString()
        {
            if (den == 1)
            {
                return num.ToString();
            }
            else
            {
                return num + "/" + den;
            }
        }
        /// <summary>
        ///   Reports whether this and o are the same rational number.
        /// </summary>
        /// <param name="o"></param>
        /// <returns>true if o contains a rational that has the same values as this</returns>
        public override bool Equals(object? o)
        {
            // is - makes sure o is a Rational object and renames it r
            return o is Rational r &&
                    this.num == r.num &&
                    this.den == r.den;
        }

        /// <summary>
        ///   Compare if two rationals are the same.  
        ///   
        ///   (WARING: The normal == operator is REFERENCE equality. 
        ///    Here we overload the equality operator for VALUE equality.)
        ///   
        ///   Uses the equals method.
        /// </summary>
        /// <param name="r1">Rational on left side of equation</param>
        /// <param name="r2">Rational on right side of equation</param>
        /// <returns>true if the reduced for</returns>
        public static bool operator ==(Rational r1, Rational r2)
        {
            return r1.Equals(r2);
        }

        /// <summary>
        /// Overload the inequality operator
        /// </summary>
        public static bool operator !=(Rational r1, Rational r2)
        {
            return !(r1 == r2);
        }

        /// <summary>
        ///   Compute a (somewhat) unique number based on the rational number.
        ///   Here we use bitwise or
        /// </summary>
        /// <returns> the hash value for this item </returns>
        public override int GetHashCode()
        {
            return num ^ den;
        }

        ///public static void InfiniteRecursion()
        /// <summary>
        /// public static void InfiniteRecursion()
        /// </summary>       }
    }


    /// <summary>
    /// <para>
    ///   This class contains extension methods that make the Rational class
    ///   easier to write.  In particular is the GCD function.
    /// </para>
    /// <para>
    ///   Remember, extension methods allow us, the "outside" developer, to add
    ///   functionality to an object/type that we don't have control over, in a way
    ///   that make it look like it is built  in.
    /// </para>
    /// </summary>
    public static class RationalExtensions
    {
        /// <summary>
        ///   Find the Greate Common Denominator (GCD) between two integers.  Do this
        ///   with the object notation 5.GCD(10).  A GCD is the largest number that
        ///   divides evenly into both.  In this example case, the answer is 5.
        ///   <list type="bullet">
        ///     <item> 10.GCD(15) == 5</item>
        ///     <item> 8.GCD(28) == 4 </item>
        ///   </list>
        /// </summary>
        /// <param name="a">integer on the left side of the object notation</param>
        /// <param name="b">integer parameter to the GCD operation</param>
        /// <returns> The greatest common denominator of the two integers.</returns>
        public static int GCD(this int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            while (b > 0)
            {
                int temp = a % b;
                a = b;
                b = temp;
            }
            return a;
        }

        public static void InfiniteRecursion()
        {
            InfiniteRecursion();
        }



    }

}
