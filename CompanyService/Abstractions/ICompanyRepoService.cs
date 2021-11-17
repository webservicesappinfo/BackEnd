using CompanyService.Models;
using System;
using System.Collections.Generic;

namespace CompanyService.Abstractions
{
    public interface ICompanyRepoService
    {
        public Boolean AddCompany( String name, String ownerGuid);

        public Company GetCompany(Guid guid);

        public List<Company> GetAllCompany();

        public Boolean UpdateCompany(Company company);

        public Boolean DelCompany(Company company);
    }
}
