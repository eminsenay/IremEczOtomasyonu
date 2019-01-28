namespace IremEczOtomasyonu.BL
{
    partial class ProductSale
    {
        public ProductSale()
        {
            // Workaround for EntityFramework problem. Associaton changes are not being notified by default.
            // For more details, see:
            // http://connect.microsoft.com/VisualStudio/feedback/details/532257/entity-framework-navigation-properties-don-t-raise-the-propertychanged-event#details
            CustomerReference.AssociationChanged += CustomerReference_AssociationChanged;
        }

        void CustomerReference_AssociationChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e)
        {
            OnPropertyChanged("Customer");
        }
    }
}
