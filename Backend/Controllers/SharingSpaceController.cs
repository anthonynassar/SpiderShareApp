﻿using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Backend.DataObjects;
using Backend.Models;
using System.Security.Claims;
using System.Net;
using Backend.Extensions;

namespace Backend.Controllers
{
    [Authorize]
    public class SharingSpaceController : TableController<SharingSpace>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<SharingSpace>(context, Request, enableSoftDelete: true);
        }

        public string UserId => IdentityProvider + "_" + NameIdentifier;

        public string NameIdentifier => ((ClaimsPrincipal)User).FindFirst(ClaimTypes.NameIdentifier).Value.Split(':')[1];

        public string IdentityProvider => ((ClaimsPrincipal)User).FindFirst("http://schemas.microsoft.com/identity/claims/identityprovider").Value;

        public void ValidateOwner(string id)
        {
            var result = Lookup(id).Queryable.PerUserFilter(UserId).FirstOrDefault<SharingSpace>();
            if (result == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        // GET tables/SharingSpace
        public IQueryable<SharingSpace> GetAllSharingSpace()
        {
            return Query().PerUserFilter(UserId);
        }

        // GET tables/SharingSpace/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<SharingSpace> GetSharingSpace(string id)
        {
            return new SingleResult<SharingSpace>(Lookup(id).Queryable.PerUserFilter(UserId));
        }

        // PATCH tables/SharingSpace/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<SharingSpace> PatchSharingSpace(string id, Delta<SharingSpace> patch)
        {
            ValidateOwner(id);
            return UpdateAsync(id, patch);
        }

        // POST tables/SharingSpace
        public async Task<IHttpActionResult> PostSharingSpace(SharingSpace item)
        {
            item.UserId = UserId;
            SharingSpace current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/SharingSpace/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteSharingSpace(string id)
        {
            ValidateOwner(id);
            return DeleteAsync(id);
        }
    }
}
