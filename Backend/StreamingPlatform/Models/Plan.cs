﻿using StreamingPlatform.Models.Enums;

namespace StreamingPlatform.Models
{
    /// <summary>
    /// Represents a Plan.
    /// </summary>
    public sealed class Plan
    {
        /// <summary>
        /// Represents an unique identifier for the Plan.
        /// </summary>
        public Guid PlanId { get; set; }

        /// <summary>
        /// Represents the monthly fee of the Plan.
        /// </summary>
        public double MontlyFee { get; set; }

        /// <summary>
        /// Represents the number of minutes of the Plan.
        /// </summary>
        public int NumberOfMinutes { get; set; }

        /// <summary>
        /// Represents the status of the Plan. Can be Active or Inactive.
        /// </summary>
        public PlanStatus Status { get; set; }
    }
}