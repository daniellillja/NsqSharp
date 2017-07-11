namespace NsqSharp
{
    /// <summary>
    /// <see cref="IBackoffStrategy" /> defines a strategy for calculating the duration of time
    /// a consumer should backoff for a given attempt. See <see cref="ExponentialStrategy"/>
    /// and <see cref="FullJitterStrategy"/>.
    /// </summary>
    public interface IBackoffStrategy
    {
        /// <summary>Calculates the backoff time.</summary>
        /// <param name="backoffConfig">Read only configuration values related to backoff.</param>
        /// <param name="backoffLevel">
        ///     The backoff level (>= 1) used to calculate backoff duration.
        ///     <paramref name="backoffLevel"/> increases/decreases with successive failures/successes.
        /// </param>
        /// <returns>A <see cref="BackoffCalculation"/> object with the backoff duration and whether to increase
        ///          the backoff level.</returns>
        BackoffCalculation Calculate(IBackoffConfig backoffConfig, int backoffLevel);
    }
}