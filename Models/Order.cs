using System.ComponentModel.DataAnnotations;

namespace FlyBuy.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Please enter your name!")]
        [StringLength(20, MinimumLength = 4)]
        public string CustomerName { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Please enter your phone number!")]
        [DataType(DataType.PhoneNumber)]

        public string CustomerPhone { get; set; }

        [Display(Name = "Email")]
        //[EmailAddress]
        [Required(ErrorMessage = "Please enter your email!")]
        [DataType(DataType.EmailAddress)]

        public string CustomerEmail { get; set; }

        [Display(Name = "Adress")]
        [Required(ErrorMessage = "Please enter your address!")]
        [StringLength(20, MinimumLength = 4)]

        public string CustomerAddress { get; set; }

        public string CreatedDate { get; set; } = DateTime.Now.Month.ToString();
        public IList<OrderItem> Details { get; set; }
    }
}