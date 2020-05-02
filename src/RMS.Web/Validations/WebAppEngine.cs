using RMS.Core;
using RMS.Core.Infrastructure;
using System;

namespace RequestsManagementSystem.Infrastructure
{
    public class WebAppEngine : AppEngine
    {
        /// <summary>
        /// Initialize WebApiEngine
        /// </summary>
        /// <param name="configuration"></param>
        public void Initialize(AppConfig config)
        {
            var fileProvider = new AppFileProvider(AppContext.BaseDirectory);
            Initialize(fileProvider, config);
        }

        public override void SetResolver()
        {
            SetContainerManager(new ContainerManager(ContainerBuilder.Build()));
        }
    }
}