We have a Shopping Cart project that calculates the price of the items in the Cart and adds any shipping costs.

Your task is to change the API to make it take exactly one Coupon Code that represents a Discount that can be applied on the Cart. [Done](551c701a272d82ed70bf192302dbc1d1354e1d9c)
You then need to change the implementation to apply the Discount associated with that Coupon Code.

The current implementation can contain bugs or questionable error handling, feel free to change the code.

There are three types of Discounts that you need to model for:
- A Supplier-based discount: X% off each item in the Cart for a specified Supplier. For example, for supplier, Apple, coupon code, BACKTOSCHOOL2019, gives you 15% off all Apple products)
- A free shipping-based discount: Free shipping regardless of total cost
- A Category-based discount : Y% off each item that is part of a specified Product Category, and Products can have more than 1 Product Category. For example, for category, Audio, coupon code, LOVEMYMUSIC, gives you 20% off of all products with category "Audio"

There are three types of Product Categories that you need to implement:
- Electronic
- Accessory
- Audio

There are three types of Suppliers:
- HP
- Dell
- Apple

Currently, the price of the Cart is calculated as follows:
- For each Product in the Cart, find its price and multiply by the number of units required
- Once all summed up, add any shipping costs as follows:
    - running total is less than 20, shipping cost is 7
    - running total is less than 40, shipping cost is 5
    - otherwise shipping cost is free
- Return the running total added to the shipping cost


YOU NEED TO IMPLEMENT THE FOLLOWING:

Given a Cart that contains
- 2 * headphones from Apple, at 10 per unit
- 1 * USB cable from Apple, at 4 per unit
- 1 * monitor from HP, at 100 per unit
- 1 * laptop from Dell, at 1000 per unit

Given these Products and associated Product Categories:
- Headphones: Accessory, Electronic, Audio
- USB cable: Accessory
- Monitor: Electronic
- Laptop: Electronic

Write passing unit tests for:

1) applying the "AUDIO10" Coupon Code to the above cart
2) applying the "APPLE5" Coupon Code to the above cart
3) applying the "FREESHIPPING" Coupon Code to the above cart
4) applying an invalid or unknown Coupon Code to the above cart

We would like to see:
- NUnit is used currently, but you are free to use an alternative.
- Add or change any of the classes/code-base as you find appropriate
- Make your solution extensible and follow SOLID principles

When we evaluate the solution we are looking for the following points (as per regular PR):

- Good understanding of SOLID Principles
- Good understanding of DI
- Unit Testing
- Good data structure choices
- Appropriate refactoring of base implementation
- Easy to follow and readable solution


Please comment (in the code, in an e-mail, or verbally) what you think of the design and the code; are there any bugs, potential bugs, or misleading elements?


BONUS QUESTION:  what would you do with this solution to handle the use case of a user being refunded the correct amount for a returned item?


Swagger endpoints:
[Swagger UI](http://localhost:5000/swagger/index.html)
[Open Api 3.0.1 Json](http://localhost:5000/swagger/v1/swagger.json)

Log File Path:
bin\Debug\netcoreapp3.1\logs
