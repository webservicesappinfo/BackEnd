using CompanyService.Protos;
using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MobileApiGetway.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UserService.Protos;

namespace MobileApiGetway
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //AppContext.SetSwitch(" System.Net.Http . SocketsHttpHandler.Http2UnencryptedSupport ", true); // allow unencrypted http / 2 protocol

            services.AddGrpc();
            services.AddGrpcClient<Notification.NotificationClient>(o => o.Address = new Uri("http://notificationservice"));
            //services.AddGrpcClient<UserRepo.UserRepoClient>(o => o.Address = new Uri("http://userreposervice"));
            services.AddGrpcClient<User.UserClient>(o => o.Address = new Uri("http://userservice"));
            services.AddGrpcClient<Company.CompanyClient>(o => o.Address = new Uri("http://companyservice"));
            services.AddGrpcClient<LocationRepo.LocationRepoClient>(o => o.Address = new Uri("http://locationservice"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<MobileApiService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
