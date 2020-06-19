using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Business
{
    public class OfxProcessBusiness
    {
        public List<STMTTRN> GetUniqueTransactionsOfFiles(List<string> ofxTexts)
        {
            var transactionsOfFiles = new List<STMTTRN>();
            foreach (var ofxText in ofxTexts)
            {
                var fileTransactions = ProcessOfxText(ofxText);
                transactionsOfFiles.AddRange(fileTransactions);
            }

            var uniqueTransactions = transactionsOfFiles.GroupBy(t => new { t.DTPOSTED, t.TRNTYPE, t.TRNAMT, t.MEMO }).Select(t => t.First()).OrderBy(t => t.DTPOSTED).ToList();

            return uniqueTransactions;
        }

        private static List<STMTTRN> ProcessOfxText(string ofxText)
        {
            ofxText = RemoveBreakLinesAndTabs(ofxText);

            var stmttrn = GetStmttrnContent(ofxText);

            var strList = getStmttrnStrings(stmttrn);

            var transactionList = new List<STMTTRN>();
            foreach (var str in strList)
            {
                var transaction = new STMTTRN();

                transaction.TRNTYPE = GetContent(str, "TRNTYPE");
                transaction.DTPOSTED = StringToDtPosted(GetContent(str, "DTPOSTED"));
                transaction.TRNAMT = StringToTrnAmt(GetContent(str, "TRNAMT"));
                transaction.MEMO = GetContent(str, "MEMO");

                transactionList.Add(transaction);
            }

            return transactionList;

        }

        private static List<string> getStmttrnStrings(string s)
        {
            var list = s.Replace("</STMTTRN>", "</STMTTRN>\n").Split("\n").ToList();

            list.RemoveAt(list.Count - 1);

            return list;
        }

        private static decimal StringToTrnAmt(string strTrnAmt)
        {
            var format = new NumberFormatInfo
            {
                NegativeSign = "-",
                NumberDecimalSeparator = "."
            };

            return Convert.ToDecimal(strTrnAmt, format);
        }

        private static DateTime StringToDtPosted(string strDtPosted)
        {
            strDtPosted = strDtPosted.Substring(0, strDtPosted.IndexOf("["));
            return DateTime.ParseExact(strDtPosted, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

        private static string GetStmttrnContent(string ofxText)
        {
            var tagName = "STMTTRN";
            var openTag = $"<{tagName}>";
            var closeTag = $"</{tagName}>";

            var startPos = ofxText.IndexOf(openTag);
            var endPos = ofxText.LastIndexOf(closeTag);

            return ofxText.Substring(startPos, (endPos + closeTag.Length) - startPos);
        }

        private static string RemoveBreakLinesAndTabs(string ofxText)
        {
            return ofxText.Replace("\r", "").Replace("\n", "").Replace("\t", "");
        }

        private static string GetContent(string text, string elSearch)
        {
            var tagSearch = $"<{elSearch}>";
            var closeTagSearch = $"</{elSearch}>";

            var posIniCont = text.IndexOf(tagSearch) + tagSearch.Length;

            var posFimCont = text.IndexOf(closeTagSearch);

            if (posFimCont == -1)
                posFimCont = text.IndexOf("<", posIniCont);

            if (posFimCont == -1)
                posFimCont = text.Length;

            return text.Substring(posIniCont, posFimCont - posIniCont).Trim();
        }
    }
}
