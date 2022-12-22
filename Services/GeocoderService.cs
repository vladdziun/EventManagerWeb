using EventManagerWeb.DTO;
using System.Net;
using System.Xml.Linq;

namespace EventManagerWeb.Services
{
    public class GeocoderService
    {
        private readonly IConfiguration _configuration;

        public GeocoderService(IConfiguration configuration) 
        {
            _configuration= configuration;
        }

        public GeocoderResponse GetLongitudeAndLatitudeFromAddress(string address)
        {
            string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false", Uri.EscapeDataString(address), _configuration["GoogleMaps:Key"]);

            WebRequest request = WebRequest.Create(requestUri);
            WebResponse response = request.GetResponse();
            XDocument xdoc = XDocument.Load(response.GetResponseStream());

            XElement result = xdoc.Element("GeocodeResponse").Element("result");
            XElement locationElement = result.Element("geometry").Element("location");
            XElement lng = locationElement.Element("lng");
            XElement lat = locationElement.Element("lat");

            return new GeocoderResponse()
            {
                Longitude = double.Parse(lng.Value, System.Globalization.CultureInfo.InvariantCulture),
                Latitude = double.Parse(lat.Value, System.Globalization.CultureInfo.InvariantCulture),
            };
        }
    }
}
