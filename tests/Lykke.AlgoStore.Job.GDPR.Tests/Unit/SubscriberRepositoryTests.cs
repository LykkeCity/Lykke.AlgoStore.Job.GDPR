using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using AzureStorage;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Job.GDPR.AzureRepositories.Entities;
using Lykke.AlgoStore.Job.GDPR.AzureRepositories.Repositories;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Repositories;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using NUnit.Framework;
using FluentAssertions;

namespace Lykke.AlgoStore.Job.GDPR.Tests.Unit
{
    [TestFixture]
    public class SubscriberRepositoryTests
    {
        private readonly Fixture _fixture = new Fixture();

        private Mock<INoSQLTableStorage<SubscriberEntity>> _storageMock;
        private Mock<INoSQLTableStorage<DeactivatedSuscriberEntity>> _deactivateSubsriberStorageMock;
        private SubscriberData _subscriberData;
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

            _subscriberData = _fixture.Build<SubscriberData>().Create();
            var subscriberEntity = Mapper.Map<SubscriberEntity>(_subscriberData);

            _storageMock = new Mock<INoSQLTableStorage<SubscriberEntity>>();

            _storageMock.Setup(x => x.InsertOrReplaceAsync(subscriberEntity)).Returns(Task.CompletedTask);
            _storageMock.Setup(x => x.GetDataAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(subscriberEntity));
            _storageMock.Setup(x => x.DeleteIfExistAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            _deactivateSubsriberStorageMock = new Mock<INoSQLTableStorage<DeactivatedSuscriberEntity>>();

            _repository = new SubscriberRepository(_storageMock.Object, _deactivateSubsriberStorageMock.Object);
        }

        [Test]
        public void SaveAsync_Test()
        {
            _repository.SaveAsync(_subscriberData).Wait();
        }

        [Test]
        public void GetByIdAsync_Test()
        {
            var result = _repository.GetByIdAsync(_subscriberData.ClientId).Result;

            Assert.IsNotNull(result);
            //ClientId is ignored by AutoMapper
            Assert.AreEqual(result.CookieConsent, _subscriberData.CookieConsent);
            Assert.AreEqual(result.DeletionStatus, _subscriberData.DeletionStatus);
            Assert.AreEqual(result.GdprConsent, _subscriberData.GdprConsent);
        }

        [Test]
        public void UpdateAsync_Test()
        {
            _repository.UpdateAsync(_subscriberData).Wait();
        }

        [Test]
        public void DeleteAsync_Test()
        {
            _repository.DeleteAsync(_subscriberData.ClientId).Wait();
        }
    }
}
