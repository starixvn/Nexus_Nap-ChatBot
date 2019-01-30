using Fanex.NCB.Swagger.AuthGate.Services;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Fanex.NCB.Common.DTO;
using Fanex.NAP.Core.Models;

namespace Fanex.NCB.Swagger.AuthGate.UnitTests
{
    public class Class1
    {
        [Fact]
        public void GetMenus_WhenCalled_ReturnListOfMenus()
        {
            // Arrange & Act
            var service = new MenuService();
            var menuItems = new List<MenuItem>
            {
                new MenuItem {
                    Id = 1,
                    Name = "1",
                    Url = "1",
                    Policy = 1,
                }
            };

            // Act
            var result = service.GetMenus(menuItems);

            // Assert
            result.Count.Should().Be(1);
        }

    }
}
