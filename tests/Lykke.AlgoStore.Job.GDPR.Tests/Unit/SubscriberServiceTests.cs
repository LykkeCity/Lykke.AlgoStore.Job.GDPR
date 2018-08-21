﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Common.Log;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Repositories;
using Lykke.AlgoStore.Job.GDPR.Core.Services;
using Lykke.AlgoStore.Job.GDPR.Services;
using Lykke.AlgoStore.Job.GDPR.Services.Strings;
using Lykke.AlgoStore.Job.Stopping.Client;
using Lykke.AlgoStore.Service.Security.Client;
using Lykke.Common.Log;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Job.GDPR.Tests.Unit
{
    [TestFixture]
    public class SubscriberServiceTests
    {
        private const string ClientId = "TEST";
        private readonly Fixture _fixture = new Fixture();
        private ISubscriberService _service;
        private Mock<ISubscriberRepository> _subscriberRepositoryMock;
        private Mock<IAlgoCommentsRepository> _commentsRepositoryMock;
        private Mock<ISecurityClient> _securityClientMock;
        private Mock<IAlgoInstanceStoppingClient> _instanceStoppingClientMock;
        private Mock<IAlgoClientInstanceRepository> _clientInstanceRepositoryMock;
        private Mock<IAlgoRepository> _algoRepositoryMock;
        private Mock<ILogFactory> _logFactoryMock;
        private Mock<IPublicAlgosRepository> _publicAlgosRepositoryMock;

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
            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.SeedAsync(string.Empty));

            Assert.That(ex.Message, Is.EqualTo(Phrases.ClientIdEmpty));
        }

        [Test]
        public void SeedAsync_ForValidRequest_WillSucceed_Test()
        {
            _service.SeedAsync(ClientId).Wait();
        }

        [Test]
        public void GetByIdAsync_ForNullRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetByIdAsync(null));
        }

        [Test]
        public void GetByIdAsync_ForEmprtyRequest_WillThrowException_Test()
        {
            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.GetByIdAsync(string.Empty));

            Assert.That(ex.Message, Is.EqualTo(Phrases.ClientIdEmpty));
        }

        [Test]
        public void GetByIdAsync_ForValidRequest_WillSucceed_Test()
        {
            var data = _service.GetByIdAsync(ClientId).Result;

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
            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.SetCookieConsentAsync(string.Empty));

            Assert.That(ex.Message, Is.EqualTo(Phrases.ClientIdEmpty));
        }

        [Test]
        public void SetCookieConsentAsync_ForValidRequest_WillSucceed_Test()
        {
            _service.SetCookieConsentAsync(ClientId).Wait();
        }

        [Test]
        public void SetCookieConsentAsync_ForValidRequest_WhenConsentIsAlreadyGiven_WillThrowException_Test()
        {
            _subscriberRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Build<SubscriberData>().With(x => x.CookieConsent, true)
                    .With(x => x.GdprConsent, false).Create()));

            _service = new SubscriberService(_subscriberRepositoryMock.Object, _commentsRepositoryMock.Object,
                _securityClientMock.Object, _instanceStoppingClientMock.Object, _clientInstanceRepositoryMock.Object,
                _algoRepositoryMock.Object, _logFactoryMock.Object, _publicAlgosRepositoryMock.Object);

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.SetCookieConsentAsync(ClientId));

            Assert.That(ex.Message, Is.EqualTo(Phrases.CookieConsentAlreadyGiven));
        }

        [Test]
        public void SetGdprConsentAsync_ForNullRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.SetGdprConsentAsync(null));
        }

        [Test]
        public void SetGdprConsentAsync_ForEmprtyRequest_WillThrowException_Test()
        {
            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.SetGdprConsentAsync(string.Empty));

            Assert.That(ex.Message, Is.EqualTo(Phrases.ClientIdEmpty));
        }

        [Test]
        public void SetGdprConsentAsync_ForValidRequest_WillSucceed_Test()
        {
            _service.SetGdprConsentAsync(ClientId).Wait();
        }

        [Test]
        public void SetGdprConsentAsync_ForValidRequest_WhenConsentIsAlreadyGiven_WillThrowException_Test()
        {
            _subscriberRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Build<SubscriberData>().With(x => x.CookieConsent, false)
                    .With(x => x.GdprConsent, true).Create()));

            _service = new SubscriberService(_subscriberRepositoryMock.Object, _commentsRepositoryMock.Object,
                _securityClientMock.Object, _instanceStoppingClientMock.Object, _clientInstanceRepositoryMock.Object,
                _algoRepositoryMock.Object, _logFactoryMock.Object, _publicAlgosRepositoryMock.Object);

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.SetGdprConsentAsync(ClientId));

            Assert.That(ex.Message, Is.EqualTo(Phrases.GdprConsentAlreadyGiven));
        }

        [Test]
        public void DeactivateAccountAsync_ForNullRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.DeactivateAccountAsync(null));
        }

        [Test]
        public void DeactivateAccountAsync_ForEmprtyRequest_WillThrowException_Test()
        {
            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.DeactivateAccountAsync(string.Empty));

            Assert.That(ex.Message, Is.EqualTo(Phrases.ClientIdEmpty));
        }

        [Test]
        public void DeactivateAccountAsync_ForValidRequest_WillSucceed_Test()
        {
            var data = _service.DeactivateAccountAsync(It.IsAny<string>());

            Assert.NotNull(data);
        }

        private ISubscriberService MockService()
        {
            _subscriberRepositoryMock = new Mock<ISubscriberRepository>();
            _subscriberRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Build<SubscriberData>().With(x => x.CookieConsent, false)
                    .With(x => x.GdprConsent, false).Create()));

            _commentsRepositoryMock = new Mock<IAlgoCommentsRepository>();
            _securityClientMock = new Mock<ISecurityClient>();
            _instanceStoppingClientMock = new Mock<IAlgoInstanceStoppingClient>();
            _clientInstanceRepositoryMock = new Mock<IAlgoClientInstanceRepository>();
            _algoRepositoryMock = new Mock<IAlgoRepository>();
            _logFactoryMock = new Mock<ILogFactory>();

            var logMock = new Mock<ILog>();

            _logFactoryMock.Setup(x => x.CreateLog(It.IsAny<object>())).Returns(logMock.Object);

            _publicAlgosRepositoryMock = new Mock<IPublicAlgosRepository>();

            return new SubscriberService(_subscriberRepositoryMock.Object, _commentsRepositoryMock.Object,
                _securityClientMock.Object, _instanceStoppingClientMock.Object, _clientInstanceRepositoryMock.Object,
                _algoRepositoryMock.Object, _logFactoryMock.Object, _publicAlgosRepositoryMock.Object);
        }
    }
}
