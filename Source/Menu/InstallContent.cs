// COPYRIGHT 2009 - 2019 by the Open Rails project.
// 
// This file is part of Open Rails.
// 
// Open Rails is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Open Rails is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Open Rails.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip; // Installed via NuGet and then referenced in this project. May move if a dedicated project is built.
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading;

namespace ORTS
{
    class InstallContent
    {
        //CJ

        public  event EventHandler<ProgressChangedEventArgs> ApplyProgressChanged;
        private const string ProductName = "OpenRails";
        private const string SourceUrl = "http://www.openrails.org/files/DemoModel1.zip";
        private const string ContentName = "DemoModel1";
        private const string SourceFolderPath = @"C:\dev\_tmp\source";
        private const string DestinationFolderPath = @"C:\dev\_tmp\dest";

        private string ContentFile = $"{ContentName}.zip";
        private string SourceFilePath { get { return Path.Combine(SourceFolderPath, ContentFile); } }
        private string DestinationFilePath { get { return Path.Combine(DestinationFolderPath, ContentFile); } }


        public void Dev()
        {
            // Given source filepath, unzip it into destination filepath

            // To work with zip archives, OpenRails uses System.IO.Package in ImportExportSaveForm
            // and Ionic.Zip in Update.UpdateManager.
            // We'll follow UpdateManager here.
            DownloadContent(SourceUrl, SourceFolderPath, SourceFilePath, 0, 100);
            ExtractContent(SourceFilePath, DestinationFolderPath, 0, 100);

        }

        private void DownloadContent(string sourceUrl, string destFolderPath, string destFilePath, int progressMin, int progressLength)
        {
            if (!Directory.Exists(destFolderPath))
                Directory.CreateDirectory(destFolderPath);

            if (!File.Exists(destFilePath))
                File.Delete(destFilePath);

            var uri = new Uri(sourceUrl);
            var client = new WebClient();
            AsyncCompletedEventArgs done = null;
            client.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) =>
            {
                try
                {
                    TriggerApplyProgressChanged(progressMin + progressLength * e.ProgressPercentage / 100);
                }
                catch (Exception error)
                {
                    done = new AsyncCompletedEventArgs(error, false, e.UserState);
                }
            };
            client.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
            {
                done = e;
            };
            client.Headers[HttpRequestHeader.UserAgent] = ProductName;

            client.DownloadFileAsync(uri, destFilePath);
            while (done == null)
            {
                Thread.Sleep(100);
            }
            if (done.Error != null)
                throw done.Error;

            TriggerApplyProgressChanged(progressMin + progressLength);
        }

        private  void ExtractContent(string sourcePath, string destPath, int progressMin, int progressLength)
        {
            if (!Directory.Exists(destPath)) throw new ArgumentException("The specified path must be valid and exist as a directory.", "destPath");

            TriggerApplyProgressChanged(0);
            using (var zip = Ionic.Zip.ZipFile.Read(sourcePath))
            {
                zip.ExtractProgress += (object sender, ExtractProgressEventArgs e) =>
                {
                    if (e.EventType == ZipProgressEventType.Extracting_BeforeExtractEntry)
                    {
                        TriggerApplyProgressChanged(progressMin + progressLength * e.EntriesExtracted / e.EntriesTotal);
                    }
                };
                zip.ExtractAll(destPath, ExtractExistingFileAction.OverwriteSilently);
            }

            TriggerApplyProgressChanged(progressMin + progressLength);
        }

        private  void TriggerApplyProgressChanged(int progressPercentage)
        {
            var progressEvent = ApplyProgressChanged;
            if (progressEvent != null)
                progressEvent(this, new ProgressChangedEventArgs(progressPercentage, null));
        }
    }
}
