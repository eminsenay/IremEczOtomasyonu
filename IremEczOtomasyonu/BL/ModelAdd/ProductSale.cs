namespace IremEczOtomasyonu.Models
{
    partial class ProductSale
    {
        // The following code had to be commented out because of the EFCore transition. 
        // So called "lifecycle hooks" are not available at the EF Core. 
        // See https://github.com/aspnet/EntityFrameworkCore/issues/626 for the latest status.
        // Without the following code, customer textbox cannot be updated at the product sale 
        // (Until finding another solution to the problem).
        /*
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
        */
    }
}
