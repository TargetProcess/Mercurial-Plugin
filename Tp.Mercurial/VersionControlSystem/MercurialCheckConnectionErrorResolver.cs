// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using Mercurial;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Mercurial.VersionControlSystem;
using Tp.SourceControl.Commands;

namespace Tp.Mercurial.VersionControlSystem
{
	public class MercurialCheckConnectionErrorResolver : ICheckConnectionErrorResolver
	{
		private readonly ILogManager _log;
		public const string INVALID_URI_OR_INSUFFICIENT_ACCESS_RIGHTS_ERROR_MESSAGE = "invalid uri or insufficient access rights";
        public const string MERCURIAL_IS_NOT_INSTALLED_ERROR_MESSAGE = "Mercurial is not installed.";

        public MercurialCheckConnectionErrorResolver(ILogManager log)
		{
			_log = log;
		}

		public void HandleConnectionError(Exception exception, PluginProfileErrorCollection errors)
		{
			_log.GetLogger("Mercurial").Warn("Check connection failed", exception);
			exception = exception.InnerException ?? exception;
			const string uriFieldName = "Uri";
			if (exception is MercurialExecutionException)
			{
				errors.Add(new PluginProfileError {FieldName = "Login", Message = exception.Message});
				errors.Add(new PluginProfileError {FieldName = "Password", Message = exception.Message});
				errors.Add(new PluginProfileError {FieldName = uriFieldName, Message = exception.Message});
				return;
			}

            if (exception is ArgumentNullException || exception is DirectoryNotFoundException)
			{
				errors.Add(new PluginProfileError{ FieldName = uriFieldName, Message = INVALID_URI_OR_INSUFFICIENT_ACCESS_RIGHTS_ERROR_MESSAGE });
			}

            if (exception is MercurialMissingException)
            {
                errors.Add(new PluginProfileError { Message = MERCURIAL_IS_NOT_INSTALLED_ERROR_MESSAGE });
            }

			var fieldName = string.Empty;
			if (exception is InvalidRevisionException)
			{
				fieldName = "Revision";
			}

			errors.Add(new PluginProfileError {FieldName = fieldName, Message = exception.Message});
		}
	}
}