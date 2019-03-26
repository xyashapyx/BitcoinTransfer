using AutoMapper;
using BitcoinTransfer.Database;
using BitcoinTransfer.Interfaces.Repositories;
using BitcoinTransfer.Interfaces.Services;
using BitcoinTransfer.Repositories;
using BitcoinTransfer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace BitcoinTransfer
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
            services.AddDbContext<BitcoinDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("BitcoinDb")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
     
            services.AddAutoMapper();

            services.AddTransient(typeof(ITransactionRepository), typeof(TransactionRepository));
            services.AddTransient(typeof(IWalletRepository), typeof(WalletRepository));
            services.AddTransient(typeof(IBitcoinService), typeof(BitcoinService));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseMvc();
        }
    }
}
