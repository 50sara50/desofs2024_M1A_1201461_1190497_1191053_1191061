using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StreamingPlatform.Controllers;
using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;
using StreamingPlatform.Services.Interfaces;
using StreamingPlatform;
using System.ComponentModel.DataAnnotations;
using StreamingPlatform.Models.Enums;
using StreamingPlatform.Controllers.Responses;

namespace StreamingService.Test.Controller
{
    [TestClass]
    public sealed class PlanControllerTest
    {
#pragma warning disable CS8618 // This is initialized in the test setup method
        private Mock<ILogger<AuthController>> _mockLogger;
        private Mock<IPlanService> _mockPlanService;
#pragma warning restore CS8618

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<AuthController>>();
            _mockPlanService = new Mock<IPlanService>();
        }

        [TestMethod]
        public async Task CreatePlanValidDataReturnsCreatedPlan()
        {
            // Arrange
            CreatePlanContract planDto = new() { PlanName = "New Plan", MonthlyFee = 15, NumberOfMinutes = 1500 };
            PlanResponse planResponse = new(planDto.PlanName, planDto.MonthlyFee, planDto.NumberOfMinutes, PlanStatus.Active);
            _mockPlanService.Setup(service => service.CreatePlan(planDto)).Returns(Task.FromResult(planResponse));
            PlanController controller = new(_mockLogger.Object, _mockPlanService.Object);

            IActionResult result = await controller.CreatePlan(planDto);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, ((OkObjectResult)result).StatusCode);
            Assert.AreEqual(planResponse, ((OkObjectResult)result).Value);
        }

        [TestMethod]
        public async Task CreatePlanInvalidDataReturnsBadRequest()
        {
            CreatePlanContract planDto = new() { PlanName = "New Plan", MonthlyFee = -1, NumberOfMinutes = -1 };
            _mockPlanService.Setup(service => service.CreatePlan(planDto)).Throws<ValidationException>();
            PlanController controller = new(_mockLogger.Object, _mockPlanService.Object);

            IActionResult result = await controller.CreatePlan(planDto);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(StatusCodes.Status400BadRequest, ((BadRequestObjectResult)result).StatusCode);
        }

        [TestMethod]
        public async Task CreatePlanDuplicateNameReturnsConflict()
        {
            CreatePlanContract planDto = new() { PlanName = "New Plan", MonthlyFee = 15, NumberOfMinutes = 1500 };
            _mockPlanService.Setup(service => service.CreatePlan(planDto)).Throws<InvalidOperationException>();
            PlanController controller = new(_mockLogger.Object, _mockPlanService.Object);

            IActionResult result = await controller.CreatePlan(planDto);

            Assert.IsInstanceOfType(result, typeof(ConflictObjectResult));
            Assert.AreEqual(StatusCodes.Status409Conflict, ((ConflictObjectResult)result).StatusCode);
        }

        [TestMethod]
        public async Task CreatePlanUnexpectedErrorReturnsInternalServerError()
        {
            CreatePlanContract planDto = new() { PlanName = "New Plan", MonthlyFee = 15, NumberOfMinutes = 1500 };
            _mockPlanService.Setup(service => service.CreatePlan(planDto)).Throws<Exception>();
            PlanController controller = new(_mockLogger.Object, _mockPlanService.Object);

            IActionResult result = await controller.CreatePlan(planDto);

            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status500InternalServerError, ((ObjectResult)result).StatusCode);
        }

        [TestMethod]
        public async Task GetPlanByNameExistingPlanReturnsPlan()
        {
            // Arrange
            string planName = "MyPlan";
            PlanResponse planResponse = new(planName, 10, 100, PlanStatus.Active);
            _mockPlanService.Setup(service => service.GetPlan(planName, false)).Returns(Task.FromResult<PlanResponse?>(planResponse));
            PlanController controller = new(_mockLogger.Object, _mockPlanService.Object);
            IActionResult result = await controller.GetPlanByName(planName);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, ((OkObjectResult)result).StatusCode);
            Assert.AreEqual(planResponse, ((OkObjectResult)result).Value);
        }

        [TestMethod]
        public async Task GetPlanByNameNonexistentPlanReturnsNotFound()
        {
            string planName = "MyPlan";
            _mockPlanService.Setup(service => service.GetPlan(planName, false)).Returns(Task.FromResult<PlanResponse?>(null));
            PlanController controller = new(_mockLogger.Object, _mockPlanService.Object);

            IActionResult result = await controller.GetPlanByName(planName);

            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual(StatusCodes.Status404NotFound, ((NotFoundObjectResult)result).StatusCode);
            ErrorResponseObject? errorResponseObject = (ErrorResponseObject?)((NotFoundObjectResult)result).Value;
            Assert.IsNotNull(errorResponseObject);
            Assert.AreEqual("No plan with the specified name", errorResponseObject.Message);
            Assert.AreEqual("NotFound", errorResponseObject.Code);
        }

        [TestMethod]
        public async Task GetPlanByName_UnexpectedError_ReturnsInternalServerError()
        {
            string planName = "MyPlan";
            _mockPlanService.Setup(service => service.GetPlan(planName, false)).Throws<Exception>();
            PlanController controller = new(_mockLogger.Object, _mockPlanService.Object);
            IActionResult result = await controller.GetPlanByName(planName);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status500InternalServerError, ((ObjectResult)result).StatusCode);
        }

        [TestMethod]
        public async Task GetPlansNoPagingReturnsAllPlans()
        {
            IEnumerable<PlanResponse> plans = [new("MyPlan", 10, 100, PlanStatus.Active)];
            _mockPlanService.Setup(service => service.GetPlans()).Returns(Task.FromResult(plans.AsEnumerable()));
            PlanController controller = new(_mockLogger.Object, _mockPlanService.Object);

            IActionResult result = await controller.GetPlans(null, null);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, ((OkObjectResult)result).StatusCode);
            ICollection<PlanResponse>? responsePlans = (ICollection<PlanResponse>?)((OkObjectResult)result).Value;
            CollectionAssert.AreEquivalent(plans.ToList(), responsePlans?.ToList());
        }

        [TestMethod]
        public async Task GetPlansWithPagingReturnsPagedPlans()
        {
            int pageSize = 1;
            int currentPage = 1;
            IEnumerable<PlanResponse> plans = [new("MyPlan", 10, 100, PlanStatus.Active), new("MyPlan2", 20, 30, PlanStatus.Active)];
            PagedResponseDTO<PlanResponse> response = new()
            {
                PageNumber = 1,
                PageSize = 1,
                TotalRecords = 2,
                TotalPages = 2,
                HasNextPage = true,
                Data = [new("MyPlan", 10, 100, PlanStatus.Active)]
            };
            _mockPlanService.Setup(service => service.GetPlans(pageSize, currentPage)).Returns(Task.FromResult<PagedResponseDTO<PlanResponse>>(response));
            PlanController controller = new(_mockLogger.Object, _mockPlanService.Object);
            IActionResult result = await controller.GetPlans(pageSize, currentPage);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, ((OkObjectResult)result).StatusCode);
            PagedResponseDTO<PlanResponse>? responsePlans = (PagedResponseDTO<PlanResponse>?)((OkObjectResult)result).Value;
            Assert.IsNotNull(responsePlans);
            Assert.IsTrue(responsePlans.Data.Count == 1);
            Assert.IsTrue(responsePlans.TotalPages == 2);
            Assert.IsTrue(responsePlans.TotalRecords == 2);
            Assert.IsTrue(responsePlans.HasNextPage);
            Assert.IsTrue(responsePlans.PageNumber == 1);
            Assert.AreEqual(plans.First().PlanName, responsePlans.Data.First().PlanName);
            Assert.AreEqual(plans.First().MonthlyFee, responsePlans.Data.First().MonthlyFee);
            Assert.AreEqual(plans.First().NumberOfMinutes, responsePlans.Data.First().NumberOfMinutes);
            Assert.AreEqual(plans.First().Status, responsePlans.Data.First().Status);
        }

        [TestMethod]
        public async Task GetPlansMissingPageSizeThrowsArgumentException()
        {
            int currentPage = 2;
            PlanController controller = new(_mockLogger.Object, _mockPlanService.Object);
            IActionResult actionResult = await controller.GetPlans(null, currentPage);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            Assert.AreEqual(StatusCodes.Status400BadRequest, ((BadRequestObjectResult)actionResult).StatusCode);
        }

        [TestMethod]
        public async Task GetPlansMissingCurrentPageThrowsArgumentException()
        {
            int pageSize = 10;
            PlanController controller = new(_mockLogger.Object, _mockPlanService.Object);
            IActionResult actionResult = await controller.GetPlans(pageSize, null);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            Assert.AreEqual(StatusCodes.Status400BadRequest, ((BadRequestObjectResult)actionResult).StatusCode);

        }

        [TestMethod]
        public async Task GetPlansUnexpectedErrorReturnsInternalServerError()
        {
            int pageSize = 10;
            int currentPage = 2;
            _mockPlanService.Setup(service => service.GetPlans(pageSize, currentPage)).Throws<Exception>();
            PlanController controller = new(_mockLogger.Object, _mockPlanService.Object);

            IActionResult result = await controller.GetPlans(pageSize, currentPage);

            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status500InternalServerError, ((ObjectResult)result).StatusCode);
        }

    }
}
