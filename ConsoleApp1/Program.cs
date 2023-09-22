using HtmlManager;
using HtmlManager.DomManager;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var file = File.ReadAllText(@"E:/html-teste-2.txt");

            var stream = new HtmlStream(file);
            var domBuilder = new DomBuilder(false);
            var parser = new HtmlParser(stream, domBuilder);

            try
            {
                parser.Parse();                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            

            Console.WriteLine("Parse Complete.");

            Document document = new Document(domBuilder);
            var body = document.Body();
            var id = body.AttributMap["id"];

            body = domBuilder.Fragment.Body;

            Console.WriteLine(body);

            Console.ReadLine();
        }
    }
}