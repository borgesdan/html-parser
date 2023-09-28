using HtmlManager;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var file = File.ReadAllText(@"E:/html-teste-2.txt");
            var file = File.ReadAllText(@"E:/psly.html");

            var htmlStream1 = new HtmlStream(file);
            var domBuilder = new DomBuilder(false);

            var parser = new HtmlParser(htmlStream1, domBuilder);            

            try
            {
                var result = parser.Parse();
                Console.WriteLine("Concluído");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.ReadLine();
        }
    }
}