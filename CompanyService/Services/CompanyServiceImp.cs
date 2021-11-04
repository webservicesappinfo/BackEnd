using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyService.Protos;
using Grpc.Core;

namespace CompanyService.Services
{
    public class CompanyServiceImp : Protos.Company.CompanyBase
    {
        public override Task<GetCompanyReply> GetCompany(GetCompanyRequest request, ServerCallContext context)
        {
            return base.GetCompany(request, context);
        }
        public override Task<GetCompaniesReply> GetCompanies(GetCompaniesRequest request, ServerCallContext context)
        {
            return base.GetCompanies(request, context);
        }
        public override Task<AddCompanyReply> AddCompany(AddCompanyRequest request, ServerCallContext context)
        {
            return base.AddCompany(request, context);
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
