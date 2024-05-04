using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Dao;
using StreamingPlatform.Dao.Repositories;
using StreamingPlatform.Models;
using StreamingPlatform.Models.Enums;
namespace StreamingService.Test.DaoTesting
{
    [TestClass]
    public sealed class GenericRepositoryTest
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
        public void GetAllRecordsTest()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            GenericRepository<Plan> repository = new GenericRepository<Plan>(this.context);
            SetupMockData(repository);
            List<Plan> activePlans = repository.GetRecords(x => x.Status == PlanStatus.Active, pageNumber: 1, numberOfRecords: 3);
            List<Plan> firstActivePlan = repository.GetRecords(x => x.Status == PlanStatus.Active, numberOfRecords: 1);
            List<Plan> inactivePlan = repository.GetRecords(x => x.Status == PlanStatus.Inactive, pageNumber: 1, numberOfRecords: 3);
            Assert.IsTrue(activePlans.Count == 3);
            Assert.IsTrue(firstActivePlan.Count == 1);
            Assert.IsTrue(inactivePlan.Count == 1);
        }

        private void SetupMockData(GenericRepository<Plan> repository)
        {
            Plan plan = new()
            {
                MontlyFee = 10,
                NumberOfMinutes = 100,
                Status = PlanStatus.Active
            };

            Plan plan2 = new()
            {
                MontlyFee = 20,
                NumberOfMinutes = 200,
                Status = PlanStatus.Active
            };

            Plan plan3 = new()
            {
                MontlyFee = 30,
                NumberOfMinutes = 300,
                Status = PlanStatus.Active
            };

            Plan plan4 = new()
            {
                MontlyFee = 40,
                NumberOfMinutes = 400,
                Status = PlanStatus.Inactive
            };
            repository.Create(plan);
            repository.Create(plan2);
            repository.Create(plan3);
            repository.Create(plan4);
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }

            this.context.SaveChanges();
        }
    }

}
