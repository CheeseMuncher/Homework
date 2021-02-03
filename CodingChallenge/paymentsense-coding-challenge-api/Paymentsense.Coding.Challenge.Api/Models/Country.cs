using System.Collections.Generic;

namespace Paymentsense.Coding.Challenge.Api.Models
{
    public class Country
    {
        public int Population { get; set; }
        public double? Area { get; set; }
        public double? Gini { get; set; }
        public string Name { get; set; }
        public string Alpha2Code { get; set; }
        public string Alpha3Code { get; set; }
        public string Flag { get; set; }
        public string Cioc { get; set; }
        public string NativeName { get; set; }
        public string NumericCode { get; set; }
        public string Capital { get; set; }
        public string Region { get; set; }
        public string Subregion { get; set; }
        public string Demonym { get; set; }
        public IEnumerable<double> Latlng { get; set; }
        public IEnumerable<string> AltSpellings { get; set; }
        public IEnumerable<string> CallingCodes { get; set; }
        public IEnumerable<string> Timezones { get; set; }
        public IEnumerable<string> TopLevelDomain { get; set; }
        public IEnumerable<string> Borders { get; set; }
        public Translations Translations { get; set; }
        public IEnumerable<Currency> Currencies { get; set; }
        public IEnumerable<Language> Languages { get; set; }
        public IEnumerable<RegionalBloc> RegionalBlocs { get; set; }
    }
}