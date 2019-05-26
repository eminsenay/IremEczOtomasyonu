İrem Eczanesi Dermokozmetik Sistemi
=========================

A small stock tracking and customer relationship management application.

It is made around 2012 for my sister's pharmacy. At that time I just granted the application to be used by her pharmacy. 
For a brief period of time she used it, but at the time she wasn't at the pharmacy, the application wasn't being used. This made the stock tracking part useless.

Currently the application isn't being used by anyone (as far as I know). It is serving as a playground project for me to try out new things.

Roadmap (as of 04.2019)
-----------------------

1. Data layer updates (in work)
	* Entity Framework 4.0 to Entity Framework Core
	* SQL Server Compact 4.0 to SQLite
2. .NET Framework to .NET Core 3.0 (in work)
3. Refactoring the UI classes to be more MVVM like (maybe sometime in the future)
4. Unit testing (maybe sometime in the future)
5. UI port to Electron (maybe sometime in the future)


Known Problems
--------------

### General

- Artifacts of Debug and release builds are generated under different folders.
- 32 bit generation folder name is not the same among projects (Win32 - x86).
- Connection string is located at the source code.
- DB doesn't have a password.

### AboutBox

Nothing.

### AddNewProductWindow

Nothing.

### AddPurchaseWindow

Nothing.

### AutoCompleteFocusableBox

Nothing.

### BarcodeWindow

Nothing. 

### CustomerListWindow

"Satış Ekranı" search window uses this window. Selected customer should be also selected at the "satış ekranı" but it is ignored. See the comment at BL\ModelAdd\ProductSale.cs for more information about this problem.

### IncomingExpirationsWindow

Nothing.

### MainWindow
### ProductDetailsWindow

* Expiration dates of the product is not displayed when the product is added for the first time.
* Other parts are not tested properly

### ProductSaleCountWindow
### PurchaseListWindow
### SaleListWindow
### SaleWindow

Not working - No expiration date can be found for the product (whenever the product is tried to be added).

### UIUtilities
### UserControlCustomers

* Müşterinin aldığı ürünler part cannot be tested right now
* Other parts seem to be working

### UserControlProducts
### UserControlSales
### ValueConverters
### WebcamImagePreviewWindow
### WebcamWindow