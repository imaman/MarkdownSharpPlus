using System;
using System.Collections.Generic;
using System.IO;

using System.Text;

namespace MarkdownSharpPlus
{


    public class Markdown
    {
        private Lexer input;
        public String Version = "0.0";


        public override String ToString()
        {
            return input.ToString();
        }

        public string Transform(string input_)
        {
            this.input = new Lexer(input_);
            Node n = parse();
            var result = n.transform();
            return result;
        }

        public String consume(int n)
        {
            return input.consume(n);
        }

        public Boolean consumeIf(String s)
        {
            return input.consumeIf(s);
        }

        public Boolean nextIs(String a)
        {
            return input.nextIs(a); 
        }

        int findNext(String a)
        {
            return input.findNext(a);
        }

        public Boolean eof()
        {
            return input.eof();
        }



        public Node parse()
        {
//            if (input.Contains("backticks"))
//                return new TextNode("<p>Fix for backticks within HTML tag: <span attr='`ticks`'>like this</span></p>");
            int count = -1;
            var res = new CompositeNode();
            while (true)
            {
                ++count;
                if (count > 0)
                    res.add(new TextNode("\n"));
                var n = parseList();
                res.add(new Wrapper("", n, "\n"));

                if (input.eof())
                    return res;
            }
        }
        public Node parseList()
        {
            if (!consumeIf("* "))
                return parseCodeBlock();

            CompositeNode c = new CompositeNode();
            while (true)
            {
                c.add(new Wrapper("<li>", line(), "</li>\n"));

                if (consumeIf("* "))
                    continue;
                consumeIf("\n");
                return new Wrapper("<ul>\n", c, "</ul>");
            }
        }

        public Node parseCodeBlock()
        {
            if (input.column != 0)
                return parseBlockquote();
            if (!consumeIf("\t") && !consumeIf("    "))
                return parseBlockquote();

            CodeBlockNode c = new CodeBlockNode();
            while (true)
            {
                c.add(new Wrapper("", rawLine(), "\n"));
                if (consumeIf("\t"))
                    continue;
                consumeIf("\n");
                return new Wrapper("<pre><code>", c, "</code></pre>");
            }
        }

        public Node rawLine()
        {
            int idx = findNext("\n");
            var s = consume(idx).Replace("<", "&lt;").Replace(">", "&gt;");
            consumeIf("\n");
            return new TextNode(s);
        }

        public Node parseBlockquote()
        {
            if(!consumeIf("> "))
                return parseElement();

            return new Wrapper("<blockquote>\n  ", parseParagraph(), "\n</blockquote>");
        }

        public Node parseElement()
        {
            if (!nextIs("<") || nextIs("<http://"))
                return parseParagraph();

            consume(1);
            int rt = findNext(">");
            int nameLen = Math.Min(rt, findNext(" "));
            String elem = consume(rt);
            String name = elem.Substring(0, nameLen);




            return parseParagraph();
        }

        public Node parseParagraph()
        {
            if (consumeIf("\n"))
                return new TextNode("");

            return new Wrapper("<p>", parseParagraphBody(), "</p>");
        }

        public Node parseParagraphBody()
        {
            var res = new CompositeNode();
            while (true)
            {
                var n = line();
                res.add(n);
                if(consumeIf("\n") || eof())
                    return res;
            }
        }

        public Node line()
        {
            if(consumeIf("\n"))
                return new TextNode("");
            if (input.eof())
                return new TextNode("");
            var res = new CompositeNode();
            res.add(fragment());
            if (nextIs("<http://"))
            {
                consume(1);
                res.add(link());
            }
            else if (consumeIf("<"))
                res.add(tag());
            else if (consumeIf("``"))
                res.add(code("``"));
            else if (consumeIf("`"))
                res.add(code("`"));
            res.add(line());


            return res;
        }

        public Node fragment()
        {
            var lt = Math.Min(findNext("`"), Math.Min(findNext("<"), findNext("\n")));
            return new TextNode(consume(lt));
        }

        public Node link()
        {
            var rt = input.IndexOf(">");
            if (rt < 0)
                return fragment();

            var lnk = consume(rt);
            consume(1);

            lnk = lnk.Replace("&", "&amp;");

            var s = "<a href=\"" + lnk + "\">" + lnk + "</a>";

            return new TextNode(s);
        }

        public Node tag()
        {
            var rt = input.IndexOf(">");
            if (rt < 0)
                return fragment();

            var body = consume(rt);
            consume(1);

            return new TextNode("<" + body + ">");
        }

        public Node code(String end)
        {
            var rt = input.IndexOf(end);
            if (rt < 0)
                return fragment();

            var s = consume(rt).Replace("<", "&lt;").Replace(">", "&gt;").Trim();
            consume(1);

            return new TextNode("<code>" + s + "</code>");
        }
    }

    public abstract class Node
    {
        public abstract String transform();
        public Boolean isEmpty
        {
            get
            {
                return transform().Length == 0;
            }
        }
    }

    public class Wrapper : Node
    {
        private String before;
        private String after;
        private Node inside;

        public Wrapper(String before, Node inside, String after)
        {
            this.before = before;
            this.inside = inside;
            this.after = after;
        }

        public override String transform() { return before + inside.transform() + after; }
    }

    public class TextNode : Node
    {
        private String s;

        public TextNode(String s) { this.s = s; }

        public override String transform() { return s; }
    }

    public class CompositeNode : Node
    {
        protected List<Node> ns = new List<Node>();

        public void add(Node n) { ns.Add(n); }

        public override String transform()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Node n in ns)
                sb.Append(n.transform());
            return sb.ToString();
        }
    }

    public class CodeBlockNode : CompositeNode
    {
        public override String transform()
        {
            for(int i = ns.Count - 1; i >= 0; --i)
            {
                var s = ns[i].transform();
                if(s.Trim().Length == 0)
                    ns.RemoveAt(i);
            }

            return base.transform();    
        }
    }
}
