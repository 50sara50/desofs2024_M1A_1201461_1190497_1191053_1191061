using System.ComponentModel.DataAnnotations;

namespace StreamingPlatform.Dtos.Contract
{
    /// <summary>
    /// Class representing a contract (data transfer object) used to create a new streaming plan.
    /// </summary>
    public class CreatePlanContract
    {
        /// <summary>
        /// The name of the streaming plan.
        /// 
        /// [Required(ErrorMessage = "Plan name is required.")] – This attribute enforces that the PlanName property must have a value, 
        /// otherwise it will throw a validation error with the specified error message.
        /// </summary>
        [Required(ErrorMessage = "Plan name is required.")]
        required public string PlanName { get; set; }

        /// <summary>
        /// The monthly fee for the streaming plan.
        /// 
        /// [Required(ErrorMessage = "Monthly fee is required.")] – This attribute enforces that the MonthlyFee property must have a value.
        /// [Range(0, double.MaxValue, ErrorMessage = "Monthly fee must be greater than 0.")] – This attribute validates that the MonthlyFee 
        /// must be between the specified minimum (0) and maximum (double.MaxValue) values, otherwise it will throw a validation error 
        /// with the specified error message.
        /// </summary>
        [Required(ErrorMessage = "Monthly fee is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Monthly fee must be greater than 0.")]
        public double MonthlyFee { get; set; }

        /// <summary>
        /// The number of minutes included in the streaming plan.
        /// 
        /// [Required(ErrorMessage = "Number of minutes is required.")] – This attribute enforces that the NumberOfMinutes property must have a value.
        /// [Range(0, int.MaxValue, ErrorMessage = "Number of minutes must be greater than 0.")] – This attribute validates that the NumberOfMinutes 
        /// must be between the specified minimum (0) and maximum (int.MaxValue) values, otherwise it will throw a validation error 
        /// with the specified error message.
        /// </summary>
        [Required(ErrorMessage = "Number of minutes is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Number of minutes must be greater than 0.")]
        public int NumberOfMinutes { get; set; }
    }
}
