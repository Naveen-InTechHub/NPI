using Microsoft.AspNetCore.Http;
using NPI.Data.Entities;
using NPI.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPI.Data.Service
{
    public interface INpiDocumentService
    {
        Task<NpiDocument> UploadFileAsync(int npiRecordId, IFormFile file, string uploadedBy);
        Task<IEnumerable<NpiDocument>> UploadFilesAsync(int npiRecordId, IEnumerable<IFormFile> files, string uploadedBy);
        Task<bool> DeleteFile(int documentId);
    }

    public class NpiDocumentService : INpiDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public NpiDocumentService(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        public async Task<bool> DeleteFile(int documentId)
        {
            try
            {
                var document = await _unitOfWork.Documents.GetByIdAsync(documentId);
                if (document == null)
                    return false;
                var filePath = Path.Combine(_env.ContentRootPath, document?.FilePath?.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                _unitOfWork.Documents.Remove(document);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<NpiDocument> UploadFileAsync(int npiRecordId, IFormFile file, string uploadedBy)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            var document = await SaveFileAsync(npiRecordId, file, uploadedBy);
            return document;
        }

        public async Task<IEnumerable<NpiDocument>> UploadFilesAsync(int npiRecordId, IEnumerable<IFormFile> files, string uploadedBy)
        {
            try
            {
                var uploadedDocuments = new List<NpiDocument>();

                foreach (var file in files)
                {
                    var doc = await SaveFileAsync(npiRecordId, file, uploadedBy);
                    uploadedDocuments.Add(doc);
                }

                return uploadedDocuments;
            }
            catch(Exception ex)
            {
                throw new Exception();
            }
        }

        private async Task<NpiDocument> SaveFileAsync(int npiRecordId, IFormFile file, string uploadedBy)
        {
            try
            {
                // Unique file name
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

                var uploadPath = Path.Combine(_env.ContentRootPath, "uploads");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, uniqueFileName);

                // Save file
                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                // Create entity
                var document = new NpiDocument
                {
                    NpiRecordId = npiRecordId,
                    FileName = file.FileName,
                    FilePath = $"/uploads/{uniqueFileName}",
                    FileSize = file.Length,
                    UploadedBy = uploadedBy,
                    UploadedDate = DateTime.UtcNow
                };

                // Save to DB via unit of work
                await _unitOfWork.Documents.AddAsync(document);
                await _unitOfWork.SaveChangesAsync();

                return document;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

}
