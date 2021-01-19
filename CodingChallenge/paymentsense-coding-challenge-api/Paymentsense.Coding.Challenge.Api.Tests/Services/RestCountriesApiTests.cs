using AutoFixture;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Paymentsense.Coding.Challenge.Api.Models;
using Paymentsense.Coding.Challenge.Api.Services;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Paymentsense.Coding.Challenge.Api.Tests.Services
{
    public class RestCountriesApiTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<HttpMessageHandler> _mockMessageHandler = new Mock<HttpMessageHandler>();
        private readonly HttpClient _httpClient;
        private readonly IRestCountriesApi _sut;

        public RestCountriesApiTests()
        {
            _httpClient = new HttpClient(_mockMessageHandler.Object);
            _sut = new RestCountriesApi(_httpClient);
        }

        [Fact]
        public async Task GetAllCountriesAsync_InvokesRestCountriesApi()
        {
            // Arrange
            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("[]") };
            HttpRequestMessage payload = null;
            _mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Callback((HttpRequestMessage message, CancellationToken token) => payload = message)
                .ReturnsAsync(response);

            // Act
            await _sut.GetAllCountriesAsync();

            // Assert
            payload.Should().NotBeNull();
            payload.Method.Should().Be(HttpMethod.Get);
            payload.RequestUri.Should().Be("https://restcountries.eu/rest/v2/all");
        }

        [Fact]
        public async Task GetAllCountriesAsync_MapsResponseData_IfGetAsyncCallSuccessful()
        {
            // Arrange
            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent($"[{ResponseExampleJson},{ResponseExampleJson}]") };

            _mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            var result = (await _sut.GetAllCountriesAsync()).ToList();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
            foreach (var country in result)
            {
                country.Population.Should().Be(48759958);
                country.Area.Should().Be(1141748.0);
                country.Gini.Should().Be(55.9);
                country.Name.Should().Be("Colombia");
                country.Alpha2Code.Should().Be("CO");
                country.Alpha3Code.Should().Be("COL");

                country.Latlng.Should().BeEquivalentTo(new[] { 4.0, -72.0 });
                country.AltSpellings.Should().BeEquivalentTo(new[] { "CO", "Republic of Colombia", "República de Colombia" });
                country.Translations.Should().BeEquivalentTo(new Translations { De = "Kolumbien", Es = "Colombia", Fr = "Colombie", Ja = "コロンビア", It = "Colombia", Br = "Colômbia", Pt = "Colômbia" });
                country.Currencies.Should().BeEquivalentTo(new[] { new Currency { Code = "COP", Name = "Colombian peso", Symbol = "$" } });
                country.Languages.Should().BeEquivalentTo(new[] { new Language { Name = "Spanish", NativeName = "Español", Iso639_1 = "es", Iso639_2 = "spa" } });
                country.RegionalBlocs.Should().BeEquivalentTo(new[]
                {
                    new RegionalBloc { Acronym = "PA", Name = "Pacific Alliance", OtherAcronyms = new string[0], OtherNames = new [] { "Alianza del Pacífico" } },
                    new RegionalBloc { Acronym = "USAN", Name = "Union of South American Nations", OtherAcronyms = new [] { "UNASUR", "UNASUL", "UZAN" },
                        OtherNames = new [] { "Unión de Naciones Suramericanas", "União de Nações Sul-Americanas", "Unie van Zuid-Amerikaanse Naties", "South American Union" } }
                });
            }
        }

        [Fact]
        public async Task GetFlagAsync_InvokesRestCountriesApi_WithCorrectArgs()
        {
            // Arrange
            var code = _fixture.Create<string>();
            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("[]") };
            HttpRequestMessage payload = null;
            _mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Callback((HttpRequestMessage message, CancellationToken token) => payload = message)
                .ReturnsAsync(response);

            // Act
            await _sut.GetFlagAsync(code);

            // Assert
            payload.Should().NotBeNull();
            payload.Method.Should().Be(HttpMethod.Get);
            payload.RequestUri.Should().Be($"https://restcountries.eu/data/{code}.svg");
        }

        [Fact]
        public async Task GetFlagAsync_MapsResponseData_IfGetAsyncCallSuccessful()
        {
            // Arrange
            var data = _fixture.Create<string>();
            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(data) };

            _mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            var result = await _sut.GetFlagAsync(_fixture.Create<string>());

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(Encoding.ASCII.GetBytes(data));
        }

        private string ResponseExampleJson => @"{
            'name': 'Colombia',
            'topLevelDomain': ['.co'],
            'alpha2Code': 'CO',
            'alpha3Code': 'COL',
            'callingCodes': ['57'],
            'capital': 'Bogotá',
            'altSpellings': ['CO', 'Republic of Colombia', 'República de Colombia'],
            'region': 'Americas',
            'subregion': 'South America',
            'population': 48759958,
            'latlng': [4.0, -72.0],
            'demonym': 'Colombian',
            'area': 1141748.0,
            'gini': 55.9,
            'timezones': ['UTC-05:00'],
            'borders': ['BRA', 'ECU', 'PAN', 'PER', 'VEN'],
            'nativeName': 'Colombia',
            'numericCode': '170',
            'currencies': [{
                'code': 'COP',
                'name': 'Colombian peso',
                'symbol': '$'
            }],
            'languages': [{
                'iso639_1': 'es',
                'iso639_2': 'spa',
                'name': 'Spanish',
                'nativeName': 'Español'
            }],
            'translations': {
                'de': 'Kolumbien',
                'es': 'Colombia',
                'fr': 'Colombie',
                'ja': 'コロンビア',
                'it': 'Colombia',
                'br': 'Colômbia',
                'pt': 'Colômbia'
            },
            'flag': 'https://restcountries.eu/data/col.svg',
            'regionalBlocs': [{
                'acronym': 'PA',
                'name': 'Pacific Alliance',
                'otherAcronyms': [],
                'otherNames': ['Alianza del Pacífico']
            }, {
            'acronym': 'USAN',
                'name': 'Union of South American Nations',
                'otherAcronyms': ['UNASUR', 'UNASUL', 'UZAN'],
                'otherNames': ['Unión de Naciones Suramericanas', 'União de Nações Sul-Americanas', 'Unie van Zuid-Amerikaanse Naties', 'South American Union']
            }],
            'cioc': 'COL'
        }";
    }
}