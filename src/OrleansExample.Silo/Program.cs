﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace OrleansExample.Silo
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                var host = new HostBuilder()
                    .UseOrleans((context, siloBuilder) =>
                    {
                        siloBuilder
                            .UseLocalhostClustering()
                            .Configure<ClusterOptions>(options =>
                            {
                                options.ClusterId = "dev";
                                options.ServiceId = "HelloWorldApp";
                            })
                            .Configure<EndpointOptions>(options =>
                            {
                                options.AdvertisedIPAddress = IPAddress.Loopback;
                                //options.GatewayPort = 30000;
                                //options.SiloPort = 11111;
                            })
                            .AddAdoNetGrainStorage("Storage1", options =>
                            {
                                options.Invariant = "System.Data.SqlClient";
                                options.ConnectionString = "Data Source=.\\instance4dev;Initial Catalog=ServicePersistence1;Integrated Security=False;User ID=sa;Password=123";
                                options.UseJsonFormat = true;
                            })
                            .AddAdoNetGrainStorage("Storage2", options =>
                            {
                                options.Invariant = "System.Data.SqlClient";
                                options.ConnectionString = "Data Source=.\\instance4dev;Initial Catalog=ServicePersistence2;Integrated Security=False;User ID=sa;Password=123";
                                options.UseJsonFormat = true;
                            })
                            .UseTransactions()
                            //.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(...).Assembly).WithReferences())
                            .ConfigureApplicationParts(parts => parts.AddFromApplicationBaseDirectory())
                            .UseDashboard(options => { });
                    })
                    .ConfigureLogging(logging => logging.AddConsole())
                    .Build();

                await host.RunAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }
    }
}
