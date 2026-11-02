#nullable enable
using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace Globals.Tests
{
    public static class GrpcTestHelpers
    {
        public static AsyncUnaryCall<T> CreateAsyncUnaryCall<T>(T response)
        {
            return new AsyncUnaryCall<T>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });
        }

        public static AsyncUnaryCall<T> CreateAsyncUnaryCallException<T>(RpcException ex)
        {
            return new AsyncUnaryCall<T>(
                Task.FromException<T>(ex),
                Task.FromResult(new Metadata()),
                () => ex.Status,
                () => ex.Trailers,
                () => { });
        }
    }
}
