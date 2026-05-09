using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Mocks;

public static class MockUserProfileRepositoryExtensions
{
    extension(Mock<IUserProfileRepository> mock)
    {
        public void SetupGetByAlias(UserProfile profile)
        {
            mock
                .Setup(x => x.GetByAliasAsync(It.IsAny<PlayerAlias>()))
                .ReturnsAsync(profile);
        }

        public void SetupGetByAliasNotFound()
        {
            mock
                .Setup(x => x.GetByAliasAsync(It.IsAny<PlayerAlias>()))
                .ReturnsAsync((UserProfile?)null);
        }

        public void SetupThrowsOnGetByAlias(Exception ex)
        {
            mock
                .Setup(x => x.GetByAliasAsync(It.IsAny<PlayerAlias>()))
                .ThrowsAsync(ex);
        }

        public void SetupGetById(UserProfile profile)
        {
            mock
                .Setup(x => x.GetByIdAsync(It.IsAny<ProfileId>()))
                .ReturnsAsync(profile);
        }

        public void SetupGetByIdNotFound()
        {
            mock
                .Setup(x => x.GetByIdAsync(It.IsAny<ProfileId>()))
                .ReturnsAsync((UserProfile?)null);
        }

        public void SetupThrowsOnGetById(Exception ex)
        {
            mock
                .Setup(x => x.GetByIdAsync(It.IsAny<ProfileId>()))
                .ThrowsAsync(ex);
        }

        public void VerifyGetByIdNeverCalled()
        {
            mock.Verify(x => x.GetByIdAsync(It.IsAny<ProfileId>()), Times.Never);
        }
    }
}
