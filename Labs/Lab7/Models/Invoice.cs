using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lab7.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int CustomerId { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "Data zamówienia")]
    public DateTime InvoiceDate { get; set; }

    [Display(Name = "Adres rozliczeniowy")]
    public string? BillingAddress { get; set; }

    [Display(Name = "Miasto")]
    public string? BillingCity { get; set; }

    [Display(Name = "Województwo")]
    public string? BillingState { get; set; }

    [Display(Name = "Kraj")]
    public string? BillingCountry { get; set; }

    [Display(Name = "Kod pocztowy")]
    public string? BillingPostalCode { get; set; }

    [DataType(DataType.Currency)]
    [Display(Name = "Kwota łączna")]
    public decimal Total { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
}
