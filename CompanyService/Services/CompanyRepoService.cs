using CompanyService.Abstractions;
using CompanyService.Models;
using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.CompanyEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public bool AddCompany(Company company)
        {
            var result = false;
            using (var companies = new CompanyContext())
            {
                companies.Values.Add(company);
                companies.SaveChanges();
                result = true;
            }
            return result;
        }

        public bool DelCompany(Company company)
        {
            using (var db = new CompanyContext())
            {
                var findCompany = db.Values.FirstOrDefault(x => x.Guid == company.Guid);
                if (findCompany == null) return false;
                db.Values.Remove(findCompany);
                db.SaveChanges();
            }
            return true;
        }

        public List<Company> GetCompanies()
        {
            using (var db = new CompanyContext())
                return db.Values.ToList();
        }

        public List<Company> GetCompaniesByOwner(Guid owner)
        {
            using (var db = new CompanyContext())
                return db.Values.Where(x=>x.User == owner).ToList();
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
