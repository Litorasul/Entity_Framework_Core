using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Ticket")]
    public class TicketCustomerImportDto
    {

        [Required]
        [XmlElement("ProjectionId")]
        public int ProjectionId { get; set; }


        [Required]
        [Range(0.01, Double.MaxValue)]
        [XmlElement("Price")]
        public decimal Price { get; set; }

    }
}