﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Customer")]
    public class CustomerImportDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [XmlElement("FirstName")]
        public string FirstName { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [XmlElement("LastName")]
        public string LastName { get; set; }

        [Required]
        [Range(12, 110)]
        [XmlElement("Age")]
        public int Age { get; set; }

        [Required]
        [Range(0.01, Double.MaxValue)]
        [XmlElement("Balance")]
        public decimal Balance { get; set; }
        
        [XmlArray("Tickets")]
        public TicketCustomerImportDto[] Tickets { get; set; }
    }
}