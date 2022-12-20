using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyService.Abstractions;
using CompanyService.Models;
using CompanyService.Protos;
using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.CompanyEvents;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MobileApiGetway;

namespace CompanyService.Services
{
    public class CompanyServiceImp : Protos.Company.CompanyBase
    {
        private readonly ILogger<CompanyServiceImp> _logger;
        private readonly IEventBus _eventBus;
        private readonly ICompanyRepoService _companyRepoService;

        public CompanyServiceImp(ILogger<CompanyServiceImp> logger, IEventBus eventBus, ICompanyRepoService companyRepoService)
        {
            _logger = logger;
            _eventBus = eventBus;
            _companyRepoService = companyRepoService;
        }

        public override Task<AddCompanyReply> AddCompany(AddCompanyRequest request, ServerCallContext context)
        {
            var company = new Models.Company() { Name = request.Name, OwnerGuid = new Guid(request.OwnerGuid), OwnerName = request.OwnerName };
            var result = _companyRepoService.AddEntity(company);
            if (result)
                _eventBus.Publish(new AddCompanyEvent(company.Name, company.Guid, company.OwnerGuid));
            return Task.FromResult(new AddCompanyReply { Result = result });
        }
        public override Task<AddMasterReply> AddMaster(AddMasterRequest request, ServerCallContext context)
        {
            var reply = new AddMasterReply();
            var companyGuid = new Guid(request.Master.CompanyGuid);
            var masterGuid = new Guid(request.Master.UidFB);
            reply.Result = _companyRepoService.JoinToCompany(companyGuid, masterGuid, request.Master.Name);
            if (reply.Result)
                _eventBus.Publish(new AddMasterEvent(companyGuid, masterGuid, request.Master.CompanyName));
            return Task.FromResult(reply);
        }

        public override Task<DelMasterReply> DelMaster(DelMasterRequest request, ServerCallContext context)
        {
            var reply = new DelMasterReply();
            var companyGuid = new Guid(request.Master.CompanyGuid);
            var masterUIDFB = new Guid(request.Master.UidFB);
            reply.Result = _companyRepoService.DelWorker(companyGuid, masterUIDFB);
            if (reply.Result)
                _eventBus.Publish(new DelMasterEvent(companyGuid, masterUIDFB, request.Master.CompanyName));
            return Task.FromResult(reply);
        }

        public override Task<GetCompanyReply> GetCompany(GetCompanyRequest request, ServerCallContext context)
        {
            var reply = new GetCompanyReply();
            if (String.IsNullOrEmpty(request.Guid)) return Task.FromResult(reply);

            var company = _companyRepoService.GetEntity(new Guid(request.Guid), nameof(Models.Company.Workers));
            if (company == null) return Task.FromResult(reply);
            reply.Company.Guid = company.Guid.ToString();
            reply.Company.Name = company.Name;
            reply.Company.OwnerGuid = company.OwnerGuid.ToString();
            reply.Company.OwnerName = company.OwnerName;
            reply.Company.Lat = company.Lat?.ToString() ?? String.Empty;
            reply.Company.Lng = company.Lng?.ToString() ?? String.Empty;

            foreach (var worker in company.Workers)
                reply.Company.Masters.Add(new MasterReply() { UidFB = worker.RefGuid.ToString() });
            return Task.FromResult(reply);
        }
        public override Task<GetCompaniesReply> GetCompanies(GetCompaniesRequest request, ServerCallContext context)
        {
            var fitCompanies = new List<Models.Company>();
            var guid = new Guid(request.UserUIDFB);
            var totalCompanies = _companyRepoService.GetEntities(nameof(Models.Company.Workers) /*nameof(Models.Company.Offers)*/);
            switch (request.Type.ToLower())
            {
                case "owner":
                    //fitCompanies = totalCompanies.Where(x => x.OwnerGuid == guid).ToList(); break;
                    fitCompanies = _companyRepoService.GetCompaniesByOwner(guid);break;
                case "forOffer":
                    fitCompanies = totalCompanies.Where(x => x.OwnerGuid == guid || x.Workers.Any(m => m.RefGuid == guid)).ToList(); break;
                case "contains":
                    fitCompanies = totalCompanies.Where(x => x.Workers.Any(m => m.RefGuid == guid)).ToList(); break;
                case "canbecontains":
                    fitCompanies = totalCompanies.Where(x => x.OwnerGuid != guid && !x.Workers.Any(x => x.RefGuid == guid)).ToList(); break;
            }
            var reply = new GetCompaniesReply();
            foreach (var company in fitCompanies)
            {
                var companyReply = new CompanyReply()
                {
                    Guid = company.Guid.ToString(),
                    Name = company.Name,
                    OwnerGuid = company.OwnerGuid.ToString(),
                    OwnerName = company.OwnerName,
                    Lat = company.Lat.ToString(),
                    Lng = company.Lng.ToString(),
                };
                foreach (var worker in company.Workers)
                {
                    var master = new MasterReply()
                    {
                        UidFB = worker.RefGuid.ToString(),
                        Name = worker.Name,
                        CompanyGuid = company.Guid.ToString(),
                        CompanyName = company.Name,
                    };
                    foreach (var offer in worker.Offers)
                        master.Offers.Add(new OfferApi()
                        {
                            Guid = offer.RefGuid.ToString(),
                            Name = offer.Name,
                            Desc = String.Empty
                        });
                    companyReply.Masters.Add(master);
                }
                reply.Companies.Add(companyReply);
            }
            return Task.FromResult(reply);
        }
        public override Task<JoinToCompanyReply> JoinToCompany(JoinToCompanyRequest request, ServerCallContext context)
        {
            var reply = new JoinToCompanyReply();
            var companyGuid = new Guid(request.CompanyGuid);
            var masterGuid = new Guid(request.UserUIDFB);
            reply.Result = _companyRepoService.JoinToCompany(companyGuid, masterGuid, request.UserName);
            if(reply.Result)
                _eventBus.Publish(new AddMasterEvent(companyGuid, masterGuid, request.CompanyName));
            return Task.FromResult(reply);
        }
        public override Task<UpdateCompanyReply> UpdateCompany(UpdateCompanyRequest request, ServerCallContext context)
        {
            return base.UpdateCompany(request, context);
        }
        public override Task<DelCompanyReply> DelCompany(DelCompanyRequest request, ServerCallContext context)
        {
            var companyGuid = new Guid(request.Guid);
            var result = _companyRepoService.DelEntity(companyGuid);
            if (result)
                _eventBus.Publish(new DelCompanyEvent(companyGuid));
            return Task.FromResult(new DelCompanyReply { Result = result });
        }

        public override Task<SetCompanyLocationReply> SetCompanyLocation(SetCompanyLocationRequest request, ServerCallContext context)
        {
            var ressult = _companyRepoService.SetCompanyLocation(new Guid(request.Guid), request.Lat, request.Lng);
            return Task.FromResult(new SetCompanyLocationReply { Result = ressult });

        }
    }
}
