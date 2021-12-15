using EventBus.Abstractions;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using OfferService.Abstractions;
using OfferService.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfferService.Services
{
    public class OfferServiceImp : Offer.OfferBase
    {
        private readonly ILogger<OfferServiceImp> _logger;
        private readonly IEventBus _eventBus;
        private readonly IOfferRepoService _offerRepoService;

        public OfferServiceImp(ILogger<OfferServiceImp> logger, IEventBus eventBus, IOfferRepoService offerRepoService)
        {
            _logger = logger;
            _eventBus = eventBus;
            _offerRepoService = offerRepoService;
        }

        public override Task<AddOfferReply> AddOffer(AddOfferRequest request, ServerCallContext context)
        {
            var offer = new Models.Offer() 
            { 
                Name = request.Name, 
                MasterGuid = new Guid(request.MasterGuid),
                MasterName = request.MasterName,
                SkillGuid= new Guid(request.SkillGuid),
                SkillName = request.SkillName 
            };
            var result = _offerRepoService.AddEntity(offer);
            /*if (result)
                _eventBus.Publish(new AddSkillEvent(company.Name, company.Guid, company.User));*/
            return Task.FromResult(new AddOfferReply { Result = result });
        }

        public override Task<GetOfferReply> GetOffer(GetOfferRequest request, ServerCallContext context)
        {
            return base.GetOffer(request, context);
        }

        public override Task<GetOffersReply> GetOffers(GetOffersRequest request, ServerCallContext context)
        {
            var offers = _offerRepoService.GetEntities();
            var reply = ConvertOffersToReply(offers);
            return Task.FromResult(reply);
        }

        public override Task<GetOffersReply> GetOffersByMaster(GetOffersByMasterRequest request, ServerCallContext context)
        {
            var masterGuid = new Guid(request.MasterGuid);
            var offers = _offerRepoService.GetEntities();
            var reply = ConvertOffersToReply(offers.Where(x=>x.MasterGuid == masterGuid).ToList());
            return Task.FromResult(reply);
        }

        public override Task<GetOffersReply> GetOffersBySkill(GetOffersBySkillRequest request, ServerCallContext context)
        {
            var skillGuid = new Guid(request.SkillGuid);
            var offers = _offerRepoService.GetEntities();
            var reply = ConvertOffersToReply(offers.Where(x => x.SkillGuid == skillGuid).ToList());
            return Task.FromResult(reply);
        }

        public override Task<UpdateOfferReply> UpdateOffer(UpdateOfferRequest request, ServerCallContext context)
        {
            return base.UpdateOffer(request, context);
        }

        public override Task<DelOfferReply> DelOffer(DelOfferRequest request, ServerCallContext context)
        {
            var result = _offerRepoService.DelEntity(new Guid(request.Guid));
            return Task.FromResult(new DelOfferReply { Result = result });
        }

        private GetOffersReply ConvertOffersToReply(List<Models.Offer> offers)
        {
            var reply = new GetOffersReply();
            foreach (var offer in offers)
            {
                reply.Guids.Add(offer.Guid.ToString());
                reply.Names.Add(offer.Name);
                reply.MasterGuids.Add(offer.MasterGuid.ToString());
                reply.MasterNames.Add(offer.MasterName);
                reply.SkillGuids.Add(offer.SkillGuid.ToString());
                reply.SkillNames.Add(offer.SkillName);
            }
            return reply;
        }
    }
}
