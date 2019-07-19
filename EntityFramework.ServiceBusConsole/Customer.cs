using System.ComponentModel.DataAnnotations.Schema;
using EntityFramework.ServiceBus;

namespace EntityFramework.ServiceBusConsole
{
    [Table("Customer")]
    public class Customer : SoftDeletable
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}