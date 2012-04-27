// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Threading;
using Mercurial;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Mercurial.VersionControlSystem;
using Tp.SourceControl.Commands;
using Tp.SourceControl.VersionControlSystem;
using System.Linq;
using MercurialSDK = Mercurial;

namespace Tp.Mercurial
{
	public class MercurialCheckConnectionCommand : VcsCheckConnectionCommand<MercurialPluginProfile>
	{
        private MercurialRepositoryFolder _folder;

		protected override void CheckStartRevision(
            MercurialPluginProfile settings, 
            IVersionControlSystem versionControlSystem, 
            PluginProfileErrorCollection errors)
		{
			settings.ValidateStartRevision(errors);
		}

		protected override void OnCheckConnection(PluginProfileErrorCollection errors, MercurialPluginProfile settings)
		{
			settings.ValidateUri(errors);

			if (!errors.Any())
			{
                _folder = MercurialRepositoryFolder.Create(settings.Uri);

                if (!_folder.Exists())
                    Directory.CreateDirectory(_folder.Value);

                CloneCommand cloneCommand = new CloneCommand().WithAdditionalArgument("--noupdate").WithTimeout(20);
                Repository repository = new Repository(_folder.Value, new NonPersistentClientFactory());

			    try
			    {
                    repository.Clone(settings.Uri);
			    }
			    catch (MercurialTimeoutException e)
			    {
                    var files = Directory.GetFiles(_folder.Value, "*.*", SearchOption.AllDirectories);

                    if (files.Count() == 0)
                        throw;
			    }
			}
		}

        protected override void OnExecuted(MercurialPluginProfile profile)
        {
            base.OnExecuted(profile);

            if (_folder != null && _folder.Exists())
            {
                _folder.Delete();
            }
        }
	}
}