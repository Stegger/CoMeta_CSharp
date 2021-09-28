using System;
using System.Linq;
using System.Threading.Tasks;
using CoMeta.Data;
using CoMeta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CoMeta.Policies
{
    public class MessageOwnerHandler : AuthorizationHandler<MessageOwnerPolicy>
    {
        private IRepository<Message> _messageRepository;
        private IHttpContextAccessor _httpContextAccessor;

        public MessageOwnerHandler(IHttpContextAccessor httpContextAccessor, IRepository<Message> messageRepository)
        {
            _messageRepository = messageRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MessageOwnerPolicy requirement)
        {
            string userName = context.User.Identity.Name;
            var routeData = _httpContextAccessor.HttpContext.GetRouteData();
            Console.WriteLine(routeData.ToString());
            return Task.FromResult(0);            
        }
    }
}