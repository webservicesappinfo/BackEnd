using CompanyService.Abstractions;
using CompanyService.EventHandlers;
using CompanyService.Models;
using EventBus.Events.ServicesEvents.OfferEvents;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CompanyService.Tests.EventHandlers
{
    public class CompanyEventHandlersTests
    {
        private readonly Mock<ICompanyRepoService> _mockRepo = new();

        // ==========================================
        // 1. AddOfferEH Tests
        // ==========================================

        [Fact]
        public async Task AddOfferEH_Handle_CallsOnAddOfferEventOnRepository()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<AddOfferEH>>();
            var handler = new AddOfferEH(mockLogger.Object, _mockRepo.Object);

            var offerGuid = Guid.NewGuid();
            var companyGuid = Guid.NewGuid();
            var masterGuid = Guid.NewGuid();
            var @event = new AddOfferEvent("Haircut", offerGuid, companyGuid, masterGuid);

            _mockRepo.Setup(r => r.OnAddOfferEvent(@event)).Returns(true);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.OnAddOfferEvent(@event), Times.Once);
        }

        // ==========================================
        // 2. DelOfferEH Tests
        // ==========================================

        [Fact]
        public async Task DelOfferEH_Handle_CallsOnDelOfferEventOnRepository()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelOfferEvent>>();
            var handler = new DelOfferEH(mockLogger.Object, _mockRepo.Object);

            var offerGuid = Guid.NewGuid();
            var masterGuid = Guid.NewGuid();
            var @event = new DelOfferEvent("Haircut", offerGuid, masterGuid);

            _mockRepo.Setup(r => r.OnDelOfferEvent(@event)).Returns(true);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.OnDelOfferEvent(@event), Times.Once);
        }

        // ==========================================
        // 3. DelUserEH Tests
        // ==========================================

        [Fact]
        public async Task DelUserEH_Handle_WhenUserHasOwnedAndWorkerCompanies_DeletesAllAppropriately()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelUserEvent>>();
            var handler = new DelUserEH(mockLogger.Object, _mockRepo.Object);

            var userGuid = Guid.NewGuid();
            var ownedCompany1 = new Company { Guid = Guid.NewGuid(), Name = "Owned1", OwnerGuid = userGuid };
            var ownedCompany2 = new Company { Guid = Guid.NewGuid(), Name = "Owned2", OwnerGuid = userGuid };
            var workerCompany = new Company { Guid = Guid.NewGuid(), Name = "WorkerCompany", OwnerGuid = Guid.NewGuid() };

            _mockRepo
                .Setup(r => r.GetCompaniesByOwner(userGuid))
                .Returns(new List<Company> { ownedCompany1, ownedCompany2 });

            _mockRepo
                .Setup(r => r.GetCompaniesByWorker(userGuid))
                .Returns(new List<Company> { workerCompany });

            var @event = new DelUserEvent("John", userGuid.ToString(), "token");

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.DelEntity(ownedCompany1.Guid), Times.Once);
            _mockRepo.Verify(r => r.DelEntity(ownedCompany2.Guid), Times.Once);
            _mockRepo.Verify(r => r.DelWorker(workerCompany.Guid, userGuid), Times.Once);
        }

        [Fact]
        public async Task DelUserEH_Handle_WhenUserHasNoCompanies_DoesNotDeleteOrDelWorker()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelUserEvent>>();
            var handler = new DelUserEH(mockLogger.Object, _mockRepo.Object);

            var userGuid = Guid.NewGuid();

            _mockRepo
                .Setup(r => r.GetCompaniesByOwner(userGuid))
                .Returns(new List<Company>());

            _mockRepo
                .Setup(r => r.GetCompaniesByWorker(userGuid))
                .Returns(new List<Company>());

            var @event = new DelUserEvent("John", userGuid.ToString(), "token");

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.DelEntity(It.IsAny<Guid>()), Times.Never);
            _mockRepo.Verify(r => r.DelWorker(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }
    }
}
