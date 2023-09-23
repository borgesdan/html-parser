using HtmlManager;
using HtmlManager.DomManager;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var file = File.ReadAllText(@"E:/html-teste-2.txt");

            var htmlStream1 = new HtmlStream(file);
            var domBuilder = new DomBuilder(false);

            var parser = new HtmlParser(htmlStream1, domBuilder);            

            try
            {
                Document document = parser.Parse().Document;
                var body = document.Body;

                body = domBuilder.Fragment.Body;

                Console.WriteLine(body);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.ReadLine();
        }
    }
}