using System;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using AutoMapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Repositories;
using Lykke.AlgoStore.Job.GDPR.Core.Services;
using Lykke.AlgoStore.Job.GDPR.Services;
using Lykke.AlgoStore.Job.Stopping.Client;
using Lykke.AlgoStore.Service.Security.Client;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Job.GDPR.Tests.Unit
{
    [TestFixture]
    public class SubscriberServiceTests
    {
        private readonly Fixture _fixture = new Fixture();
        private ISubscriberService _service;

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

            _service = MockService();
        }

        [Test]
        public void SeedAsync_ForNullRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.SeedAsync(null));
        }

        [Test]
        public void SeedAsync_ForEmprtyRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ValidationException>(() => _service.SeedAsync(string.Empty));
        }

        [Test]
        public void SeedAsync_ForValidRequest_WillSucceed_Test()
        {
            _service.SeedAsync(It.IsAny<string>());
        }

        [Test]
        public void GetByIdAsync_ForNullRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetByIdAsync(null));
        }

        [Test]
        public void GetByIdAsync_ForEmprtyRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ValidationException>(() => _service.GetByIdAsync(string.Empty));
        }

        [Test]
        public void GetByIdAsync_ForValidRequest_WillSucceed_Test()
        {
            var data = _service.GetByIdAsync(It.IsAny<string>());

            Assert.NotNull(data);
        }

        [Test]
        public void SetCookieConsentAsync_ForNullRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.SetCookieConsentAsync(null));
        }

        [Test]
        public void SetCookieConsentAsync_ForEmprtyRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ValidationException>(() => _service.SetCookieConsentAsync(string.Empty));
        }

        [Test]
        public void SetCookieConsentAsync_ForValidRequest_WillSucceed_Test()
        {
            var data = _service.SetCookieConsentAsync(It.IsAny<string>());

            Assert.NotNull(data);
        }

        [Test]
        public void SetGdprConsentAsync_ForNullRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.SetGdprConsentAsync(null));
        }

        [Test]
        public void SetGdprConsentAsync_ForEmprtyRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ValidationException>(() => _service.SetGdprConsentAsync(string.Empty));
        }

        [Test]
        public void SetGdprConsentAsync_ForValidRequest_WillSucceed_Test()
        {
            var data = _service.SetGdprConsentAsync(It.IsAny<string>());

            Assert.NotNull(data);
        }

        [Test]
        public void DeactivateAccountAsync_ForNullRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.DeactivateAccountAsync(null));
        }

        [Test]
        public void DeactivateAccountAsync_ForEmprtyRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ValidationException>(() => _service.DeactivateAccountAsync(string.Empty));
        }

        [Test]
        public void DeactivateAccountAsync_ForValidRequest_WillSucceed_Test()
        {
            var data = _service.DeactivateAccountAsync(It.IsAny<string>());

            Assert.NotNull(data);
        }

        private ISubscriberService MockService()
        {
            var subscriberRepositoryMock = new Mock<ISubscriberRepository>();
            var commentsRepositoryMock = new Mock<IAlgoCommentsRepository>();
            var securityClientMock = new Mock<ISecurityClient>();
            var instanceStoppingClientMock = new Mock<IAlgoInstanceStoppingClient>();
            var clientInstanceRepositoryMock = new Mock<IAlgoClientInstanceRepository>();
            var algoRepositoryMock = new Mock<IAlgoRepository>();

            return new SubscriberService(subscriberRepositoryMock.Object, commentsRepositoryMock.Object,
                securityClientMock.Object, instanceStoppingClientMock.Object, clientInstanceRepositoryMock.Object,
                algoRepositoryMock.Object);
        }
    }
}
