using Newtonsoft.Json;

namespace WebApplication1
{
    public class PremiumClass
    {
        public string Carrier { get; set; }
        public List<string> Plan { get; set; }
        public string State { get; set; }
        public string MonthOfBirth { get; set; }
        public int AgeRangeStart { get; set; }
        public int AgeRangeEnd { get; set; }
        public double Premium { get; set; }
        public long? Identifier { get; set; }
        public static int CalculateAge(string dob)
        {
            // the Age is calculated both in front and server side in order to validate it
            DateTime calculatedDateTime = DateTime.ParseExact(dob, "yyyy-MM-dd", null);
            DateTime actualDateTime = DateTime.Now;

            int age = actualDateTime.Year - calculatedDateTime.Year;
            if (actualDateTime.Month < calculatedDateTime.Month || (actualDateTime.Month == calculatedDateTime.Month && actualDateTime.Day < calculatedDateTime.Day))
            {
                age--;
            }

            return age;
        }
        public static List<object> USstates() {
            // a list with the US states is returned.
            return new List<object> {
                    new { short_name = "AL", long_name = "Alabama" },
                    new { short_name = "AK", long_name = "Alaska" },
                    new { short_name = "AZ", long_name = "Arizona" },
                    new { short_name = "AR", long_name = "Arkansas" },
                    new { short_name = "CA", long_name = "California" },
                    new { short_name = "CO", long_name = "Colorado" },
                    new { short_name = "CT", long_name = "Connecticut" },
                    new { short_name = "DE", long_name = "Delaware" },
                    new { short_name = "FL", long_name = "Florida" },
                    new { short_name = "GA", long_name = "Georgia" },
                    new { short_name = "HI", long_name = "Hawaii" },
                    new { short_name = "ID", long_name = "Idaho" },
                    new { short_name = "IL", long_name = "Illinois" },
                    new { short_name = "IN", long_name = "Indiana" },
                    new { short_name = "IA", long_name = "Iowa" },
                    new { short_name = "KS", long_name = "Kansas" },
                    new { short_name = "KY", long_name = "Kentucky" },
                    new { short_name = "LA", long_name = "Louisiana" },
                    new { short_name = "ME", long_name = "Maine" },
                    new { short_name = "MD", long_name = "Maryland" },
                    new { short_name = "MA", long_name = "Massachusetts" },
                    new { short_name = "MI", long_name = "Michigan" },
                    new { short_name = "MN", long_name = "Minnesota" },
                    new { short_name = "MS", long_name = "Mississippi" },
                    new { short_name = "MO", long_name = "Missouri" },
                    new { short_name = "MT", long_name = "Montana" },
                    new { short_name = "NE", long_name = "Nebraska" },
                    new { short_name = "NV", long_name = "Nevada" },
                    new { short_name = "NH", long_name = "New Hampshire" },
                    new { short_name = "NJ", long_name = "New Jersey" },
                    new { short_name = "NM", long_name = "New Mexico" },
                    new { short_name = "NY", long_name = "New York" },
                    new { short_name = "NC", long_name = "North Carolina" },
                    new { short_name = "ND", long_name = "North Dakota" },
                    new { short_name = "OH", long_name = "Ohio" },
                    new { short_name = "OK", long_name = "Oklahoma" },
                    new { short_name = "OR", long_name = "Oregon" },
                    new { short_name = "PA", long_name = "Pennsylvania" },
                    new { short_name = "RI", long_name = "Rhode Island" },
                    new { short_name = "SC", long_name = "South Carolina" },
                    new { short_name = "SD", long_name = "South Dakota" },
                    new { short_name = "TN", long_name = "Tennessee" },
                    new { short_name = "TX", long_name = "Texas" },
                    new { short_name = "UT", long_name = "Utah" },
                    new { short_name = "VT", long_name = "Vermont" },
                    new { short_name = "VA", long_name = "Virginia" },
                    new { short_name = "WA", long_name = "Washington" },
                    new { short_name = "WV", long_name = "West Virginia" },
                    new { short_name = "WI", long_name = "Wisconsin" },
                    new { short_name = "WY", long_name = "Wyoming" }
            };
        }

        public static List<PremiumClass> GetPremiums(string filePath)
        // A list of premiums is extracted from the csv file, in case of the file is empty or doesn't exist an empty list is returned
        {

            List<PremiumClass> premiums = new List<PremiumClass>();

            if (File.Exists(filePath))
            {
                foreach (string line in File.ReadLines(filePath).Skip(1))
                {
                    string[] fields = line.Split(',');
                    PremiumClass premium = new PremiumClass();
                    if (fields.Length >= 8)
                    {
                        premium = new PremiumClass()
                        {
                            Identifier = long.Parse(fields[0]),
                            Carrier = fields[1],
                            Plan = fields[2].Split('|').ToList(),
                            State = fields[3],
                            MonthOfBirth = fields[4],
                            AgeRangeStart = int.Parse(fields[5]),
                            AgeRangeEnd = int.Parse(fields[6]),
                            Premium = double.Parse(fields[7])
                        };

                    }
                    if (premium != null)
                    {
                        premiums.Add(premium);
                    }
                }
            }

            return premiums;
            
        }

    }
}
