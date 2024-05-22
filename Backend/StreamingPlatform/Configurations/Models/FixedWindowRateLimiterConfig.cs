namespace StreamingPlatform.Configurations.Models
{
    /// <summary>
    /// Class to represent the configuration for a fixed bucket rate limiter.
    /// </summary>
    public class FixedWindowRateLimiterConfig
    {
        /// <summary>
        /// The maximum number of permits that the window can hold.
        /// </summary>
        public int PermitLimit { get; set; }

        /// <summary>
        /// The time period after which the window should replenish permits in seconds.
        /// </summary>
        public double Window { get; set; }
    }
}
