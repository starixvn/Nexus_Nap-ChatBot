namespace Fanex.NCB.AutoJobs.Core.Tests.Repository
{
    using Data;
    using Fanex.Data.Repository;
    using Fanex.NCB.AutoJobs.Core.Models.Core;
    using Fanex.NCB.AutoJobs.Core.Repository;
    using NSubstitute;
    using System.Threading.Tasks;
    using Xunit;

    public class BaseRepositoryTests
    {
        private readonly IObjectDb objectDb;

        public BaseRepositoryTests()
        {
            DbSettingProviderManager
                .StartTestingSession();
            objectDb = Substitute.For<IObjectDb>();
            var mockFactory = Substitute.For<CreateObjectDbFunc>();
            mockFactory(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>()).Returns(objectDb);
            ObjectDbFactory.Mock(mockFactory);
        }

        #region ExecuteNonQueryAsync

        [Fact]
        public async Task ExecuteNonQueryAsync_CriteriaNotNull_CallExecuteNonQueryAsync()
        {
            // Arrange
            var command = FakeCommand();
            var repository = new BaseRepository();

            // Action
            var result = await repository.ExecuteNonQueryAsync(command);

            // Assert
            Assert.Equal(0, result.Code);
        }

        [Fact]
        public async Task ExecuteNonQueryAsync_OutputParams_ReturnAsResult()
        {
            // Arrange
            var repository = new BaseRepository();
            var command = FakeCommand();
            var expected = GetSuccessResult();

            // Action
            var result = await repository.ExecuteNonQueryAsync(command);

            // Assert
            Assert.Equal(expected.Code, result.Code);
            Assert.Equal(expected.Message, result.Message);
        }

        #endregion ExecuteNonQueryAsync

        #region Helpers

        private static INonQueryCommand FakeCommand()
        {
            var command = Substitute.For<INonQueryCommand>();

            command.GetSettingKey().Returns("key");
            command.IsValid().Returns(true);

            return command;
        }

        private static CommandResult GetSuccessResult()
        {
            return new CommandResult
            {
                Code = 0,
                Message = string.Empty
            };
        }

        #endregion Helpers
    }
}