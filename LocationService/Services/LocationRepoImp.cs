using EventBus.Abstractions;
using Grpc.Core;
using LocationService.Abstractions;
using LocationService.Protos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationService.Services
{
    public class LocationRepoImp: LocationRepo.LocationRepoBase
    {
        private readonly ILogger<LocationRepoImp> _logger;
        private readonly IEventBus _eventBus;
        private readonly ILocationRepoService _locationRepoService;

        public LocationRepoImp(ILogger<LocationRepoImp> logger, IEventBus eventBus, ILocationRepoService locationRepoService)
        {
            _logger = logger;
            _eventBus = eventBus;
            _locationRepoService = locationRepoService;
        }

        public override Task<GetUserLocationReply> GetUserLocation(GetUserLocationRequest request, ServerCallContext context)
        {
            _locationRepoService.GetUserLocation(request.Guid, out string lat, out string lng);
            return Task.FromResult(new GetUserLocationReply() { ForGuid = request.Guid, Lat = lat, Lng = lng});
        }

        public override Task<SetUserLocationReply> SetUserLocation(SetUserLocationRequest request, ServerCallContext context)
        {
            var result = _locationRepoService.SetUserLocation(request.ForGuid, request.Lat, request.Lng);
            return Task.FromResult(new SetUserLocationReply() { IsSet = result });
        }
    }
}
