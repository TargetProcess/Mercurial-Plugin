// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Mercurial;
using NGit;
using NGit.Revwalk;
using NGit.Revwalk.Filter;
using NGit.Transport;
using StructureMap;
using Tp.Core;
using Tp.Git.VersionControlSystem;
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

		public MercurialClient(ISourceControlConnectionSettingsSource settings)
		{
			_settings = settings;
			_repository = GetClient(settings);
		}

		public IEnumerable<RevisionRange> GetFromTillHead(DateTime from, int pageSize)
		{
            var command = new LogCommand();
            var pages = _repository.Log(command).Where(ch => ch.Timestamp >= from).ToArray().Split(pageSize);

		    var result = pages.Select(page => new RevisionRange(page.First().ToRevisionId(), page.Last().ToRevisionId()));

		    return result;
		}

        public IEnumerable<RevisionRange> GetAfterTillHead(RevisionId revisionId, int pageSize)
        {
            var command = new LogCommand();
            var pages = _repository.Log(command).Where(ch => ch.Timestamp > revisionId.Time.Value).ToArray().Split(pageSize);

            var result = pages.Select(page => new RevisionRange(page.First().ToRevisionId(), page.Last().ToRevisionId()));

            return result;
        }

        public IEnumerable<RevisionRange> GetFromAndBefore(RevisionId fromRevision, RevisionId toRevision, int pageSize)
        {
            var command = new LogCommand();
            var pages = _repository.Log(command)
                .Where(ch => (ch.Timestamp >= fromRevision.Time.Value && ch.Timestamp <= toRevision.Time.Value))
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
                .Select(ch => ch.AuthorName).ToArray();

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

        private static Repository GetClient(ISourceControlConnectionSettingsSource settings)
        {
            try
            {
                Uri uri = new Uri(settings.Uri);
                return new Repository(uri.AbsolutePath);
            }
            catch (ArgumentNullException exception)
            {
                throw new ArgumentException("Invalid URI", exception);
                //GitCheckConnectionErrorResolver.INVALID_URI_OR_INSUFFICIENT_ACCESS_RIGHTS_ERROR_MESSAGE, exception);
            }
        }

        private static IStorageRepository Repository
        {
            get { return ObjectFactory.GetInstance<IStorageRepository>(); }
        }

        //private void Fetch()
        //{
        //    var credentialsProvider = new UsernamePasswordCredentialsProvider(_settings.Login, _settings.Password);

        //    _git.Clean().Call();
        //    _git.Fetch().SetCredentialsProvider(credentialsProvider).SetRemoveDeletedRefs(true).Call();
        //}

        //private static bool IsRepositoryUriChanged(MercurialRepositoryFolder repositoryFolder,
        //                                           ISourceControlConnectionSettingsSource settings)
        //{
        //    return (settings.Uri.ToLower() != repositoryFolder.RepoUri.ToLower()) && repositoryFolder.Exists();
        //}

        //private static MercurialRepositoryFolder GetLocalRepository(ISourceControlConnectionSettingsSource settings)
        //{
        //    var repoFolderStorage = Repository.Get<MercurialRepositoryFolder>();
        //    if (repoFolderStorage.Empty())
        //    {
        //        var repositoryFolder = MercurialRepositoryFolder.Create(settings.Uri);
        //        repoFolderStorage.ReplaceWith(repositoryFolder);
        //        return repositoryFolder;
        //    }

        //    return repoFolderStorage.Single();
        //}

        //private static RevFilter ApplyNoMergesFilter(RevFilter filter)
        //{
        //    return AndRevFilter.Create(new[] {RevFilter.NO_MERGES, filter});
        //}

        //private RevWalk CreateRevWalker()
        //{
        //    var repository = _git.GetRepository();
        //    try
        //    {
        //        var revWalk = new RevWalk(repository);

        //        foreach (var reference in repository.GetAllRefs())
        //        {
        //            revWalk.MarkStart(revWalk.ParseCommit(reference.Value.GetObjectId()));
        //        }
        //        return revWalk;
        //    }
        //    finally
        //    {
        //        repository.Close();
        //    }
        //}


        //~MercurialClient()
        //{
        //    BatchingProgressMonitor.ShutdownNow();
        //}
	}
}