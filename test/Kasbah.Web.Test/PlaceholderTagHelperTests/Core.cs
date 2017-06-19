using Kasbah.Content;
using Kasbah.Web.TagHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Kasbah.Web.Test.PlaceholderTagHelperTests
{
    public class Core
    {
        [Fact]
        public void CreateTagHelper_WithDefaults_CreatesSuccessfully()
        {
            var mockLogger = new Moq.Mock<ILogger<PlaceholderTagHelper>>();
            var mockViewComponentHelper = new Moq.Mock<IViewComponentHelper>();
            var mockContentService = new Moq.Mock<ContentService>();
            var mockComponentRegistry = new Moq.Mock<ComponentRegistry>();

            var vc = new PlaceholderTagHelper(mockLogger.Object, mockViewComponentHelper.Object, mockContentService.Object, mockComponentRegistry.Object);

            Assert.NotNull(vc);
        }
    }
}
