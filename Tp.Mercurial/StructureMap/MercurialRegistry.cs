// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Mercurial.RevisionStorage;
using Tp.Mercurial.Workflow;
using Tp.Mercurial.VersionControlSystem;
using Tp.SourceControl.Commands;
using Tp.SourceControl.RevisionStorage;
using Tp.SourceControl.Settings;
using Tp.SourceControl.StructureMap;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Workflow.Workflow;

namespace Tp.Mercurial.StructureMap
{
	public class MercurialRegistry : SourceControlRegistry
	{
		protected override void ConfigureCheckConnectionErrorResolver()
		{
			For<ICheckConnectionErrorResolver>().Use<MercurialCheckConnectionErrorResolver>();
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
			For<IVersionControlSystem>().Use<MercurialVersionControlSystem>();
		}

		protected override void ConfigureRevisionStorage()
		{
			For<IRevisionStorageRepository>().Use<MercurialRevisionStorageRepository>();
		}

		protected override void ConfigureUserMapper()
		{
			For<UserMapper>().Use<MercurialUserMapper>();
		}
	}
}