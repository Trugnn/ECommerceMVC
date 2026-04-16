namespace ECommerceMVC.Helpers
{
    public class MyUtil
    {
        public static string UploadHinh(IFormFile Hinh, string folder)
        {
            try
            {
                var extension = Path.GetExtension(Hinh.FileName);
                var fileName = Guid.NewGuid().ToString() + extension;

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    Hinh.CopyTo(stream);
                }

                return fileName;
            }
            catch
            {
                return string.Empty;
            }
        }

    }
}
