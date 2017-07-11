using System;

namespace NsqSharp
{
    /// <summary>
    /// <see cref="ExponentialStrategy"/> implements an exponential backoff strategy (default).
    /// </summary>
    public class ExponentialStrategy : IBackoffStrategy
    {
        /// <summary>
        /// Calculate returns a duration of time: 2^(backoffLevel-1) * <see cref="IBackoffConfig.BackoffMultiplier"/>.
        /// </summary>
        /// <param name="backoffConfig">Read only configuration values related to backoff.</param>
        /// <param name="backoffLevel">
        ///     The backoff level (>= 1) used to calculate backoff duration.
        ///     <paramref name="backoffLevel"/> increases/decreases with successive failures/successes.
        /// </param>
        /// <returns>A <see cref="BackoffCalculation"/> object with the backoff duration and whether to increase
        ///          the backoff level.</returns>
        public BackoffCalculation Calculate(IBackoffConfig backoffConfig, int backoffLevel)
        {
            var backoffDuration = new TimeSpan(backoffConfig.BackoffMultiplier.Ticks *
                                               (long)Math.Pow(2, backoffLevel - 1));

            return new BackoffCalculation
            {
                Duration = backoffDuration,
                IncreaseBackoffLevel = backoffDuration < backoffConfig.MaxBackoffDuration
            };
        }
    }
}