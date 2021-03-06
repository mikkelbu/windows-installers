﻿using System;
using System.IO.Abstractions;
using Elastic.Installer.Domain.Configuration.Service;
using Elastic.Installer.Domain.Configuration.Wix.Session;
using Elastic.Installer.Domain.Model.Elasticsearch;

namespace Elastic.InstallerHosts.Elasticsearch.Tasks
{
	public class StartServiceTask : ElasticsearchInstallationTask
	{
		private IServiceStateProvider ServiceStateProvider { get; }

		public StartServiceTask(string[] args, ISession session) : base(args, session)
		{
			this.ServiceStateProvider = new ServiceStateProvider(session, "Elasticsearch");
		}

		public StartServiceTask(ElasticsearchInstallationModel model, ISession session, IFileSystem fileSystem, IServiceStateProvider serviceConfig) 
			: base(model, session, fileSystem)
		{
			this.ServiceStateProvider = serviceConfig;
		}

		protected override bool ExecuteTask()
		{
			if (this.InstallationModel.NoticeModel.ExistingVersionInstalled)
			{
				this.Session.Log($"Skipping {nameof(StartServiceTask)}. Existing version already installed, service will need to be manually started after installation");
				return true;
			}

			this.Session.Log("Executing start service");
			if (!this.InstallationModel.ServiceModel.StartAfterInstall)
				return true;
			var seesService = this.ServiceStateProvider.SeesService;
			this.Session.Log($"Trying to execute StartServiceTask seeing service: " + seesService);
			if (!seesService) return true;
			var totalTicks = 1000;
			this.Session.SendActionStart(totalTicks, ActionName, "Starting Elasticsearch service");
			this.ServiceStateProvider.StartAndWaitForRunning(TimeSpan.FromSeconds(60), 2000);
			this.Session.SendProgress(1000, "Elasticsearch service started");
			return true;
		}
	}
}
