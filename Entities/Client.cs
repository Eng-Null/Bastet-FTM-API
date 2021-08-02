using System;
using System.Collections.Generic;

namespace BastetAPI.Entities
{
    public class ScuffedClient
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Suffix { get; set; }
    }
    public class Client
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Suffix { get; set; }
        public List<AddressInfo> Addresses { get; set; } = new();
        public List<PhoneInfo> PhoneNumbers { get; set; } = new();
        public List<EmailInfo> Emails { get; set; } = new();
        public List<TicketInfo> Tickets { get; set; } = new();
        public List<NoteInfo> Notes { get; set; } = new();
    }

    public class TicketInfo
    {
        public Guid Id { get; set; }
        public string Destination { get; set; }
        public DateTimeOffset IssuenceDate { get; set; }
        public string TicketNumber { get; set; }
        public string PNR { get; set; }
        public string TicketTotal { get; set; }
        public string Airline { get; set; }
        public string TicketSoldFor { get; set; }
        public string TicketFare { get; set; }
        public string ToBePaid { get; set; }
        public string ToBePaidFor { get; set; }
        public string Comission { get; set; }
        public List<NoteInfo> Notes { get; set; }
    }

    public class AddressInfo
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }

    public class PhoneInfo
    {
        public Guid Id { get; set; }
        public string Phone { get; set; }
    }

    public class EmailInfo
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
    }

    public class NoteInfo
    {
        public Guid Id { get; set; }
        public string Note { get; set; }
    }
}
