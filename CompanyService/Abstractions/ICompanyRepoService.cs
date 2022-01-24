using CompanyService.Models;
using Globals.Abstractions;
using System;
using System.Collections.Generic;

namespace CompanyService.Abstractions
{
    public interface ICompanyRepoService: IRepoServiceBase<Company>
    {
        List<Company> GetCompaniesByOwner(Guid owner);

        List<Company> GetCompaniesByMaster(Guid master);

        Boolean JoinToCompany(Guid guid, Guid masterGuid, String masterName);

        Boolean DelMaster(Guid company, Guid master);

        Boolean SetCompanyLocation(Guid companyGuid, Double? lat, Double? lng);
    }
}
