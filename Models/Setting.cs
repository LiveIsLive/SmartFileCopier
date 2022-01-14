﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.SmartFileCopier.Models
{
	public class Setting
	{
		public string SelectedCultureName { get; set; }

		public byte MaxRecentFileCount { get; set; } = 10;

		private System.Collections.ObjectModel.ObservableCollection<string> _RecentFiles;
		public System.Collections.ObjectModel.ObservableCollection<string> RecentFiles
		{
			get
			{
				if (this._RecentFiles == null)
					this._RecentFiles = new System.Collections.ObjectModel.ObservableCollection<string>();
				return this._RecentFiles;
			}
			set
			{
				this._RecentFiles = value;
			}
		}

		protected static readonly string SavePath = System.AppDomain.CurrentDomain.BaseDirectory + "Setting.json";

		private static Setting _Instance;
		public static Setting Instance
		{
			get
			{
				if (_Instance == null)
					if (System.IO.File.Exists(SavePath))
					{
						System.IO.StreamReader reader = new System.IO.StreamReader(SavePath);
						//_Instance = NetJSON.NetJSON.Deserialize<Setting>(reader);
						_Instance = new Newtonsoft.Json.JsonSerializer().Deserialize<Setting>(new Newtonsoft.Json.JsonTextReader(reader));
						reader.Close();
					}
					else _Instance = new Setting();
				return _Instance;
			}
		}

		public void AddRecentFile(string path)
		{
			int index = this.RecentFiles.IndexOf(path);
			if (index >= 0)
				this.RecentFiles.Move(index, 0);
			else this.RecentFiles.Insert(0, path);
			if (this.RecentFiles.Count > this.MaxRecentFileCount)
				this.RecentFiles.RemoveAt(this.RecentFiles.Count - 1);
		}

		public void Save()
		{
			System.IO.StreamWriter writer = new System.IO.StreamWriter(SavePath);
			//NetJSON.NetJSON.Serialize(this, writer);
			new Newtonsoft.Json.JsonSerializer().Serialize(writer, this);
			writer.Close();
		}
	}
}