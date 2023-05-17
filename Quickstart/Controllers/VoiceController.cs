using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Quickstart.Models.Configuration;
using Quickstart.Models;
using Twilio.TwiML;
using Twilio.TwiML.Voice;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Linq;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using System.Collections.Generic;
using Twilio.Types;
using System.Net.Mime;
using Twilio.Http;

namespace Quickstart.Controllers
{
    public class VoiceController : Controller
    {
        private readonly ILogger<VoiceController> _logger;
        private readonly TwilioAccountDetails _twilioAccountDetails;

        public VoiceController(IOptions<TwilioAccountDetails> twilioAccountDetails, ILogger<VoiceController> logger)
        {
            _twilioAccountDetails = twilioAccountDetails.Value ?? throw new ArgumentException(nameof(twilioAccountDetails));
            _logger = logger;
        }

        // POST: /voice
        [HttpPost]
        public IActionResult Index(string to, string callingDeviceIdentity)
        {
            var callerId = _twilioAccountDetails.CallerId;

            var twiml = new VoiceResponse();
            _logger.LogInformation($"to: {to}, callingDeviceIdentity: {callingDeviceIdentity}, thisDevice.Identity: {Device.Identity}");

            var dial = new Dial(callerId: callerId);
            dial.Record = Dial.RecordEnum.RecordFromAnswerDual;
            dial.RecordingStatusCallback = new Uri("https://twilio-test-qs.azurewebsites.net/RecordStatusCallBack");

            if (Regex.IsMatch(to, "^[\\d\\+\\-\\(\\) ]+$"))
            {
                _logger.LogInformation("Match is true");
                dial.Number(to);
            }
            twiml.Append(dial);

            _logger.LogInformation(twiml.ToString());

            return Content(twiml.ToString(), "text/xml");
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RecordStatusCallBack(int recordingDuration, string recordingUrl)
        {
            _logger.LogInformation("Parametrii trimis");
            _logger.LogInformation(recordingDuration.ToString());
            _logger.LogInformation(recordingUrl);

            using (var client = new System.Net.Http.HttpClient())
            {
                var uri = new Uri($"{recordingUrl}.mp3");
                client.BaseAddress = uri;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    System.Net.Http.HttpContent content = response.Content;
                    var contentStream = await content.ReadAsStreamAsync(); // get the actual content stream
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }

            return Ok();
        }
    }
}
