﻿using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.OfferEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using UserService.Abstractions;
using UserService.Models;

namespace UserService.EventHendlers
{
    public class DelOfferEH : IIntegrationEventHandler<DelOfferEvent>
    {
        private readonly ILogger<DelOfferEvent> _logger;
        private readonly IUserRepoService _userRepoService;

        public DelOfferEH(ILogger<DelOfferEvent> logger, IUserRepoService userRepoService)
        {
            _logger = logger;
            _userRepoService = userRepoService;
        }

        public Task Handle(DelOfferEvent @event)
        {
            Console.WriteLine(@event.Guid);
            //@event.ResponseReceivedEvent.Set();
            _userRepoService.DelOffer(@event.Guid, @event.MasterGuid);
            return Task.FromResult(0);
        }
    }
}
