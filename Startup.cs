using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommanderGQL.Data;
using CommanderGQL.GraphQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GraphQL.Server.Ui.Voyager;
using CommanderGQL.GraphQL.Platforms;
using CommanderGQL.GraphQL.Commands;

namespace CommanderGQL
{
    public class Startup
    {
        private readonly IConfiguration Configuration;
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }



        public void ConfigureServices(IServiceCollection services)
        {
            services.AddPooledDbContextFactory<AppDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("CommandConStr")));
            //services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("CommandConStr")));

            services.AddGraphQLServer()
                    .AddQueryType<Query>()
                    .AddMutationType<Mutation>()
                    .AddSubscriptionType<Subscription>()
                    .AddType<PlatformType>()
                    .AddType<CommandType>()
                    .AddFiltering()
                    .AddSorting()
                    // allows us to manage in memory all the subscribers
                    .AddInMemorySubscriptions();
                    
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();

            app.UseRouting();

            app.UseGraphQLVoyager(new VoyagerOptions() 
            {
                GraphQLEndPoint = "/graphql"                
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
            });
        }
    }
}
