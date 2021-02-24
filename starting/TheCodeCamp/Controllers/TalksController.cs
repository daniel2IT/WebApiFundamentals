
using AutoMapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using TheCodeCamp.Data;
using TheCodeCamp.Models;

namespace TheCodeCamp.Controllers
{
    [RoutePrefix("api/camps/{moniker}/talks")]
    public class TalksController : ApiController
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;

        public TalksController(ICampRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // ?includeSpeakers = true
        [Route()]
        public async Task<IHttpActionResult> Get(string moniker, bool includeSpeakers = false)
        {
            try
            {
                var results = await _repository.GetTalksByMonikerAsync(moniker, includeSpeakers);

                return Ok(_mapper.Map<IEnumerable<TalkModel>>(results));
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //Get Individual 
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Get(string moniker, int id, bool includeSpeakers = false)
        {
            try
            {
                var result = await _repository.GetTalkByMonikerAsync(moniker, id, includeSpeakers);
                if (result == null) return NotFound();

                return Ok(_mapper.Map<TalkModel>(result));
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
