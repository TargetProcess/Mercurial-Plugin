// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
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
		    bool exists = Directory.Exists(Value);
            ObjectFactory.GetInstance<IActivityLogger>().WarnFormat(
                "[MercurialRepositoryFolder.Exists()] exists: {0}, _wasMarkedAsDeleted: {1}", 
                exists, 
                _wasMarkedAsDeleted);

			return exists && !_wasMarkedAsDeleted;
		}

        private void DeleteDirectory()
		{
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