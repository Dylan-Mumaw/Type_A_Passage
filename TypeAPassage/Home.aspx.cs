using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using System.Windows.Input;
using HtmlAgilityPack;
using System.Diagnostics;

namespace TypeAPassage
{
    public partial class Home : System.Web.UI.Page
    {
        protected int wordCount = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            //TypingTimer.Interval = 1000;
            var random = new Random(); //Variable for selecting random book

            //<-----Begin Retrieve Book----->
            string bookListUrl = "https://www.gutenberg.org/ebooks/search/?sort_order=downloads";
            var bookListResponse = CallUrl(bookListUrl).Result;
            var bookLinkList = ParseTopBookList(bookListResponse); //Returns list of top 25 books after receiving URL response

            //Selects random book from previous list
            int index = random.Next(bookLinkList.Count);
            var selectedBook = bookLinkList[index]; //Returns final part of link for selected book

            //Pull book-text HTML link for selected book
            string selectedBookLink = "https://www.gutenberg.org" + selectedBook.ToString();
            var appendedLinkNumber = ReadBookLink(selectedBookLink);

            string bookTextTypeTest = "https://www.gutenberg.org" + appendedLinkNumber[0];
            string selectedBookTest = ParseSelectedBook(bookTextTypeTest).ToString();
            bookTextTypeTest = GetTitle(bookTextTypeTest).ToString();
            selectedBookTest = RemoveWhiteSpace(selectedBookTest);
            wordCount = Int32.Parse(CountWords(selectedBookTest));

            BookLinkLabel.Text = bookTextTypeTest.ToString();
            BookLinkBox.Text = selectedBookTest;
        }

        //<-----Begin Retrieve Book Logic----->

        private static Task<string> CallUrl(string fullURL)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            client.DefaultRequestHeaders.Accept.Clear();

            var response = client.GetStringAsync(fullURL);
            return response;
        }

        private List<string> ParseTopBookList(string html) //Returns list of top 25 books after receiving URL response
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            //NEW HREF SCRAPE
            //Selects all 'li' nodes of the class 'booklink'
            var texts = htmlDoc.DocumentNode.SelectNodes("//li[contains(@class, 'booklink')]");
            //Retrieves all 'a' tags in 'li' nodes of the class 'booklink'
            var hrefs = texts.Descendants("a")
            //Selects 'href' tag attribute and assigns to list
           .Select(node => node.GetAttributeValue("href", ""))
           .ToList();
            List<string> bookIndexLinks = new List<string>();

            foreach (var link in hrefs)
            {
                if (link.Count() > 0)
                    bookIndexLinks.Add(link.ToString());
            }
            return bookIndexLinks;
        }

        private string ParseSelectedBook(string book)//Parses FIRST paragraph FROM selected book
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            List<string> content = new List<string>();
            var response = CallUrl(book).Result;
            string selectedBook;
            char[] delimiter = new char[] { ' ' };

            htmlDoc.LoadHtml(response);

            foreach (string word in htmlDoc.DocumentNode.SelectNodes("//p/text()").Select(node => node.InnerText)) //Can be Innerhtml or innertext
            {
                var words = word.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).Where(s => Char.IsLetter(s[0]));
                if (words.Count() > 20 && words.Count() < 50)
                {
                    content.Add(word);
                }
            }

            selectedBook = content[1].ToString();
            selectedBook = HttpUtility.HtmlDecode(selectedBook); //Decodes alphanumeric values into characters 
            selectedBook = selectedBook.Replace('“', '"').Replace('”', '"').Replace('’', '\'');
            selectedBook = selectedBook.Replace('\n', ' ').Replace('\r', ' ');
            return selectedBook;
        }

        private List<string> ReadBookLink(string readLink)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            List<string> bookIndexLinks = new List<string>();
            var response = CallUrl(readLink).Result;

            htmlDoc.LoadHtml(response);

            var texts = htmlDoc.DocumentNode.SelectNodes("//tr[contains(@class, 'even')]");
            var hrefs = texts.Descendants("a")
            .Select(node => node.GetAttributeValue("href", ""))
            .ToList();

            foreach (var link in hrefs)
            {
                if (link.Count() > 0)
                    bookIndexLinks.Add(link.ToString());
            }
            return bookIndexLinks;
        }

        //<-----End Retrieve Book Logic----->

        //<-----Begin Removing Extra Spaces----->
        private static string RemoveWhiteSpace(string longString)
        {
            StringBuilder sb = new StringBuilder();
            bool lastWasSpace = true; //True to eliminate leading spaces

            for (int i = 0; i < longString.Length; i++)
            {
                if (Char.IsWhiteSpace(longString[i]) && lastWasSpace)
                {
                    continue;
                }

                lastWasSpace = Char.IsWhiteSpace(longString[i]);

                sb.Append(longString[i]);
            }

            //Last character might be space
            if (Char.IsWhiteSpace(sb[sb.Length - 1]))
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
        //<-----End Removing Extra Spaces----->

        //<-----Begin Getting Title----->
        private string GetTitle(string titleURL)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            List<string> title = new List<string>();
            var response = CallUrl(titleURL).Result;
            htmlDoc.LoadHtml(response);

            foreach (string word in htmlDoc.DocumentNode.SelectNodes("//h1/text()").Select(node => node.InnerText))
            {
                title.Add(word);
            }

            string bookTitle = title[0].ToString();
            return bookTitle;
        }
        //<-----End Getting Title----->

        //<-----Begin Counting Words----->
        private static string CountWords(string selectedBookParagraph)
        {
            int words = 0;
            List<string> paragraph = selectedBookParagraph.Split(new[] { ' ', '\n', '\r' }).ToList();
            foreach (string word in paragraph)
            {
                words++;
            }
            return words.ToString();
        }
        //<-----End Counting Words----->

        //<-----Begin Typing Test Buttons----->
        protected void StartTestButton_Click(object sender, EventArgs e)
        {
            TypingTestBox.Enabled = true;
            TypingTestBox.Focus();
        }

        //<-----End Typing Test Buttons----->
    }
}