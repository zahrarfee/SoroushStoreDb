using System;
using System.Collections.Generic;
using System.Text;

namespace ShopManagement.Application.Contracts.Product
{
    public class EditProduct:CreateProduct
    {
        public long Id { get; set; }
    }
}
