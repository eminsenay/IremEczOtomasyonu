namespace IremEczOtomasyonu.BL
{
    partial class SaleItem
    {
        public int PrevNumSold { get; set; }
// ReSharper disable UnusedParameter.Local
        partial void OnNumSoldChanging(int value)
// ReSharper restore UnusedParameter.Local
        {
            PrevNumSold = NumSold;
        }
    }
}
