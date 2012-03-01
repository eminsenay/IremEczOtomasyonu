using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace IremEczOtomasyonu
{
    class ProductCollection: ObservableCollection<Product>
    {
        private readonly Model1Container _context;
        public Model1Container Context
        {
            get { return _context; }
        }

        public ProductCollection(IEnumerable<Product> products, Model1Container context):base(products)
        {
            _context = context;
        }

        protected override void InsertItem(int index, Product item)
        {
            Context.AddToProducts(item);
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            Context.DeleteObject(this[index]);
            base.RemoveItem(index);
        }
    }
}
