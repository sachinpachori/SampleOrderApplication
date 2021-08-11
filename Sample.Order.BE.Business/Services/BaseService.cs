using Microsoft.Extensions.Logging;

namespace Sample.Order.BE.Business.Services
{
    public abstract class BaseService
    {
        internal readonly ILogger _logger;

        /// <summary>
        /// The service logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
        }

       

        public BaseService( ILogger logger)
        {
          
            _logger = logger;
        }
    }
}
