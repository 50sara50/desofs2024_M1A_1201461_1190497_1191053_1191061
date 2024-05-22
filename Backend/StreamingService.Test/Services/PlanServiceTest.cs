using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Dao;
using StreamingPlatform.Dao.Interfaces;
using StreamingPlatform.Dao.Repositories;
using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;
using StreamingPlatform.Models;
using StreamingPlatform.Models.Enums;
using StreamingPlatform.Services;
using System.ComponentModel.DataAnnotations;


namespace StreamingService.Test.Services
{
    [TestClass]
    public class PlanServiceTest
    {

        private StreamingDbContext? context;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<StreamingDbContext>()
               .UseInMemoryDatabase(databaseName: "TestingDatabase")
               .Options;

            this.context = new StreamingDbContext(options);
        }



        [TestCleanup]

        public void Cleanup()
        {
            if (this.context != null)
            {
                this.context.Database.EnsureDeleted();
                this.context.Dispose();
            }
        }


        [TestMethod]
        public async Task CreatePlanValidPlanCreatesPlanAndReturnsResponse()
        {

            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }

            CreatePlanContract planDto = new() { PlanName = "New Plan", MonthlyFee = 15, NumberOfMinutes = 1500 };
            IUnitOfWork unitOfWork = new UnitOfWork(context);
            PlanService planService = new(unitOfWork);
            PlanResponse response = await planService.CreatePlan(planDto);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(planDto.PlanName, response.PlanName);
            Assert.AreEqual(planDto.MonthlyFee, response.MonthlyFee);
            Assert.AreEqual(planDto.NumberOfMinutes, response.NumberOfMinutes);
            Assert.AreEqual(response.Status, PlanStatus.Active);
        }

        [TestMethod]
        public async Task GetPlanNonexistentPlanNameReturnsNull()
        {

            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            string planName = "Nonexistent Plan";
            IUnitOfWork unitOfWork = new UnitOfWork(context);
            PlanService planService = new(unitOfWork);
            PlanResponse? response = await planService.GetPlan(planName, true);
            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task GetPlanEmptyPlanNameThrowsArgumentException()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            IUnitOfWork unitOfWork = new UnitOfWork(context);
            PlanService planService = new(unitOfWork);
            ArgumentException exception = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await planService.GetPlan("", false));
            Assert.AreEqual("Plan name cannot be null or empty", exception.Message);
        }

        [TestMethod]
        public async Task GetPlanAsAnAdmin()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            Plan plan = new()
            {
                PlanName = "New Plan",
                MonthlyFee = 15,
                NumberOfMinutes = 1500,
                Status = PlanStatus.Inactive,
            };

            context.Plans.Add(plan);
            IUnitOfWork unitOfWork = new UnitOfWork(context);
            await unitOfWork.SaveChangesAsync();
            PlanService planService = new(unitOfWork);
            PlanResponse? response = await planService.GetPlan(plan.PlanName, true);
            Assert.IsNotNull(response);
            Assert.AreEqual(plan.PlanName, response.PlanName);
            Assert.AreEqual(plan.MonthlyFee, response.MonthlyFee);
            Assert.AreEqual(plan.NumberOfMinutes, response.NumberOfMinutes);
            Assert.AreEqual(plan.Status, response.Status);
        }

        [TestMethod]
        public async Task GetPlanAsAnNormalUserReturnsNull()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            Plan plan = new()
            {
                PlanName = "New Plan",
                MonthlyFee = 15,
                NumberOfMinutes = 1500,
                Status = PlanStatus.Inactive,
            };

            context.Plans.Add(plan);
            IUnitOfWork unitOfWork = new UnitOfWork(context);
            await unitOfWork.SaveChangesAsync();
            PlanService planService = new(unitOfWork);
            PlanResponse? response = await planService.GetPlan(plan.PlanName, false);
            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task GetPlansWithPaginationAsAnAdminReturnsPaginatedResults()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            int pageSize = 2;
            int currentPage = 1;
            UnitOfWork unitOfWork = new(context);
            PlanService planService = new(unitOfWork);
            CreatePlanContract planDto = new() { PlanName = "New Plan", MonthlyFee = 15, NumberOfMinutes = 1500 };
            PlanResponse planCreated1 = await planService.CreatePlan(planDto);
            CreatePlanContract plan2 = new() { PlanName = "New Plan 2", MonthlyFee = 15, NumberOfMinutes = 1500 };
            await planService.CreatePlan(plan2);
            Plan plan = new()
            {
                PlanName = "Inactive Plan",
                MonthlyFee = 15,
                NumberOfMinutes = 1500,
                Status = PlanStatus.Inactive,
            };
            context.Plans.Add(plan);
            await unitOfWork.SaveChangesAsync();
            PagedResponseDTO<PlanResponse> response = await planService.GetPlans(pageSize, currentPage, true);

            Assert.IsNotNull(response);
            Assert.AreEqual(pageSize, response.Data.Count);
            Assert.AreEqual(2, response.TotalPages);
            Assert.AreEqual(1, response.PageNumber);
            Assert.AreEqual(3, response.TotalRecords);
            Assert.AreEqual(2, response.TotalPages);
            Assert.AreEqual(true, response.HasNextPage);
            Assert.AreEqual(planCreated1.PlanName, response.Data.First().PlanName);
        }

        [TestMethod]
        public async Task GetPlansWithPaginationAsANormalUserReturnsActivePaginatedResults()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            int pageSize = 2;
            int currentPage = 1;
            UnitOfWork unitOfWork = new(context);
            PlanService planService = new(unitOfWork);
            CreatePlanContract planDto = new() { PlanName = "New Plan", MonthlyFee = 15, NumberOfMinutes = 1500 };
            PlanResponse planCreated1 = await planService.CreatePlan(planDto);
            CreatePlanContract plan2 = new() { PlanName = "New Plan 2", MonthlyFee = 15, NumberOfMinutes = 1500 };
            await planService.CreatePlan(plan2);
            Plan plan = new()
            {
                PlanName = "Inactive Plan",
                MonthlyFee = 15,
                NumberOfMinutes = 1500,
                Status = PlanStatus.Inactive,
            };
            context.Plans.Add(plan);
            await unitOfWork.SaveChangesAsync();
            PagedResponseDTO<PlanResponse> response = await planService.GetPlans(pageSize, currentPage, false);
            Assert.IsNotNull(response);
            Assert.AreEqual(pageSize, response.Data.Count);
            Assert.AreEqual(1, response.TotalPages);
            Assert.AreEqual(1, response.PageNumber);
            Assert.AreEqual(2, response.TotalRecords);
            Assert.AreEqual(1, response.TotalPages);
            Assert.AreEqual(false, response.HasNextPage);
            Assert.AreEqual(planCreated1.PlanName, response.Data.First().PlanName);
        }

        [TestMethod]
        public async Task GetPlanWithoutPaginationForNonAdminReturnsAllActiveRecords()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(context);
            PlanService planService = new(unitOfWork);
            CreatePlanContract planDto = new() { PlanName = "New Plan", MonthlyFee = 15, NumberOfMinutes = 1500 };
            PlanResponse planCreated1 = await planService.CreatePlan(planDto);
            CreatePlanContract plan2 = new() { PlanName = "New Plan 2", MonthlyFee = 15, NumberOfMinutes = 1500 };
            await planService.CreatePlan(plan2);
            Plan plan = new()
            {
                PlanName = "Inactive Plan",
                MonthlyFee = 15,
                NumberOfMinutes = 1500,
                Status = PlanStatus.Inactive,
            };
            context.Plans.Add(plan);
            await unitOfWork.SaveChangesAsync();

            IEnumerable<PlanResponse> response = await planService.GetPlans(false);

            Assert.IsNotNull(response);
            Assert.AreEqual(2, response.Count());
            Assert.AreEqual(planCreated1.PlanName, response.First().PlanName);
            Assert.AreEqual(plan2.PlanName, response.Last().PlanName);
        }

        [TestMethod]
        public async Task GetPlanWithoutPaginationAsANormalUserReturnsAllRecords()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(context);
            PlanService planService = new(unitOfWork);
            CreatePlanContract planDto = new() { PlanName = "New Plan", MonthlyFee = 15, NumberOfMinutes = 1500 };
            PlanResponse planCreated1 = await planService.CreatePlan(planDto);
            CreatePlanContract plan2 = new() { PlanName = "New Plan 2", MonthlyFee = 15, NumberOfMinutes = 1500 };
            await planService.CreatePlan(plan2);
            Plan plan = new()
            {
                PlanName = "Inactive Plan",
                MonthlyFee = 15,
                NumberOfMinutes = 1500,
                Status = PlanStatus.Inactive,
            };
            context.Plans.Add(plan);
            await unitOfWork.SaveChangesAsync();
            IEnumerable<PlanResponse> response = await planService.GetPlans(false);

            Assert.IsNotNull(response);
            Assert.AreEqual(2, response.Count());
            Assert.AreEqual(planCreated1.PlanName, response.First().PlanName);
            Assert.AreEqual(plan2.PlanName, response.Last().PlanName);
        }

        [TestMethod]
        public async Task GetPlanWithoutPaginationAsAnAdminReturnsAllRecords()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(context);
            PlanService planService = new(unitOfWork);
            CreatePlanContract planDto = new() { PlanName = "New Plan", MonthlyFee = 15, NumberOfMinutes = 1500 };
            PlanResponse planCreated1 = await planService.CreatePlan(planDto);
            CreatePlanContract plan2 = new() { PlanName = "New Plan 2", MonthlyFee = 15, NumberOfMinutes = 1500 };
            await planService.CreatePlan(plan2);
            Plan plan = new()
            {
                PlanName = "Inactive Plan",
                MonthlyFee = 15,
                NumberOfMinutes = 1500,
                Status = PlanStatus.Inactive,
            };
            context.Plans.Add(plan);
            await unitOfWork.SaveChangesAsync();
            IEnumerable<PlanResponse> response = await planService.GetPlans(true);

            Assert.IsNotNull(response);
            Assert.AreEqual(3, response.Count());
            Assert.AreEqual(planCreated1.PlanName, response.First().PlanName);
            Assert.AreEqual(plan.PlanName, response.Last().PlanName);
        }


        [TestMethod]
        public async Task CreatePlanNegativeFeeOrMinutesThrowsValidationException()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            var planDto1 = new CreatePlanContract { PlanName = "Test Plan", MonthlyFee = -10, NumberOfMinutes = 1000 };
            var planDto2 = new CreatePlanContract { PlanName = "Test Plan", MonthlyFee = 10, NumberOfMinutes = -500 };
            UnitOfWork unitOfWork = new(context);
            PlanService planService = new(unitOfWork);
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await planService.CreatePlan(planDto1));
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await planService.CreatePlan(planDto2));
        }

        [TestMethod]
        public async Task CreatePlanEmptyPlanNameThrowsValidationException()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            var planDto = new CreatePlanContract { PlanName = "", MonthlyFee = 10, NumberOfMinutes = 1000 };
            UnitOfWork unitOfWork = new(context);
            PlanService planService = new(unitOfWork);
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await planService.CreatePlan(planDto));
        }
    }
}
