using System;
using ArgumentsLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArgumentsTest {

    [TestClass]
    public class ConverterTest {

        [TestMethod]
        public void Constructor_Ok() {
            var converter = new Converter();
        }

        [TestMethod]
        public void RegisterTypeConverter_ValidConverter() {
            var converter = new Converter();
            converter.RegisterTypeConverter<int>(int.Parse);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void RegisterTypeConverter_Null() {
            var converter = new Converter();
            converter.RegisterTypeConverter<string>(null);
        }

        [TestMethod]
        public void Convert_Int_ValidInt() {
            var converter = new Converter();
            converter.RegisterTypeConverter<int>(int.Parse);
            var number = converter.Convert<int>("12345");
            Assert.AreEqual(number, 12345);
        }

        [TestMethod]
        [ExpectedException(typeof (FormatException))]
        public void Convert_Int_InvalidInt() {
            var converter = new Converter();
            converter.RegisterTypeConverter<int>(int.Parse);
            var number = converter.Convert<int>("123ABC");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void Convert_Int_Null() {
            var converter = new Converter();
            converter.RegisterTypeConverter<int>(int.Parse);
            var number = converter.Convert<int>(null);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void Convert_UndefinedTypeConverter() {
            var converter = new Converter();
            var number = converter.Convert<int>("12345");
        }

    }

}