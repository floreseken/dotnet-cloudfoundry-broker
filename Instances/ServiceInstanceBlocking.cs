using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenServiceBroker.Instances;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace Broker.Instances
{
    public class ServiceInstanceBlocking : IServiceInstanceBlocking
    {
        private readonly ILogger<ServiceInstanceBlocking> _log;
        private readonly IOptions<CloudFoundryApplicationOptions> _appConfig;

        public ServiceInstanceBlocking(ILogger<ServiceInstanceBlocking> log, IOptions<CloudFoundryApplicationOptions> appConfig)
        {
            _log = log;
            this._appConfig = appConfig;
        }

        public async Task<ServiceInstanceProvision> ProvisionAsync(ServiceInstanceContext context, ServiceInstanceProvisionRequest request)
        {
            LogContext(_log, "Provision", context);
            LogRequest(_log, request);

            _log.LogInformation($"The CF api is: {_appConfig.Value.CF_Api}");

            var orgId = request.OrganizationGuid;
            var spaceId = request.SpaceGuid;
            var resourceGroupName = $"{orgId}_{spaceId}";

            return await Task.FromResult(new ServiceInstanceProvision());
        }

        public async Task DeprovisionAsync(ServiceInstanceContext context, string serviceId, string planId)
        {
            LogContext(_log, "Deprovision", context);
            _log.LogInformation($"Deprovision: {{ service_id = {serviceId}, planId = {planId} }}");

        }

        public Task<ServiceInstanceResource> FetchAsync(string instanceId)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(ServiceInstanceContext context, ServiceInstanceUpdateRequest request)
        {
            throw new System.NotImplementedException();
        }

        private static void LogContext(ILogger log, string operation, ServiceInstanceContext context)
        {
            log.LogInformation(
                $"{operation} - context: {{ instance_id = {context.InstanceId}, " +
                                        $"originating_identity = {{ platform = {context.OriginatingIdentity?.Platform}, " +
                                                                  $"value = {context.OriginatingIdentity?.Value} }} }}");
        }

        private static void LogRequest(ILogger log, ServiceInstanceProvisionRequest request)
        {
            log.LogInformation(
                $"Provision - request: {{ organization_guid = {request.OrganizationGuid}, " +
                                        $"space_guid = {request.SpaceGuid}, " +
                                        $"service_id = {request.ServiceId}, " +
                                        $"plan_id = {request.PlanId}, " +
                                        $"parameters = {request.Parameters}, " +
                                        $"context = {request.Context} }}");
        }
    }
}
