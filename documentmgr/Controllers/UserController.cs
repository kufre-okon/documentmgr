using documentmgr.Models;
using documentmgr.Controllers;
using documentmgr.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using documentmgr.business.Services;
using documentmgr.dto;
using documentmgr.business.Utilities;
using System.IO;
using documentmgr.core.Models;

namespace documentmgr.Controllers
{
    public class UserController : BaseController
    {
        private readonly IDocumentService docService;
        private readonly IUserService userService;
        private readonly IFileUtility fileUtility;
       
        public UserController(IServiceProvider serviceProvider, IDocumentService docService
            , IUserService userService, IFileUtility fileUtility) : base(serviceProvider)
        {
            this.docService = docService;
            this.userService = userService;
            this.fileUtility = fileUtility;
        }

        [HttpGet]
        public IActionResult New()
        {
            var createUserDto = new CreateUserDto();

            return View(createUserDto);
        }

        [HttpPost]
        public async Task<IActionResult> New(CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
                return View(createUserDto);

            try
            {
                _unitOfWork.BeginTransaction();
                var user = await userService.CreateUser(createUserDto);
                await docService.UpdateDocument(user.Id, createUserDto.Documents.Select(d => d.Id));

                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();

                await userService.SendDocumentEmail(user);

                TempData["success"] = "User created successfully. Your transaction number is " + user.TransactionNumber;
                return RedirectToAction("index", "home");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();

                _logger.Error(ex, ex.Message);
                ViewBag.error = ex.Message;

                return View(createUserDto);
            }
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> SearchUser([FromQuery] string email, [FromQuery] string transactionNumber)
        {
            string errorMessage = "";
            try
            {
                var user = await userService.SearchUser(email, transactionNumber);

                var model = new SearchUserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    MiddleName = user.LastName,
                    TransactionNumber = user.TransactionNumber
                };

                foreach (var doc in user.DOCUMENTS)
                {
                    bool isImage = this.fileUtility.IsImage(doc.ContentType, doc.Size);
                    model.Documents.Add(new FileSearchDto
                    {
                        IsImage = isImage,
                        Extension = doc.Extension,
                        Id = doc.Id,
                        Title = doc.Title,
                        FilePath = doc.FilePath,
                        ContentType = doc.ContentType
                    });
                }

                return PartialView("_UserSearchResult", model);
            }
            catch (ApplicationException ex)
            {
                errorMessage = ex.Message;              
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                errorMessage = "An error occurred while processing your request. Please try again later or contact support.";
            }
            return BadRequest(new ResponseBase
            {
                Status = false,
                Message = errorMessage
            });
        }

        [HttpGet]
        public async Task<ActionResult> DownloadDoc(int id)
        {
            try
            {
                var info = await docService.GetDocumentInfo(id);
                return File(System.IO.File.OpenRead(info.path), info.contentType, info.fileName);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return NotFound();
            }
        }

        public async Task<ResponseBase<FileUploadDto>> Upload(IFormFile file)
        {
            return await ExecuteRequestAsync(async () =>
            {
                var result = await docService.CreateDocument(file);

               await  _unitOfWork.SaveChangesAsync();

                return new FileUploadDto()
                {
                    Title = result.Title,
                    Id = result.Id,
                    Extension = result.Extension
                };
            });
        }

       
    }
}