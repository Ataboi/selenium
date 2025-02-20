using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium.Environment;

namespace OpenQA.Selenium.DevTools
{
    [TestFixture]
    public class DevToolsSecurityTest : DevToolsTestFixture
    {
        //[Test]
        [IgnoreBrowser(Selenium.Browser.EdgeLegacy, "Legacy Edge does not support Chrome DevTools Protocol")]
        [IgnoreBrowser(Selenium.Browser.IE, "IE does not support Chrome DevTools Protocol")]
        [IgnoreBrowser(Selenium.Browser.Firefox, "Firefox does not support Chrome DevTools Protocol")]
        [IgnoreBrowser(Selenium.Browser.Safari, "Safari does not support Chrome DevTools Protocol")]
        public async Task LoadInsecureWebsite()
        {
            var domains = session.GetVersionSpecificDomains<V100.DevToolsSessionDomains>();
            await domains.Security.Enable();

            await domains.Security.SetIgnoreCertificateErrors(new V100.Security.SetIgnoreCertificateErrorsCommandSettings()
            {
                Ignore = false
            });

            string summary = null;
            ManualResetEventSlim sync = new ManualResetEventSlim(false);
            EventHandler<V100.Security.SecurityStateChangedEventArgs> securityStateChangedHandler = (sender, e) =>
            {
                summary = e.Summary;
                sync.Set();
            };
            domains.Security.SecurityStateChanged += securityStateChangedHandler;

            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("devToolsSecurityTest");
            sync.Wait(TimeSpan.FromSeconds(5));

            await domains.Security.Disable();

            Assert.That(driver.PageSource, Contains.Substring("Security Test"));
            Assert.That(summary, Contains.Substring("This page has a non-HTTPS secure origin"));
        }

        [Test]
        [IgnoreBrowser(Selenium.Browser.EdgeLegacy, "Legacy Edge does not support Chrome DevTools Protocol")]
        [IgnoreBrowser(Selenium.Browser.IE, "IE does not support Chrome DevTools Protocol")]
        [IgnoreBrowser(Selenium.Browser.Firefox, "Firefox does not support Chrome DevTools Protocol")]
        [IgnoreBrowser(Selenium.Browser.Safari, "Safari does not support Chrome DevTools Protocol")]
        public async Task LoadSecureWebsite()
        {
            var domains = session.GetVersionSpecificDomains<V100.DevToolsSessionDomains>();
            await domains.Security.Enable();

            await domains.Security.SetIgnoreCertificateErrors(new V100.Security.SetIgnoreCertificateErrorsCommandSettings()
            {
                Ignore = true
            });

            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("devToolsSecurityTest");
            Assert.That(driver.PageSource, Contains.Substring("Security Test"));
        }
    }
}
