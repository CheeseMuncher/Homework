using System.Collections.Generic;

namespace Paymentsense.Coding.Challenge.Api.Models
{
    public class RegionalBloc
    {
        public string Acronym { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> OtherNames { get; set; }
        public IEnumerable<string> OtherAcronyms { get; set; }
    }
}