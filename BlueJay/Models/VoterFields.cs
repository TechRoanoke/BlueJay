using Newtonsoft.Json;
using System;
namespace BlueJay
{
    public class VoterFields
    {
        public string rnc_regid { get; set; }

        [JsonConverter(typeof(TrimmingConverter))]
        public string statevoterid { get; set; }

        public string firstname { get; set; }
        public string lastname { get; set; }
        public string middlename { get; set; }
        public string namesuffix { get; set; }

        public Gender sex { get; set; }

        public string birthyear { get; set; }
        public string birthmonth { get; set; }
        public string birthday { get; set; }

        [JsonIgnore]
        public DateTime? Birthdate
        {
            get
            {
                int year, month, day;
                if (int.TryParse(this.birthyear, out year) &&
                    int.TryParse(this.birthmonth, out month) &&
                    int.TryParse(this.birthday, out day))
                {
                    return new DateTime(year, month, day);
                }
                return null;
            }
        }

        public string state { get; set; }

        public string reg_state { get; set; }
        public string reg_city { get; set; }

        public string reg_housenum { get; set; }
        public string reg_house_sfx { get; set; }

        public string reg_st_prefix { get; set; }
        public string reg_st_name { get; set; }
        public string reg_st_post { get; set; }
        public string reg_st_type { get; set; }
        public string reg_unit_type { get; set; }
        public string reg_unit_number { get; set; }

        public string reg_zip5 { get; set; }

        public override string ToString()
        {
            return string.Format("{0}) {1} {2}", statevoterid, firstname, lastname);
        }
    }

    public static class VoterFieldsExtensions
    {

        // Helper to create a voter contact record for a given voter record. 
        public static VoterContactRequest NewContact(this VoterFields fields)
        {
            return new VoterContactRequest
            {
                state = fields.state,
                targetvoterkey = fields.rnc_regid,
                targetstatevoterid = fields.statevoterid
            };
        }
    }

}