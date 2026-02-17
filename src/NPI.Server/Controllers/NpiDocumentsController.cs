using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPI.Data.Service;

namespace NPI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NpiDocumentsController : ControllerBase
    {
        private readonly INpiDocumentService _documentService;

        public NpiDocumentsController(INpiDocumentService documentService)
        {
            _documentService = documentService;
        }

        // ── Upload single file ──
        [HttpPost("{npiRecordId}/upload")]
        public async Task<IActionResult> UploadFile(int npiRecordId, [FromForm] IFormFile file)
        {
            if (file == null)
                return BadRequest("No file uploaded.");

            string uploadedBy = User.Identity?.Name ?? "System";

            var document = await _documentService.UploadFileAsync(npiRecordId, file, uploadedBy);
            return Ok(new { document.Id, document.FileName, document.FilePath });
        }

        [HttpPost("{npiRecordId}/upload-multiple")]
        public async Task<IActionResult> UploadMultiple( int npiRecordId, [FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files uploaded");

            var uploadedBy = User.Identity?.Name ?? "System";

            foreach (var file in files)
            {
                await _documentService.UploadFileAsync(
                    npiRecordId,
                    file,
                    uploadedBy);
            }

            return Ok();
        }

    }

}
