

namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
    using Sitecore.Diagnostics.Base;
    using Sitecore.Diagnostics.Objects;
    using Sitecore.DiagnosticsTool.Core.Categories;
    using Sitecore.DiagnosticsTool.Core.Resources.Logging;
    using Sitecore.DiagnosticsTool.Core.Tests;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class AnonymousContactsIndexingEnabled : KbTest
    {

        protected string SettingName = "ContentSearch.Analytics.IndexAnonymousContacts";
        public override string KbNumber => "171238";

        public override string KbName { get; } = "Issues caused by indexing anonymous contacts";

        public override IEnumerable<Category> Categories { get; } = new[] { Category.Analytics };

        protected override bool IsActual(ISitecoreVersion sitecoreVersion)
        {
            return sitecoreVersion.MajorMinorUpdateInt >= 812;
        }

        public override void Process(ITestResourceContext data, ITestOutputContext output)
        {
            Assert.ArgumentNotNull(data, nameof(data));

            if (data.SitecoreInfo.GetBoolSetting(SettingName))
            {
                 Report(output);
            }
        }
    }
}

