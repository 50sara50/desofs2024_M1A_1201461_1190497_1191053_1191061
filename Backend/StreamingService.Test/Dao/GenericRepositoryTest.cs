﻿using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Dao;
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
            List<Plan> activePlans = planRepository.GetRecords(x => x.Status == PlanStatus.Active, pageNumber: 1, numberOfRecords: 3);
            List<Plan> firstActivePlan = planRepository.GetRecords(x => x.Status == PlanStatus.Active, numberOfRecords: 1);
            List<Plan> inactivePlan = planRepository.GetRecords(x => x.Status == PlanStatus.Inactive, pageNumber: 1, numberOfRecords: 3);
            Assert.IsTrue(activePlans.Count == 3);
            Assert.IsTrue(firstActivePlan.Count == 1);
            Assert.IsTrue(inactivePlan.Count == 1);
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
            string pepper = PasswordEncryptor.GenerateSalt(); // TODO: Use the value from the configuration file/secrets instead of generating it.
            User user = new()
            {
                UserName = "test",
                Email = Faker.Internet.Email(),
                Password = new Password(userPassword, pepper)
            };
            userRepository.Create(user);
            unitOfWork.SaveChanges();
            List<User> users = userRepository.GetAllRecords().ToList();
            Assert.IsTrue(users.Count == 1);
            Assert.IsTrue(users[0].UserName == user.UserName);
            Assert.IsTrue(users[0].Email == user.Email);
        }

        private void SetupMockData(GenericRepository<Plan> repository, UnitOfWork unitOfWork)
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
            unitOfWork.SaveChanges();
        }
    }

}