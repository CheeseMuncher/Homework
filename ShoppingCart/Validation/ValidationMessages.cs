namespace ShoppingCart.Validation
{
    public static class ValidationMessages
    {
        public const string ExactlyOneCode = "API takes exactly one Coupon Code";
        public const string ExactlyOneCodeAlphanumeric = "API takes exactly one alphanumeric Coupon Code";
        public const string PositiveQuantities = "API takes positive quantities";
        public const string ProductNotFound = "Product not found";
    }
}