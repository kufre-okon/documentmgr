using documentmgr.data.Repositories.Interfaces;
using documentmgr.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace documentmgr.Controllers
{
    public abstract class BaseController : Controller
    {
        public readonly ILogger _logger;
        public readonly IUnitOfWork _unitOfWork;

        public BaseController(IServiceProvider serviceProvider)
        {           
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            _logger = serviceProvider.GetRequiredService<ILogger>();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual ResponseBase OnError(string errorMessage)
        {
            ResponseBase responseBase = new ResponseBase();
            responseBase.Message = errorMessage;
            responseBase.Status = false;

            return responseBase;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual ResponseBase<T> OnError<T>(string errorMessage)
        {
            ResponseBase<T> responseBase = new ResponseBase<T>();
            responseBase.Message = errorMessage;
            responseBase.Status = false;

            return responseBase;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual ResponseBase ExecuteRequest(Action executeMethod)
        {
            ResponseBase responseBase = new ResponseBase();
            try
            {
                executeMethod();
            }
            catch (ApplicationException ex)
            {
                responseBase.Message = ex.Message;
                responseBase.Status = false;
            }
            catch (Exception ex)
            {
                  _logger.Error(ex, ex.Message);
                responseBase.Message = "An error occured while processing your request";
                responseBase.Status = false;

            }
            return responseBase;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual async Task<ResponseBase> ExecuteRequestAsync(Func<Task> executeMethod)
        {
            ResponseBase responseBase = new ResponseBase();
            try
            {
                await executeMethod();
            }
            catch (ApplicationException ex)
            {
                responseBase.Message = ex.Message;
                responseBase.Status = false;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                responseBase.Message = "An error occured while processing your request";
                responseBase.Status = false;

            }
            return responseBase;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual async Task<ResponseBase<T>> ExecuteRequestAsync<T>(Func<Task<T>> executeMethod)
        {
            ResponseBase<T> responseBase = new ResponseBase<T>();
            try
            {
                responseBase.Payload = await executeMethod();
            }
            catch (ApplicationException ex)
            {
                responseBase.Message = ex.Message;
                responseBase.Status = false;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                responseBase.Message = "An error occured while processing your request";
                responseBase.Status = false;

            }
            return responseBase;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual ResponseBase<T> ExecuteRequest<T>(Func<T> executeMethod)
        {
            ResponseBase<T> responseBase = new ResponseBase<T>();
            try
            {
                responseBase.Payload = executeMethod();
            }
            catch (ApplicationException ex)
            {
                responseBase.Message = ex.Message;
                responseBase.Status = false;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                responseBase.Message = "An error occured while processing your request";
                responseBase.Status = false;

            }
            return responseBase;
        }
    }
}
