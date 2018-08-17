using System.Linq;
using AutoFixture;
using AutoMapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.Job.GDPR.Client;
using Lykke.AlgoStore.Job.GDPR.Client.Model;
using NUnit.Framework;

namespace Lykke.AlgoStore.Job.GDPR.Tests.Unit
{
    [TestFixture]
    public class GdprClientTests
    {
        private readonly Fixture _fixture = new Fixture();
        private IGdprClient _client;

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

            //REMARK: Must use auth token for existing algo instance
            //var authHandler = new AlgoAuthorizationHeaderHttpClientHandler("4fba109d-2c42-4b90-9bba-5f75842d012e");

            _client = HttpClientGenerator.HttpClientGenerator
                .BuildForUrl("http://localhost:5000")
                //.WithAdditionalDelegatingHandler(authHandler)
                .Create()
                .Generate<IGdprClient>();
        }

        [Test]
        [Explicit("This test will try to initiate REST API client on localhost. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void HandleCandlesAsync_Test()
        {
            var result = _client.GetLegalConsentsAsync("TEST").Result;

            Assert.IsNotNull(result);
        }
    }
}
