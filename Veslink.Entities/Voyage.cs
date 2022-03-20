using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veslink.Entities
{
    public class Voyage
    {
        public int VoyageNo { get; set; }
        public DateTime? CommenceDateGmt { get; set; }
        public List<Charterer> Charterers { get; set; }
        public Charterer ChartererSelected { get; set; }
        public LinkedListNode<VoyageItinerary> VoyageItineraries { get; set; }
    }
}
