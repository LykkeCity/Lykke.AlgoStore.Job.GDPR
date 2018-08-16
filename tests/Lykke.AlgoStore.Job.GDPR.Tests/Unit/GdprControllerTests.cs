using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Common.Log;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.Job.GDPR.Controllers;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities;
using Lykke.AlgoStore.Job.GDPR.Core.Services;
using Lykke.AlgoStore.Job.GDPR.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Job.GDPR.Tests.Unit
{
    [TestFixture]
    public class GdprControllerTests
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<ISubscriberService> _usersServiceMock;
        private Mock<ILog> _logMock;
        private SubscribersController _controller;
        private Mock<HttpContext> _httpContextMock;

        [SetUp]
        public void SetUp()
        {
            //REMARK: http://docs.automapper.org/en/stable/Configuration.html#resetting-static-mapping-configuration
            //Reset should not be used in production code. It is intended to support testing scenarios only.
            Mapper.Reset();

            Mapper.Initialize(cfg => { cfg.AddProfile<AutoMapperModelProfile>(); });

            Mapper.AssertConfigurationIsValid();

            _logMock = new Mock<ILog>();

            //REMARK: Cannot mock extension methods, but it will work without mocking those :)
            //_logMock.Setup(x => x.LogElapsedTimeAsync(It.IsAny<string>(), It.IsAny<Func<Task>>()))
            //    .Returns(Task.CompletedTask);
            //_logMock.Setup(x => x.LogElapsedTimeAsync(It.IsAny<string>(), It.IsAny<Func<Task<UserData>>>()))
            //    .Returns(Task.FromResult(_fixture.Build<UserData>().Create()));

            _usersServiceMock = new Mock<ISubscriberService>();

            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(x => x.Request.Headers.Add("TEST", It.IsAny<string>()));

            _controller = new SubscribersController(_usersServiceMock.Object, _logMock.Object)
                {ControllerContext = new ControllerContext {HttpContext = _httpContextMock.Object}};
        }

        [Test]
        public void GetLegalConsents_WillReturnCorrectResult_Test()
        {
            var result = _controller.GetLegalConsents(It.IsAny<string>()).Result;

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void SetUserGdprConsent_WillReturnCorrectResult_Test()
        {
            var result = _controller.SetUserGdprConsent(It.IsAny<string>()).Result;

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public void SetUserCookieConsent_WillReturnCorrectResult_Test()
        {
            var result = _controller.SetUserCookieConsent(It.IsAny<string>()).Result;

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public void DeactivateUserAccount_WillReturnCorrectResult_Test()
        {
            var result = _controller.DeactivateUserAccount(It.IsAny<string>()).Result;

            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}
