// See https://aka.ms/new-console-template for more information

using LinqSample;

var animals = new List<string>() { "Cat", "Dog", "Pig" };

//foreach (var animla in animals)
//{
//    Console.WriteLine(animla);
//}
//Console.WriteLine("-----------");

//var enumerator = animals.GetEnumerator();
//while (enumerator.MoveNext())
//{
//    Console.WriteLine(enumerator.Current);
//}

//var result= animals.MyWhere(t => t is "Cat" or "Dog").Select(t=>t.ToUpper()).ToList();

//var result = (from t in animals
//              where t is "Cat" or "Dog"
//              select t.ToUpper()).ToList();
//result.ForEach(t => Console.WriteLine(t));

var result1 = (from t in animals
                             where (t.Equals( "Cat") || t.Equals("Dog"))
                             select t.ToUpper()).AsQueryable();
Console.WriteLine($"Expression:{ result1.Expression.ToString()}");
Console.WriteLine($"ExpressionType:{result1.Expression.GetType()}");
foreach (var item in result1)
{
    Console.WriteLine(item);
}

Console.WriteLine("---------------");
var result2 = from t in result1
              where  t.Contains("CAT")
              select t;
Console.WriteLine($"Expression:{ result2.Expression.ToString()}");
Console.WriteLine($"ExpressionType:{result2.Expression.GetType()}");
foreach (var item in result2)
{
    Console.WriteLine(item);
}
Console.ReadLine();
