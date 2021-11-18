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

        public Boolean UpdateCompany(Company company);

        public Boolean DelCompany(Company company);
    }
}
