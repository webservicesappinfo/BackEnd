using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MobileApiGetway.Tests.Helpers
{
    public class TestServerCallContext : ServerCallContext
    {
        private readonly Metadata _requestHeaders;
        private readonly CancellationToken _cancellationToken;
        private readonly Metadata _responseTrailers;
        private Status _status;

        public TestServerCallContext(Metadata? requestHeaders = null, CancellationToken cancellationToken = default)
        {
            _requestHeaders = requestHeaders ?? new Metadata();
            _cancellationToken = cancellationToken;
            _responseTrailers = new Metadata();
        }

        protected override string MethodCore => "TestMethod";
        protected override string HostCore => "localhost";
        protected override string PeerCore => "127.0.0.1";
        protected override DateTime DeadlineCore => DateTime.MaxValue;
        protected override Metadata RequestHeadersCore => _requestHeaders;
        protected override CancellationToken CancellationTokenCore => _cancellationToken;
        protected override Metadata ResponseTrailersCore => _responseTrailers;
        protected override Status StatusCore { get => _status; set => _status = value; }
        protected override WriteOptions? WriteOptionsCore { get; set; }
        protected override AuthContext AuthContextCore => new(null, new Dictionary<string, List<AuthProperty>>());

        protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions? options)
            => throw new NotImplementedException();

        protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
            => Task.CompletedTask;

        public static TestServerCallContext Create(CancellationToken cancellationToken = default)
            => new(cancellationToken: cancellationToken);
    }
}
