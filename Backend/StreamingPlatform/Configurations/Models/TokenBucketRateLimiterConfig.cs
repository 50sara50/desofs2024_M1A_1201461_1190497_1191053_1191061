namespace StreamingPlatform.Configurations.Models
{
    /// <summary>
    /// Class to represent the configuration for a token bucket rate limiter.
    /// </summary>
    public class TokenBucketRateLimiterConfig
    {
        /// <summary>
        /// The maximum number of tokens that the bucket can hold.
        /// </summary>
        public int TokenLimit { get; set; }

        /// <summary>
        /// Indicates whether the bucket should automatically replenish tokens.
        /// </summary>
        public bool AutoReplenishment { get; set; }

        /// <summary>
        /// The time period after which the bucket should replenish tokens in seconds.
        /// </summary>
        public double ReplenishmentPeriod { get; set; }

        /// <summary>
        /// The maximum number of tokens that the bucket can queue.
        /// </summary>
        public int QueueLimit { get; set; }

        /// <summary>
        /// The order in which the tokens in the queue should be processed.
        /// </summary>
        public int TokensPerPeriod { get; set; }
    }
}
