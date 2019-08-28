using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SimpleGraphQLClient.UnitTests.TestObjects;
using Tests.TestObjects;
using SimpleGraphQLClient.Extensions;


namespace Tests
{
    //Playground
    //https://fireql.dev/?url=https://fakeql.com/graphql/8085b4e0c4f41b997bda34805bc006dd

    //GraphQL Endpoint: https://fakeql.com/graphql/8085b4e0c4f41b997bda34805bc006dd
    //GraphQL Fragile Endpoint:https://fakeql.com/fragilegraphql/8085b4e0c4f41b997bda34805bc006dd
    //Rest Endpoint: https://fakeql.com/rest/8085b4e0c4f41b997bda34805bc006dd

    [TestFixture]
    public class SimpleGraphQLClientTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestQueryWithSuccess()
        {
            // ARRANGE
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"data\": {\"user\": {\"id\": \"1\",\"firstname\": \"Mike\"}}}"),
                })
                .Verifiable();

            using (var client = new SimpleGraphQLClient.SimpleGraphQLClient(new HttpClient(handlerMock.Object)))
            {
                client.ServiceUrl = "https://fakeql.com/rest/8085b4e0c4f41b997bda34805bc006dd";
                client.OperationName = "getElement";
                client.Parameters.Add("id", 16);
                var result = client.Post<User>();
                Assert.IsNotNull(result);
                Assert.AreEqual("Mike", result.FirstName);
            }
            Assert.Pass();
        }

        [Test]
        public void TestServiceUrlNotSet()
        {
            using (var client = new SimpleGraphQLClient.SimpleGraphQLClient())
            {
                client.OperationName = "getElement";
                client.Parameters.Add("id", 16);

                var ex = Assert.Throws<ApplicationException>(() =>
                {
                    var result = client.Post<User>();
                });
                Assert.AreEqual("ServiceUrl is required!", ex.Message);
            }
        }

        [Test, Explicit]
        public void CallFakeQLService()
        {
            using (var client = new SimpleGraphQLClient.SimpleGraphQLClient())
            {
                client.ServiceUrl = "https://fakeql.com/rest/8085b4e0c4f41b997bda34805bc006dd";
                client.OperationName = "user";
                client.Parameters.Add("id", "1");
                var result = client.Post<User>();
                Assert.AreEqual("Mike", result.FirstName);
            }
        }
        [Test]
        public void Extensions_OutputParameters_WithFullObject()
        {
            //Arrange
            string expected = " { Status Username Debt Id Address { HouseNumber Street ZipCode { ISO Format StreetLevelFormat CountryId } } }";
            StringBuilder builder = new StringBuilder();
            var type = typeof(UserFull);
            //Act
            string actual = builder.Append(type.OutputParameters()).ToString();
            //Assert
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void Extensions_OutputParamters_ObjectWithGenericType()
        {
            //Arrange
            string expected = " { Status Username Debt Id Addresses { HouseNumber Street ZipCode { ISO Format StreetLevelFormat CountryId } } }";
            StringBuilder builder = new StringBuilder();
            var type = typeof(UserFullWithGenericType);
            //Act
            string actual = builder.Append(type.OutputParameters()).ToString();
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Extensions_OutputParamters_IgnoresEmptyComplexProperty()
        {
            //Arrange
            string expected = " { Id }";
            StringBuilder builder = new StringBuilder();
            var type = typeof(UserWithEmptyClass);
            //Act
            string actual = builder.Append(type.OutputParameters()).ToString();
            //Assert
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void Extensions_ToCamelCase()
        {
            string expected = "{ status username debt id address { houseNumber street zipCode { ISO format streetLevelFormat countryId } } }";
            StringBuilder builder = new StringBuilder();
            var type = typeof(UserFull);
            //Act
            string actual = builder.Append(type.OutputParameters()).ToString().ToCamelCase();
            //Assert
            Assert.AreEqual(expected, actual);
        }
    }
}