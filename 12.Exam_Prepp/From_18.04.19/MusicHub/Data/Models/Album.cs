using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MusicHub.Data.Models
{
    public class Album
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Name { get; set; }
        
        [Required]
        public DateTime ReleaseDate { get; set; }
        
        [NotMapped]
        public decimal Price => this.Songs.Sum(p => p.Price);
        
        [ForeignKey(nameof(Producer))]
        public int? ProducerId { get; set; }
        public Producer Producer { get; set; }

        public ICollection<Song> Songs { get; set; } = new HashSet<Song>();

    }
}