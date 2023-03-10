using documentmgr.business.Utilities;
using documentmgr.core.Models;
using documentmgr.data.Repositories.Interfaces;
using documentmgr.dto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace documentmgr.business.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailSender emailSender;
        private readonly ILogger _logger;
        private readonly IFileUtility fileUtility;

        public UserService(IUnitOfWork unitOfWork, IEmailSender emailSender, IFileUtility fileUtility, ILogger _logger)
        {
            this.unitOfWork = unitOfWork;
            this.emailSender = emailSender;
            this.fileUtility = fileUtility;
            this._logger = _logger;
        }

        private string generateTransactionNumber()
        {
            return "TX" + Helper.GeneralRandomNumber(10000, 99999) + Helper.GenerateRandomString(10, 2);
        }

        public async Task<User> CreateUser(CreateUserDto createUserDto)
        {
            var userRepo = unitOfWork.GetRepository<User>();
            var userExist = userRepo.Single(u => u.Email == createUserDto.Email);
            if (userExist != null)
                throw new ApplicationException("Email already taken.");

            string transactionNumber = generateTransactionNumber();

            var user = await userRepo.AddAsync(new User
            {
                Email = createUserDto.Email,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                MiddleName = createUserDto.MiddleName,
                TransactionNumber = transactionNumber
            });

            await unitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task<User> SearchUser(string email, string transactionNumber)
        {
            var userRepo = unitOfWork.GetRepository<User>();
            var user = await userRepo.SingleAsync(
                predicate: u => u.Email == email && u.TransactionNumber == transactionNumber,
                includes: new Func<IQueryable<User>, IIncludableQueryable<User, object>>[]
                    {
                        u => u.Include(u => u.DOCUMENTS),
                    });
            if (user == null)
                throw new ApplicationException("Invalid email or transaction number");

            return user;
        }
        private (string path, string contentType, string fileName) getDocumentInfo(Document document)
        {
            var path = fileUtility.MapPath(document.FilePath);

            if (!File.Exists(path))
                return (null, null, null);

            var contentType = document.ContentType.IsNotEmpty() ? document.ContentType : "application/octet-stream";
            return (path, contentType, Path.GetFileName(path));
        }

        public async Task<bool> SendDocumentEmail(User user)
        {
            var sb = new StringBuilder();
            sb.Append($"Hi <b> {user.FirstName} {user.LastName}</b><br/>");
            sb.Append("Attached are your documents uploaded using the Document Manager");

            var attachments = new List<(string fileName, byte[] fileBytes, string contentType)>();
            try
            {
                foreach (var doc in user.DOCUMENTS)
                {
                    var fileInfo = getDocumentInfo(doc);
                    if (fileInfo.path.IsNotEmpty())
                    {
                        byte[] fileBytes;
                        using (var fs = new FileStream(fileInfo.path, FileMode.Open, FileAccess.Read))
                        {
                            using (var ms = new MemoryStream())
                            {
                                fs.CopyTo(ms);
                                fileBytes = ms.ToArray();
                                attachments.Add(new(fileInfo.fileName, fileBytes, fileInfo.contentType));
                            }
                        }
                    }
                }

                await emailSender.SendEmailAsync(
                    email: user.Email,
                    subject: "Document Upload",
                    htmlMessage: sb.ToString(),
                    attachments: attachments
                );
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return false;
            }
        }
    }

    public interface IUserService
    {
        Task<User> CreateUser(CreateUserDto createUserDto);
        Task<User> SearchUser(string email, string transactionNumber);
        Task<bool> SendDocumentEmail(User user);
    }
}