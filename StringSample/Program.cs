//string s1 = "ab";
//s1 += "c";
//Console.WriteLine(string.IsInterned(s1) ?? "null");
//string s2 = "abc";
//Console.WriteLine(s1 == s2);

//string s1 = "abc";
//string s2 = "ab";
//s2 += "c";
//string s3 = "ab";
//string s4 = "abc";
//Console.WriteLine(string.IsInterned(s1) ?? "null");
//Console.WriteLine(string.IsInterned(s2) ?? "null");
//Console.WriteLine(string.IsInterned(s3) ?? "null");

//Console.WriteLine(ReferenceEquals(s1, s2));
//Console.WriteLine(s1 == s2);

//Console.WriteLine(ReferenceEquals(s1, s4));
//Console.WriteLine(s1 == s4);


//string s1 = "abc";
//string s2 = "ab";
//string s3 = s2 + "c";
//Console.WriteLine(string.IsInterned(s3) ?? "null");


using System.Globalization;
using System.Text;

class Program
{
    static void Main()
    {
        //string s1 = "abc";
        //string s2 =  "ab";
        //s2 += "c";
        //Console.WriteLine(string.IsInterned(s2) ?? "null");
        //Console.WriteLine(string.IsInterned("ab") ?? "null");
        //string s3 = "abc";
        //Console.WriteLine(ReferenceEquals(s1,s2));
        //Console.WriteLine(ReferenceEquals(s1,s3));

        Console.WriteLine(string.IsInterned("abc") ?? "null");

        //string.Format
        string str = "我是";
        string str1 = "LPL";
        string str2 = "电竞选手!";
        Console.WriteLine(string.Format("{0}{1}{2}",str, str1, str2));
        Console.WriteLine($"{str}{str1}{str2}");

        //StringBuilder.Append
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(str);
        stringBuilder.Append(str1);
        stringBuilder.Append(str2);
        Console.WriteLine(stringBuilder.ToString());

        //string.Create
        Console.WriteLine(string.Create(CultureInfo.CurrentCulture,$"{str}{str1}{str2}"));
    }

}








