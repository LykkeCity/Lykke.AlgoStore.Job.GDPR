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
        private const string ClientId = "TEST";
        private Mock<ILogFactory> _logFactory;
        private ISubscriberRepository _repository;
        private SubscriberData _subscriberData;

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

            _subscriberData = new SubscriberData
            {
                ClientId = ClientId,
                CookieConsent = true,
                DeletionStatus = DeletionStatus.None,
                GdprConsent = true
            };

            _logFactory = new Mock<ILogFactory>();

            _repository = new SubscriberRepository(AzureTableStorage<SubscriberEntity>.Create(
                SettingsMock.GetDataStorageConnectionString(), SubscriberRepository.TableName, _logFactory.Object), AzureTableStorage<DeactivatedSuscriberEntity>.Create(
                SettingsMock.GetDataStorageConnectionString(), SubscriberRepository.TableName, _logFactory.Object));
        }

        [Test]
        [Explicit(
            "This test will try to write data into local storage. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void SubscriberRepository_SaveAsync_Test()
        {
            _repository.SaveAsync(_subscriberData).Wait();
        }

        [Test]
        [Explicit(
            "This test will try to write data into local storage. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void SubscriberRepository_GetByIdAsync_Test()
        {
            var entity = _repository.GetByIdAsync(ClientId).Result;

            Assert.IsNotNull(entity);
            //Compare that values are same as the one we used in SubscriberRepository_SaveAsync_Test
            Assert.AreEqual(entity.ClientId, _subscriberData.ClientId);
            Assert.AreEqual(entity.CookieConsent, _subscriberData.CookieConsent);
            Assert.AreEqual(entity.DeletionStatus, _subscriberData.DeletionStatus);
            Assert.AreEqual(entity.GdprConsent, _subscriberData.GdprConsent);
        }

        [Test]
        [Explicit(
            "This test will try to write data into local storage. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void SubscriberRepository_UpdateAsync_Test()
        {
            _repository.UpdateAsync(_subscriberData).Wait();
        }

        [Test]
        [Explicit(
            "This test will try to write data into local storage. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void SubscriberRepository_DeleteAsync_Test()
        {
            _repository.DeleteAsync(ClientId).Wait();
        }
    }
}
