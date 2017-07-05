using System;
using System.IO;
using System.Web;

namespace SignixDemo.Helpers
{
    public class FileUploaderHelper
    {
        public static string ServerUploadFolderPath => HttpContext.Current.Server.MapPath("~/UploadFiles");

        public static Guid SaveFile(byte[] fileBytes)
        {
            if (!Directory.Exists(ServerUploadFolderPath))
            {
                Directory.CreateDirectory(ServerUploadFolderPath);
            }

            var guid = Guid.NewGuid();
            var fullPath = ServerUploadFolderPath + "\\" + guid;

            using (var fs = File.Create(fullPath))
            {
                fs.Write(fileBytes, 0, fileBytes.Length);
            }

            return guid;
        }

        public static byte[] GetFileBytes(Guid fileGuid)
        {
            return File.ReadAllBytes(ServerUploadFolderPath + "\\" + fileGuid);
        }
    }
}