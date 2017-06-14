using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using NHibernate;
using NHibernate.Cfg;

namespace DataAccess
{
	public static class SessionFactory
	{
		private const string ConfigName = "library.configuration.bin";

		private static ISessionFactory _factory;

		private static readonly Object ThreadLock = new Object();

		private static string _connectionString;
		public static string ConnectionString
		{
			get
			{
				if (_connectionString == null)
					using (var session = GetFactory().OpenSession())
						_connectionString = session.Connection.ConnectionString;
				return _connectionString;
			}
		}

		public static ISessionFactory GetFactory()
		{

			if (_factory != null) 
                return _factory;

			lock (ThreadLock)
			{
				if (_factory == null)
				{					
					var cfg = GetConfig();
					
					var f = cfg.BuildSessionFactory();
					Interlocked.Exchange(ref _factory, f);
				}
			}

			return _factory;
		}

		private static Configuration GetConfig()
		{
			Configuration cfg = LoadConfigFile();

			if (cfg != null)
				return cfg;

			cfg = new Configuration();
			cfg.Configure();

			// add mappings assembly
			cfg.AddAssembly(typeof (SessionFactory).Assembly);
			
			SaveConfigFile(cfg);

			return cfg;
		}

		private static Configuration LoadConfigFile()
		{
			if (false == IsConfigValid())
				return null;
			try
			{
				using (var file = File.Open(ConfigName, FileMode.Open))
				{
					var bf = new BinaryFormatter();
					return bf.Deserialize(file) as Configuration;
				}
			}
			catch
			{
				return null;
			}
		}

		private static bool IsConfigValid()
		{
			// if there is no cached config, 
			// force a new one to be built
			if (false == File.Exists(ConfigName))
				return false;

			var configInfo = new FileInfo(ConfigName);
			var asm = typeof(SessionFactory).Assembly;
			
            // if the assembly is newer, 
			// the serialized config is stale
			var asmInfo = new FileInfo(asm.Location);
			if (asmInfo.LastWriteTime > configInfo.LastWriteTime)
				return false;

			// if the app.config is newer, 
			// the serialized config is stale
			var appDomain = AppDomain.CurrentDomain;
			var appConfigPath = appDomain.SetupInformation.ConfigurationFile;
			var appConfigInfo = new FileInfo(appConfigPath);
			if (appConfigInfo.LastWriteTime > configInfo.LastWriteTime)
				return false;
			            
			return true;
		}

		private static void SaveConfigFile(Configuration cfg)
		{
			using (var file = File.Open(ConfigName, FileMode.Create))
			{
				var bf = new BinaryFormatter();
				bf.Serialize(file, cfg);
			}
		}
	}
}
