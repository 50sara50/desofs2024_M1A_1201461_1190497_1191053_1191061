using StreamingPlatform.Configurations.Models;

namespace StreamingPlatform.Configurations.Mapper
{
    /// <summary>
    /// Class to map the rate limiter configuration settings to the corresponding configuration objects.
    /// </summary>
    public static class RateLimiterConfigMapper
    {
        /// <summary>
        /// Maps the provided configuration section to a FixedWindowRateLimiterConfig object.
        /// </summary>
        /// <param name="configuration">The IConfiguration instance containing the configuration settings.</param>
        /// <returns>A FixedWindowRateLimiterConfig object populated with the mapped values.</returns>
        public static FixedWindowRateLimiterConfig MapToFixedWindowRateLimiterConfig(IConfiguration configuration)
        {
            int permitLimit = configuration.GetValue<int>("FixedWindowRateLimiterConfig:PermitLimit");
            double window = configuration.GetValue<double>("FixedWindowRateLimiterConfig:Window");
            return new FixedWindowRateLimiterConfig
            {
                PermitLimit = permitLimit,
                Window = window,
            };
        }

        /// <summary>
        /// Maps the provided configuration section to a TokenBucketRateLimiterConfig object.
        /// </summary>
        /// <param name="configuration">The IConfiguration instance containing the configuration settings.</param>
        /// <returns>A TokenBucketRateLimiterConfig object populated with the mapped values.</returns>
        public static TokenBucketRateLimiterConfig MapToTokenBucketRateLimiterConfig(IConfiguration configuration)
        {
            int tokenLimit = configuration.GetValue<int>("TokenBucketRateLimiterConfig:TokenLimit");
            bool autoReplenishment = configuration.GetValue<bool>("TokenBucketRateLimiterConfig:AutoReplenishment");
            int queueLimit = configuration.GetValue<int>("TokenBucketRateLimiterConfig:QueueLimit");
            int tokensPerPeriod = configuration.GetValue<int>("TokenBucketRateLimiterConfig:TokensPerPeriod");
            double replenishmentPeriod = configuration.GetValue<double>("TokenBucketRateLimiterConfig:ReplenishmentPeriod");
            return new TokenBucketRateLimiterConfig
            {
                TokenLimit = tokenLimit,
                AutoReplenishment = autoReplenishment,
                QueueLimit = queueLimit,
                TokensPerPeriod = tokensPerPeriod,
                ReplenishmentPeriod = replenishmentPeriod,
            };
        }
    }
}
