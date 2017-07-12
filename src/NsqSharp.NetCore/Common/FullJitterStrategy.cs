using System;
using System.Security.Cryptography;
using NsqSharp.Utils;
using NsqSharp.Utils.Extensions;

namespace NsqSharp
{
    /// <summary>
    /// FullJitterStrategy returns a random duration of time in the
    /// range [0, 2^(backoffLevel-1) * <see cref="IBackoffConfig.BackoffMultiplier"/>).
    /// Implements http://www.awsarchitectureblog.com/2015/03/backoff.html.
    /// </summary>
    public class FullJitterStrategy : IBackoffStrategy
    {
        private readonly Once rngOnce = new Once();
        private RandomNumberGenerator rng;

        /// <summary>
        /// Calculate returns a random duration of time in the
        /// range [0, 2^(backoffLevel-1) * <see cref="IBackoffConfig.BackoffMultiplier"/>).
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
            rngOnce.Do(() =>
                {
                    // lazily initialize the RNG
                    if (rng != null)
                        return;
                    rng = RandomNumberGenerator.Create();
                }
            );

            var backoffDuration = new TimeSpan(backoffConfig.BackoffMultiplier.Ticks *
                                               (long)Math.Pow(2, backoffLevel - 1));

            int maxBackoffMilliseconds = (int)backoffDuration.TotalMilliseconds;
            int backoffMilliseconds = maxBackoffMilliseconds == 0 ? 0 : rng.Intn(maxBackoffMilliseconds);

            return new BackoffCalculation
            {
                Duration = TimeSpan.FromMilliseconds(backoffMilliseconds),
                IncreaseBackoffLevel = backoffDuration < backoffConfig.MaxBackoffDuration
            };
        }
    }
}