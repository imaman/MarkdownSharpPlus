using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;


namespace MarkdownSharpPlus
{
    public class Tester
    {
        static void Main(String[] args)
        {
            new Tester().run("testfiles\\mdtest-1.1\\Code_Blocks.text").reportFirst();

            //new Markdown_Tests().list();

            Console.WriteLine("-the end-");
        }

        public void reportFirst()
        {
            foreach (var e in es)
                e.emitException();
        }

        private void report()
        {
            foreach (var e in es)
            {
                Console.WriteLine(e.Message);
            }
        }

        public abstract class Err
        {
            public abstract String Message
            {
                get;
            }

            public abstract void emitException();
        }


        private List<Err> es = new List<Err>();

        public class FailureError : Err
        {
            public Exception e;

            public FailureError(Exception e) { this.e = e; }

            public override String Message
            {
                get
                {
                    return e.Message;
                }
            }

            public override void emitException()
            {
                throw e;
            }            
        }

        public class MismatchError : Err
        {
            public String file;
            public String kind;
            public String exepcted;
            public String actual;

            public MismatchError(String k, String f, String ex, String ac)
            {
                kind = k;
                file = f;
                exepcted = ex;
                actual = ac;
            }

            public override String ToString() { return kind + ": " + file; }

            public override String Message
            {
                get
                {
                    return "Expected\n" + exepcted + "\n--------------\n\nBut got\n" + actual + "\n-----------------\n";
                }
            }

            public override void emitException()
            {
                Assert.AreEqual(exepcted, actual);
            }            
        }

        public Tester run(String file)
        {
            var m = new Markdown();
            var expected = FileContents(Path.ChangeExtension(file, "html"));
            try
            {
                var output = m.Transform(FileContents(file));
                if (output != expected)
                    es.Add(new MismatchError("mismatch", file, expected, output));
            }
            catch (Exception e)
            {
                es.Add(new FailureError(e));
            }

            return this;
        }

        static string FileContents(string filename)
        {
            try
            {
                return File.ReadAllText(Path.Combine(ExecutingAssemblyPath, filename));
            }
            catch (FileNotFoundException)
            {
                return "";
            }
        }

        static private string ExecutingAssemblyPath
        {
            get
            {
                string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                // removes executable part
                path = Path.GetDirectoryName(path);
                // we're typically in \bin\debug or bin\release so move up two folders
                path = Path.Combine(path, "..");
                path = Path.Combine(path, "..");
                path = Path.Combine(path, "..");
                path = Path.Combine(path, "MarkdownSharpTests");
                return path;
            }
        }
    }
}
