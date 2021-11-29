using CompanyService.Models;
using Globals.Abstractions;
using System;
using System.Collections.Generic;

namespace CompanyService.Abstractions
{
    public interface ICompanyRepoService: IRepoServiceBase<Company>
    {

        public List<Company> GetCompaniesByOwner(Guid owner);

        public List<Company> GetCompaniesByMaster(Guid master);

        public bool JoinToCompany(Guid guid, Guid masterGuid);

        public Boolean DelMaster(Guid company, Guid master);
    }
}
