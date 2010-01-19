using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace MarkdownSharpPlus
{
    [TestFixture]
    class Markdown_Tests
    {

        private void check(String input, String expected)
        {
            var m = new Markdown();
            var actual = m.Transform(input);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void simpleLink()
        {
            check("Link: <http://example.com/>.", 
                "<p>Link: <a href=\"http://example.com/\">http://example.com/</a>.</p>\n");
        }

        [Test]
        public void linkWithAmpersand()
        {
            check("With an ampersand: <http://example.com/?foo=1&bar=2>",
                "<p>With an ampersand: <a href=\"http://example.com/?foo=1&amp;bar=2\">http://example.com/?foo=1&amp;bar=2</a></p>\n");
        }

        [Test]
        public void blockquotedLink()
        {
            check("> Blockquoted: <http://example.com/>", 
                "<blockquote>\n" +
                "  <p>Blockquoted: <a href=\"http://example.com/\">http://example.com/</a></p>\n" + 
                "</blockquote>\n");
        }

        [Test]
        public void code()
        {
            check("ab`something`cd",
                "<p>ab<code>something</code>cd</p>\n");
        }

        [Test]
        public void noLinksInsideCode()
        {
            check("Auto-links should not occur here: `<http://example.com/>`",
                "<p>Auto-links should not occur here: <code>&lt;http://example.com/&gt;</code></p>\n");
        }

        [Test]
        public void codeBlock()
        {
            check("\tor here: <http://example.com/>", 
                "<pre><code>or here: &lt;http://example.com/&gt;\n" +
                "</code></pre>\n");
        }

        [Test]
        public void list()
        {
            check("* In a list?\n" +
                    "* <http://example.com/>\n" +
                    "* It should.", 
                "<ul>\n" +
                    "<li>In a list?</li>\n" +
                    "<li><a href=\"http://example.com/\">http://example.com/</a></li>\n" +
                    "<li>It should.</li>\n" +
                    "</ul>\n");
        }

        [Test]
        public void trailingSpaceInCodeBlock()
        {
            check(
                "\tthe lines in this block  \n" +
                    "\tall contain trailing spaces  ",
                "<pre><code>the lines in this block  \n" +
                    "all contain trailing spaces  \n" +
                    "</code></pre>\n");
        }

        [Test]
        public void backticksWithinTags()
        {
            check("Fix for backticks within HTML tag: <span attr='`ticks`'>like this</span>",
                "<p>Fix for backticks within HTML tag: <span attr='`ticks`'>like this</span></p>\n");
        }

        [Test]
        public void backticksInsideCodeSpan()
        {
            check("Here's how you put `` `backticks` `` in a code span.",
                "<p>Here's how you put <code>`backticks`</code> in a code span.</p>\n");

        }

        //[Test]
        public void codeSpans()
        {
            new Tester().run("testfiles\\mdtest-1.1\\Code_Spans.text").reportFirst();
        }

        //[Test]
        public void test()
        {
            check(
                "<div>\n" + 
                "    foo\n" + 
                "</div>",
                "<div>\n" + 
                "    foo\n" +
                "</div>\n");


        }
    }

    
}
