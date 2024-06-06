using StreamingPlatform.Models.Enums;

namespace StreamingPlatform.Dtos.Response
{
    /// <summary>
    /// Class representing a response containing details about a streaming plan.
    /// </summary>
    /// <remarks>
    public class PlanResponse
    {
        /// <summary>
        /// Creates a new instance of PlanResponse.
        /// </summary>
        /// <param name="planName"> the name of the plan</param>
        /// <param name="monthlyFee">the monthly fee</param>
        /// <param name="numberOfMinutes"> the number of minutes </param>
        /// <param name="status">the status of plan (active or inactive)</param>
        public PlanResponse(string planName, double monthlyFee, int numberOfMinutes, PlanStatus status)
        {
            this.PlanName = planName;
            this.MonthlyFee = monthlyFee;
            this.NumberOfMinutes = numberOfMinutes;
            this.Status = status;
        }

        /// <summary>
        /// Creates a new instance of PlanResponse.
        /// </summary>
        /// <param name="planName"> the name of the plan</param>
        /// <param name="monthlyFee">the monthly fee</param>
        /// <param name="numberOfMinutes"> the number of minutes </param>
        public PlanResponse(string planName, double monthlyFee, int numberOfMinutes)
        {
            this.PlanName = planName;
            this.MonthlyFee = monthlyFee;
            this.NumberOfMinutes = numberOfMinutes;
        }

        /// <summary>
        /// The name of the streaming plan.
        /// </summary>
        public string PlanName { get; set; }

        /// <summary>
        /// The monthly fee for the streaming plan.
        /// </summary>
        public double MonthlyFee { get; set; }

        /// <summary>
        /// The number of minutes included in the streaming plan.
        /// </summary>
        public int NumberOfMinutes { get; set; }

        /// <summary>
        /// The status of the streaming plan (Active, Inactive).
        /// </summary>
        public PlanStatus? Status { get; set; }
    }
}