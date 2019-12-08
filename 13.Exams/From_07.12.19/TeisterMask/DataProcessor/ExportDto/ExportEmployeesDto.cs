namespace TeisterMask.DataProcessor.ExportDto
{
    public class ExportEmployeesDto
    {
        public string Username { get; set; }
        public ExportTaskEmployeesDto[] Tasks { get; set; }
    }
}