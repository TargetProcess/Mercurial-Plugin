﻿// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mercurial;
using StructureMap;
using Tp.Core;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;
using MercurialSDK = Mercurial;
using Repository = Mercurial.Repository;

namespace Tp.Mercurial.VersionControlSystem
{
	public class MercurialClient
	{
	    private readonly Repository _repository;
		private readonly ISourceControlConnectionSettingsSource _settings;
        private readonly IStorage<MercurialRepositoryFolder> _folder;

        public MercurialClient(ISourceControlConnectionSettingsSource settings, IStorage<MercurialRepositoryFolder> folder)
		{
            _settings = settings;
            _folder = folder;
			_repository = GetClient(settings);
		}

        public IEnumerable<RevisionRange> GetFromTillHead(DateTime from, int pageSize)
		{
            var command = new LogCommand();
            var pages = _repository.Log(command).
                Where(ch => ch.Timestamp >= from).
                OrderBy(ch => ch.Timestamp).
                ToArray().
                Split(pageSize);

		    var result = pages.Select(page => new RevisionRange(page.First().ToRevisionId(), page.Last().ToRevisionId()));

		    return result;
		}

        public IEnumerable<RevisionRange> GetAfterTillHead(RevisionId revisionId, int pageSize)
        {
            var command = new LogCommand();
            var pages = _repository.Log(command)
                .Where(ch => ch.Timestamp > revisionId.Time.Value)
                .OrderBy(ch => ch.Timestamp)
                .ToArray()
                .Split(pageSize);

            var result = pages.Select(page => new RevisionRange(page.First().ToRevisionId(), page.Last().ToRevisionId()));

            return result;
        }

        public IEnumerable<RevisionRange> GetFromAndBefore(RevisionId fromRevision, RevisionId toRevision, int pageSize)
        {
            var command = new LogCommand();
            var pages = _repository.Log(command)
                .Where(ch => (ch.Timestamp >= fromRevision.Time.Value && ch.Timestamp <= toRevision.Time.Value))
                .OrderBy(ch => ch.Timestamp)
                .ToArray()
                .Split(pageSize);

            var result = pages.Select(page => new RevisionRange(page.First().ToRevisionId(), page.Last().ToRevisionId()));

            return result;
        }

        public Changeset GetParentCommit(Changeset commit)
        {
            var command = new LogCommand().WithIncludePathActions();
            var changeset = _repository.Log(command)
                .FirstOrDefault(ch => ch.RevisionNumber == (commit.RevisionNumber > 0 ? commit.RevisionNumber - 1 : commit.RevisionNumber));
            return changeset;
        }

        public Changeset GetCommit(RevisionId id)
        {
            var command = new LogCommand().WithIncludePathActions();
            var changeset = _repository.Log(command).FirstOrDefault(ch => ch.Hash == id.Value);
            return changeset;
        }

        public string[] RetrieveAuthors(DateRange dateRange)
        {
            var command = new LogCommand();
            var authors = _repository.Log(command)
                .Where(ch => ch.Timestamp >= dateRange.StartDate && ch.Timestamp <= dateRange.EndDate)
                .Select(ch => ch.AuthorName)
                .Distinct()
                .ToArray();

            return authors;
        }

        public RevisionInfo[] GetRevisions(RevisionId fromChangeset, RevisionId toChangeset)
        {
            var command = new LogCommand().WithIncludePathActions();
            var revisionInfos = _repository.Log(command)
                .Where(ch => ch.Timestamp >= fromChangeset.Time.Value && ch.Timestamp <= toChangeset.Time.Value)
                .Select(ch => ch.ToRevisionInfo())
                .ToArray();

            return revisionInfos;
        }

        public string GetFileContent(Changeset commit, string path)
		{
            var command = new CatCommand().WithAdditionalArgument(string.Format("-r {0}", commit.RevisionNumber)).WithFile(path);
            _repository.Execute(command);
            return command.RawStandardOutput;
		}

        private Repository GetClient(ISourceControlConnectionSettingsSource settings)
        {
            var repositoryFolder = GetLocalRepository(settings);
            if (IsRepositoryUriChanged(repositoryFolder, settings))
            {
                repositoryFolder.Delete();
                repositoryFolder = MercurialRepositoryFolder.Create(settings.Uri);
                _folder.ReplaceWith(repositoryFolder);
            }

            Repository repository;

            try
            {
                if (repositoryFolder.Exists())
                {
                    repository = new Repository(repositoryFolder.Value, new NonPersistentClientFactory());
                    repository.Pull(settings.Uri);
                }
                else
                {
                    Directory.CreateDirectory(repositoryFolder.Value);
                    CloneCommand cloneCommand = new CloneCommand().WithUpdate(false);
                    repository = new Repository(repositoryFolder.Value, new NonPersistentClientFactory());
                    repository.Clone(settings.Uri, cloneCommand);
                }
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentException(
                    MercurialCheckConnectionErrorResolver.INVALID_URI_OR_INSUFFICIENT_ACCESS_RIGHTS_ERROR_MESSAGE, e);
            }
            catch (FileNotFoundException e)
            {
                throw new ArgumentException(
                    MercurialCheckConnectionErrorResolver.INVALID_URI_OR_INSUFFICIENT_ACCESS_RIGHTS_ERROR_MESSAGE, e);
            }
            catch (MercurialMissingException e)
            {
                throw new ArgumentException(
                    MercurialCheckConnectionErrorResolver.MERCURIAL_IS_NOT_INSTALLED_ERROR_MESSAGE, e);
            }
            
            return repository;
        }

        private MercurialRepositoryFolder GetLocalRepository(ISourceControlConnectionSettingsSource settings)
        {
            if (_folder.Empty())
            {
                var repositoryFolder = MercurialRepositoryFolder.Create(settings.Uri);
                _folder.ReplaceWith(repositoryFolder);
                return repositoryFolder;
            }

            return _folder.Single();
        }

        private static bool IsRepositoryUriChanged(
            MercurialRepositoryFolder repositoryFolder,
            ISourceControlConnectionSettingsSource settings)
        {
            return (settings.Uri.ToLower() != repositoryFolder.RepoUri.ToLower()) && repositoryFolder.Exists();
        }
	}
}