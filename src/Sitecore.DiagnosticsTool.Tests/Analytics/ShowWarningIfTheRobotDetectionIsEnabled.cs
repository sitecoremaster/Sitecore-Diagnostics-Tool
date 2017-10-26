using System.Collections.Generic;
using System.Linq;

using Sitecore.DiagnosticsTool.Core.Categories;
using Sitecore.DiagnosticsTool.Core.Tests;

namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
    public class ShowWarningIfTheRobotDetectionIsEnabled : Test
    {
        protected const string ErrorMessage = "Sitecore automatic robot detection functionality is disabled. This functionality filters out unwanted interactions from automated browsers and robots. Sitecore recommends enabling 'Analytics.AutoDetectBots' setting to reduce the growth of the xDB";
        
        public override string Name => "Sitecore robot detection functionality is disabled.";

        public override IEnumerable<Category> Categories => new[] { Category.Analytics };

        protected override bool IsActual(ITestResourceContext data)
        {
            var isCd = data.ServerRoles.Any(x => x == ServerRole.ContentDelivery);
            return !data.SitecoreInfo.GetBoolSetting("Analytics.AutoDetectBots") && data.SitecoreInfo.IsAnalyticsEnabled && isCd;
        }

        public override void Process(ITestResourceContext data, ITestOutputContext output)
        {
            output.Warning(ErrorMessage);
        }
    }
}
