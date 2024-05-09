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

        private string TestPasswordPepper = PasswordEncryptor.GenerateSalt();



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
            string? passwordPepper = configuration.GetValue<string?>("Keys:PasswordPepper");
            if (passwordPepper != null)
            {
                TestPasswordPepper = passwordPepper;
            }
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
        public void CreateTest()
        {

            if (this.context == null)
            {
                Assert.Fail("Context is null");
            }
            string userPassword = Faker.Lorem.Sentence().Trim();
            UnitOfWork unitOfWork = new(this.context);
            GenericRepository<User> userRepository = (GenericRepository<User>)unitOfWork.Repository<User>();
            string address = Faker.Address.StreetAddress();
            Guid userId = Guid.NewGuid();

            User user = new()
            {
                Id = userId,
                UserName = "test",
                Email = Faker.Internet.Email(),
                Password = new Password(userPassword, TestPasswordPepper),
                Address = address,
            };
            userRepository.Create(user);
            unitOfWork.SaveChanges();
            List<User> users = userRepository.GetAllRecords().ToList();
            Assert.IsTrue(users.Count == 1);
            Assert.IsTrue(users[0].UserName == user.UserName);
            Assert.IsTrue(users[0].Email == user.Email);
            Assert.IsTrue(users[0].Address == address);
        }

        private void SetupMockData(GenericRepository<Plan> repository, UnitOfWork unitOfWork)
        {
            Plan plan = new()
            {
                MonthlyFee = 10,
                NumberOfMinutes = 100,
                Status = PlanStatus.Active
            };

            Plan plan2 = new()
            {
                MonthlyFee = 20,
                NumberOfMinutes = 200,
                Status = PlanStatus.Active
            };

            Plan plan3 = new()
            {
                MonthlyFee = 30,
                NumberOfMinutes = 300,
                Status = PlanStatus.Active
            };

            Plan plan4 = new()
            {
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
