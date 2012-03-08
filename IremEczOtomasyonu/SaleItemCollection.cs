using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace IremEczOtomasyonu
{
    class SaleItemCollection : ObservableCollection<SaleItem>
    {
        private readonly Model1Container _context;
        public Model1Container Context
        {
            get { return _context; }
        }

        public SaleItemCollection(IEnumerable<SaleItem> saleItems, Model1Container context)
            : base(saleItems)
        {
            _context = context;
        }

        protected override void InsertItem(int index, SaleItem item)
        {
            //Context.AddToProducts(item);
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            Context.DeleteObject(this[index]);
            base.RemoveItem(index);
        }

    }
}
