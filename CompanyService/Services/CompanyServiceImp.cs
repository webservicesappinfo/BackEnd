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
            return base.GetCompany(request, context);
        }
        public override Task<GetCompaniesReply> GetCompanies(GetCompaniesRequest request, ServerCallContext context)
        {
            var fitCompanyNames = new List<String>();
            using (var companies = new CompanyContext())
            {
                var guid = new Guid(request.UserGuid);
                if (request.Owner)
                    fitCompanyNames = companies.Values.Where(x => x.User == guid).Select(x => x.Name).ToList();
                else
                    fitCompanyNames = companies.Values.Where(x => x.Masters.Any(m => m.Guid == guid)).Select(x => x.Name).ToList();
            }
            var reply = new GetCompaniesReply();
            reply.Names.AddRange(fitCompanyNames);
            return Task.FromResult(reply);
        }
        public override Task<AddCompanyReply> AddCompany(AddCompanyRequest request, ServerCallContext context)
        {
            var result = _companyRepoService.AddCompany(request.Name, request.UserGuid);
            return Task.FromResult(new AddCompanyReply { Result = result });
        }
        public override Task<UpdateCompanyReply> UpdateCompany(UpdateCompanyRequest request, ServerCallContext context)
        {
            return base.UpdateCompany(request, context);
        }
        public override Task<RemoveCompanyReply> RemoveCompany(RemoveCompanyRequest request, ServerCallContext context)
        {
            return base.RemoveCompany(request, context);
        }

    }
}
