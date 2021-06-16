using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;

namespace Hishop.TransferManager
{
    internal class TransferContainer
    {

        private static readonly object LockHelper = new object();
        private static volatile TransferContainer _instance = null;
        private static volatile Cache TransferCache = HttpRuntime.Cache;
        private const string CacheKey = "Hishop_TransferIndexes";

        private TransferContainer()
        {
            TransferCache.Remove(CacheKey);
        }

        internal static TransferContainer Instance()
        {
            if (_instance == null)
            {
                lock (LockHelper)
                {
                    if (_instance == null)
                    {
                        _instance = new TransferContainer();
                    }
                }
            }

            Init();
            return _instance;
        }

        internal Type GetExporter(string fullName)
        {
            return GetPlugin(fullName, "Exporters");
        }

        internal Type GetImporter(string fullName)
        {
            return GetPlugin(fullName, "Importers");
        }

        internal DataRow[] GetExporterList(string sourceName, string exportToName)
        {
            DataSet indexSet = TransferCache.Get(CacheKey) as DataSet;
            return
                indexSet.Tables["Exporters"].Select(
                    string.Format("sourceName='{0}' and exportToName='{1}'", sourceName, exportToName),
                    "sourceVersion desc");
        }

        internal DataRow[] GetImporterList(string sourceName, string importToName)
        {
            DataSet indexSet = TransferCache.Get(CacheKey) as DataSet;
            return indexSet.Tables["Importers"].Select(
                    string.Format("sourceName='{0}' and importToName='{1}'", sourceName, importToName),
                    "importToVersion desc");
        }

        private static void Init()
        {
            if (TransferCache.Get(CacheKey) != null)
                return;

            string pluginPath = HttpContext.Current.Request.MapPath("~/plugins/transfer");
            DataSet indexSet = new DataSet();
            DataTable dtExporters = new DataTable("Exporters");
            DataTable dtImporters = new DataTable("Importers");

            InitTable(dtExporters);
            InitTable(dtImporters);

            dtExporters.Columns.Add(new DataColumn("exportToName"));
            dtExporters.Columns.Add(new DataColumn("exportToVersion"));

            dtImporters.Columns.Add(new DataColumn("importToName"));
            dtImporters.Columns.Add(new DataColumn("importToVersion"));

            indexSet.Tables.Add(dtExporters);
            indexSet.Tables.Add(dtImporters);

            BuildIndex(pluginPath, dtExporters, dtImporters);
            TransferCache.Insert(CacheKey, indexSet, new CacheDependency(pluginPath));
        }

        private static void InitTable(DataTable table)
        {
            table.Columns.Add(new DataColumn("fullName") {Unique = true});
            table.Columns.Add(new DataColumn("filePath"));
            table.Columns.Add(new DataColumn("sourceName"));
            table.Columns.Add(new DataColumn("sourceVersion"));
            table.PrimaryKey = new[] {table.Columns["fullName"]};
        }

        private static void BuildIndex(string pluginPath, DataTable dtExporters, DataTable dtImporters)
        {
            if (!Directory.Exists(pluginPath))
                return;

            string[] pluginDlls = Directory.GetFiles(pluginPath, "*.dll", SearchOption.AllDirectories);

            foreach (string file in pluginDlls)
            {
                Assembly assembly = Assembly.Load(LoadPluginFile(file));
                foreach (Type t in assembly.GetExportedTypes())
                {
                    if (t.BaseType == null)
                        continue;

                    if (t.BaseType.Name == "ExportAdapter")
                    {
                        AddToExportIndex(t, file, dtExporters);
                    }
                    else if (t.BaseType.Name == "ImportAdapter")
                    {
                        AddToImportIndex(t, file, dtImporters);
                    }
                }
            }
        }

        private static byte[] LoadPluginFile(string filename)
        {
            byte[] buffer;
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                buffer = new byte[(int)fs.Length];
                fs.Read(buffer, 0, buffer.Length);
            }
            return buffer;
        }

        private static void AddToExportIndex(Type t, string filename, DataTable dtExporters)
        {
            ExportAdapter adapter = Activator.CreateInstance(t) as ExportAdapter;
            DataRow row = dtExporters.NewRow();

            row["fullName"] = t.FullName.ToLower();
            row["filePath"] = filename;
            row["sourceName"] = adapter.Source.Name;
            row["sourceVersion"] = adapter.Source.Version.ToString();
            row["exportToName"] = adapter.ExportTo.Name;
            row["exportToVersion"] = adapter.ExportTo.Version.ToString();

            dtExporters.Rows.Add(row);
        }

        private static void AddToImportIndex(Type t, string filename, DataTable dtImporters)
        {
            ImportAdapter adapter = Activator.CreateInstance(t) as ImportAdapter;
            DataRow row = dtImporters.NewRow();

            row["fullName"] = t.FullName.ToLower();
            row["filePath"] = filename;
            row["sourceName"] = adapter.Source.Name;
            row["sourceVersion"] = adapter.Source.Version.ToString();
            row["importToName"] = adapter.ImportTo.Name;
            row["importToVersion"] = adapter.ImportTo.Version.ToString();

            dtImporters.Rows.Add(row);
        }

        private static Type GetPlugin(string fullName, string tableName)
        {
            DataSet indexSet = TransferCache.Get(CacheKey) as DataSet;
            DataRow[] result = indexSet.Tables[tableName].Select("fullName='" + fullName.ToLower() + "'");

            if (result.Length == 0 || !File.Exists(result[0]["filePath"].ToString()))
                return null;

            Assembly assembly = Assembly.Load(LoadPluginFile(result[0]["filePath"].ToString()));
            return assembly.GetType(fullName, false, true);
        }

    }
}