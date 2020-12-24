We have a Shopping Cart project that calculates the price of the items in the Cart and adds any shipping costs.

Your task is to change the API to make it take exactly one Coupon Code that represents a Discount that can be applied on the Cart. [Done here](https://github.com/CheeseMuncher/ShoppingCart/commit/551c701a272d82ed70bf192302dbc1d1354e1d9c)
You then need to change the implementation to apply the Discount associated with that Coupon Code. Done [here](https://github.com/CheeseMuncher/ShoppingCart/commit/b4cac9ef6b179dcb13fcc2e6fd8daa3fab74bbfa), [here](https://github.com/CheeseMuncher/ShoppingCart/commit/1e6e91a0affcc9d8ca90f1249e238cc3343cf3d0) and [here](https://github.com/CheeseMuncher/ShoppingCart/commit/ac5c8860d126d2e46d45bde0fdfc41f06b4387a8)

The current implementation can contain bugs or questionable error handling, feel free to change the code. [For Example](https://github.com/CheeseMuncher/ShoppingCart/commit/c3470e8bdb7bf03fecccbeafe154d5f733377f2f) or [this](https://github.com/CheeseMuncher/ShoppingCart/commit/c461e582a063f04d8454a8c0181476bb7d04bbd4)

There are three types of Discounts that you need to model for: [added here](https://github.com/CheeseMuncher/ShoppingCart/commit/906578aa31d5d5d5fcd178972c1666027d07355e)
- A Supplier-based discount: X% off each item in the Cart for a specified Supplier. For example, for supplier, Apple, coupon code, BACKTOSCHOOL2019, gives you 15% off all Apple products) [done here](https://github.com/CheeseMuncher/ShoppingCart/commit/b4cac9ef6b179dcb13fcc2e6fd8daa3fab74bbfa)
- A free shipping-based discount: Free shipping regardless of total cost [implemented here](https://github.com/CheeseMuncher/ShoppingCart/commit/ac5c8860d126d2e46d45bde0fdfc41f06b4387a8)
- A Category-based discount : Y% off each item that is part of a specified Product Category, and Products can have more than 1 Product Category. For example, for category, Audio, coupon code, LOVEMYMUSIC, gives you 20% off of all products with category "Audio" [also here](https://github.com/CheeseMuncher/ShoppingCart/commit/b4cac9ef6b179dcb13fcc2e6fd8daa3fab74bbfa)

There are three types of Product Categories that you need to implement: [added here](https://github.com/CheeseMuncher/ShoppingCart/commit/906578aa31d5d5d5fcd178972c1666027d07355e)
- Electronic
- Accessory
- Audio

There are three types of Suppliers: [as well](https://github.com/CheeseMuncher/ShoppingCart/commit/906578aa31d5d5d5fcd178972c1666027d07355e)
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

Given a Cart that contains [this commit](https://github.com/CheeseMuncher/ShoppingCart/commit/ac5c8860d126d2e46d45bde0fdfc41f06b4387a8)
- 2 * headphones from Apple, at 10 per unit
- 1 * USB cable from Apple, at 4 per unit
- 1 * monitor from HP, at 100 per unit
- 1 * laptop from Dell, at 1000 per unit

Given these Products and associated Product Categories: [added here](https://github.com/CheeseMuncher/ShoppingCart/commit/db5b029574ce9c77c9cdf69a4d8a055f9fa01d1d)
- Headphones: Accessory, Electronic, Audio
- USB cable: Accessory
- Monitor: Electronic
- Laptop: Electronic

Write passing unit tests for: [also this commit](https://github.com/CheeseMuncher/ShoppingCart/commit/ac5c8860d126d2e46d45bde0fdfc41f06b4387a8)

1) applying the "AUDIO10" Coupon Code to the above cart
2) applying the "APPLE5" Coupon Code to the above cart
3) applying the "FREESHIPPING" Coupon Code to the above cart
4) applying an invalid or unknown Coupon Code to the above cart (see below)

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

Instructions to run the above test cases:
- Checkout the develop branch
- Pull latest changes, build and run - the application should default to run on http://localhost:5000
- Use the TestCases request in the Postman collection (you'll need to change the coupon code for each test)

Assumptions, Comments & Notes:
- I have assumed that zero quantity is an acceptable input, it doesn't prevent correct calculation
- I have assumed that an unknown product ID is not acceptable. Although the original code ignored such cart items, this seems like a valid assumption to make.
- Repositories return nulls if the item isn't found. Most of them are only called once so null checks are limited for now but may have to revisit 
- The new IRepository interface does not cover cases where ID is unknown at the point of insertion, an additional method should be added
- Regarding test case 4 above: applying an invalid or unknown Coupon Code to the above cart. As per SRP, the ShoppingCartCalculator is only responsible for calculating cart totals. Invalid or unknown coupon codes are handled by the appropriate validator. The Calculator handles this as best as it can. It will still calculate cart totals correctly (including if the requirement for having a coupon code was made optional) and there is a unit test for this. Making an api request with an invalid code will result in a validation error.

Further work & Bonus:
- Could do with validating duplicate product ids in the same request object
- In memory data collections are fine for this exercise, but realistically all the repositories in the Data namespace need to be backed with proper persistance and CRUD support needs to be added. As indicated in in-line comments, if the Repository interface becomes async, the validation strategy may have to be re-visited
- If the repositories are injected with a connection string of some sort, they can stay as singletons, but in any situation where the data may become stale they should be registered as scoped.
- Refunds. There are various considerations here, for example applying the discount, quantity checks and shipping. The short answer is that the logic in place will work with the same inputs, with one exception: consumers don't expect to pay for shipping for refunds. Therefore a slight modification to the logic is required. We can either have a RefundCartCalculator that never adds shipping. Or we could modify the Calculator to accept a collection of discounts and make sure that FREESHIPPING is added for refunds.
    - Quantities. Ideally in the case of a refund, the customer should be seeking a refund for a quantity less than or equal the original purchase. So a record of the original cart would be needed for this.
    - Applying the discount. As above, a record is required to understand what discount was applied at the time of purchase to ensure the refund is correct. Discounts tend to be transient in nature, the discount may no longer be in the repository when the refund is requested. One approach would be to add valid from and valid to dates. That way discounts can be added and withdrawn in an orderly fashion and they would still be available to calculate refunds.

