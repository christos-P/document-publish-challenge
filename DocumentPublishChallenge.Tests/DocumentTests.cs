using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using DocumentPublishChallenge.DataAccessLayer;
using DocumentPublishChallenge.Domain;
using DocumentPublishChallenge.Service.Controllers;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DocumentPublishChallenge.Tests
{
    // https://www.asp.net/web-api/overview/testing-and-debugging/unit-testing-controllers-in-web-api

    [TestFixture]
    public class DocumentTests
    {
        [Test]
        public async Task ValidateNumberOfDocunents()
        {
            // Arrange
            var mockRepository = new Mock<IDocumentRepository>();
            mockRepository.Setup(m => m.GetUserDocuments(1))
                .ReturnsAsync(new List<DocumentEntity>
                {
                    new DocumentEntity
                    {
                        Id = Guid.NewGuid(),
                        Link = string.Empty,
                        UserId = 1,
                        Name = string.Empty,
                        Created = DateTime.UtcNow.AddDays(-1),
                    }
                });

            var controller = new DocumentController(mockRepository.Object);

            // Act
            var actionResult = controller.GetUserDocuments(1);
            var contentResult = await actionResult as OkNegotiatedContentResult<string>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            var documents = JsonConvert.DeserializeObject<IEnumerable<object>>(contentResult.Content);
            Assert.AreEqual(documents.Count(), 1);
        }
    }
}