﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IremEczOtomasyonu.BL
{
    partial class Product: IDataErrorInfo
    {
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    // Empty checks of the non-empty string fields are done here because validationrule classes are 
                    // first fired when the user enters a value but these are fired directly after initialization.
                    case "Barcode":
                        if (string.IsNullOrEmpty(Barcode) || Barcode.Trim() == string.Empty)
                        {
                            return "Lütfen bir değer giriniz.";
                        }
                        break;
                    case "Name":
                        if (string.IsNullOrEmpty(Name) || Name.Trim() == string.Empty)
                        {
                            return "Lütfen bir isim giriniz.";
                        }
                        break;
                    case "Brand":
                        if (string.IsNullOrEmpty(Brand) || Brand.Trim() == string.Empty)
                        {
                            return "Lütfen bir marka ismi giriniz.";
                        }
                        break;
                    case "CurrentBuyingPrice":
                        // don't need to check it since it is checked by moneyvalidationrule
                        break;
                    case "CurrentSellingPrice":
                        // don't need to check it since it is checked by moneyvalidationrule
                        break;
                }
                return null;
            }
        }

        public string Error
        {
            get { return null; }
        }

        public string Validate()
        {
            // check sum of the items which have an expiration date and compare it with the item count
            return NumItems == ExpirationDates.Sum(x => x.NumItems) ? null : 
                "Son kullanma tarihi listesindeki ürünlerin sayısı stoktaki toplam ürün sayısından farklı.\nLütfen düzeltip tekrar deneyin.";
        }

        public List<Deal> GetAllDeals()
        {
            List<Deal> deals = new List<Deal>();
            foreach (ProductPurchase purchase in ProductPurchases)
            {
                Deal deal = new Deal
                            {
                                Details = purchase.Remarks,
                                NumItems = purchase.NumItems,
                                TotalPrice = purchase.Price,
                                TransactionDate = purchase.PurchaseDate,
                                TransactionType = DealType.Purchase
                            };
                deals.Add(deal);
            }

            foreach (SaleItem saleItem in SaleItems)
            {
                Deal deal = new Deal
                            {
                                Details = saleItem.ProductSale.Remarks,
                                NumItems = saleItem.NumSold,
                                TotalPrice = saleItem.NumSold * saleItem.UnitPrice,
                                TransactionDate = saleItem.ProductSale.SaleDate,
                                TransactionType = DealType.Sale,
                                Buyer = saleItem.ProductSale.Customer
                            };
                deals.Add(deal);
            }

            deals.Sort((deal1, deal2) => deal1.TransactionDate.CompareTo(deal2.TransactionDate));

            return deals;
        }
    }
}