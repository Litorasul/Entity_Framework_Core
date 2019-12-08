using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ExportDto
{
    [XmlType("Project")]
    public class ExportProjectsDto
    {
        [XmlElement("ProjectName")]
        public string ProjectName { get; set; }
        
        [XmlElement("HasEndDate")]
        public string HasEndDate { get; set; }
        [XmlArray("Tasks")]
        public ExportTasksDto[] Tasks { get; set; }
        
        [XmlAttribute("TasksCount")]
        public int TasksCount { get; set; }
    }
}