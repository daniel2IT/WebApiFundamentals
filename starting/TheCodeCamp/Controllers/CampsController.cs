using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using TheCodeCamp.Data;
using TheCodeCamp.Models;

namespace TheCodeCamp.Controllers
{
    // All methods will start with
    [RoutePrefix("api/camps")]
    public class CampsController : ApiController
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;

        public CampsController(ICampRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        [Route()]
        public async Task<IHttpActionResult> Get(bool includeTalks = false)
        {
            try
            {
            var result = await _repository.GetAllCampsAsync(includeTalks);

                // Mapping (Models)
                var mappedResult = _mapper.Map<IEnumerable<CampModel>>(result);

                return Ok(mappedResult);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // localhost:6060/api/camps + /{moniker}
        [Route("{moniker}", Name = "NewDanielCamp")]
        public async Task<IHttpActionResult> Get(string moniker)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker);
                if (result == null){ return NotFound(); }
                return Ok(_mapper.Map<CampModel>(result));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        //http://localhost:6600/api/camps/SearchByDate/2018-10-18
        [Route("SearchByDate/{eventDate:datetime}")]
        [HttpGet]
        public async Task<IHttpActionResult> SearchByEventDate(DateTime eventDate, bool includeTalks = false)
        {
            try
            {
                var result = await _repository.GetAllCampsByEventDate(eventDate, includeTalks);

                return Ok(_mapper.Map<CampModel[]>(result));        
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //Post
        [Route()]
        public async Task<IHttpActionResult> Post(CampModel modelForBinding)
        {
             try
            {
                // we need our moniker(is uniq in our way .. so)
                if (await _repository.GetCampAsync(modelForBinding.Moniker) != null)
                {
                    ModelState.AddModelError("Moniker", "Moniker in use");
                   /* return BadRequest("Moniker in use");*/
                }
                if (ModelState.IsValid) 
                {
                    var camp = _mapper.Map<Camp>(modelForBinding);

                    _repository.AddCamp(camp);

                    if(await _repository.SaveChangesAsync())
                    {
                        var newModel = _mapper.Map<CampModel>(camp);

                        return CreatedAtRoute("NewDanielCamp", new { moniker = newModel.Moniker }, newModel);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            // tell that sent data isn't good
            return BadRequest(ModelState);
        }
    }
}
