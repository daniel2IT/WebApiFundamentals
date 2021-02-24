
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
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //Get Individual 
        [Route("{id:int}", Name = "GetTalk")]
        public async Task<IHttpActionResult> Get(string moniker, int id, bool includeSpeakers = false)
        {
            try
            {
                var result = await _repository.GetTalkByMonikerAsync(moniker, id, includeSpeakers);
                if (result == null) return NotFound();

                return Ok(_mapper.Map<TalkModel>(result));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //post 
        [Route()]
        public async Task<IHttpActionResult> Post(string moniker, TalkModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var camp = await _repository.GetCampAsync(moniker);
                    if (camp != null)
                    {
                        var talk = _mapper.Map<Talk>(model);
                        talk.Camp = camp;

                        //Map the Speaker if Necessary
                        if (model.Speaker != null)
                        {
                            var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                            if (speaker != null) talk.Speaker = speaker;
                        }


                        _repository.AddTalk(talk);

                        if (await _repository.SaveChangesAsync())
                        {
                            return CreatedAtRoute("GetTalk",
                                new { moniker = moniker, id = talk.TalkId },
                                _mapper.Map<TalkModel>(talk));
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return BadRequest(ModelState);// ModelState - Contains all Validation Errors
        }

        //Put
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Put(string moniker, int talkId,
            TalkModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var talk = await _repository.GetTalkByMonikerAsync(moniker, talkId, true);
                    if (talk == null) return NotFound();

                    // it's going to ignore the Speaker 
                    _mapper.Map(model, talk);

                    // Change speaker if needed
                    //talk.Speaker.SpeakerId - Existing ID
                    // model.Speaker.SpeakerId - passed ID
                    if (talk.Speaker.SpeakerId != model.Speaker.SpeakerId)
                    {
                        var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                        if (speaker != null) talk.Speaker = speaker;
                    }

                    if(await _repository.SaveChangesAsync())
                    {
                        return Ok(_mapper.Map<TalkModel>(model));
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return BadRequest(ModelState);
        } 
    }
}
