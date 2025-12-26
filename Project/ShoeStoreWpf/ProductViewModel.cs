using ShoeStoreLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeStoreWpf
{
    public class ProductViewModel 
    {
        public int ProductId { get; set; }
        public string Article { get; set; }
        public string CategoryName { get; set; }
        public string Unit { get; set; }
        public int Price { get; set; }
        public string SupplierName { get; set; }
        public string ManufacturerName { get; set; }
        public int ManufacturerId { get; set; }
        public byte Discount { get; set; }
        public int Quantity { get; set; }
        public string Gender { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public byte Size { get; set; }
        public string Image { get; set; }

        public string FullImagePath => GetFullImagePath();

        /// <summary>
        /// Получение полного пути к изображению товара, проверка существования файла
        /// </summary>
        private string GetFullImagePath()
        {
            if (!string.IsNullOrEmpty(Image) && !string.IsNullOrWhiteSpace(Image))
            {
                string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Images",Image);

                if (System.IO.File.Exists(fullPath))
                    return $"/Images/{Image}";
            }
            return "/Images/picture.png";
        }
        
        public string OriginalPrice { get; set; }
        public string FinalPrice { get; set; }
        public bool HasDiscount { get; set; }
        public string Background { get; set; }

    }
}
