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

        public override Task<GetCompanyReply> GetCompany(GetCompanyRequest request, ServerCallContext context)
        {
            var reply = new GetCompanyReply();
            var company = _companyRepoService.GetEntity(new Guid(request.Guid), nameof(Models.Company.Masters));
            if (company == null) return Task.FromResult(reply);
            reply.Guid = company.Guid.ToString();
            reply.Name = company.Name;
            reply.OwnerGuid = company.OwnerGuid.ToString();
            reply.OwnerName = company.OwnerName;

            foreach (var master in company.Masters)
            {
                reply.MasterGuids.Add(master.RefGuid.ToString());
                reply.MasterNames.Add(master.Name);
            }

            return Task.FromResult(reply);
        }
        public override Task<GetCompaniesReply> GetCompanies(GetCompaniesRequest request, ServerCallContext context)
        {
            var fitCompanies = new List<Models.Company>();
            var guid = new Guid(request.UserGuid);
            var totalCompanies = _companyRepoService.GetEntities(nameof(Models.Company.Masters));
            switch (request.Type.ToLower())
            {
                case "owner":
                    fitCompanies = totalCompanies.Where(x => x.OwnerGuid == guid).ToList(); break;
                case "forOffer":
                    fitCompanies = totalCompanies.Where(x => x.OwnerGuid == guid || x.Masters.Any(m => m.RefGuid == guid)).ToList(); break;
                case "contains":
                    fitCompanies = totalCompanies.Where(x => x.Masters.Any(m => m.RefGuid == guid)).ToList(); break;
                case "canbecontains":
                    fitCompanies = totalCompanies.Where(x => x.OwnerGuid != guid && !x.Masters.Any(x => x.RefGuid == guid)).ToList(); break;
            }
            var reply = new GetCompaniesReply();
            reply.Guids.AddRange(fitCompanies.Select(x => x.Guid.ToString()));
            reply.Names.AddRange(fitCompanies.Select(x => x.Name));
            return Task.FromResult(reply);
        }
        public override Task<AddCompanyReply> AddCompany(AddCompanyRequest request, ServerCallContext context)
        {
            var company = new Models.Company() { Name = request.Name, OwnerGuid = new Guid(request.OwnerGuid), OwnerName = request.OwnerName };
            var result = _companyRepoService.AddEntity(company);
            if (result)
                _eventBus.Publish(new AddCompanyEvent(company.Name, company.Guid, company.OwnerGuid));
            return Task.FromResult(new AddCompanyReply { Result = result });
        }
        public override Task<JoinToCompanyReply> JoinToCompany(JoinToCompanyRequest request, ServerCallContext context)
        {
            var reply = new JoinToCompanyReply();
            var companyGuid = new Guid(request.CompanyGuid);
            var masterGuid = new Guid(request.UserGuid);
            reply.Result = _companyRepoService.JoinToCompany(companyGuid, masterGuid, request.UserName);
            if(reply.Result)
                _eventBus.Publish(new JoinToCompanyEvent(companyGuid, masterGuid, request.CompanyName));
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
    }
}
