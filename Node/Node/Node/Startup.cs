using BlockChain.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Node
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    );
            });
            services.AddSingleton<Domain.NodeInfo>();

            services.AddSingleton<ICryptoUtil, CryptoUtil>();
            services.AddSingleton<IProofOfWork, ProofOfWork>();
            services.AddSingleton<ITransactionValidator, TransactionValidator>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<Domain.ITransactionQuery, Domain.TransactionQuery>();
            services.AddSingleton<Domain.IInfoQuery, Domain.InfoQuery>();
            services.AddSingleton<Domain.IBlockQuery, Domain.BlockQuery>();

            services.AddSingleton<Domain.INodeSynchornizator>(s =>
            {
                string url = Configuration.GetValue<string>("server.urls");
                return new Domain.NodeSynchornizator(new Domain.Peer(url, s.GetService<Domain.NodeInfo>().Name));
            });

            services.AddSingleton<Domain.Node>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ApplicationServices.GetService<Domain.Node>().Init();

            app.UseCors("CorsPolicy");

            app.UseMvc();
        }
    }
}
