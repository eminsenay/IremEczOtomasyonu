İrem Eczanesi Otomasyon Sistemi
=========================

A small stock tracking and customer relationship management application.

It is made around 2012 for my sister's pharmacy. At that time I just granted the application to be used by her pharmacy. 
For a brief period of time she used it, but at the time she wasn't at the pharmacy, the application wasn't being used. This made the stock tracking part useless.

Currently the application isn't being used by anyone (as far as I know). It is serving as a playground project for me to try out new things.

Roadmap (as of 10.2019)
-----------------------

1. ClickOnce (or something similar) Setup &rarr; Note: OpenCVSharp has a bug preventing this to be functioning correctly.
2. DB pass issue needs to be taken care of.
3. Refactoring the UI classes to be more MVVM like (maybe sometime in the future)
4. Unit testing (maybe sometime in the future)
5. UI port to Electron (maybe sometime in the future)


Known Problems
--------------

- Connection string is located at the source code.
- DB doesn't have a password.
- Automatic refresh problems:
  * If the user is at the products view, after purchasing a product, stock numbers are not automatically being updated.
  * If the user is at the customers view, when the user opens Extra - Previous Sales Window and changes association of 
  sales to customers, these changes are not being reflected automatically to the "Bought Products" grid.