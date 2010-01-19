using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace MarkdownSharpPlus
{
    [TestFixture]
    class Lexer_Tests
    {
        [Test]
        public void testEof()
        {
            Assert.IsTrue(new Lexer("").eof());
            Assert.IsFalse(new Lexer("a").eof());
        }

        [Test]
        public void testConsumeIf()
        {
            Lexer lexer = new Lexer("abcd");
            Assert.IsFalse(lexer.consumeIf("xab"));
            Assert.IsTrue(lexer.consumeIf("ab"));
            Assert.IsFalse(lexer.consumeIf("ycd"));
            Assert.IsTrue(lexer.consumeIf("cd"));
            Assert.IsTrue(lexer.eof());
        }

        [Test]
        public void testNextIs()
        {
            Lexer lexer = new Lexer("abcd");
            Assert.IsFalse(lexer.nextIs("xab"));
            Assert.IsFalse(lexer.nextIs("abx"));
            Assert.IsTrue(lexer.nextIs("ab"));
            Assert.IsTrue(lexer.consumeIf("ab"));
            Assert.IsFalse(lexer.nextIs("ycd"));
            Assert.IsTrue(lexer.nextIs("cd"));
        }

        [Test]
        public void testConsume()
        {
            Lexer lexer = new Lexer("123");
            lexer.consume(2);
            Assert.IsTrue(lexer.consumeIf("3"));
            Assert.IsTrue(lexer.eof());
        }

        [Test]
        public void testIndexOf()
        {
            Lexer lexer = new Lexer("1234");
            Assert.AreEqual(2, lexer.IndexOf("34"));
            Assert.AreEqual(2, lexer.IndexOf("3"));
            Assert.AreEqual(0, lexer.IndexOf("12"));
            Assert.AreEqual(0, lexer.IndexOf("1"));
            Assert.AreEqual(1, lexer.IndexOf("2"));
            Assert.AreEqual(1, lexer.IndexOf("234"));
            Assert.AreEqual(-1, lexer.IndexOf("2345"));
            Assert.AreEqual(-1, lexer.IndexOf("x"));
        }

        [Test]
        public void testColumn()
        {
            Lexer lexer = new Lexer("abcd\nwx\n\ny");
            Assert.AreEqual(0, lexer.column);

            lexer.consume(1);
            Assert.AreEqual(1, lexer.column);

            lexer.consume(2);
            Assert.AreEqual(3, lexer.column);

            lexer.consume(1);
            Assert.AreEqual(4, lexer.column);

            lexer.consume(1);
            Assert.AreEqual(0, lexer.column);

            lexer.consume(1);
            Assert.AreEqual(1, lexer.column);

            lexer.consume(1);
            Assert.AreEqual(2, lexer.column);

            lexer.consume(1);
            Assert.AreEqual(0, lexer.column);

            lexer.consume(1);
            Assert.AreEqual(0, lexer.column);

            lexer.consume(1);
            Assert.AreEqual(1, lexer.column);
        }
    }
}
