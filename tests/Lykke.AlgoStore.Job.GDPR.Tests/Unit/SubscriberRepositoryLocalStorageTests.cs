using AutoMapper;
using AzureStorage.Tables;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.Job.GDPR.AzureRepositories.Entities;
using Lykke.AlgoStore.Job.GDPR.AzureRepositories.Repositories;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Repositories;
using Lykke.AlgoStore.Job.GDPR.Tests.Infrastructure;
using Lykke.Common.Log;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Job.GDPR.Tests.Unit
{
    [TestFixture]
    public class SubscriberRepositoryLocalStorageTests
    {
        private Mock<ILogFactory> _logFactory;
        private ISubscriberRepository _repository;

        [SetUp]
        public void SetUp()
        {
            //REMARK: http://docs.automapper.org/en/stable/Configuration.html#resetting-static-mapping-configuration
            //Reset should not be used in production code. It is intended to support testing scenarios only.
            Mapper.Reset();

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<AutoMapperModelProfile>();
                cfg.AddProfile<AutoMapperProfile>();
            });

            Mapper.AssertConfigurationIsValid();

            _logFactory = new Mock<ILogFactory>();

            _repository = new SubscriberRepository(AzureTableStorage<SubscriberEntity>.Create(
                SettingsMock.GetDataStorageConnectionString(), SubscriberRepository.TableName, _logFactory.Object));
        }

        [Test]
        [Explicit(
            "This test will try to write data into local storage. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void SubscriberRepository_SaveAsync_Test()
        {
            var data = new SubscriberData
            {
                ClientId = "TEST",
                CookieConsent = true,
                DeletionStatus = DeletionStatus.None,
                GdprConsent = true
            };

            _repository.SaveAsync(data).Wait();
        }

        [Test]
        [Explicit(
            "This test will try to write data into local storage. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void SubscriberRepository_GetByIdAsync_Test()
        {
            var entity = _repository.GetByIdAsync("TEST").Result;

            Assert.IsNotNull(entity);
            //Compare that values are same as the one we used in SubscriberRepository_SaveAsync_Test
            Assert.AreEqual(entity.ClientId, "TEST");
            Assert.AreEqual(entity.CookieConsent, true);
            Assert.AreEqual(entity.DeletionStatus, DeletionStatus.None);
            Assert.AreEqual(entity.GdprConsent, true);
        }
    }
}
