using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class Metadata
    {
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Guid UniqueKey { get; set; }

        public Metadata()
        {
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
            UniqueKey = Guid.NewGuid();
        }
    }
}