using System.Collections.Generic;

namespace Cinema.DataProcessor.ExportDto
{
    public class MoviesExportDto
    {
        public string MovieName { get; set; }

        public string Rating { get; set; }

        public string TotalIncomes { get; set; }

        public ICollection<CustomersMoviesExportDto> Customers { get; set; }
    }
}