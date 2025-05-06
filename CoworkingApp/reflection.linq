<Query Kind="Program" />

using System.Reflection;

enum Foo 
{
	Bar, Buzz
}

void Main()
{
	var assembly = Assembly.LoadFile(@"C:\Users\phatt\Desktop\CSharp2-Project-master\CSharp2-Project-master\CoworkingApp\bin\Debug\net9.0\CoworkingApp.dll");
	
	assembly.GetTypes();
	
}

// You can define other methods, fields, classes and namespaces here
