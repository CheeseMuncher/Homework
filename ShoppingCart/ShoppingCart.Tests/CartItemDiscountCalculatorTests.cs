﻿using AutoFixture;
using NUnit.Framework;
using ShoppingCart.Implementation;
using ShoppingCart.Interfaces;
using ShoppingCart.Models;
using Shouldly;
using System.Collections.Generic;

namespace ShoppingCart.Tests
{
    public class CartItemDiscountCalculatorTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly ICartItemDiscountCalculator _sut = new CartItemDiscountCalculator(new Dictionary<DiscountType, ICartItemDiscountCalculator>
        {
            [DiscountType.Category] = new CartItemCategoryDiscountCalculator(),
            [DiscountType.Shipping] = new CartItemShippingDiscountCalculator(),
            [DiscountType.Supplier] = new CartItemSupplierDiscountCalculator()
        });

        [Test]
        public void CalculateLineTotal_AppliesNoDiscount_IfShippingDiscount()
        {
            var product = _fixture.Create<Product>();
            var quantity = _fixture.Create<int>();
            var discount = new Discount { DiscountType = DiscountType.Shipping };

            var result = _sut.CalculateLineTotal(product, quantity, discount);

            result.ShouldBe(product.Price * quantity);
        }

        [TestCase("APPLE")]
        [TestCase("Apple")]
        [TestCase("apple")]
        public void CalculateLineTotal_AppliesSupplierDiscount(string supplierDiscount)
        {
            var product = _fixture.Create<Product>();
            product.Supplier = supplierDiscount.ToUpper();

            var quantity = _fixture.Create<int>();
            var discount = new Discount { DiscountType = DiscountType.Supplier, DiscountedSupplier = supplierDiscount, DiscountPercentage = 1.23m };

            var result = _sut.CalculateLineTotal(product, quantity, discount);

            result.ShouldBe(product.Price * quantity * (1 - discount.DiscountPercentage / 100));
        }

        [Test]
        public void CalculateLineTotal_DoesNotApplySupplierDiscount_WithoutMatch()
        {
            var product = _fixture.Create<Product>();
            var quantity = _fixture.Create<int>();
            var discount = new Discount { DiscountType = DiscountType.Supplier, DiscountedSupplier = _fixture.Create<string>(), DiscountPercentage = 2.34m };

            var result = _sut.CalculateLineTotal(product, quantity, discount);

            result.ShouldBe(product.Price * quantity);
        }

        [TestCase("AUDIO")]
        [TestCase("Audio")]
        [TestCase("audio")]
        public void CalculateLineTotal_AppliesCategoryDiscount(string categoryDiscount)
        {
            var product = _fixture.Create<Product>();
            product.Categories = new[] { categoryDiscount.ToUpper() };

            var quantity = _fixture.Create<int>();
            var discount = new Discount { DiscountType = DiscountType.Category, DiscountedCategory = categoryDiscount, DiscountPercentage = 3.45m };

            var result = _sut.CalculateLineTotal(product, quantity, discount);

            result.ShouldBe(product.Price * quantity * (1 - discount.DiscountPercentage / 100));
        }

        [Test]
        public void CalculateLineTotal_DoesNotApplyCategoryDiscount_WithoutMatch()
        {
            var product = _fixture.Create<Product>();
            var quantity = _fixture.Create<int>();
            var discount = new Discount { DiscountType = DiscountType.Category, DiscountedCategory = _fixture.Create<string>(), DiscountPercentage = 4.56m };

            var result = _sut.CalculateLineTotal(product, quantity, discount);

            result.ShouldBe(product.Price * quantity);
        }

        [Test]
        public void CalculateLineTotal_HandlesNullDiscount()
        {
            var product = _fixture.Create<Product>();
            var quantity = _fixture.Create<int>();

            var result = _sut.CalculateLineTotal(product, quantity, null);

            result.ShouldBe(product.Price * quantity);
        }
    }
}