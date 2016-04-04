<Query Kind="Program" />

void Main()
{
	
	
}

public class Point
{
	public float X;
	public float Y;
}

public class Line
{
	public float Slope;
	public float XIntercept;
}

public Line CreateLine(Point first, Point second)
{
	var rise = first.Y - second.Y;
	var run = first.X - second.X;
	var slope = rise/run;
	
	var intercept = first.Y - (first.X * slope);
	
	return new Line {Slope = slope, XIntercept = intercept};
}

public Line CreateLine2(Point first, Point second)
{
	var rise = first.Y - second.Y;
	var run = first.X - second.X;
	if(run == 0) 
	{
		return null;
	}
	var slope = rise/run;
	
	var intercept = first.Y - (first.X * slope);
	
	return new Line {Slope = slope, XIntercept = intercept};
}

public Line CreateLine3(Point first, Point second)
{
	var rise = first.Y - second.Y;
	var run = first.X - second.X;
	if(run == 0) 
	{
		throw new Exception("Slope of line is infinite");
	}
	var slope = rise/run;
	
	var intercept = first.Y - (first.X * slope);
	
	return new Line {Slope = slope, XIntercept = intercept};
}

