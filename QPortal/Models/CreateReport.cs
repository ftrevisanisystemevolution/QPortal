using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QPortal.Models
{
    public class CreateReport
    {
        [Display(Name = "Nome report")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Descrizione")]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Scegli un template dalla Galleria")]
        public List<string> Templates { get; set; }
    }
}