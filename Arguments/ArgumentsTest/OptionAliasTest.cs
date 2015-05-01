using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using ArgumentsLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArgumentsTest {

    [TestClass]
    public class OptionAliasTest {

        [TestMethod]
        public void Constructor_Ok() {
            var alias = new OptionAlias("a", OptionType.Short);
            Assert.AreEqual(alias.Alias, "a");
            Assert.AreEqual(alias.Type, OptionType.Short);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void Constructor_EmptyAlias() {
            var alias = new OptionAlias("", OptionType.Short);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void Constructor_NullAlias() {
            var alias = new OptionAlias(null, OptionType.Short);
        }

        [TestMethod]
        public void GetHashCode_ValidComparison() {
            var alias1 = new OptionAlias("a", OptionType.Short);
            var alias2 = new OptionAlias("a", OptionType.Short);
            Assert.AreEqual(alias1.GetHashCode(), alias2.GetHashCode());
        }

        [TestMethod]
        public void Equals_EqualAliasAndType() {
            var alias1 = new OptionAlias("a", OptionType.Short);
            var alias2 = new OptionAlias("a", OptionType.Short);
            Assert.IsTrue(alias1.Equals(alias2));
        }

        [TestMethod]
        public void Equals_EqualAliasUnequalType() {
            var alias1 = new OptionAlias("a", OptionType.Short);
            var alias2 = new OptionAlias("a", OptionType.Long);
            Assert.IsFalse(alias1.Equals(alias2));
        }

        [TestMethod]
        public void Equals_UnequalAliasEqualType() {
            var alias1 = new OptionAlias("a", OptionType.Short);
            var alias2 = new OptionAlias("b", OptionType.Short);
            Assert.IsFalse(alias1.Equals(alias2));
        }

        [TestMethod]
        public void OptionAliasUsage_DictionaryContainsKey_Present() {
            var dictionary = new Dictionary<OptionAlias, int>();
            dictionary.Add(new OptionAlias("a", OptionType.Short), 1);

            Assert.IsTrue(
                dictionary.ContainsKey(new OptionAlias("a", OptionType.Short)));
            Assert.AreEqual(dictionary[new OptionAlias("a", OptionType.Short)],
                1);
        }

        [TestMethod]
        public void OptionAliasUsage_DictionaryContainsKey_Absent() {
            var dictionary = new Dictionary<OptionAlias, int>();
            dictionary.Add(new OptionAlias("a", OptionType.Short), 1);

            Assert.IsFalse(
                dictionary.ContainsKey(new OptionAlias("a", OptionType.Long)));
            Assert.IsFalse(
                dictionary.ContainsKey(new OptionAlias("b", OptionType.Short)));
        }

    }

}