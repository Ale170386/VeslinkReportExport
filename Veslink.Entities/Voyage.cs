using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veslink.Entities
{
    public class Voyage
    {
        /// <summary>
        /// Viaje
        /// </summary>
        public int VoyageNo { get; set; }
        /// <summary>
        /// Fecha de inicio del viaje
        /// </summary>
        public DateTime? CommenceDateGmt { get; set; }
        /// <summary>
        /// Cargos del viaje
        /// </summary>
        public List<VoyageCargo> VoyageCargos { get; set; }
        /// <summary>
        /// Itinerario del viaje
        /// </summary>
        public List<VoyageItinerary> Itinerary { get; set; }
        /// <summary>
        /// Itinerario del viaje
        /// </summary>
        public List<VoyageItinerary> DisplayItinerary { get; set; }
        /// <summary>
        /// Charterers del Viaje
        /// </summary>
        public List<Charterer> Charterers { get; set; }
        /// <summary>
        /// Charterer Seleccionado
        /// </summary>
        public Charterer ChartererSelected { get; set; }
        /// <summary>
        /// Información de contacto de la Nave Viaje
        /// </summary>
        public ContactInformation ContactInformation { get; set; }

    }
}
