using System.ComponentModel.DataAnnotations.Schema;
using EntityFramework.ServiceBus;

namespace EntityFramework.ServiceBusTests
{
    [Table("Customer")]
    public class Customer : SoftDeletable
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}