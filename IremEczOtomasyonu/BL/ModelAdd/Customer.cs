using System.ComponentModel.DataAnnotations.Schema;

namespace IremEczOtomasyonu.Models
{
    public partial class Customer
    {
        [NotMapped]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }
}
