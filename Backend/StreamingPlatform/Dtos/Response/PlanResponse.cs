using StreamingPlatform.Models.Enums;

namespace StreamingPlatform.Dtos.Response
{
    /// <summary>
    /// Class representing a response containing details about a streaming plan.
    /// </summary>
    /// <remarks>
    public class PlanResponse(string name, double monthlyFee, int numberOfMinutes, PlanStatus status)
    {
        /// <summary>
        /// The name of the streaming plan.
        /// </summary>
        public string PlanName { get; set; } = name;

        /// <summary>
        /// The monthly fee for the streaming plan.
        /// </summary>
        public double MonthlyFee { get; set; } = monthlyFee;

        /// <summary>
        /// The number of minutes included in the streaming plan.
        /// </summary>
        public int NumberOfMinutes { get; set; } = numberOfMinutes;

        /// <summary>
        /// The status of the streaming plan (Active, Inactive).
        /// </summary>
        public PlanStatus Status { get; set; } = status;
    }
}