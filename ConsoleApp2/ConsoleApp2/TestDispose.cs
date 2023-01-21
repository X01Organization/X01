using System.Diagnostics;
using System.Text;

namespace ConsoleApp2
{
    internal class TestDispose
    {
        public void test1()
        {
            string testtext = "hallo world";
            using var stream = new MemoryStream();
            using var streamWritter = new StreamWriter(stream, Encoding.UTF8, 1000, true);
            streamWritter.Write(testtext);
            stream.Position = 0;
            using var streamReader = new StreamReader(stream);
            string resulttest = streamReader.ReadToEnd();
            Debug.Assert(testtext == resulttest);
        }
        public void test2()
        {
            string testtext = "hallo world";
            using (var stream = new MemoryStream())
            {
                using (var streamWritter = new StreamWriter(stream, Encoding.UTF8, 1000, true))
                {
                    streamWritter.Write(testtext);
                }
                stream.Position = 0;
                using (var streamReader = new StreamReader(stream))
                {
                    string resulttest = streamReader.ReadToEnd();
                    Debug.Assert(testtext == resulttest);
                }
            }
        }
    }
}
