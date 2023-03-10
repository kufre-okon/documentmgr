using documentmgr.business.Utilities;
using documentmgr.core.Models;
using documentmgr.data.Repositories.Interfaces;
using documentmgr.dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace documentmgr.business.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IFileUtility fileUtility;
        private readonly IUnitOfWork unitOfWork;
        public DocumentService(IFileUtility fileUtility, IUnitOfWork unitOfWork)
        {
            this.fileUtility = fileUtility;
            this.unitOfWork = unitOfWork;
        }
        public async Task<CreateDocumentDto> CreateDocument(IFormFile file, int? userId = null)
        {
            var docRepo = unitOfWork.GetRepository<Document>();

            var uploadResult = fileUtility.UploadFile(file, "/documents");

            var document = await docRepo.AddAsync(new Document
            {
                Extension = uploadResult.Extension,
                FilePath = uploadResult.LocalFilePath,
                ContentType = uploadResult.ContentType,
                Size = uploadResult.FileLength,
                Title = uploadResult.FileName,
                UserId = userId
            });

            await unitOfWork.SaveChangesAsync();

            return new CreateDocumentDto
            {
                DateCreated = document.DateCreated,
                Extension = document.Extension,
                Id = document.Id,
                LocalFilePath = document.FilePath,
                ContentType = uploadResult.ContentType,
                Size = document.Size,
                UserId = document.UserId,
                Title = document.Title
            };
        }

        public async Task<Document> FindById(int id)
        {
            var docRepo = unitOfWork.GetRepository<Document>();
            var doc = await docRepo.GetByIdAsync(id);
            return doc;
        }

        public async Task<(string path, string contentType, string fileName)> GetDocumentInfo(int id)
        {
            var result = await FindById(id);

            return getDocumentInfo(result);
        }

        public (string path, string contentType, string fileName) GetDocumentInfo(Document document)
        {
            return getDocumentInfo(document);

        }

        private (string path, string contentType, string fileName) getDocumentInfo(Document document)
        {
            if (document == null)
                throw new ApplicationException("Document not found");

            var path = fileUtility.MapPath(document.FilePath);

            if (!File.Exists(path))
                throw new ApplicationException("Document file not found");

            var contentType = document.ContentType.IsNotEmpty() ? document.ContentType : "application/octet-stream";


            return (path, contentType, Path.GetFileName(path));

        }

        public async Task UpdateDocument(int userId, IEnumerable<int> documentIds)
        {
            var docRepo = unitOfWork.GetRepository<Document>();
            var docs = docRepo.GetList(d => documentIds.Contains(d.Id));
            foreach (var doc in docs)
            {
                doc.UserId = userId;
            }
            docRepo.Update(docs);
            await unitOfWork.SaveChangesAsync();
        }
    }


    public interface IDocumentService
    {
        Task<CreateDocumentDto> CreateDocument(IFormFile file, int? userId = null);
        Task UpdateDocument(int id, IEnumerable<int> documentIds);
        Task<Document> FindById(int id);
        Task<(string path, string contentType, string fileName)> GetDocumentInfo(int id);
        (string path, string contentType, string fileName) GetDocumentInfo(Document document);
    }
}