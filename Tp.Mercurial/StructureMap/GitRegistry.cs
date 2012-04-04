// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NGit.Util;
using Tp.Git.RevisionStorage;
using Tp.Git.VersionControlSystem;
using Tp.Git.Workflow;
using Tp.Mercurial;
using Tp.Mercurial.VersionControlSystem;
using Tp.SourceControl.Commands;
using Tp.SourceControl.RevisionStorage;
using Tp.SourceControl.Settings;
using Tp.SourceControl.StructureMap;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Workflow.Workflow;

namespace Tp.Git.StructureMap
{
	public class GitRegistry : SourceControlRegistry
	{
		protected override void ConfigureCheckConnectionErrorResolver()
		{
			For<ICheckConnectionErrorResolver>().Use<GitCheckConnectionErrorResolver>();
		}

		protected override void ConfigureSourceControlConnectionSettingsSource()
		{
			For<ISourceControlConnectionSettingsSource>().Use<MercurialCurrentProfileToConnectionSettingsAdapter>();
		}

		protected override void ConfigureRevisionIdComparer()
		{
			For<IRevisionIdComparer>().HybridHttpOrThreadLocalScoped().Use<MercurialRevisionIdComparer>();
		}

		protected override void ConfigureVersionControlSystem()
		{
			var mockSystemReader = new MockSystemReader(SystemReader.GetInstance());
			SystemReader.SetInstance(mockSystemReader);

			For<IVersionControlSystem>().Use<MercurialVersionControlSystem>();
		}

		protected override void ConfigureRevisionStorage()
		{
			For<IRevisionStorageRepository>().Use<GitRevisionStorageRepository>();
		}

		protected override void ConfigureUserMapper()
		{
			For<UserMapper>().Use<GitUserMapper>();
		}
	}
}