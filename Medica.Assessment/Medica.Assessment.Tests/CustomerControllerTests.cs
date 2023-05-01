using Moq.Protected;
using Moq;
using System.Net;
using Medica.Assessment.Model;
using Medica.Assessment.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Medica.Assessment.Tests
{
    public class CustomerControllerTests
    {
        private readonly Mock<IHttpClientFactory> _mockClientFactory;
        private readonly CustomerController _controller;

        public CustomerControllerTests()
        {
            _mockClientFactory = new Mock<IHttpClientFactory>();
            _controller = new CustomerController(_mockClientFactory.Object);
        }

        [Fact]
        public async Task PostCustomer_ReturnsOkResult_WhenResponseIsSuccessful()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(Properties.Resources.resSendAsync, ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

            var testCustomer = testCustomerObject();

            var result = await _controller.PostCustomer(testCustomer);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task PostCustomer_ReturnsBadRequestResult_WhenResponseIsUnsuccessful()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(Properties.Resources.resSendAsync, ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

            var testCustomer = testCustomerObject();

            var result = await _controller.PostCustomer(testCustomer);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PostCustomer_SendsCorrectJsonContent_WhenCalled()
        {
            var testCustomer = testCustomerObject();
            var expectedJson = JsonSerializer.Serialize(testCustomer);

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(Properties.Resources.resSendAsync, ItExpr.Is<HttpRequestMessage>(req => RequestIsNotNull(req) && req.Content.ReadAsStringAsync().Result == expectedJson), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

            var result = await _controller.PostCustomer(testCustomer);

            mockHttpMessageHandler.Protected().Verify
                (
                    Properties.Resources.resSendAsync, 
                    Times.Once(), 
                    ItExpr.Is<HttpRequestMessage>(req => RequestIsNotNull(req) && req.Content.ReadAsStringAsync().Result == expectedJson), 
                    ItExpr.IsAny<CancellationToken>()
                );
        }

        [Fact]
        public async Task PostCustomer_SendsCorrectHttpRequest_WhenCalled()
        {
            var testCustomer = testCustomerObject();
            var expectedUri = new Uri(Properties.Resources.resCustomerPOSTurl);
            var expectedMethod = HttpMethod.Post;

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(Properties.Resources.resSendAsync, ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);


            var result = await _controller.PostCustomer(testCustomer);

            mockHttpMessageHandler.Protected().Verify
                (
                    Properties.Resources.resSendAsync, 
                    Times.Once(), 
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri == expectedUri && req.Method == expectedMethod), 
                    ItExpr.IsAny<CancellationToken>()
                );
        }

        [Fact]
        public async Task PostCustomer_SendsCorrectMediaType_WhenCalled()
        {
            var testCustomer = testCustomerObject();
            var expectedMediaType = Properties.Resources.resApplicationJson;

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(Properties.Resources.resSendAsync, ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

            var result = await _controller.PostCustomer(testCustomer);

            mockHttpMessageHandler.Protected().Verify
                (
                    Properties.Resources.resSendAsync, 
                    Times.Once(), 
                    ItExpr.Is<HttpRequestMessage>(req => RequestIsNotNull(req) && req.Content.Headers.ContentType.MediaType == expectedMediaType), 
                    ItExpr.IsAny<CancellationToken>()
                );
        }

        private customer testCustomerObject()
        { 
            var result = new customer
            {
                Id = Convert.ToInt16(Properties.Resources.resTestID1),
                name = Properties.Resources.resTestName1,
                surname = Properties.Resources.resTestSurname1
            };

            return result;
        }

        private static bool RequestIsNotNull(HttpRequestMessage req)
        {
            bool result = 
                req != null &&
                req.Content != null &&
                req.Content.Headers != null &&
                req.Content.Headers.ContentType != null &&
                req.Content.Headers.ContentType.MediaType != null;

            return result;
        }
    }
}