using CompanyService.Abstractions;
using CompanyService.Models;
using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.CompanyEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace CompanyService.Services
{
    public class CompanyRepoService : ICompanyRepoService
    {
        private readonly ILogger<CompanyRepoService> _logger;
        private readonly IEventBus _eventBus;

        public CompanyRepoService(ILogger<CompanyRepoService> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        public bool AddCompany(string name, string ownerGuid)
        {
            var result = false;
            using (var companies = new CompanyContext())
            {
                var company = new Models.Company { Name = name, User = new Guid(ownerGuid) };
                companies.Values.Add(company);
                companies.SaveChanges();
                result = true;
                _eventBus.Publish(new AddCompanyEvent(company.Name, company.Guid, company.User));
            }
            return result;
        }

        public bool DelCompany(Company company)
        {
            throw new NotImplementedException();
        }

        public List<Company> GetAllCompany()
        {
            throw new NotImplementedException();
        }

        public Company GetCompany(Guid guid)
        {
            throw new NotImplementedException();
        }

        public bool UpdateCompany(Company company)
        {
            throw new NotImplementedException();
        }
    }
}
