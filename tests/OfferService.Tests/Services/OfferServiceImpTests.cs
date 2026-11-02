using System;
using System.Threading.Tasks;
using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.OfferEvents;
using FluentAssertions;
using Globals.Tests;
using Microsoft.Extensions.Logging;
using MobileApiGetway;
using Moq;
using OfferService.Abstractions;
using OfferService.Models;
using OfferService.Services;
using Xunit;
using Offer = OfferService.Models.Offer;

namespace OfferService.Tests.Services
{
    public class OfferServiceImpTests
    {
        private readonly Mock<ILogger<OfferServiceImp>> _mockLogger;
        private readonly Mock<IEventBus> _mockEventBus;
        private readonly Mock<IOfferRepoService> _mockRepo;
        private readonly TestServerCallContext _callContext;
        private readonly OfferServiceImp _service;

        public OfferServiceImpTests()
        {
            _mockLogger = new Mock<ILogger<OfferServiceImp>>();
            _mockEventBus = new Mock<IEventBus>();
            _mockRepo = new Mock<IOfferRepoService>();
            _callContext = new TestServerCallContext();

            _service = new OfferServiceImp(
                _mockLogger.Object,
                _mockEventBus.Object,
                _mockRepo.Object);
        }

        #region AddOffer Tests

        [Fact]
        public async Task AddOffer_WhenValid_PersistsEntityAndPublishesAddOfferEvent()
        {
            // Arrange
            var companyGuid = Guid.NewGuid();
            var masterGuid = Guid.NewGuid();
            var skillGuid = Guid.NewGuid();
            var generatedOfferGuid = Guid.NewGuid();

            var request = new AddOfferRequest
            {
                Offer = new OfferApi
                {
                    Name = "Haircut & Beard Trim",
                    CompanyGuid = companyGuid.ToString(),
                    CompanyName = "Top Barber",
                    MasterGuid = masterGuid.ToString(),
                    MasterName = "Alex",
                    SkillGuid = skillGuid.ToString(),
                    SkillName = "Barbering"
                }
            };

            _mockRepo.Setup(r => r.AddEntity(It.IsAny<Offer>()))
                .Callback<Offer>(o => o.Guid = generatedOfferGuid)
                .Returns(true);

            // Act
            var reply = await _service.AddOffer(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Result.Should().BeTrue();

            _mockRepo.Verify(r => r.AddEntity(It.Is<Offer>(o =>
                o.Name == "Haircut & Beard Trim" &&
                o.CompanyGuid == companyGuid &&
                o.CompanyName == "Top Barber" &&
                o.MasterGuid == masterGuid &&
                o.MasterName == "Alex" &&
                o.SkillGuid == skillGuid &&
                o.SkillName == "Barbering")), Times.Once);

            _mockEventBus.Verify(b => b.Publish(It.Is<AddOfferEvent>(e =>
                e.Name == "Haircut & Beard Trim" &&
                e.Guid == generatedOfferGuid &&
                e.CompanyGuid == companyGuid &&
                e.MasterGuid == masterGuid)), Times.Once);
        }

        [Fact]
        public async Task AddOffer_WhenRepositoryFails_DoesNotPublishEventAndReturnsFalse()
        {
            // Arrange
            var request = new AddOfferRequest
            {
                Offer = new OfferApi
                {
                    Name = "Haircut",
                    CompanyGuid = Guid.NewGuid().ToString(),
                    CompanyName = "Company",
                    MasterGuid = Guid.NewGuid().ToString(),
                    MasterName = "Master",
                    SkillGuid = Guid.NewGuid().ToString(),
                    SkillName = "Skill"
                }
            };

            _mockRepo.Setup(r => r.AddEntity(It.IsAny<Offer>()))
                .Returns(false);

            // Act
            var reply = await _service.AddOffer(request, _callContext);

            // Assert
            reply.Result.Should().BeFalse();
            _mockEventBus.Verify(b => b.Publish(It.IsAny<AddOfferEvent>()), Times.Never);
        }

        [Fact]
        public async Task AddOffer_WhenGuidInvalid_ThrowsFormatException()
        {
            // Arrange
            var request = new AddOfferRequest
            {
                Offer = new OfferApi
                {
                    Name = "Haircut",
                    CompanyGuid = "invalid-guid",
                    CompanyName = "Company",
                    MasterGuid = Guid.NewGuid().ToString(),
                    MasterName = "Master",
                    SkillGuid = Guid.NewGuid().ToString(),
                    SkillName = "Skill"
                }
            };

            // Act
            var act = async () => await _service.AddOffer(request, _callContext);

            // Assert
            await act.Should().ThrowAsync<FormatException>();
            _mockRepo.Verify(r => r.AddEntity(It.IsAny<Offer>()), Times.Never);
        }

        #endregion

        #region GetOffer Tests

        [Fact]
        public async Task GetOffer_WhenOfferExists_ReturnsPopulatedReply()
        {
            // Arrange
            var offerGuid = Guid.NewGuid();
            var companyGuid = Guid.NewGuid();
            var masterGuid = Guid.NewGuid();
            var skillGuid = Guid.NewGuid();
            var orderGuid = Guid.NewGuid();

            var entity = new Offer
            {
                Guid = offerGuid,
                Name = "Spa Treatment",
                Description = "Luxury relax spa package",
                CompanyGuid = companyGuid,
                CompanyName = "Grand Spa",
                MasterGuid = masterGuid,
                MasterName = "Elena",
                SkillGuid = skillGuid,
                SkillName = "Massage",
                OrderGuid = orderGuid,
                OrderName = "Order #1024",
                Status = OfferStatus.Actived
            };

            _mockRepo.Setup(r => r.GetEntity(offerGuid))
                .Returns(entity);

            var request = new GetOfferRequest { Guid = offerGuid.ToString() };

            // Act
            var reply = await _service.GetOffer(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Result.Should().BeTrue();
            reply.Offer.Should().NotBeNull();
            reply.Offer.Guid.Should().Be(offerGuid.ToString());
            reply.Offer.Name.Should().Be("Spa Treatment");
            reply.Offer.Desc.Should().Be("Luxury relax spa package");
            reply.Offer.CompanyGuid.Should().Be(companyGuid.ToString());
            reply.Offer.CompanyName.Should().Be("Grand Spa");
            reply.Offer.MasterGuid.Should().Be(masterGuid.ToString());
            reply.Offer.MasterName.Should().Be("Elena");
            reply.Offer.SkillGuid.Should().Be(skillGuid.ToString());
            reply.Offer.SkillName.Should().Be("Massage");
            reply.Offer.OrderGuid.Should().Be(orderGuid.ToString());
            reply.Offer.OrderName.Should().Be("Order #1024");
            reply.Offer.Status.Should().Be("Actived");
        }

        [Fact]
        public async Task GetOffer_WhenOfferNotFound_ReturnsResultFalse()
        {
            // Arrange
            var offerGuid = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetEntity(offerGuid))
                .Returns((Offer)null!);

            var request = new GetOfferRequest { Guid = offerGuid.ToString() };

            // Act
            var reply = await _service.GetOffer(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Result.Should().BeFalse();
            reply.Offer.Should().BeNull();
        }

        [Fact]
        public async Task GetOffer_WhenGuidInvalid_ThrowsFormatException()
        {
            // Arrange
            var request = new GetOfferRequest { Guid = "not-a-valid-guid" };

            // Act
            var act = async () => await _service.GetOffer(request, _callContext);

            // Assert
            await act.Should().ThrowAsync<FormatException>();
        }

        #endregion

        #region GetOffers Tests

        [Fact]
        public async Task GetOffers_WhenOffersExist_ReturnsPopulatedOffersList()
        {
            // Arrange
            var offer1Guid = Guid.NewGuid();
            var offer2Guid = Guid.NewGuid();

            var offer1 = new Offer
            {
                Guid = offer1Guid,
                Name = "Service 1",
                CompanyGuid = Guid.NewGuid(),
                MasterGuid = Guid.NewGuid(),
                SkillGuid = Guid.NewGuid(),
                Status = OfferStatus.Actived
            };
            var offer2 = new Offer
            {
                Guid = offer2Guid,
                Name = "Service 2",
                CompanyGuid = Guid.NewGuid(),
                MasterGuid = Guid.NewGuid(),
                SkillGuid = Guid.NewGuid(),
                Status = OfferStatus.Submitted
            };

            _mockRepo.Setup(r => r.GetEntity(offer1Guid)).Returns(offer1);
            _mockRepo.Setup(r => r.GetEntity(offer2Guid)).Returns(offer2);

            var request = new GetOffersRequest();
            request.Guids.Add(offer1Guid.ToString());
            request.Guids.Add(offer2Guid.ToString());

            // Act
            var reply = await _service.GetOffers(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Offers.Should().HaveCount(2);
            reply.Offers[0].Guid.Should().Be(offer1Guid.ToString());
            reply.Offers[0].Name.Should().Be("Service 1");
            reply.Offers[1].Guid.Should().Be(offer2Guid.ToString());
            reply.Offers[1].Name.Should().Be("Service 2");
        }

        [Fact]
        public async Task GetOffers_WhenSomeOffersNotFound_SkipsMissingOffers()
        {
            // Arrange
            var offer1Guid = Guid.NewGuid();
            var missingGuid = Guid.NewGuid();

            var offer1 = new Offer
            {
                Guid = offer1Guid,
                Name = "Service 1",
                CompanyGuid = Guid.NewGuid(),
                MasterGuid = Guid.NewGuid(),
                SkillGuid = Guid.NewGuid(),
                Status = OfferStatus.Actived
            };

            _mockRepo.Setup(r => r.GetEntity(offer1Guid)).Returns(offer1);
            _mockRepo.Setup(r => r.GetEntity(missingGuid)).Returns((Offer)null!);

            var request = new GetOffersRequest();
            request.Guids.Add(offer1Guid.ToString());
            request.Guids.Add(missingGuid.ToString());

            // Act
            var reply = await _service.GetOffers(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Offers.Should().HaveCount(1);
            reply.Offers[0].Guid.Should().Be(offer1Guid.ToString());
        }

        [Fact]
        public async Task GetOffers_WhenGuidsEmpty_ReturnsEmptyList()
        {
            // Arrange
            var request = new GetOffersRequest();

            // Act
            var reply = await _service.GetOffers(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Offers.Should().BeEmpty();
            _mockRepo.Verify(r => r.GetEntity(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetOffers_WhenGuidInvalid_ThrowsFormatException()
        {
            // Arrange
            var request = new GetOffersRequest();
            request.Guids.Add("invalid-guid");

            // Act
            var act = async () => await _service.GetOffers(request, _callContext);

            // Assert
            await act.Should().ThrowAsync<FormatException>();
        }

        #endregion

        #region DelOffer Tests

        [Fact]
        public async Task DelOffer_WhenValid_DeletesEntityAndPublishesDelOfferEventWithOfferGuid()
        {
            // Arrange
            var offerGuid = Guid.NewGuid();
            var companyGuid = Guid.NewGuid();
            var masterGuid = Guid.NewGuid();

            var request = new DelOfferRequest
            {
                Offer = new OfferApi
                {
                    Guid = offerGuid.ToString(),
                    Name = "Manicure",
                    CompanyGuid = companyGuid.ToString(),
                    MasterGuid = masterGuid.ToString()
                }
            };

            _mockRepo.Setup(r => r.DelEntity(offerGuid)).Returns(true);

            // Act
            var reply = await _service.DelOffer(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Result.Should().BeTrue();

            _mockRepo.Verify(r => r.DelEntity(offerGuid), Times.Once);

            // Verify that DelOfferEvent receives the offer's Guid (not companyGuid!)
            _mockEventBus.Verify(b => b.Publish(It.Is<DelOfferEvent>(e =>
                e.Name == "Manicure" &&
                e.Guid == offerGuid &&
                e.MasterGuid == masterGuid)), Times.Once);
        }

        [Fact]
        public async Task DelOffer_WhenRepositoryFails_DoesNotPublishEventAndReturnsFalse()
        {
            // Arrange
            var offerGuid = Guid.NewGuid();
            var request = new DelOfferRequest
            {
                Offer = new OfferApi
                {
                    Guid = offerGuid.ToString(),
                    Name = "Pedicure",
                    CompanyGuid = Guid.NewGuid().ToString(),
                    MasterGuid = Guid.NewGuid().ToString()
                }
            };

            _mockRepo.Setup(r => r.DelEntity(offerGuid)).Returns(false);

            // Act
            var reply = await _service.DelOffer(request, _callContext);

            // Assert
            reply.Result.Should().BeFalse();
            _mockEventBus.Verify(b => b.Publish(It.IsAny<DelOfferEvent>()), Times.Never);
        }

        [Fact]
        public async Task DelOffer_WhenGuidInvalid_ThrowsFormatException()
        {
            // Arrange
            var request = new DelOfferRequest
            {
                Offer = new OfferApi
                {
                    Guid = "invalid-guid",
                    Name = "Service",
                    CompanyGuid = Guid.NewGuid().ToString(),
                    MasterGuid = Guid.NewGuid().ToString()
                }
            };

            // Act
            var act = async () => await _service.DelOffer(request, _callContext);

            // Assert
            await act.Should().ThrowAsync<FormatException>();
            _mockRepo.Verify(r => r.DelEntity(It.IsAny<Guid>()), Times.Never);
        }

        #endregion
    }
}
