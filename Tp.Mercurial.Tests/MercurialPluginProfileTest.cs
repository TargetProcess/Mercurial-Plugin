using Tp.Mercurial;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Mercurial.Tests
{
    [TestClass()]
    public class ModelsTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for ValidateCredentialsInUri
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Tp.Mercurial.dll")]
        public void ValidateCredentialsInUriTest()
        {
            MercurialPluginProfile profile = new MercurialPluginProfile();
            profile.Uri = "https://vanya@server.com";
            profile.Login = "vanya";
            profile.Password = "123456";

            MercurialPluginProfile_Accessor target = new MercurialPluginProfile_Accessor(new PrivateObject(profile));
            PluginProfileErrorCollection errors = new PluginProfileErrorCollection();
            
            target.ValidateCredentialsInUri(errors);

            Assert.AreEqual("https://vanya:123456@server.com", profile.Uri, "Result Uri is not valid.");
        }
    }
}
