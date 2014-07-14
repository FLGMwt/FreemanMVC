using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"}
            });
            var adminController = new AdminController(mock.Object);

            var result = ((IEnumerable<Product>)adminController.Index().ViewData.Model).ToList();

            Assert.AreEqual(result.Count, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }

        [TestMethod]
        public void Can_Edit_Product()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"}
            });
            var adminController = new AdminController(mock.Object);

            var p1 = adminController.Edit(1).Model as Product;
            var p2 = adminController.Edit(2).Model as Product;
            var p3 = adminController.Edit(3).Model as Product;

            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Product()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"}
            });
            var adminController = new AdminController(mock.Object);

            var result = adminController.Edit(4).Model as Product;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            var mock = new Mock<IProductRepository>();
            var adminController = new AdminController(mock.Object);
            var product = new Product { Name = "Test" };

            var result = adminController.Edit(product);

            mock.Verify(m => m.SaveProduct(product));
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            var mock = new Mock<IProductRepository>();
            var adminController = new AdminController(mock.Object);
            var product = new Product { Name = "Test" };
            adminController.ModelState.AddModelError("error", "error");

            var result = adminController.Edit(product);

            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Products()
        {
            var prod = new Product { ProductID = 2, Name = "Test" };
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[] {
                new Product { ProductID = 1, Name = "P1" },
                prod,
                new Product { ProductID = 3, Name = "P3" }
            });
            var adminController = new AdminController(mock.Object);

            adminController.Delete(prod.ProductID);

            mock.Verify(m => m.DeleteProduct(prod.ProductID));
        }

    }


}
