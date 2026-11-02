using CompanyService.Abstractions;
using Company = CompanyService.Models.Company;
using CompanyService.Models;
using CompanyService.Protos;
using CompanyService.Services;
using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.CompanyEvents;
using FluentAssertions;
using Globals.Tests;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MobileApiGetway;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CompanyService.Tests.Services
{
    public class CompanyServiceImpTests
    {
        private readonly Mock<ILogger<CompanyServiceImp>> _mockLogger = new();
        private readonly Mock<IEventBus> _mockEventBus = new();
        private readonly Mock<ICompanyRepoService> _mockRepo = new();

        private readonly CompanyServiceImp _service;
        private readonly TestServerCallContext _context;

        public CompanyServiceImpTests()
        {
            _service = new CompanyServiceImp(_mockLogger.Object, _mockEventBus.Object, _mockRepo.Object);
            _context = TestServerCallContext.Create();
        }

        // ==========================================
        // 1. AddCompany Tests
        // ==========================================

        [Fact]
        public async Task AddCompany_WhenValid_PersistsAndPublishesAddCompanyEvent()
        {
            // Arrange
            var ownerGuid = Guid.NewGuid();
            var request = new AddCompanyRequest
            {
                Name = "Glow Studio",
                OwnerGuid = ownerGuid.ToString(),
                OwnerName = "Alice Owner"
            };

            _mockRepo
                .Setup(r => r.AddEntity(It.Is<Company>(c =>
                    c.Name == "Glow Studio" &&
                    c.OwnerGuid == ownerGuid &&
                    c.OwnerName == "Alice Owner")))
                .Returns(true);

            // Act
            var reply = await _service.AddCompany(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.Result.Should().BeTrue();

            _mockRepo.Verify(r => r.AddEntity(It.IsAny<Company>()), Times.Once);
            _mockEventBus.Verify(b => b.Publish(It.Is<AddCompanyEvent>(e =>
                e.Name == "Glow Studio" &&
                e.Owner == ownerGuid
            )), Times.Once);
        }

        [Fact]
        public async Task AddCompany_WhenPersistFails_DoesNotPublishEvent()
        {
            // Arrange
            var ownerGuid = Guid.NewGuid();
            var request = new AddCompanyRequest
            {
                Name = "Failed Salon",
                OwnerGuid = ownerGuid.ToString(),
                OwnerName = "Bob"
            };

            _mockRepo.Setup(r => r.AddEntity(It.IsAny<Company>())).Returns(false);

            // Act
            var reply = await _service.AddCompany(request, _context);

            // Assert
            reply.Result.Should().BeFalse();
            _mockEventBus.Verify(b => b.Publish(It.IsAny<AddCompanyEvent>()), Times.Never);
        }

        [Fact]
        public async Task AddCompany_WhenOwnerGuidInvalid_ThrowsFormatException()
        {
            // Arrange
            var request = new AddCompanyRequest
            {
                Name = "Invalid Guid Salon",
                OwnerGuid = "not-a-guid",
                OwnerName = "Bob"
            };

            // Act
            Func<Task> act = async () => await _service.AddCompany(request, _context);

            // Assert
            await act.Should().ThrowAsync<FormatException>();
            _mockRepo.Verify(r => r.AddEntity(It.IsAny<Company>()), Times.Never);
            _mockEventBus.Verify(b => b.Publish(It.IsAny<AddCompanyEvent>()), Times.Never);
        }

        // ==========================================
        // 2. AddMaster Tests
        // ==========================================

        [Fact]
        public async Task AddMaster_WhenValid_JoinsAndPublishesAddMasterEvent()
        {
            // Arrange
            var companyGuid = Guid.NewGuid();
            var masterGuid = Guid.NewGuid();
            var request = new AddMasterRequest
            {
                Master = new MasterReply
                {
                    CompanyGuid = companyGuid.ToString(),
                    UidFB = masterGuid.ToString(),
                    Name = "Master Jane",
                    CompanyName = "Elite Cuts"
                }
            };

            _mockRepo
                .Setup(r => r.JoinToCompany(companyGuid, masterGuid, "Master Jane"))
                .Returns(true);

            // Act
            var reply = await _service.AddMaster(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.Result.Should().BeTrue();

            _mockRepo.Verify(r => r.JoinToCompany(companyGuid, masterGuid, "Master Jane"), Times.Once);
            _mockEventBus.Verify(b => b.Publish(It.Is<AddMasterEvent>(e =>
                e.CompanyGuid == companyGuid &&
                e.MasterGuid == masterGuid &&
                e.CompanyName == "Elite Cuts"
            )), Times.Once);
        }

        [Fact]
        public async Task AddMaster_WhenJoinFails_DoesNotPublishEvent()
        {
            // Arrange
            var companyGuid = Guid.NewGuid();
            var masterGuid = Guid.NewGuid();
            var request = new AddMasterRequest
            {
                Master = new MasterReply
                {
                    CompanyGuid = companyGuid.ToString(),
                    UidFB = masterGuid.ToString(),
                    Name = "Master Jane",
                    CompanyName = "Elite Cuts"
                }
            };

            _mockRepo.Setup(r => r.JoinToCompany(companyGuid, masterGuid, "Master Jane")).Returns(false);

            // Act
            var reply = await _service.AddMaster(request, _context);

            // Assert
            reply.Result.Should().BeFalse();
            _mockEventBus.Verify(b => b.Publish(It.IsAny<AddMasterEvent>()), Times.Never);
        }

        // ==========================================
        // 3. DelMaster Tests
        // ==========================================

        [Fact]
        public async Task DelMaster_WhenValid_DelsWorkerAndPublishesDelMasterEvent()
        {
            // Arrange
            var companyGuid = Guid.NewGuid();
            var masterGuid = Guid.NewGuid();
            var request = new DelMasterRequest
            {
                Master = new MasterReply
                {
                    CompanyGuid = companyGuid.ToString(),
                    UidFB = masterGuid.ToString(),
                    CompanyName = "Elite Cuts"
                }
            };

            _mockRepo
                .Setup(r => r.DelWorker(companyGuid, masterGuid))
                .Returns(true);

            // Act
            var reply = await _service.DelMaster(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.Result.Should().BeTrue();

            _mockRepo.Verify(r => r.DelWorker(companyGuid, masterGuid), Times.Once);
            _mockEventBus.Verify(b => b.Publish(It.Is<DelMasterEvent>(e =>
                e.CompanyGuid == companyGuid &&
                e.MasterUIDFB == masterGuid &&
                e.CompanyName == "Elite Cuts"
            )), Times.Once);
        }

        [Fact]
        public async Task DelMaster_WhenDelWorkerFails_DoesNotPublishEvent()
        {
            // Arrange
            var companyGuid = Guid.NewGuid();
            var masterGuid = Guid.NewGuid();
            var request = new DelMasterRequest
            {
                Master = new MasterReply
                {
                    CompanyGuid = companyGuid.ToString(),
                    UidFB = masterGuid.ToString(),
                    CompanyName = "Elite Cuts"
                }
            };

            _mockRepo.Setup(r => r.DelWorker(companyGuid, masterGuid)).Returns(false);

            // Act
            var reply = await _service.DelMaster(request, _context);

            // Assert
            reply.Result.Should().BeFalse();
            _mockEventBus.Verify(b => b.Publish(It.IsAny<DelMasterEvent>()), Times.Never);
        }

        // ==========================================
        // 4. GetCompany Tests
        // ==========================================

        [Fact]
        public async Task GetCompany_WhenGuidEmpty_ReturnsEmptyReplyImmediately()
        {
            // Act
            var reply = await _service.GetCompany(new GetCompanyRequest { Guid = "" }, _context);

            // Assert
            reply.Should().NotBeNull();
            _mockRepo.Verify(r => r.GetEntity(It.IsAny<Guid>(), It.IsAny<string[]>()), Times.Never);
        }

        [Fact]
        public async Task GetCompany_WhenCompanyNotFound_ReturnsEmptyReply()
        {
            // Arrange
            var guid = Guid.NewGuid();
            _mockRepo
                .Setup(r => r.GetEntity(guid, nameof(Company.Workers)))
                .Returns((Company)null!);

            // Act
            var reply = await _service.GetCompany(new GetCompanyRequest { Guid = guid.ToString() }, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.Company.Should().BeNull();
        }

        [Fact]
        public async Task GetCompany_WhenCompanyFound_ReturnsPopulatedReplySafely()
        {
            // Arrange
            var companyGuid = Guid.NewGuid();
            var ownerGuid = Guid.NewGuid();
            var workerGuid = Guid.NewGuid();

            var company = new Company
            {
                Guid = companyGuid,
                Name = "Glow Studio",
                OwnerGuid = ownerGuid,
                OwnerName = "Owner Alice",
                Lat = 50.4501,
                Lng = 30.5234
            };
            company.Workers.Add(new CompanyUserRef { RefGuid = workerGuid, Name = "Worker Jane" });

            _mockRepo
                .Setup(r => r.GetEntity(companyGuid, nameof(Company.Workers)))
                .Returns(company);

            // Act
            var reply = await _service.GetCompany(new GetCompanyRequest { Guid = companyGuid.ToString() }, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.Company.Should().NotBeNull();
            reply.Company.Guid.Should().Be(companyGuid.ToString());
            reply.Company.Name.Should().Be("Glow Studio");
            reply.Company.OwnerGuid.Should().Be(ownerGuid.ToString());
            reply.Company.OwnerName.Should().Be("Owner Alice");
            reply.Company.Lat.Should().Be(company.Lat.ToString());
            reply.Company.Lng.Should().Be(company.Lng.ToString());
            reply.Company.Masters.Should().HaveCount(1);
            reply.Company.Masters[0].UidFB.Should().Be(workerGuid.ToString());
        }

        // ==========================================
        // 5. GetCompanies Tests
        // ==========================================

        [Fact]
        public async Task GetCompanies_WhenTypeIsOwner_QueriesCompaniesByOwner()
        {
            // Arrange
            var ownerGuid = Guid.NewGuid();
            var workerGuid = Guid.NewGuid();
            var offerGuid = Guid.NewGuid();

            var company = new Company
            {
                Guid = Guid.NewGuid(),
                Name = "Owned Salon",
                OwnerGuid = ownerGuid,
                OwnerName = "Owner",
                Lat = 50.1,
                Lng = 30.2
            };
            var worker = new CompanyUserRef { RefGuid = workerGuid, Name = "Worker 1" };
            worker.Offers.Add(new CompanyOfferRef { RefGuid = offerGuid, Name = "Haircut" });
            company.Workers.Add(worker);

            _mockRepo
                .Setup(r => r.GetCompaniesByOwner(ownerGuid))
                .Returns(new List<Company> { company });

            var request = new GetCompaniesRequest
            {
                UserUIDFB = ownerGuid.ToString(),
                Type = "owner"
            };

            // Act
            var reply = await _service.GetCompanies(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.Companies.Should().HaveCount(1);
            reply.Companies[0].Name.Should().Be("Owned Salon");
            reply.Companies[0].Masters.Should().HaveCount(1);
            reply.Companies[0].Masters[0].Name.Should().Be("Worker 1");
            reply.Companies[0].Masters[0].Offers.Should().HaveCount(1);
            reply.Companies[0].Masters[0].Offers[0].Name.Should().Be("Haircut");
        }

        [Fact]
        public async Task GetCompanies_WhenTypeIsForOffer_FiltersCorrectly()
        {
            // Arrange
            var userGuid = Guid.NewGuid();
            var c1 = new Company { Guid = Guid.NewGuid(), Name = "C1", OwnerGuid = userGuid };
            var c2 = new Company { Guid = Guid.NewGuid(), Name = "C2", OwnerGuid = Guid.NewGuid() };
            c2.Workers.Add(new CompanyUserRef { RefGuid = userGuid, Name = "UserAsWorker" });
            var c3 = new Company { Guid = Guid.NewGuid(), Name = "C3", OwnerGuid = Guid.NewGuid() };

            _mockRepo
                .Setup(r => r.GetEntities(nameof(Company.Workers)))
                .Returns(new List<Company> { c1, c2, c3 });

            var request = new GetCompaniesRequest
            {
                UserUIDFB = userGuid.ToString(),
                Type = "forOffer"
            };

            // Act
            var reply = await _service.GetCompanies(request, _context);

            // Assert
            reply.Companies.Should().HaveCount(2);
            reply.Companies.Select(c => c.Name).Should().Contain(new[] { "C1", "C2" });
        }

        [Fact]
        public async Task GetCompanies_WhenTypeIsContains_FiltersCompaniesWhereUserIsWorker()
        {
            // Arrange
            var userGuid = Guid.NewGuid();
            var c1 = new Company { Guid = Guid.NewGuid(), Name = "C1", OwnerGuid = userGuid };
            var c2 = new Company { Guid = Guid.NewGuid(), Name = "C2", OwnerGuid = Guid.NewGuid() };
            c2.Workers.Add(new CompanyUserRef { RefGuid = userGuid, Name = "Worker" });

            _mockRepo
                .Setup(r => r.GetEntities(nameof(Company.Workers)))
                .Returns(new List<Company> { c1, c2 });

            var request = new GetCompaniesRequest
            {
                UserUIDFB = userGuid.ToString(),
                Type = "contains"
            };

            // Act
            var reply = await _service.GetCompanies(request, _context);

            // Assert
            reply.Companies.Should().HaveCount(1);
            reply.Companies[0].Name.Should().Be("C2");
        }

        [Fact]
        public async Task GetCompanies_WhenTypeIsCanBeContains_FiltersCompaniesWhereUserNotAssociated()
        {
            // Arrange
            var userGuid = Guid.NewGuid();
            var c1 = new Company { Guid = Guid.NewGuid(), Name = "OwnedCompany", OwnerGuid = userGuid };
            var c2 = new Company { Guid = Guid.NewGuid(), Name = "WorkerCompany", OwnerGuid = Guid.NewGuid() };
            c2.Workers.Add(new CompanyUserRef { RefGuid = userGuid });
            var c3 = new Company { Guid = Guid.NewGuid(), Name = "AvailableCompany", OwnerGuid = Guid.NewGuid() };

            _mockRepo
                .Setup(r => r.GetEntities(nameof(Company.Workers)))
                .Returns(new List<Company> { c1, c2, c3 });

            var request = new GetCompaniesRequest
            {
                UserUIDFB = userGuid.ToString(),
                Type = "canbecontains"
            };

            // Act
            var reply = await _service.GetCompanies(request, _context);

            // Assert
            reply.Companies.Should().HaveCount(1);
            reply.Companies[0].Name.Should().Be("AvailableCompany");
        }

        // ==========================================
        // 6. JoinToCompany Tests
        // ==========================================

        [Fact]
        public async Task JoinToCompany_WhenValid_CallsJoinAndPublishesAddMasterEvent()
        {
            // Arrange
            var companyGuid = Guid.NewGuid();
            var userGuid = Guid.NewGuid();
            var request = new JoinToCompanyRequest
            {
                CompanyGuid = companyGuid.ToString(),
                UserUIDFB = userGuid.ToString(),
                UserName = "Jane",
                CompanyName = "Beauty Salon"
            };

            _mockRepo
                .Setup(r => r.JoinToCompany(companyGuid, userGuid, "Jane"))
                .Returns(true);

            // Act
            var reply = await _service.JoinToCompany(request, _context);

            // Assert
            reply.Result.Should().BeTrue();
            _mockEventBus.Verify(b => b.Publish(It.Is<AddMasterEvent>(e =>
                e.CompanyGuid == companyGuid &&
                e.MasterGuid == userGuid &&
                e.CompanyName == "Beauty Salon"
            )), Times.Once);
        }

        [Fact]
        public async Task JoinToCompany_WhenFails_DoesNotPublishEvent()
        {
            // Arrange
            var companyGuid = Guid.NewGuid();
            var userGuid = Guid.NewGuid();
            var request = new JoinToCompanyRequest
            {
                CompanyGuid = companyGuid.ToString(),
                UserUIDFB = userGuid.ToString(),
                UserName = "Jane",
                CompanyName = "Beauty Salon"
            };

            _mockRepo.Setup(r => r.JoinToCompany(companyGuid, userGuid, "Jane")).Returns(false);

            // Act
            var reply = await _service.JoinToCompany(request, _context);

            // Assert
            reply.Result.Should().BeFalse();
            _mockEventBus.Verify(b => b.Publish(It.IsAny<AddMasterEvent>()), Times.Never);
        }

        // ==========================================
        // 7. UpdateCompany Test
        // ==========================================

        [Fact]
        public async Task UpdateCompany_CallsBaseMethod_ThrowsRpcException()
        {
            // Act
            Func<Task> act = async () => await _service.UpdateCompany(new UpdateCompanyRequest(), _context);

            // Assert
            var ex = await act.Should().ThrowAsync<RpcException>();
            ex.Which.StatusCode.Should().Be(StatusCode.Unimplemented);
        }

        // ==========================================
        // 8. DelCompany Tests
        // ==========================================

        [Fact]
        public async Task DelCompany_WhenValid_DeletesEntityAndPublishesDelCompanyEvent()
        {
            // Arrange
            var companyGuid = Guid.NewGuid();
            var request = new DelCompanyRequest { Guid = companyGuid.ToString() };

            _mockRepo.Setup(r => r.DelEntity(companyGuid)).Returns(true);

            // Act
            var reply = await _service.DelCompany(request, _context);

            // Assert
            reply.Result.Should().BeTrue();
            _mockRepo.Verify(r => r.DelEntity(companyGuid), Times.Once);
            _mockEventBus.Verify(b => b.Publish(It.Is<DelCompanyEvent>(e => e.Guid == companyGuid)), Times.Once);
        }

        [Fact]
        public async Task DelCompany_WhenDeleteFails_DoesNotPublishEvent()
        {
            // Arrange
            var companyGuid = Guid.NewGuid();
            var request = new DelCompanyRequest { Guid = companyGuid.ToString() };

            _mockRepo.Setup(r => r.DelEntity(companyGuid)).Returns(false);

            // Act
            var reply = await _service.DelCompany(request, _context);

            // Assert
            reply.Result.Should().BeFalse();
            _mockEventBus.Verify(b => b.Publish(It.IsAny<DelCompanyEvent>()), Times.Never);
        }

        // ==========================================
        // 9. SetCompanyLocation Tests
        // ==========================================

        [Fact]
        public async Task SetCompanyLocation_CallsRepoAndReturnsResult()
        {
            // Arrange
            var companyGuid = Guid.NewGuid();
            var request = new SetCompanyLocationRequest
            {
                Guid = companyGuid.ToString(),
                Lat = 50.4501,
                Lng = 30.5234
            };

            _mockRepo
                .Setup(r => r.SetCompanyLocation(companyGuid, 50.4501, 30.5234))
                .Returns(true);

            // Act
            var reply = await _service.SetCompanyLocation(request, _context);

            // Assert
            reply.Result.Should().BeTrue();
            _mockRepo.Verify(r => r.SetCompanyLocation(companyGuid, 50.4501, 30.5234), Times.Once);
        }
    }
}
