using System;
using System.Globalization;
using CenterSpace.NMath.Core;


namespace Test;

class OneVariableCurveFittingExample
{
    /// <summary>
    /// The OneVariableFunctionFitter&lt;T&gt; Needs a parameterized function
    /// and a set of data points. One way to specify the parameterized function,
    /// and optionally its gradient with respect to the parameters, is to
    /// implement an instance of the abstract class DoubleParameterizedFunction.
    /// You must overwrite the Evaluate() method which computes and returns the
    /// parameterized function value at a specified set of parameters and 
    /// point. It is optional to overwrite the GradientWithRespectToParams() method.
    /// If you do not overwrite it, a numerical approximation using finite differences
    /// will be used to approximate the gradient if it is needed.
    /// 
    /// Here the parameterized function we are defining is a three parameter 
    /// exponential function given by the formula
    /// 
    /// p0 * exp(p1 * x) + p2
    /// 
    /// </summary>
    class ThreeParamExponential : DoubleParameterizedFunction
    {
        /// <summary>
        /// Override the abstract evaluate function.
        /// </summary>
        /// <param name="parameters">The parameter values.</param>
        /// <param name="x">The point to evaluate at.</param>
        /// <returns>The value of the parameterized function at the given
        /// point and parameters.</returns>
        public override double Evaluate(DoubleVector parameters, double x)
        {
            if (parameters.Length != 3)
            {
                throw new InvalidArgumentException("Incorrect number of function parameters to ThreeParameterExponential: " + parameters.Length);
            }
            return parameters[0] * Math.Exp(parameters[1] * x) + parameters[2];
        }

        /// <summary>
        /// Since the gradient of our function is rather easy to derive, we will
        /// override the GradientWithRespectToParams() function. Remember, this is
        /// the vector of partial derivatives with respect to the parameters, NOT the variables.
        /// </summary>
        /// <param name="parameters">Evaluate the gradient at these parameter values.</param>
        /// <param name="x">Evaluate the gradient at this point.</param>
        /// <param name="grad">Place the value of the gradient in this vector.</param>
        /// <remarks>Note how this function does not return the gradient as a new
        /// vector, but places the gradient value in a vector supplied by the 
        /// calling routine. This is for optimization purposes. The curve fitter uses 
        /// an optimization algorithm that will most likely be iterative, and thus may 
        /// need to evaluate the gradient many times. Having the vector 
        /// passed in to the routine allows the calling code to allocate space for the 
        /// gradient once and reuse it on successive calls, thus avoiding the potential 
        /// of allocating a large number of small objects on the managed heap.</remarks>
        public override void GradientWithRespectToParams(DoubleVector parameters, double x, ref DoubleVector grad)
        {
            grad[0] = Math.Exp(parameters[1] * x);
            grad[1] = parameters[0] * x * Math.Exp(parameters[1] * x);
            grad[2] = 1.0;
        }
    }

    /// <summary>
    /// A .NET example in C# showing how to fit a generalized multivariable function to a set 
    /// of points.
    /// </summary>
    /// <remarks>
    /// Uses the trust-region algorithm.
    /// </remarks>
    static void Main(string[] args)
    {
        string CultureName = Thread.CurrentThread.CurrentCulture.Name;
        CultureInfo ci = new CultureInfo(CultureName);
        if (ci.NumberFormat.NumberDecimalSeparator != ".")
        {
            // Forcing use of decimal separator for numerical values
            ci.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = ci;
        }
        // Class OneVariableFunctionFitter fits a parameterized function to a
        // set of points. In the space of the function parameters, beginning at a specified
        // starting point, the Fit() method finds a minimum (possibly local) in the sum of
        // the squared residuals with respect to the data. Fit() uses a nonlinear least
        // squares minimizer specified as a generic argument.

        var xValues = new DoubleVector("[-3 -2 -1 0 1 2 3]");
        var yValues = new DoubleVector("[1 1.2 1.8 2.8 6.6 14.6 40]");

        // Starting guess in the space of the function parameters.
        var start = new DoubleVector("[1 .6 .7]");

        // Construct a curve fitting object for our function, then perform the fit. We will use the
        // TrustRegionMinimizer implementation of the non-linear least squares minimizer to find the optimal
        // set of parameters. 
        var f = new ThreeParamExponential();
        var fitter = new OneVariableFunctionFitter<TrustRegionMinimizer>(f);
        DoubleVector solution = fitter.Fit(xValues, yValues, start);

        Console.WriteLine();

        // Display the results
        Console.WriteLine("Fit #1");
        Console.WriteLine("NMath solution: " + solution);
        Console.WriteLine("NMath residual: " + fitter.Minimizer.FinalResidual);
        Console.WriteLine();

        // The parameterized function used by the fitter may also be specified using a delegate.
        // Here we define a delegate for the same three parameter exponential function
        // p0*exp(p1*x) + p2
        Func<DoubleVector, double, double> fdelegate = delegate (DoubleVector p, double x)
        {
            if (p.Length != 3)
            {
                throw new InvalidArgumentException("Incorrect number of function parameters to ThreeParameterExponential: " + p.Length);
            }
            return p[0] * Math.Exp(p[1] * x) + p[2];
        };

        // The delegate for the parameterized function may be used directly in OneVariableFunctionFitter
        // constructors, or may be wrapped by the DoubleParameterizedDelegate, which implements 
        // DoubleParameterizedFunction. Here we do the latter.
        // Note that we do not supply the gradient with respect
        // to parameters. The gradient will be computed using a finite difference algorithm if
        // needed.
        fitter.Function = new DoubleParameterizedDelegate(fdelegate);

        // Perform the fit and display the results
        solution = fitter.Fit(xValues, yValues, start);
        Console.WriteLine("Fit #1 (Repeated without user specified Partial Derivatives)");
        Console.WriteLine("NMath solution: " + solution);
        Console.WriteLine("NMath residual: " + fitter.Minimizer.FinalResidual);
        Console.WriteLine();

        // Now lets perform the fit again using some random data. First we generate
        // 50 random x,y points in range (-4,4).
        xValues = new DoubleVector(50, new RandGenUniform(-4, 4));

        //// The target solution (parameter values).
        var target = new DoubleVector("2 1 1");

        // When calculating the y values, we add some noise, so the points
        // dont lie exactly on the target curve.
        yValues = new DoubleVector(50, new RandGenUniform(-.1, .1));
        for (int i = 0; i < yValues.Length; i++)
        {
            yValues[i] += fdelegate(target, xValues[i]);
        }

        // Perform the fit and display the results
        solution = fitter.Fit(xValues, yValues, start);
        Console.WriteLine("Fit #2");
        Console.WriteLine("Target solution: " + target);
        Console.WriteLine("Actual solution: " + solution);
        Console.WriteLine("Residual: " + fitter.Minimizer.FinalResidual);
        Console.WriteLine();

        Console.WriteLine();
        Console.WriteLine("Press Enter Key");
        Console.Read();

    }  // Main

}  // class
