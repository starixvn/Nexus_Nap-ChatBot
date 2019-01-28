using Fanex.NCB.Core.Models.DTO;
using FluentAssertions;
using System;
using Xunit;

namespace Fanex.NCB.Core.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            // Arrange & Act
            var model = new AuthorizedFunction() {
                FunctionKey = string.Empty,
                Policy = 1
            };

            // Assert
            model.Policy.Should().Be(1);
        }
    }
}
