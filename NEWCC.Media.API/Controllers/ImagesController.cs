using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NEWCC.Media.API.Controllers
{
    using System.Threading.Tasks;

    using Swashbuckle.Swagger.Annotations;

    public class ImagesController : ApiController
    {
        [HttpPost]
        [SwaggerOperation("PostImages")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [Route("{id}/images")]
        public async Task<IHttpActionResult> PostImages(Guid id)
        {
            try
            {
                var file = (await HttpHelper.ReadFileFromRequestAsync(this.Request)).First();

                //Todo: Pull this from the principal 
                var customerId = TestCustomerId;

                var result = await this._svc.SaveImage(id, customerId, file.Filename, file.FileSize, file.Contents,

                file.ContentType, new List<KeyValuePair<string, string>>());

                // Image uploaded was over the file size limit return 400 Bad Request w/ message ErrorOverFileSizeLimit to inform the client
                if (result.Status == ServiceActionStatus.ErrorOverFileSizeLimit)
                    return this.BadRequest("ErrorOverFileSizeLimit");

                // We were not able to complete the create request with the values provided return (400 Bad request)
                if (result.Status != ServiceActionStatus.Created) return this.BadRequest();


                return this.Created(
                    this.Request.RequestUri + "/" + result.Id,
                    result.Entity);
            }
            catch (Exception ex)
            {
                //Todo: Logging
                throw;

            }

        }
    }
}
