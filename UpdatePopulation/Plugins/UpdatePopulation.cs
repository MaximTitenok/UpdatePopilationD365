﻿using Microsoft.Xrm.Sdk;
using System;
using UpdatePopulation.Services;

namespace UpdatePopulation
{
    public class UpdatePopulation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            WritePopulation writePopulation = new WritePopulation(context, service);
            writePopulation.UpdatePopulation();
        }

    }
}
