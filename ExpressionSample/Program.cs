
using ExpressionSample;
using System.Linq.Expressions;

//Expression<Func<int, int>> expression = (num) => num + 5;
////Console.WriteLine($"NodeType:{expression.NodeType}");
////Console.WriteLine($"Body:{expression.Body}");
////Console.WriteLine($"Body Type: {expression.Body.GetType()}");
////Console.WriteLine($"Body NodeType: {expression.Body.NodeType}");
//if (expression.NodeType == ExpressionType.Lambda)
//{
//    var lambda = (LambdaExpression)expression;
//    var parameter = lambda.Parameters.Single();
//    Console.WriteLine($"parameter.Name:{parameter.Name}");
//    Console.WriteLine($"parameter.Type:{parameter.Type}");
//    Console.WriteLine($"parameter.ReturnType:{lambda.ReturnType}");
//}
////var @delegate = expression.Compile();
////Console.WriteLine(@delegate?.Invoke(2));


//if (expression.Body.NodeType == ExpressionType.Add)
//{
//    var binaryExpreesion = (BinaryExpression)expression.Body;


//    Console.WriteLine($"Left Type:{binaryExpreesion.Left.GetType()}");
//    Console.WriteLine($"Left NodeType:{binaryExpreesion.Left.NodeType}");

//    Console.WriteLine($"Right Type:{binaryExpreesion.Right.GetType()}");
//    Console.WriteLine($"Right NodeType:{binaryExpreesion.Right.NodeType}");

//    if (binaryExpreesion.Left is ParameterExpression parameterExpreesion)
//    {
//        Console.WriteLine($"parameterExpreesion.Name:{parameterExpreesion.Name}");
//        Console.WriteLine($"parameterExpreesion.Type:{parameterExpreesion.Type}");
//    }

//    if (binaryExpreesion.Right is ConstantExpression constantExpreesion)
//    {
//        Console.WriteLine($"constantExpreesion.Value:{constantExpreesion.Value}");
//    }



var parameterExpreesion1 = Expression.Parameter(typeof(int), "num");
BinaryExpression binaryExpression1 = Expression.MakeBinary(ExpressionType.Add, parameterExpreesion1, Expression.Constant(5));
Expression<Func<int, int>> expression1 = Expression.Lambda<Func<int, int>>(binaryExpression1, parameterExpreesion1);

if (expression1.Body.NodeType == ExpressionType.Add)
{
    var binaryExpreesion1 = (BinaryExpression)expression1.Body;


    Console.WriteLine($"Left Type:{binaryExpreesion1.Left.GetType()}");
    Console.WriteLine($"Left NodeType:{binaryExpreesion1.Left.NodeType}");

    Console.WriteLine($"Right Type:{binaryExpreesion1.Right.GetType()}");
    Console.WriteLine($"Right NodeType:{binaryExpreesion1.Right.NodeType}");

    if (binaryExpreesion1.Left is ParameterExpression parameterExpreesion2)
    {
        Console.WriteLine($"parameterExpreesion.Name:{parameterExpreesion2.Name}");
        Console.WriteLine($"parameterExpreesion.Type:{parameterExpreesion2.Type}");
    }

    if (binaryExpreesion1.Right is ConstantExpression constantExpreesion1)
    {
        Console.WriteLine($"constantExpreesion.Value:{constantExpreesion1.Value}");
    }

    var @delegate1 = expression1.Compile();
    Console.WriteLine($"result:{@delegate1(2)}");
}



