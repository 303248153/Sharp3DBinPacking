# 3D bin packing algorithms

[![NuGet](https://img.shields.io/nuget/vpre/Sharp3DBinPacking.svg)](http://www.nuget.org/packages/Sharp3DBinPacking)
[![license](https://img.shields.io/github/license/303248153/Sharp3DBinPacking.svg)]() 
[![GitHub release](https://img.shields.io/github/release/303248153/Sharp3DBinPacking.svg)]()

[Bin packing problem](https://en.wikipedia.org/wiki/Bin_packing_problem).

This library is translated from [https://github.com/krris/3d-bin-packing](https://github.com/krris/3d-bin-packing).

# Example

The coordinates are defined as:

``` text
       o--------o
      /|       /|
     / |      / |
  h o--------o  |
  e |  o-----|--o h
y i | /      | / t
  g |/       |/ p z
  h o--------o e
  t | width   d
    |  x
 (0, 0, 0)
```

Example code:

``` csharp
// Define the size of bin
var binWidth = 1000;
var binHeight = 1000;
var binDepth = 1000;

// Define the cuboids to pack
var parameter = new BinPackParameter(binWidth, binHeight, binDepth, new[]
{
	new Cuboid(150, 100, 150),
	new Cuboid(500, 500, 500),
	new Cuboid(500, 550, 700),
	new Cuboid(350, 350, 350),
	new Cuboid(650, 750, 850),
});

// Create a bin packer instance
// The default bin packer will test all algorithms and try to find the best result
// BinPackerVerifyOption is used to avoid bugs, it will check whether the result is correct
var binPacker = BinPacker.GetDefault(BinPackerVerifyOption.BestOnly);

// The result contains bins which contains packed cuboids whith their coordinates
var result = binPacker.Pack(parameter);
foreach (var bins in result.BestResult)
{
	Console.WriteLine("Bin:");
	foreach (var cuboid in bins)
	{
		Console.WriteLine(cuboid);
	}
}
```

Outputs:

``` text
Bin:
Cuboid(X: 0, Y: 0, Z:0, Width: 750, Height:850, Depth:650)
Cuboid(X: 0, Y: 0, Z:650, Width: 350, Height:350, Depth:350)
Cuboid(X: 750, Y: 0, Z:0, Width: 150, Height:150, Depth:100)
Bin:
Cuboid(X: 0, Y: 0, Z:0, Width: 700, Height:500, Depth:550)
Cuboid(X: 0, Y: 500, Z:0, Width: 500, Height:500, Depth:500)
```

# License

MIT License<br/>
Copyright © 2018 303248153@github<br/>
If you have any license issue please contact 303248153@qq.com.<br/>
