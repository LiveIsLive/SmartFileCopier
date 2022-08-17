﻿using ColdShineSoft.CustomFileCopier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.CustomFileCopier.Handlers
{
	public class Sftp : Models.ResultHandler
	{
		public override string Name { get; } = "SFTP";

		public override bool Remote => true;

		public override bool TargetDirectoryEmpty(Job job)
		{
			Renci.SshNet.SftpClient sftpClient = new Renci.SshNet.SftpClient(job.TargetServer, job.TargetPort, job.TargetUserName, job.TargetPassword);
			try
			{
				if (!sftpClient.Exists(job.TargetDirectoryPath))
					return true;
				return sftpClient.ListDirectory(job.TargetDirectoryPath).FirstOrDefault() == null;
			}
			finally
			{
				sftpClient.Disconnect();
			}
		}

		public override void Execute(Models.Job job)
		{
			Renci.SshNet.SftpClient sftpClient = new Renci.SshNet.SftpClient(job.TargetServer, job.TargetPort, job.TargetUserName, job.TargetPassword);
			try
			{
				foreach (Models.File sourceFile in job.SourceFiles)
				{
					sourceFile.Result = Models.CopyResult.Copying;
					string targetFilePath = job.GetTargetAbsoluteFilePath(sourceFile.Path);
					string targetDirectory = System.IO.Path.GetDirectoryName(targetFilePath).Replace('\\', '/');
					if (!sftpClient.Exists(targetDirectory))
						try
						{
							this.CreateServerDirectoryIfItDoesntExist(sftpClient, targetDirectory);
						}
						catch (System.Exception exception)
						{
							sourceFile.Result = Models.CopyResult.Failure;
							sourceFile.Error = exception.Message;
							continue;
						}
					try
					{
						this.CopyFile(sftpClient, sourceFile.Path, targetFilePath);
					}
					catch (System.Exception exception)
					{
						sourceFile.Result = Models.CopyResult.Failure;
						sourceFile.Error = exception.Message;
						continue;
					}
					sourceFile.Result = Models.CopyResult.Success;
					job.Task.CopiedFileCount++;
					job.Task.CopiedFileSize += sourceFile.FileInfo.Length;
				}
			}
			finally
			{
				sftpClient.Disconnect();
			}
		}

		protected void CreateServerDirectoryIfItDoesntExist(Renci.SshNet.SftpClient sftpClient, string serverDestinationPath)
		{
			if (serverDestinationPath[0] == '/')
				serverDestinationPath = serverDestinationPath.Substring(1);

			string[] directories = serverDestinationPath.Split('/');
			for (int i = 0; i < directories.Length; i++)
			{
				string dirName = string.Join("/", directories, 0, i + 1);
				if (!sftpClient.Exists(dirName))
					sftpClient.CreateDirectory(dirName);
			}
		}


		public void CopyFile(Renci.SshNet.SftpClient sftpClient, string localFilePath, string remoteFilePath)
		{
			System.IO.Stream stream = System.IO.File.OpenRead(localFilePath);
			sftpClient.UploadFile(stream, remoteFilePath, true);
			stream.Close();
		}
	}
}
