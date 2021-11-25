using CompanyService.Models;
using System;
using System.Collections.Generic;

namespace CompanyService.Abstractions
{
    public interface ICompanyRepoService
    {
        public Boolean AddCompany( Company company);

        public Company GetCompany(Guid guid);

        public List<Company> GetCompanies();

        public List<Company> GetCompaniesByOwner(Guid owner);

        public List<Company> GetCompaniesByMaster(Guid master);

        public Boolean UpdateCompany(Company company);

        public bool JoinToCompany(Guid guid, Guid masterGuid);

        public Boolean DelCompany(Guid guid);

        public Boolean DelMaster(Guid company, Guid master);
    }
}
