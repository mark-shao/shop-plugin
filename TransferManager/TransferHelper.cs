using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;

namespace Hishop.TransferManager
{
    public static class TransferHelper
    {

        public static Dictionary<string, string> GetExportAdapters(Target source,string exportToName)
        {
            Dictionary<string, string> adapters = new Dictionary<string, string>();
            DataRow[] rows = TransferContainer.Instance().GetExporterList(source.Name, exportToName);

            if (rows == null || rows.Length == 0)
                return adapters;

            string versionStr = null;
            int counter = 0;

            do
            {
                Version v = new Version(rows[counter]["sourceVersion"].ToString());
                if (v <= source.Version)
                {
                    versionStr = rows[counter]["sourceVersion"].ToString();
                }
                counter++;
            } while (string.IsNullOrEmpty(versionStr) && counter < rows.Length);

            if (!string.IsNullOrEmpty(versionStr))
            {
                foreach (DataRow row in rows)
                {
                    string sourceVersion = row["sourceVersion"].ToString();
                    if (sourceVersion.Equals(versionStr))
                    {
                        adapters.Add(row["fullName"].ToString(),
                                     row["exportToName"].ToString() + row["exportToVersion"].ToString());
                    }
                }
            }

            return adapters;
        }

        public static Dictionary<string, string> GetImportAdapters(Target importTo, string sourceName)
        {
            Dictionary<string, string> adapters = new Dictionary<string, string>();
            DataRow[] rows = TransferContainer.Instance().GetImporterList(sourceName, importTo.Name);

            if (rows == null || rows.Length == 0)
                return adapters;

            string versionStr = null;
            int counter = 0;

            do
            {
                Version v = new Version(rows[counter]["importToVersion"].ToString());
                if (v <= importTo.Version)
                {
                    versionStr = rows[counter]["importToVersion"].ToString();
                }
                counter++;
            } while (string.IsNullOrEmpty(versionStr) && counter < rows.Length);

            if (!string.IsNullOrEmpty(versionStr))
            {
                foreach (DataRow row in rows)
                {
                    string importToVersion = row["importToVersion"].ToString();
                    if (importToVersion.Equals(versionStr))
                    {
                        adapters.Add(row["fullName"].ToString(),
                                     row["sourceName"].ToString() + row["sourceVersion"].ToString());
                    }
                }
            }

            return adapters;
        }

        public static ExportAdapter GetExporter(string fullName, params object[] exportParams)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            Type type = TransferContainer.Instance().GetExporter(fullName);
            if (type == null)
                return null;

            if (exportParams != null && exportParams.Length > 0)
                return Activator.CreateInstance(type, exportParams) as ExportAdapter;

            return Activator.CreateInstance(type) as ExportAdapter;
        }

        public static ImportAdapter GetImporter(string fullName, params object[] exportParams)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            Type type = TransferContainer.Instance().GetImporter(fullName);
            if (type == null)
                return null;

            if (exportParams != null && exportParams.Length >0)
                return Activator.CreateInstance(type, exportParams) as ImportAdapter;

            return Activator.CreateInstance(type) as ImportAdapter;
        }

        public static byte[] ConvertToBytes(string imageUrl)
        {
            byte[] data = new byte[] { };

            if (String.IsNullOrEmpty(imageUrl))
                return data;

            string filePath = HttpContext.Current.Request.MapPath("~" + imageUrl);

            if (!File.Exists(filePath))
                return data;

            try
            {
                data = File.ReadAllBytes(filePath);
            }
            catch { }

            return data;
        }

        public static void WriteImageElement(XmlWriter writer, string nodeName, bool includeImages, string imageUrl, DirectoryInfo destDir)
        {
            writer.WriteStartElement(nodeName);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                if (includeImages)
                {
                    string filePath = HttpContext.Current.Request.MapPath("~" + imageUrl);
                    string filename = Path.GetFileName(filePath);

                    writer.WriteString(filename);

                    if (File.Exists(filePath))
                    {
                        File.Copy(filePath, Path.Combine(destDir.FullName, filename), true);
                    }
                }
                else
                {
                    writer.WriteString(imageUrl);
                }
            }

            writer.WriteEndElement();
        }

        public static void WriteImageElement(XmlWriter writer, string nodeName, bool includeImages, string imageUrl)
        {
            writer.WriteStartElement(nodeName);

            if (includeImages)
            {
                byte[] data = ConvertToBytes(imageUrl);
                writer.WriteBase64(data, 0, data.Length);
            }
            else
            {
                writer.WriteString(imageUrl);
            }

            writer.WriteEndElement();
        }

        public static void WriteCDataElement(XmlWriter writer, string nodeName, string text)
        {
            writer.WriteStartElement(nodeName);
            writer.WriteCData(text);
            writer.WriteEndElement();
        }

    }
}