// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Threading;
using Mercurial;
using NGit.Storage.File;
using StructureMap;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using MercurialSDK = Mercurial;

namespace Tp.Mercurial.VersionControlSystem
{
	[Serializable]
    public class MercurialRepositoryFolder
	{
		public string Value { get; set; }
		public string RepoUri { get; set; }

		[NonSerialized]
		private bool _wasMarkedAsDeleted;

		public void Delete()
		{
			if (!Exists())
			{
				return;
			}

            try
			{
				DeleteDirectory();
			}
			catch (Exception ex)
			{
				_wasMarkedAsDeleted = true;
				ObjectFactory.GetInstance<IActivityLogger>().Error(ex);
			}
		}

		public bool Exists()
		{
			return Directory.Exists(Value) && !_wasMarkedAsDeleted;
		}

        private void DeleteDirectory()
		{
            //var files = Directory.GetFiles("d:\\testmerc", "*.*", SearchOption.AllDirectories);
            //if (files.Length == 0)
            //    throw new Exception();

            //using (WaitHandle handle = new AutoResetEvent(false))
            //{
            //    while (true)
            //    {
            //        handle.WaitOne(100);

            //        try
            //        {
            //            Directory.Delete("d:\\testmerc", true);
            //            break;
            //        }
            //        catch
            //        {
            //            continue;
            //        }
            //    }
            //}

			Value.DeleteDirectory();
		}

        public static MercurialRepositoryFolder Create(string repoUri)
		{
            return new MercurialRepositoryFolder { Value = Path.Combine(MercurialCloneRootFolder, Guid.NewGuid().ToString()), RepoUri = repoUri };
		}

		protected static string MercurialCloneRootFolder
		{
			get { return ObjectFactory.GetInstance<PluginDataFolder>().Path; }
		}
	}
}