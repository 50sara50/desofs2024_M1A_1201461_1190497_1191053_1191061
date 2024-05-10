using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StreamingPlatform.Dao;
using StreamingPlatform.Dao.Helper;
using StreamingPlatform.Dao.Repositories;
using StreamingPlatform.Models;
using StreamingPlatform.Models.Enums;
using StreamingPlatform.Utils;
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
            // the type specified here is just so the secrets library can 
            // find the UserSecretId we added in the csproj file
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddUserSecrets<StreamingDbContext>();
            IConfigurationRoot configuration = builder.Build();
            string encryptionKey = configuration.GetValue<string>("Keys:SecureDataKey") ?? PasswordEncryptor.GenerateSalt();
            SecureDataEncryptionHelper.SetEncryptionKey(encryptionKey);
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
        public void GetRecordsTestWithPagination()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            SetupMockData(planRepository, unitOfWork);
            PagedResponseOffset<Plan> activePlans = planRepository.GetRecords(x => x.Status == PlanStatus.Active, pageNumber: 1, numberOfRecords: 2);
            IEnumerable<Plan> allActivePlans = planRepository.GetRecords(x => x.Status == PlanStatus.Active);
            PagedResponseOffset<Plan> inactivePlan = planRepository.GetRecords(x => x.Status == PlanStatus.Inactive, pageNumber: 1, numberOfRecords: 3);
            Assert.IsTrue(activePlans.Data.Count == 2);
            Assert.IsTrue(allActivePlans.Count() == 3);
            Assert.IsTrue(inactivePlan.Data.Count == 1);
        }

        [TestMethod]
        public async Task GetRecordsTestWithPaginationAsync()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            SetupMockData(planRepository, unitOfWork);
            PagedResponseOffset<Plan> activePlans = await planRepository.GetRecordsAsync(x => x.Status == PlanStatus.Active, pageNumber: 1, numberOfRecords: 2);
            PagedResponseOffset<Plan> inactivePlan = await planRepository.GetRecordsAsync(x => x.Status == PlanStatus.Inactive, pageNumber: 1, numberOfRecords: 3);
            Assert.IsTrue(activePlans.Data.Count == 2);
            Assert.IsTrue(inactivePlan.Data.Count == 1);
        }

        [TestMethod]
        public void GetRecordsTest()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            SetupMockData(planRepository, unitOfWork);
            IEnumerable<Plan> activePlans = planRepository.GetRecords(x => x.Status == PlanStatus.Active);
            IEnumerable<Plan> inactivePlans = planRepository.GetRecords(x => x.Status == PlanStatus.Inactive);
            Assert.IsTrue(activePlans.Count() == 3);
            Assert.IsTrue(inactivePlans.Count() == 1);
        }

        [TestMethod]
        public async Task GetRecordsTestAsync()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            SetupMockData(planRepository, unitOfWork);
            IEnumerable<Plan> activePlans = await planRepository.GetRecordsAsync(x => x.Status == PlanStatus.Active);
            IEnumerable<Plan> inactivePlans = await planRepository.GetRecordsAsync(x => x.Status == PlanStatus.Inactive);
            Assert.IsTrue(activePlans.Count() == 3);
            Assert.IsTrue(inactivePlans.Count() == 1);
        }

        [TestMethod]
        public void GetAllRecords()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            SetupMockData(planRepository, unitOfWork);
            IEnumerable<Plan> allPlans = planRepository.GetAllRecords();
            Assert.IsTrue(allPlans.Count() == 4);
        }

        [TestMethod]
        public void GetAllRecordsWithPagination()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            SetupMockData(planRepository, unitOfWork);
            PagedResponseOffset<Plan> allPlans = planRepository.GetAllRecords(pageNumber: 1, numberOfRecords: 2);
            Assert.IsTrue(allPlans.Data.Count == 2);
            Assert.IsTrue(allPlans.TotalRecords == 4);
            Assert.IsTrue(allPlans.TotalPages == 2);
            Assert.IsTrue(allPlans.PageNumber == 1);
            Assert.IsTrue(allPlans.HasNextPage);
            Assert.IsTrue(allPlans.PageSize == 2);
            Plan plan = allPlans.Data.First();
            Assert.IsTrue(plan.MonthlyFee == 10);
            Assert.IsTrue(plan.NumberOfMinutes == 100);
            Assert.IsTrue(plan.Status == PlanStatus.Active);
            Assert.IsTrue(plan.PlanName == "Premium");
        }

        [TestMethod]
        public async Task GetAllRecordsAsync()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            SetupMockData(planRepository, unitOfWork);
            IEnumerable<Plan> allPlans = await planRepository.GetAllRecordsAsync();
            Assert.IsTrue(allPlans.Count() == 4);
        }

        [TestMethod]
        public async Task GetAllRecordsAsyncWithPagination()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            SetupMockData(planRepository, unitOfWork);
            PagedResponseOffset<Plan> allPlans = await planRepository.GetAllRecordsAsync(pageNumber: 1, numberOfRecords: 2);
            Assert.IsTrue(allPlans.Data.Count == 2);
            Assert.IsTrue(allPlans.TotalRecords == 4);
            Assert.IsTrue(allPlans.TotalPages == 2);
            Assert.IsTrue(allPlans.PageNumber == 1);
            Assert.IsTrue(allPlans.HasNextPage);
            Assert.IsTrue(allPlans.PageSize == 2);
            Plan plan = allPlans.Data.First();
            Assert.IsTrue(plan.MonthlyFee == 10);
            Assert.IsTrue(plan.NumberOfMinutes == 100);
            Assert.IsTrue(plan.Status == PlanStatus.Active);
            Assert.IsTrue(plan.PlanName == "Premium");
        }

        [TestMethod]
        public void GetRecordByIdTest()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            Guid id = Guid.NewGuid();
            Plan newPlan = new()
            {
                PlanId = id,
                PlanName = "Premium",
                MonthlyFee = 10,
                NumberOfMinutes = 100,
                Status = PlanStatus.Active
            };
            planRepository.Create(newPlan);
            unitOfWork.SaveChanges();
            Plan? plan = planRepository.GetRecordById(id);
            Assert.IsNotNull(plan);
            Assert.IsTrue(plan.MonthlyFee == 10);
            Assert.IsTrue(plan.NumberOfMinutes == 100);
            Assert.IsTrue(plan.Status == PlanStatus.Active);
            Assert.IsTrue(plan.PlanName == "Premium");
        }

        [TestMethod]
        public void GetRecordByIdDoesNotExistReturnNull()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            Guid id = Guid.NewGuid();
            Plan? plan = planRepository.GetRecordById(id);
            Assert.IsNull(plan);
        }

        [TestMethod]
        public async Task GetRecordByIdAsync()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            Guid id = Guid.NewGuid();
            Plan newPlan = new()
            {
                PlanId = id,
                PlanName = "Premium",
                MonthlyFee = 10,
                NumberOfMinutes = 100,
                Status = PlanStatus.Active
            };
            planRepository.Create(newPlan);
            await unitOfWork.SaveChangesAsync();
            Plan? plan = await planRepository.GetRecordByIdAsync(id);
            Assert.IsNotNull(plan);
            Assert.IsTrue(plan.MonthlyFee == 10);
        }

        [TestMethod]
        public async Task GetRecordByIdAsyncDoesNotExistReturnNull()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            Guid id = Guid.NewGuid();
            Plan? plan = await planRepository.GetRecordByIdAsync(id);
            Assert.IsNull(plan);
        }

        [TestMethod]
        public void GetRecordByFilterExpression()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            Guid id = Guid.NewGuid();
            Plan newPlan = new()
            {
                PlanId = id,
                PlanName = "Premium",
                MonthlyFee = 10,
                NumberOfMinutes = 100,
                Status = PlanStatus.Active
            };
            planRepository.Create(newPlan);
            unitOfWork.SaveChanges();
            Plan? plan = planRepository.GetRecord(x => x.PlanName == "Premium");
            Assert.IsNotNull(plan);
            Assert.IsTrue(plan.Status == PlanStatus.Active);
            Assert.IsTrue(plan.PlanId == id);
            Assert.IsTrue(plan.PlanName == "Premium");
            Assert.IsTrue(plan.MonthlyFee == 10);
            Assert.IsTrue(plan.NumberOfMinutes == 100);
        }

        [TestMethod]
        public void GetRecordByFilterExpressionThatDoesNotExistReturnNull()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            Guid id = Guid.NewGuid();
            Plan? plan = planRepository.GetRecord(x => x.PlanName == "Premium");
            Assert.IsNull(plan);
        }

        [TestMethod]
        public async Task GetRecordByFilterExpressionAsync()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            Guid id = Guid.NewGuid();
            Plan newPlan = new()
            {
                PlanId = id,
                PlanName = "Premium",
                MonthlyFee = 10,
                NumberOfMinutes = 100,
                Status = PlanStatus.Active
            };
            planRepository.Create(newPlan);
            await unitOfWork.SaveChangesAsync();
            Plan? plan = await planRepository.GetRecordAsync(x => x.PlanName == "Premium");
            Assert.IsNotNull(plan);
            Assert.IsTrue(plan.Status == PlanStatus.Active);
            Assert.IsTrue(plan.PlanId == id);
            Assert.IsTrue(plan.PlanName == "Premium");
            Assert.IsTrue(plan.MonthlyFee == 10);
            Assert.IsTrue(plan.NumberOfMinutes == 100);
        }


        [TestMethod]
        public async Task GetRecordByFilterExpressionThatDoesNotExistReturnNullAsync()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            Guid id = Guid.NewGuid();
            Plan? plan = await planRepository.GetRecordAsync(x => x.PlanName == "Premium");
            Assert.IsNull(plan);
        }

        [TestMethod]
        public void CreateTest()
        {

            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }

            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            Plan plan = new()
            {
                PlanName = "Test Plan",
                MonthlyFee = 50,
                NumberOfMinutes = 500,
                Status = PlanStatus.Active
            };
            planRepository.Create(plan);
            unitOfWork.SaveChanges();
            IEnumerable<Plan> allPlans = planRepository.GetAllRecords();
            Assert.IsTrue(allPlans.Count() == 1);
        }

        [TestMethod]
        public void UpdateEntity()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }

            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            Guid guid = Guid.NewGuid();
            Plan plan = new()
            {
                PlanId = guid,
                PlanName = "Premium",
                MonthlyFee = 10,
                NumberOfMinutes = 100,
                Status = PlanStatus.Active
            };
            planRepository.Create(plan);
            unitOfWork.SaveChanges();
            Plan? createdPlan = planRepository.GetRecordById(guid);
            Assert.IsNotNull(createdPlan);
            Assert.IsTrue(createdPlan.Status == PlanStatus.Active);
            Assert.IsTrue(createdPlan.PlanId == guid);
            Assert.IsTrue(createdPlan.PlanName == "Premium");
            Assert.IsTrue(createdPlan.MonthlyFee == 10);
            Assert.IsTrue(createdPlan.NumberOfMinutes == 100);
            plan.PlanName = "Updated Plan";
            plan.MonthlyFee = 100;
            plan.NumberOfMinutes = 1000;
            plan.Status = PlanStatus.Inactive;
            planRepository.Update(plan);
            unitOfWork.SaveChanges();
            Plan? updatedPlan = planRepository.GetRecordById(guid);
            Assert.IsNotNull(updatedPlan);
            Assert.IsTrue(updatedPlan.Status == PlanStatus.Inactive);
            Assert.IsTrue(updatedPlan.PlanId == guid);
            Assert.IsTrue(updatedPlan.PlanName == "Updated Plan");
            Assert.IsTrue(updatedPlan.MonthlyFee == 100);
            Assert.IsTrue(updatedPlan.NumberOfMinutes == 1000);
        }

        [TestMethod]
        public void DeleteEntityById()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }

            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            Guid guid = Guid.NewGuid();
            Plan plan = new()
            {
                PlanId = guid,
                PlanName = "Premium",
                MonthlyFee = 10,
                NumberOfMinutes = 100,
                Status = PlanStatus.Active
            };
            planRepository.Create(plan);
            unitOfWork.SaveChanges();
            Plan? createdPlan = planRepository.GetRecordById(guid);
            Assert.IsNotNull(createdPlan);
            Assert.IsTrue(createdPlan.Status == PlanStatus.Active);
            Assert.IsTrue(createdPlan.PlanId == guid);
            Assert.IsTrue(createdPlan.PlanName == "Premium");
            Assert.IsTrue(createdPlan.MonthlyFee == 10);
            Assert.IsTrue(createdPlan.NumberOfMinutes == 100);
            planRepository.Delete(guid);
            unitOfWork.SaveChanges();
            Plan? deletedPlan = planRepository.GetRecordById(guid);
            Assert.IsNull(deletedPlan);
        }

        [TestMethod]
        public void DeleteEntity()
        {
            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }

            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<Plan> planRepository = (GenericRepository<Plan>)unitOfWork.Repository<Plan>();
            Guid guid = Guid.NewGuid();
            Plan plan = new()
            {
                PlanId = guid,
                PlanName = "Premium",
                MonthlyFee = 10,
                NumberOfMinutes = 100,
                Status = PlanStatus.Active
            };
            planRepository.Create(plan);
            unitOfWork.SaveChanges();
            Plan? createdPlan = planRepository.GetRecordById(guid);
            Assert.IsNotNull(createdPlan);
            Assert.IsTrue(createdPlan.Status == PlanStatus.Active);
            Assert.IsTrue(createdPlan.PlanId == guid);
            Assert.IsTrue(createdPlan.PlanName == "Premium");
            Assert.IsTrue(createdPlan.MonthlyFee == 10);
            Assert.IsTrue(createdPlan.NumberOfMinutes == 100);
            planRepository.Delete(createdPlan);
            unitOfWork.SaveChanges();
            Plan? deletedPlan = planRepository.GetRecordById(guid);
            Assert.IsNull(deletedPlan);
        }


        private static void SetupMockData(GenericRepository<Plan> repository, UnitOfWork unitOfWork)
        {
            Plan plan = new()
            {
                PlanName = "Premium",
                MonthlyFee = 10,
                NumberOfMinutes = 100,
                Status = PlanStatus.Active
            };

            Plan plan2 = new()
            {
                PlanName = "Free",
                MonthlyFee = 20,
                NumberOfMinutes = 200,
                Status = PlanStatus.Active
            };

            Plan plan3 = new()
            {
                PlanName = "Diamond",
                MonthlyFee = 30,
                NumberOfMinutes = 300,
                Status = PlanStatus.Active
            };

            Plan plan4 = new()
            {
                PlanName = "Inactive",
                MonthlyFee = 40,
                NumberOfMinutes = 400,
                Status = PlanStatus.Inactive
            };
            repository.Create(plan);
            repository.Create(plan2);
            repository.Create(plan3);
            repository.Create(plan4);
            unitOfWork.SaveChanges();
        }
    }

}
