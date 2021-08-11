using Sample.Order.BE.Api.DTOs;
using Sample.Order.BE.Business.Helpers;
using Sample.Order.BE.Business.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Order.BE.Api.Controllers
{
    /// <summary>
    /// Base controller
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseController<T> : ControllerBase
    {
        /// <summary>
        /// Instance of the Logger
        /// </summary>
        internal readonly ILogger _logger;
        /// <summary>
        /// Instance of the Mapper
        /// </summary>
        internal readonly IMapper _mapper;

        /// <summary>
        /// Constructor for the Base Controller.
        /// </summary>
        /// <param name="logger">Logger dependency object</param>
        /// <param name="mapper">Mapper dependency object</param>
        public BaseController(ILogger<T> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }
    }
}
