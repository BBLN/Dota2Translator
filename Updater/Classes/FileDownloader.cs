﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Updater
{
    class FileDownloader
    {
        // The path to the temporary downloaded file.
        private String TemporaryFile;

        // Whether or not the download succeeded.
        private Boolean Downloaded = false;

        // The name of the file.
        private String _FileName;
        public String FileName
        {
            get
            {
                return _FileName;
            }
        }

        public FileDownloader(String fileName)
        {
            this._FileName = fileName;
        }

        // Converts an array of file names into an array of FileDownloader instances.
        public static FileDownloader[] GetFiles(String[] fileNames)
        {
            FileDownloader[] files = new FileDownloader[fileNames.Length];
            for (int i = 0; i < fileNames.Length; i++)
            {
                files[i] = new FileDownloader(fileNames[i]);
            }

            return files;
        }

        // Downloads the file to a temporary location.
        public void Download()
        {
            try
            {
                // Send a request to the server.
                WebRequest request = WebRequest.Create(App.UpdateUrl + "files/" + FileName);
                request.Proxy = null; // Don't use a proxy.

                // Get the response stream.
                Stream stream = request.GetResponse().GetResponseStream();

                // Create a temporary file.
                TemporaryFile = Path.Combine(Path.GetTempPath(), "Dota 2 Translator/" + Guid.NewGuid().ToString());
                Directory.CreateDirectory(Path.GetDirectoryName(TemporaryFile));
                Stream outStream = new FileStream(TemporaryFile, FileMode.Create);

                // Download the data to the newly created file.
                stream.CopyTo(outStream);

                // Mark the file as succeeded.
                Downloaded = true;
            }
            catch
            {
            }
        }

        // Applies the update to the specified directory.
        public Boolean Apply(String path)
        {
            // Don't apply a file that has not been downloaded.
            if (!Downloaded)
                return false;

            // Write to tmp_Updater.exe instead of Updater.exe.
            if (FileName.Equals("Updater.exe"))
            {
                _FileName = "tmp_Updater.exe";
            }

            try
            {
                // Copy the temporary file to the correct directory.
                File.Copy(TemporaryFile, Path.Combine(path, FileName), true);
            }
            catch
            {
                return false;
            }

            return true;
        }

        // Deletes the temporary file.
        public void Cleanup()
        {
            try
            {
                File.Delete(TemporaryFile);
            }
            catch
            {
            }
        }

    }
}
