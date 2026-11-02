using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventBus.Events.ServicesEvents.CompanyEvents;
using EventBus.Events.ServicesEvents.OrderEvents;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OfferService.Abstractions;
using OfferService.EventHandlers;
using OfferService.Models;
using Xunit;
using Offer = OfferService.Models.Offer;

namespace OfferService.Tests.EventHandlers
{
    public class OfferEventHandlersTests
    {
        private readonly Mock<IOfferRepoService> _mockRepo;

        public OfferEventHandlersTests()
        {
            _mockRepo = new Mock<IOfferRepoService>();
        }

        #region AddOrderEH Tests

        [Fact]
        public async Task AddOrderEH_Handle_CallsSetStatusWithSubmittedAndReturnsResult()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<AddOrderEvent>>();
            var handler = new AddOrderEH(mockLogger.Object, _mockRepo.Object);

            var offerGuid = Guid.NewGuid();
            var orderGuid = Guid.NewGuid();
            var masterGuid = Guid.NewGuid();
            var clientGuid = Guid.NewGuid();

            var @event = new AddOrderEvent(
                "Order #1",
                orderGuid,
                offerGuid,
                masterGuid,
                clientGuid,
                "John Client");

            _mockRepo.Setup(r => r.SetStatus(offerGuid, OfferStatus.Submitted))
                .Returns(true);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.SetStatus(offerGuid, OfferStatus.Submitted), Times.Once);
        }

        #endregion

        #region DelCompanyEH Tests

        [Fact]
        public async Task DelCompanyEH_Handle_CallsOnDelCompanyEHonRepository()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelCompanyEvent>>();
            var handler = new DelCompanyEH(mockLogger.Object, _mockRepo.Object);

            var companyGuid = Guid.NewGuid();
            var @event = new DelCompanyEvent(companyGuid);

            _mockRepo.Setup(r => r.OnDelCompanyEH(companyGuid))
                .Returns(true);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.OnDelCompanyEH(companyGuid), Times.Once);
        }

        #endregion

        #region DelOrderEH Tests

        [Fact]
        public async Task DelOrderEH_Handle_WhenOfferIsSubmitted_SetsStatusToActivedAndReturnsTrue()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelOrderEvent>>();
            var handler = new DelOrderEH(mockLogger.Object, _mockRepo.Object);

            var offerGuid = Guid.NewGuid();
            var orderGuid = Guid.NewGuid();
            var @event = new DelOrderEvent("Order Cancelled", orderGuid, offerGuid);

            var offer = new Offer
            {
                Guid = offerGuid,
                Status = OfferStatus.Submitted
            };

            _mockRepo.Setup(r => r.GetEntity(offerGuid)).Returns(offer);
            _mockRepo.Setup(r => r.SetStatus(offerGuid, OfferStatus.Actived)).Returns(true);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.SetStatus(offerGuid, OfferStatus.Actived), Times.Once);
        }

        [Fact]
        public async Task DelOrderEH_Handle_WhenOfferIsAccepted_SetsStatusToActivedAndReturnsTrue()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelOrderEvent>>();
            var handler = new DelOrderEH(mockLogger.Object, _mockRepo.Object);

            var offerGuid = Guid.NewGuid();
            var orderGuid = Guid.NewGuid();
            var @event = new DelOrderEvent("Order Cancelled", orderGuid, offerGuid);

            var offer = new Offer
            {
                Guid = offerGuid,
                Status = OfferStatus.Accepted
            };

            _mockRepo.Setup(r => r.GetEntity(offerGuid)).Returns(offer);
            _mockRepo.Setup(r => r.SetStatus(offerGuid, OfferStatus.Actived)).Returns(true);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.SetStatus(offerGuid, OfferStatus.Actived), Times.Once);
        }

        [Fact]
        public async Task DelOrderEH_Handle_WhenOfferIsActivedOrOther_DoesNotSetStatusAndReturnsFalse()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelOrderEvent>>();
            var handler = new DelOrderEH(mockLogger.Object, _mockRepo.Object);

            var offerGuid = Guid.NewGuid();
            var orderGuid = Guid.NewGuid();
            var @event = new DelOrderEvent("Order Cancelled", orderGuid, offerGuid);

            var offer = new Offer
            {
                Guid = offerGuid,
                Status = OfferStatus.Actived
            };

            _mockRepo.Setup(r => r.GetEntity(offerGuid)).Returns(offer);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.SetStatus(It.IsAny<Guid>(), It.IsAny<OfferStatus>()), Times.Never);
        }

        [Fact]
        public async Task DelOrderEH_Handle_WhenOfferNotFound_ReturnsFalseSafelyWithoutThrowing()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelOrderEvent>>();
            var handler = new DelOrderEH(mockLogger.Object, _mockRepo.Object);

            var offerGuid = Guid.NewGuid();
            var orderGuid = Guid.NewGuid();
            var @event = new DelOrderEvent("Order Cancelled", orderGuid, offerGuid);

            _mockRepo.Setup(r => r.GetEntity(offerGuid)).Returns((Offer)null!);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.SetStatus(It.IsAny<Guid>(), It.IsAny<OfferStatus>()), Times.Never);
        }

        #endregion

        #region DelUserEH Tests

        [Fact]
        public async Task DelUserEH_Handle_WhenUserHasMatchingOffers_DeletesAllMatchingOffers()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelUserEvent>>();
            var handler = new DelUserEH(mockLogger.Object, _mockRepo.Object);

            var userGuid = Guid.NewGuid();
            var otherUserGuid = Guid.NewGuid();

            var userOffer1 = new Offer { Guid = Guid.NewGuid(), MasterGuid = userGuid };
            var userOffer2 = new Offer { Guid = Guid.NewGuid(), MasterGuid = userGuid };
            var otherOffer = new Offer { Guid = Guid.NewGuid(), MasterGuid = otherUserGuid };

            var offers = new List<Offer> { userOffer1, userOffer2, otherOffer };

            _mockRepo.Setup(r => r.GetEntities()).Returns(offers);
            _mockRepo.Setup(r => r.DelEntity(It.IsAny<Guid>())).Returns(true);

            var @event = new DelUserEvent("John", userGuid.ToString(), "token123");

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.DelEntity(userOffer1.Guid), Times.Once);
            _mockRepo.Verify(r => r.DelEntity(userOffer2.Guid), Times.Once);
            _mockRepo.Verify(r => r.DelEntity(otherOffer.Guid), Times.Never);
        }

        [Fact]
        public async Task DelUserEH_Handle_WhenUserHasNoMatchingOffers_DoesNotCallDelete()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelUserEvent>>();
            var handler = new DelUserEH(mockLogger.Object, _mockRepo.Object);

            var userGuid = Guid.NewGuid();
            var otherOffer = new Offer { Guid = Guid.NewGuid(), MasterGuid = Guid.NewGuid() };

            var offers = new List<Offer> { otherOffer };

            _mockRepo.Setup(r => r.GetEntities()).Returns(offers);

            var @event = new DelUserEvent("Jane", userGuid.ToString(), "token123");

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.DelEntity(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DelUserEH_Handle_WhenGetEntitiesReturnsNull_HandlesSafely()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelUserEvent>>();
            var handler = new DelUserEH(mockLogger.Object, _mockRepo.Object);

            var userGuid = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetEntities()).Returns((List<Offer>)null!);

            var @event = new DelUserEvent("Jane", userGuid.ToString(), "token123");

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.DelEntity(It.IsAny<Guid>()), Times.Never);
        }

        #endregion

        #region GetInfoFromCompany Tests

        [Fact]
        public async Task GetInfoFromCompany_Handle_CallsSetInfoFromCompanyOnRepository()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SendInfoForOffer>>();
            var handler = new GetInfoFromCompany(mockLogger.Object, _mockRepo.Object);

            var offerGuid = Guid.NewGuid();
            var @event = new SendInfoForOffer(offerGuid, "Beauty Lounge", 50.4501, 30.5234);

            _mockRepo.Setup(r => r.SetInfoFromCompany(@event)).Returns(true);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.SetInfoFromCompany(@event), Times.Once);
        }

        #endregion
    }
}
