using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos;

public class MoneyDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }
}
