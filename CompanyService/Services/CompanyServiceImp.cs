using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public CompanyServiceImp(ILogger<CompanyServiceImp> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
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
            var result = false;
            using (var companies = new CompanyContext())
            {
                var company = new Models.Company { Name = request.Name, User = new Guid(request.UserGuid) };
                companies.Values.Add(company);
                companies.SaveChanges();
                result = true;
                _eventBus.Publish(new AddCompanyEvent(company.Name, company.Guid, company.User));
            }
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
