using System.Reflection;
using NUnit.Framework;
using Tp.Mercurial;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Testing.Common.NUnit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace Tp.Mercurial.Tests
{
    [TestFixture]
    public class ModelsTest
    {
        [Test]
        public void ValidateCredentialsInUriTest()
        {
            string uri = RunValidation("https://vanya@server.com", "vanya", "123456");
            uri.Should(Be.EqualTo("https://vanya:123456@server.com"));
        }

        [Test]
        public void ValidateCredentialsInUriWithoutLoginTest()
        {
            string uri = RunValidation("https://server.com", "vanya", "123456");
            uri.Should(Be.EqualTo("https://vanya:123456@server.com"));
        }

        [Test]
        public void ValidateCredentialsInUriWithoutPasswordTest()
        {
            string uri = RunValidation("https://server.com", "vanya", "");
            uri.Should(Be.EqualTo("https://vanya@server.com"));
        }

        private string RunValidation(string uri, string login, string password)
        {
            var profile = new MercurialPluginProfile();
            PluginProfileErrorCollection errors = new PluginProfileErrorCollection();

            profile.Uri = uri;
            profile.Login = login;
            profile.Password = password;

            var methodInfo = typeof(MercurialPluginProfile).GetMethod(
                "ValidateCredentialsInUri",
                BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(profile, new object[] { errors });

            return profile.Uri;
        }
    }
}
