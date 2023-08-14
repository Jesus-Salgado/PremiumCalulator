using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Security.Principal;
using Newtonsoft.Json.Linq;

namespace WebApplication1.Controllers
{
    public class Identity
    {
        public string identity { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PremiumController : ControllerBase
    {
        [HttpPost("create")]
        public IActionResult create([FromBody] PremiumClass premium) {
        //This route is used to create a new Premium row
            try
            {
                //By the moment, in order to give some persistence to the aplication, is saving the new premiums in a csv file.
                string filePath = Path.Combine("Data", "Premiums.csv");

                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                StringBuilder csvLine = new StringBuilder();
                DateTimeOffset currentTime = DateTimeOffset.Now;

                //A timestamp is used as identifier for the row and the values are appended to the file
                csvLine.Append(currentTime.ToUnixTimeSeconds());
                csvLine.Append(",");
                csvLine.Append(premium.Carrier);
                csvLine.Append(",");
                csvLine.Append(string.Join("|", premium.Plan));
                csvLine.Append(",");
                csvLine.Append(premium.State.ToUpper());
                csvLine.Append(",");
                csvLine.Append(premium.MonthOfBirth);
                csvLine.Append(",");
                csvLine.Append(premium.AgeRangeStart);
                csvLine.Append(",");
                csvLine.Append(premium.AgeRangeEnd);
                csvLine.Append(",");
                csvLine.Append(premium.Premium);


                using (StreamWriter writer = new StreamWriter(filePath,true))
                {
                    //The file is now beign saved, if a header doesn't exists, it is added.
                    if (writer.BaseStream.Length == 0)
                    {
                        writer.WriteLine("Identifier,Carrier,Plan,State,MonthOfBirth,AgeRangeStart,AgeRangeEnd,Premium");
                    }
                    writer.WriteLine(csvLine);
                }

                var response = new { message = "Premium created sucesfully." };

                // The response is a simple confirmation message
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("delete_premium")]
        public IActionResult delete_premium([FromBody] Identity identity) {
            //This route is used to delete a Premium row
            try
            {
                //An identifier is used in order to delete the correct row
                string remove = identity.identity;

                //The file is opened
                string filePath = Path.Combine("Data", "Premiums.csv");

                List<string> lines = System.IO.File.ReadAllLines(filePath).ToList();

                //The row is removed
                bool removed = lines.RemoveAll(line => line.Contains(remove)) > 0;

                // A message is showed
                if (removed)
                {
                    System.IO.File.WriteAllLines(filePath, lines);
                    return Ok(new { message = "Removed sucesfully" });
                }
                else
                {
                    return Ok(new { message = "Doesn't exists" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("edit")]
        public IActionResult edit([FromBody] PremiumClass premium) {
        //This route is used to edit a Premium row
            try
            {
                //First, we remove from the csv the selected row
                string filePath = Path.Combine("Data", "Premiums.csv");

                List<string> lines = System.IO.File.ReadAllLines(filePath).ToList();

                bool removed = lines.RemoveAll(line => line.Contains(premium.Identifier.ToString())) > 0;

                if (removed)
                {
                    System.IO.File.WriteAllLines(filePath, lines);
                }
                else
                {
                    return Ok(new { Error = "Premiun doesn't exists" });
                }

                //then we add a new row with the modified data
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                StringBuilder csvLine = new StringBuilder();
                DateTimeOffset currentTime = DateTimeOffset.Now;

                csvLine.Append(premium.Identifier);
                csvLine.Append(",");
                csvLine.Append(premium.Carrier);
                csvLine.Append(",");
                csvLine.Append(string.Join("|", premium.Plan));
                csvLine.Append(",");
                csvLine.Append(premium.State.ToUpper());
                csvLine.Append(",");
                csvLine.Append(premium.MonthOfBirth);
                csvLine.Append(",");
                csvLine.Append(premium.AgeRangeStart);
                csvLine.Append(",");
                csvLine.Append(premium.AgeRangeEnd);
                csvLine.Append(",");
                csvLine.Append(premium.Premium);


                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    if (writer.BaseStream.Length == 0)
                    {
                        writer.WriteLine("Identifier,Carrier,Plan,State,MonthOfBirth,AgeRangeStart,AgeRangeEnd,Premium");
                    }
                    writer.WriteLine(csvLine);
                }

                var response = new { message = "Premium modified sucesfully." };
                //same as the create method, a message is returned with the confirmation. This is only used for test purposes
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(ex.Message);
            }
        }
        


        [HttpGet("list")]
        public IActionResult list()
        {
            // A list with the Premiums is returned, if doesnt exists any, an empty list is returned
            string filePath = Path.Combine("Data", "Premiums.csv");
            var response = PremiumClass.GetPremiums(filePath);
            return Ok(response);
        }

        [HttpPost("calculator")]
        public IActionResult PostCalculator([FromBody] Clients client) {
        // This route is used to calculate the premium with the client data.
            try
            {
                //first we get the premiums from the csv file
                string filePath = Path.Combine("Data", "Premiums.csv");
                var premiums = PremiumClass.GetPremiums(filePath);

                //then, the DOB is calculated and validated, if its not valid a message is returned 
                var calculated_datetime = DateTime.Now;

                try
                {
                    calculated_datetime = DateTime.ParseExact(client.Dob, "yyyy-MM-dd", null);
                    if (calculated_datetime > DateTime.Now) {
                        throw new ArgumentException("Date of birth is invalid.");
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                    var Errorresponse = new { Error = "Date of birth is invalid." , Field = "dob" };
                    return Ok(Errorresponse);
                }

                //then, the age is calculated and validated, if its not valid a message is returned
                var age = PremiumClass.CalculateAge(client.Dob);

                if (age != client.Age)
                {
                    var Errorresponse = new { Error = "Age is not valid according to the date of birth.", Field = "age" };
                    return Ok(Errorresponse);
                }

                //then, the plan is validated, if its not valid a message is returned
                if ("" == client.Plan.Trim())
                {
                    var Errorresponse = new { Error = "Plan is invalid.", Field = "plans" };
                    return Ok(Errorresponse);
                }
                //then, the states are validated, if its not valid a message is returned. the USstates method is used to get the list of states
                var usStates = PremiumClass.USstates();

                if (!usStates.Any(state => {
                    var shortNameProperty = state.GetType().GetProperty("short_name");
                    if (shortNameProperty != null)
                    {
                        var shortNameValue = shortNameProperty.GetValue(state);
                        if (shortNameValue != null)
                        {
                            return shortNameValue.ToString() == client.State.ToUpper();
                        }
                    }
                    return false;
                })){
                    var Errorresponse = new { Error = "Invalid US state.", Field = "states" };
                    return Ok(Errorresponse);
                }

                var response = new List<object> { };
                //The premium data is run in a loop to find the ones with the coincidences
                foreach (var premium in premiums)
                {
                    if (premium.Plan == null || premium.AgeRangeEnd == null || premium.AgeRangeEnd == null || premium.MonthOfBirth == null || premium.State == null) {
                        continue;
                    }

                    if ((premium.Plan.Contains(client.Plan) || premium.Plan.Contains("*")) && (premium.AgeRangeStart <= age && age <= premium.AgeRangeEnd) && (calculated_datetime.Month.ToString() == premium.MonthOfBirth || premium.MonthOfBirth == "*") && (premium.State == client.State.ToUpper() || premium.State == "*"))
                    {
                        response.Add(new { Carrier = premium.Carrier, Premium = premium.Premium });
                    }
                }

                return Ok(response);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("states")]
        public IActionResult USStates() {
            // a list with the US states is returned.
            return Ok(PremiumClass.USstates());
        }
    }
}
