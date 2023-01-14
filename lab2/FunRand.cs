using MathNet.Numerics.Distributions;

namespace lab2
{
    public class FunRand
    {
        /// <summary>
        /// Generates a random value according to an exponential distribution
        /// </summary>
        public static double Exp(double timeMean)
        {
            double a = 0;
            while (a == 0)
            {
                a = new Random().NextSingle();
            }
            a = -timeMean * Math.Log(a);
            return a;
        }

        /// <summary>
        /// Generates a random value according to a uniform distribution
        /// </summary>
        public static double Unif(double timeMin, double timeMax)
        {
            double a = 0;
            while (a == 0)
            {
                a = new Random().NextSingle();
            }
            a = timeMin + a * (timeMax - timeMin);
            return a;
        }

        public static double Norm(double timeMean, double
            timeDeviation)
        {
            return timeMean + timeDeviation * new Normal().Sample();
        }
    }
}
