using EventBus.Events.ServicesEvents.CompanyEvents;
using EventBus.Events.ServicesEvents.OfferEvents;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Abstractions;
using UserService.EventHandlers;
using UserService.Models;
using Xunit;

namespace UserService.Tests.EventHandlers
{
    public class EventHandlersTests
    {
        private readonly Mock<IUserRepoService> _mockUserRepo = new();

        // ==========================================
        // 1. AddCompanyEH Tests
        // ==========================================

        [Fact]
        public async Task AddCompanyEH_Handle_CallsAddCompanyOnRepository()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<AddCompanyEH>>();
            var handler = new AddCompanyEH(mockLogger.Object, _mockUserRepo.Object);

            var companyGuid = Guid.NewGuid();
            var ownerGuid = Guid.NewGuid();
            var @event = new AddCompanyEvent("Hair Studio", companyGuid, ownerGuid);

            _mockUserRepo
                .Setup(r => r.AddCompany(ownerGuid, companyGuid, "Hair Studio"))
                .Returns(true);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockUserRepo.Verify(r => r.AddCompany(ownerGuid, companyGuid, "Hair Studio"), Times.Once);
        }

        // ==========================================
        // 2. AddMasterEH Tests
        // ==========================================

        [Fact]
        public async Task AddMasterEH_Handle_CallsJoinToCompanyOnRepository()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<AddMasterEH>>();
            var handler = new AddMasterEH(mockLogger.Object, _mockUserRepo.Object);

            var companyGuid = Guid.NewGuid();
            var masterGuid = Guid.NewGuid();
            var @event = new AddMasterEvent(companyGuid, masterGuid, "Barbershop");

            _mockUserRepo
                .Setup(r => r.JoinToCompany(masterGuid, companyGuid, "Barbershop"))
                .Returns(true);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockUserRepo.Verify(r => r.JoinToCompany(masterGuid, companyGuid, "Barbershop"), Times.Once);
        }

        // ==========================================
        // 3. AddOfferEH Tests
        // ==========================================

        [Fact]
        public async Task AddOfferEH_Handle_CallsAddOfferOnRepository()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<AddOfferEH>>();
            var handler = new AddOfferEH(mockLogger.Object, _mockUserRepo.Object);

            var offerGuid = Guid.NewGuid();
            var companyGuid = Guid.NewGuid();
            var masterGuid = Guid.NewGuid();
            var @event = new AddOfferEvent("Haircut", offerGuid, companyGuid, masterGuid);

            _mockUserRepo
                .Setup(r => r.AddOffer(offerGuid, "Haircut", masterGuid))
                .Returns(true);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockUserRepo.Verify(r => r.AddOffer(offerGuid, "Haircut", masterGuid), Times.Once);
        }

        // ==========================================
        // 4. DelCompanyEH Tests
        // ==========================================

        [Fact]
        public async Task DelCompanyEH_Handle_WhenUsersOwnCompany_DeletesCompanyForThoseUsers()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelCompanyEH>>();
            var handler = new DelCompanyEH(mockLogger.Object, _mockUserRepo.Object);

            var targetCompanyGuid = Guid.NewGuid();
            var otherCompanyGuid = Guid.NewGuid();

            var user1 = new User { Name = "Owner1", UIDFB = Guid.NewGuid() };
            user1.OwnCompanies.Add(new UserCompanyRef { RefGuid = targetCompanyGuid, Name = "Target Salon" });

            var user2 = new User { Name = "Owner2", UIDFB = Guid.NewGuid() };
            user2.OwnCompanies.Add(new UserCompanyRef { RefGuid = targetCompanyGuid, Name = "Target Salon" });

            var user3 = new User { Name = "OtherOwner", UIDFB = Guid.NewGuid() };
            user3.OwnCompanies.Add(new UserCompanyRef { RefGuid = otherCompanyGuid, Name = "Other Salon" });

            _mockUserRepo
                .Setup(r => r.GetEntities())
                .Returns(new List<User> { user1, user2, user3 });

            var @event = new DelCompanyEvent(targetCompanyGuid);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockUserRepo.Verify(r => r.DelCompany(user1.UIDFB, targetCompanyGuid), Times.Once);
            _mockUserRepo.Verify(r => r.DelCompany(user2.UIDFB, targetCompanyGuid), Times.Once);
            _mockUserRepo.Verify(r => r.DelCompany(user3.UIDFB, targetCompanyGuid), Times.Never);
        }

        [Fact]
        public async Task DelCompanyEH_Handle_WhenNoUsersOwnCompany_DoesNotCallDelCompany()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelCompanyEH>>();
            var handler = new DelCompanyEH(mockLogger.Object, _mockUserRepo.Object);

            var targetCompanyGuid = Guid.NewGuid();
            var user = new User { Name = "Owner", UIDFB = Guid.NewGuid() };

            _mockUserRepo
                .Setup(r => r.GetEntities())
                .Returns(new List<User> { user });

            var @event = new DelCompanyEvent(targetCompanyGuid);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockUserRepo.Verify(r => r.DelCompany(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DelCompanyEH_Handle_WhenNoUsersExist_DoesNotCallDelCompany()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelCompanyEH>>();
            var handler = new DelCompanyEH(mockLogger.Object, _mockUserRepo.Object);

            _mockUserRepo
                .Setup(r => r.GetEntities())
                .Returns(new List<User>());

            var @event = new DelCompanyEvent(Guid.NewGuid());

            // Act
            await handler.Handle(@event);

            // Assert
            _mockUserRepo.Verify(r => r.DelCompany(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        // ==========================================
        // 5. DelMasterEH Tests
        // ==========================================

        [Fact]
        public async Task DelMasterEH_Handle_CallsUnJoinCompanyOnRepository()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelMasterEH>>();
            var handler = new DelMasterEH(mockLogger.Object, _mockUserRepo.Object);

            var companyGuid = Guid.NewGuid();
            var masterUid = Guid.NewGuid();
            var @event = new DelMasterEvent(companyGuid, masterUid, "Barbershop");

            _mockUserRepo
                .Setup(r => r.UnJoinCompany(masterUid, companyGuid))
                .Returns(true);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockUserRepo.Verify(r => r.UnJoinCompany(masterUid, companyGuid), Times.Once);
        }

        // ==========================================
        // 6. DelOfferEH Tests
        // ==========================================

        [Fact]
        public async Task DelOfferEH_Handle_CallsOnDelOfferOnRepository()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelOfferEH>>();
            var handler = new DelOfferEH(mockLogger.Object, _mockUserRepo.Object);

            var offerGuid = Guid.NewGuid();
            var masterGuid = Guid.NewGuid();
            var @event = new DelOfferEvent("Haircut", offerGuid, masterGuid);

            _mockUserRepo
                .Setup(r => r.OnDelOffer(offerGuid, masterGuid))
                .Returns(true);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockUserRepo.Verify(r => r.OnDelOffer(offerGuid, masterGuid), Times.Once);
        }
    }
}
