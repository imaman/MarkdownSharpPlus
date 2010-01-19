using System;
using System.Collections.Generic;
using System.Text;

namespace MarkdownSharpPlus
{
    public class Lexer
    {
        private String input;

        public Lexer(String input_)
        {
            this.input = input_;
        }

        public override string ToString()
        {
            return input;
        }

        internal string consume(int n)
        {
            String res = input.Substring(0, n);
            input = input.Substring(n);

            foreach (var c in res)
            {
                ++col;
                if (c == '\n')
                    col = 0;
            }

            return res;
        }

        internal bool consumeIf(string s)
        {
            if (!nextIs(s))
                return false;

            consume(s.Length);
            return true;
        }

        internal bool nextIs(string a)
        {
            return input.StartsWith(a);
        }

        internal int findNext(string a)
        {
            var i = input.IndexOf(a);
            if (i >= 0)
                return i;

            return input.Length;
        }

        internal bool eof()
        {
            return input.Length == 0;
        }

        internal int IndexOf(string s)
        {
            return input.IndexOf(s);
        }

        private int col = 0;

        public int column
        {
            get
            {
                return col;
            }
        }
    }
}
